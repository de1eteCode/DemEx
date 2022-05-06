using DemExApp.Data;
using DemExApp.Models;
using DemExApp.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DemExApp {
    public partial class WindowOfAgents : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _searchQuery = string.Empty;
        private ItemWithTitle<IOrdering> _selectedOrder = null;
        private ItemWithTitle<Filter> _selectedFilter = null;
        private int _minPage = 1;
        private int _currentPage = 1;
        private int _displayItems = 20;

        public WindowOfAgents() {
            InitializeComponent();
            Loaded += (s, e) => {
                LoadCollection();
            };
        }

        private void LoadCollection() {
            Task.Run(() => {
                var db = Data.DataContext.Instance();
                AgentsCollection = db.Agent.ToList();
                OnPropertyChanged(nameof(Agents));
            });
        }

        private IEnumerable<Agent> AgentsCollection { get; set; } = new List<Agent>();

        public IEnumerable<Agent> Agents {
            get {
                var res = SelectedOrder
                    .Item
                    .Order(AgentsCollection.Where(e => SelectedFilter.Item.IsEqual(e, SearchQuery)), _selectedFilter.Item.Property)
                    .ToList();
                FilteredCount = res.Count();
                OnPropertyChanged(nameof(Pages));
                return res
                    .Skip(_displayItems * (CurrentPage - 1))
                    .Take(_displayItems);
            }
        }
        public Agent LastSelectedAgent { get; set; }

        public ObservableCollection<ItemWithTitle<IOrdering>> OrderingList { get; } = new ObservableCollection<ItemWithTitle<IOrdering>>() {
            new ItemWithTitle<IOrdering>(new AZOrder(), "А - Я"),
            new ItemWithTitle<IOrdering>(new ZAOrder(), "Я - А")
        };

        public ItemWithTitle<IOrdering> SelectedOrder {
            get {
                if (_selectedOrder is null) {
                    _selectedOrder = OrderingList.First();
                }
                return _selectedOrder;
            }
            set {
                _selectedOrder = value;
                OnPropertyChanged(nameof(Agents));
            }
        }

        public ObservableCollection<ItemWithTitle<Filter>> FilterList { get; } = new ObservableCollection<ItemWithTitle<Filter>>() {

            new ItemWithTitle<Filter>(new Filter("Name"), "Имя"),
            new ItemWithTitle<Filter>(new Filter("Email"), "Почта"),
            new ItemWithTitle<Filter>(new Filter("Phone"), "Телефон"),
            new ItemWithTitle<Filter>(new Filter("Priority"), "Приоритет"),
            new ItemWithTitle<Filter>(new Filter("Director"), "Директор"),
            new ItemWithTitle<Filter>(new Filter("INN"), "ИНН"),
            new ItemWithTitle<Filter>(new Filter("KPP"), "КПП"),
            new ItemWithTitle<Filter>(new Filter("TypeString"), "Тип агента"),
            new ItemWithTitle<Filter>(new Filter("Discount"), "Скидка")
        };

        public ItemWithTitle<Filter> SelectedFilter {
            get {
                if (_selectedFilter is null) {
                    _selectedFilter = FilterList.First();
                }
                return _selectedFilter;
            }
            set {
                _selectedFilter = value;
                OnPropertyChanged(nameof(Agents));
            }
        }

        public string SearchQuery {
            get {
                return _searchQuery;
            }
            set {
                _searchQuery = value;
                OnPropertyChanged(nameof(Agents));
            }
        }

        private int _filteredCount;
        private int FilteredCount {
            get { return _filteredCount; }
            set {
                _filteredCount = value;
                OnPropertyChanged(nameof(MaxPage));
            }
        }
        public int CurrentPage {
            get {
                return _currentPage;
            }
            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException("Страниц не может быть меньше 1");
                }
                _currentPage = value;
                OnPropertyChanged();
            }
        }
        public int MaxPage {
            get {
                var res = (int)Math.Ceiling(FilteredCount / (float)_displayItems);
                res = res > 0 ? res : 1;
                if (CurrentPage > res)
                {
                    CurrentPage = res;
                }
                return res;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string property = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public IEnumerable<int> Pages {
            get {
                return Enumerable.Range(_minPage, MaxPage);
            }
        }

        private void SelectPage_Click(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count < 1)
            {
                return;
            }
            var page = e.AddedItems[0] as int?;
            CurrentPage = (int)page;
            OnPropertyChanged(nameof(Agents));
        }

        private void PreviousPage(object sender, MouseButtonEventArgs e) {
            if (CurrentPage > _minPage) {
                CurrentPage--;
            }
        }

        private void NextPage(object sender, MouseButtonEventArgs e) {
            if (CurrentPage < MaxPage) {
                CurrentPage++;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var list = sender as ListView;
            if (list.SelectedItems.Count < 2) {
                SelectedItems.Clear();
            }
            else {
                SelectedItems = list.SelectedItems.Cast<Agent>().ToList();
            }
            OnPropertyChanged(nameof(IsAllowChangedPriority));
        }

        public List<Agent> SelectedItems { get; set; } = new List<Agent>();
        public bool IsAllowChangedPriority => SelectedItems.Count() > 1;

        private void ChangePriority_Click(object sender, RoutedEventArgs e) {
            var wind = new GetterValueWindow(SelectedItems.Max(ev => ev.Priority));

            var res = wind.ShowDialog();

            if (res != null && (bool)res) {
                var value = int.Parse(wind.Value);

                var db = Data.DataContext.Instance();
                foreach (var item in SelectedItems) {
                    item.Priority = value;
                }
                db.SaveChanges();
                LoadCollection();
                MessageBox.Show("Приоритет изменен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddAgent_Click(object sender, RoutedEventArgs e) {
            var agent = new Agent();
            EditAgent(agent);
        }

        private void EditAgent_Click(object sender, RoutedEventArgs e) {
            var db = Data.DataContext.Instance();
            var editAgent = db.Agent.First(s => s.UID == LastSelectedAgent.UID);
            //db.Entry(editAgent).State = EntityState.Detached;

            if (editAgent == null) {
                MessageBox.Show("Агент для редактирования не выбран", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            EditAgent(editAgent);
        }

        private void EditAgent(Agent agent) {
            var wind = new EditAgentWindow(agent);
            wind.ShowDialog();
            LoadCollection();
        }
    }
}

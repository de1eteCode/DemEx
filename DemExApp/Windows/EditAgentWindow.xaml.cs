using DemExApp.Data;
using DemExApp.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DemExApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditAgentWindow.xaml
    /// </summary>
    public partial class EditAgentWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _searchQuery = string.Empty;
        public EditAgentWindow(Agent agent)
        {
            Agent = agent;
            Loaded += (s, e) =>
            {
                LoadCollections();
            };

            InitializeComponent();
        }

        public IEnumerable<TypeAgent> TypeAgents { get; set; } = new List<TypeAgent>();
        public IEnumerable<ProductAgent> ProductAgents { get; set; } = new List<ProductAgent>();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<ItemWithTitle<TypeProduct>> TypeProducts { get; set; } = new List<ItemWithTitle<TypeProduct>>();
        public IEnumerable<Product> FilteredProducts
        {
            get
            {
                return Products.Where(e => e.Name.Contains(SearchQuery)).OrderBy(e => e.Name);
            }
        }
        public IEnumerable<ProductAgent> AgentProducts => Agent.ProductAgent.OrderBy(e => e.Product.Name);

        private List<ProductAgent> _productAgentsForDelete = new List<ProductAgent>();

        public string SearchQuery
        {
            get
            {
                return _searchQuery;
            }
            set
            {
                _searchQuery = value;
                OnPropertyChanged(nameof(FilteredProducts));
            }
        }

        public Agent Agent { get; set; }
        public TypeAgent TypeAgent
        {
            get
            {
                if (Agent.TypeAgent == null)
                {
                    Agent.TypeAgent = TypeAgents.FirstOrDefault();
                }
                return Agent.TypeAgent;
            }
            set
            {
                Agent.TypeAgent = value;
            }
        }
        public ProductAgent SelectedProductAgent { get; set; }

        private void LoadCollections()
        {
            Task.Run(() =>
            {
                var db = Data.DataContext.Instance();
                Products = db.Product
                    .Include(e => e.ProductAgent)
                    .Include(e => e.TypeProduct)
                    .ToList();

                TypeAgents = db.TypeAgent.ToList();

                TypeProducts = Products.Select(e => new ItemWithTitle<TypeProduct>(e.TypeProduct, e.TypeProduct.Name));

                if (Agent.UID != Guid.Empty)
                {
                    ProductAgents = db.ProductAgent.Where(e => e.UID == Agent.UID).ToList();
                    OnPropertyChanged(nameof(ProductAgents));
                }

                OnPropertyChanged(nameof(TypeProducts));
                OnPropertyChanged(nameof(TypeAgents));
                OnPropertyChanged(nameof(TypeAgent));
                OnPropertyChanged(nameof(FilteredProducts));
            });
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (Agent.UID != Guid.Empty)
            {
                if (_productAgentsForDelete.Count() > 0)
                {
                    foreach (var item in _productAgentsForDelete)
                    {
                        Agent.ProductAgent.Add(item);
                    }
                }

                var db = Data.DataContext.Instance();
                db.Entry(Agent).Reload();
            }

            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Agent.UID == Guid.Empty)
            {
                MessageBox.Show("Нельзя удалить создаваемого агента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Вы хотите удалить агента?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            var db = Data.DataContext.Instance();
            var ag = db.Agent.Where(s => s.UID == Agent.UID).FirstOrDefault();
            var anySales = ag.ProductAgent.Any();

            if (anySales)
            {
                MessageBox.Show("Нельзя удалить агента с продажами", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            db.Entry(ag).State = EntityState.Deleted;
            db.SaveChanges();

            MessageBox.Show("Агент удален", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Validate() == false)
            {
                MessageBox.Show("Введенная форма не корректна", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var db = Data.DataContext.Instance();

            if (_productAgentsForDelete.Count() > 0)
            {
                foreach (var item in _productAgentsForDelete)
                {
                    db.Entry(item).State = EntityState.Deleted;
                }
            }

            if (Agent.UID == Guid.Empty)
            {
                db.Entry(Agent).State = EntityState.Added;
            }

            foreach (var pa in Agent.ProductAgent)
            {
                if (pa.UID == Guid.Empty)
                {
                    pa.UID = Guid.NewGuid();
                    db.Entry(pa).State = EntityState.Added;
                }
            }

            db.SaveChanges();

            MessageBox.Show("Агент сохранен", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void ComboBox_AddProduct(object sender, RoutedEventArgs e)
        {
            var cb = sender as ComboBox;
            var product = cb.SelectedItem as Product;

            if (product is null)
            {
                return;
            }

            var wind = new GetterValueWindow(1);
            var res = wind.ShowDialog();

            if (res == null || (bool)res == false)
            {
                return;
            }

            var val = int.Parse(wind.Value);

            var item = Agent.ProductAgent.FirstOrDefault(s => s.ProductUID == product.UID);
            if (item != null)
            {
                item.Count += val;
            }
            else
            {
                Agent.ProductAgent.Add(new ProductAgent()
                {
                    Agent = Agent,
                    Product = product,
                    Date = DateTime.Now,
                    Count = val
                });
            }

            OnPropertyChanged(nameof(AgentProducts));
        }

        private void RemoveProductAgent(object sender, RoutedEventArgs e)
        {
            if (SelectedProductAgent == null)
            {
                MessageBox.Show("Не выбран объект для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            
            Agent.ProductAgent.Remove(SelectedProductAgent);
            _productAgentsForDelete.Add(SelectedProductAgent);

            SelectedProductAgent = null;
            OnPropertyChanged(nameof(AgentProducts));
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Выберете новое лого",
                Filter = "Любые картинки|*.jpg;*.jpeg;*.png"
            };
            if (dialog.ShowDialog() == true)
            {
                var bytes = File.ReadAllBytes(dialog.FileName);
                Agent.Logo = bytes;
                OnPropertyChanged(nameof(Agent));
            }
        }

        private bool Validate()
        {
            return
                string.IsNullOrEmpty(Agent.Name) == false &&
                string.IsNullOrEmpty(Agent.Email) == false &&
                string.IsNullOrEmpty(Agent.Phone) == false &&
                string.IsNullOrEmpty(Agent.Address) == false &&
                Agent.Priority > -1 &&
                string.IsNullOrEmpty(Agent.Director) == false &&
                string.IsNullOrEmpty(Agent.INN) == false &&
                Agent.INN.Length < 31 &&
                string.IsNullOrEmpty(Agent.KPP) == false &&
                Agent.KPP.Length < 31 &&
                Agent.TypeAgent != null;
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void _this_Closed(object sender, EventArgs e)
        {
            Close_Click(null, null);
        }
    }
}

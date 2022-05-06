using System.ComponentModel;
using System.Windows;

namespace DemExApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для GetterValueWindow.xaml
    /// </summary>
    public partial class GetterValueWindow : Window
    {
        public GetterValueWindow(int value)
        {
            InitializeComponent();
            Value = value.ToString();
            DataContext = this;
        }

        public string Value { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int val = -1;
            if (int.TryParse(Value, out val) == false)
            {
                MessageBox.Show("Введенные параметры не являются корректными");
                return;
            }

            if (val >= 0 || val < int.MaxValue)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Введенное число не является валидным");
            }
        }
    }
}

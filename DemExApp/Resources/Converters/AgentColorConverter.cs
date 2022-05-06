using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DemExApp.Resources.Converters
{
    class AgentColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;

            if (value is int == false)
                return value;

            if ((int)value >= 25)
                return Brushes.LightGreen;

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

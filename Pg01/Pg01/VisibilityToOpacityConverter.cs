using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pg01
{
    public class VisibilityToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value != null)
            {
                var v = value as Visibility? ?? Visibility.Hidden;
                return v == Visibility.Visible ? 0.5 : 0.0;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
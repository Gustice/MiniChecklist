using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MiniChecklist.Converter
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;

            if (((string)parameter) == "!")
            {
                if (val)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }

            if (val)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

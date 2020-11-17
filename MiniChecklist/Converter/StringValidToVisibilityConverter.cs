using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MiniChecklist.Converter
{
    public class StringValidToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string)value;
            var constraint = (string)parameter;

            if (constraint.Equals("NotEmpty"))
            {
                if (string.IsNullOrEmpty(text) )
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

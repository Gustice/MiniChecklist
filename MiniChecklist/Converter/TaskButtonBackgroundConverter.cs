using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MiniChecklist.Converter
{
    class TaskButtonBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            var color = Color.FromRgb(0x60, 0x20, 0x20);

            if (val)
                color = Color.FromRgb(0x20, 0x60, 0x20);

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

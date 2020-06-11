using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MiniChecklist.Converter
{
    class ColorConverter : IValueConverter
    {
        public double Amount { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = (SolidColorBrush)value;
            return new SolidColorBrush(new HslColor(source.Color).Lighten(Amount).ToRgb());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
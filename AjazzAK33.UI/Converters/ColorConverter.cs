using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AjazzAK33.UI
{
    public class ColorConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is Dictionary<Key, Color>))
                return Avalonia.AvaloniaProperty.UnsetValue;

            var dict = (Dictionary<Key, Color>)values[0];
            var i = Enum.Parse<Key>(values[1].ToString());
            ColorUtils.ToHsv(dict[i].ToDrawingClr(), out var h, out var s, out var v);
            if (v < 100)
                return Brushes.White;
            else
                return Brushes.Black;
        }
    }
}

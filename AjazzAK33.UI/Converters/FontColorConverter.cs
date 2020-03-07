using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AjazzAK33.UI.Converters
{
    public class FontColorConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count != 2)
                throw new ArgumentException("There must be 2 values passed in");

            if (!(values[0] is IDictionary<Key, Color>))
                return Avalonia.AvaloniaProperty.UnsetValue;

            if (!(values[1] is string))
                return Avalonia.AvaloniaProperty.UnsetValue;

            var dict = (IDictionary<Key, Color>)values[0];
            if (!Enum.TryParse<Key>((string)values[1], out var k))
                return Avalonia.AvaloniaProperty.UnsetValue;

            return new SolidColorBrush(dict[k].GetFontColorFromBackground());
        }
    }
}

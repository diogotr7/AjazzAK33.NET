using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AjazzAK33.UI
{
    public class DictConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(values[0] is Dictionary<Key, Color>))
                    return Avalonia.AvaloniaProperty.UnsetValue;

                var dict = (Dictionary<Key, Color>)values[0];
                var i = Enum.Parse<Key>(values[1].ToString());
                return new SolidColorBrush(dict[i]);
            }
            catch
            {
                return Avalonia.AvaloniaProperty.UnsetValue;
            }
        }
    }
}

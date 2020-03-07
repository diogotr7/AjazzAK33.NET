using AjazzAK33.UI.Utils;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace AjazzAK33.UI.Converters
{
    public class EnumKeyNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum.TryParse<Key>(value as string, out var r))
                return EnumUtils.GetEnumDescription(r);

            return Avalonia.AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}

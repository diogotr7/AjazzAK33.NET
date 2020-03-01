using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AjazzAK33;
using System.Threading.Tasks;

namespace AjazzAK33.UI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Dictionary<Key, Color> colors;
        public Dictionary<Key, Color> Colors
        {
            get => colors;
            set
            {
                colors = value;
                OnPropertyChanged();
            }
        }

        private readonly List<Tuple<Key, System.Drawing.Color>> kbColors = new List<Tuple<Key, System.Drawing.Color>>();


        public MainWindowViewModel()
        {
            Color c = Color.FromRgb(0, 0, 255);
            Colors = new Dictionary<Key, Color>();
            foreach (var k in (Key[])Enum.GetValues(typeof(Key)))
            {
                Colors.Add(k, c);
                c = ColorUtils.ChangeHue(c.ToDrawingClr(), 1).ToAvaloniaClr();
            }
        }

        public async void Click(string name)
        {
            if (!Enum.TryParse<Key>(name, out var r))
                return;
            ColorPicker cp = new ColorPicker()
            {
                DataContext = new ColorPickerViewModel()
                {
                    Color = Colors[r]
                }
            };
            var res = await cp.ShowDialog<string>((Avalonia.Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);
            Colors[r] = (cp.DataContext as ColorPickerViewModel).Color;
            OnPropertyChanged(nameof(Colors));
        }

        public async void Apply()
        {
            var kb = Ajazz.GetKeyboard();
            if (Colors.All(o => o.Value.ToString() == Colors[0].ToString()))
                kb.SetColor(Colors[0].ToDrawingClr());
            else
            {
                foreach(var k in Colors)
                {
                    kbColors.Add(new Tuple<Key, System.Drawing.Color>(k.Key, k.Value.ToDrawingClr()));
                }
                await Task.Run(() => kb.SetKey(kbColors));
            }
        }

        #region inpc
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    public class EnumKeyNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum.TryParse<Key>(value as string, out var r))
                return GetEnumDescription(r);

            return Avalonia.AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }

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

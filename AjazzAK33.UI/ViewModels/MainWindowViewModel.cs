using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AjazzAK33;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
                },
                Title = name
            };
            await cp.ShowDialog<string>((Avalonia.Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);
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
                var kbColors = new List<Tuple<Key, System.Drawing.Color>>();
                foreach (var k in Colors)
                {
                    kbColors.Add(new Tuple<Key, System.Drawing.Color>(k.Key, k.Value.ToDrawingClr()));
                }
                await Task.Run(() => kb.SetKey(kbColors));
            }
        }

        public async void Fill()
        {
            ColorPicker cp = new ColorPicker()
            {
                DataContext = new ColorPickerViewModel()
                {
                    Color = Color.FromRgb(255, 0, 0)
                },
                Title = "Fill"
            };
            await cp.ShowDialog((Avalonia.Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);
            var color = (cp.DataContext as ColorPickerViewModel).Color;
            SetAllKeys(color);
        }

        private void SetAllKeys(Color clr)
        {
            foreach(var k in (Key[])Enum.GetValues(typeof(Key)))
            {
                Colors[k] = clr;
            }
            OnPropertyChanged(nameof(Colors));
        }

        #region inpc
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}

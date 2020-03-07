using Avalonia.Collections;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AjazzAK33.UI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly Ajazz keyboard;
        public AvaloniaDictionary<Key, Color> KeyColors { get; set; }

        public MainWindowViewModel()
        {
            if (!Ajazz.TryGetKeyboard(out keyboard))
                throw new Exception("Keyboard not found");//TODO
            //colors = kb.getcolors; maybe?
            KeyColors = new AvaloniaDictionary<Key, Color>();
            KeyColors.CollectionChanged += (a,b) => OnPropertyChanged(nameof(KeyColors));
            Color c = Colors.Blue;
            foreach (var k in (Key[])Enum.GetValues(typeof(Key)))
            {
                KeyColors.Add(k, c);
                c = ColorUtils.ChangeHue(c, 1);
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
                    Color = KeyColors[r]
                },
                Title = name
            };
            await cp.ShowDialog<string>((Avalonia.Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);
            KeyColors[r] = (cp.DataContext as ColorPickerViewModel).Color;
            //OnPropertyChanged(nameof(KeyColors));
        }

        public async void Apply()
        {
            if (KeyColors.All(o => o.Value.ToUint32() == KeyColors[0].ToUint32()))
                keyboard.SetColor(KeyColors[0].ToDrawingClr());
            else
            {
                var kbColors = new List<Tuple<Key, System.Drawing.Color>>();
                foreach (var k in KeyColors)
                {
                    kbColors.Add(new Tuple<Key, System.Drawing.Color>(k.Key, k.Value.ToDrawingClr()));
                }
                await Task.Run(() => keyboard.SetKey(kbColors));
            }
        }

        public async void Fill()
        {
            SetAllKeys(await GetColorFromDialog("Fill"));
        }

        private void SetAllKeys(Color clr)
        {
            foreach(var k in (Key[])Enum.GetValues(typeof(Key)))
            {
                KeyColors[k] = clr;
            }
        }

        private async Task<Color> GetColorFromDialog(string title)
        {
            ColorPicker cp = new ColorPicker()
            {
                DataContext = new ColorPickerViewModel()
                {
                    Color = Color.FromRgb(255, 0, 0)
                },
                Title = title
            };
            await cp.ShowDialog((Avalonia.Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);
            return (cp.DataContext as ColorPickerViewModel).Color;
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

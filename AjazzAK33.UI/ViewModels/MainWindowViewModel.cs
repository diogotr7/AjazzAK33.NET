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
        #region Fields
        private Ajazz keyboard;
        private static readonly Key[] AllKeys = (Key[])Enum.GetValues(typeof(Key));
        #endregion

        #region Properties
        public AvaloniaDictionary<Key, Color> KeyColors { get; set; }

        private bool keyboardConnected;

        public bool KeyboardConnected
        {
            get => keyboardConnected;
            set
            {
                keyboardConnected = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            CheckForKeyboard();
            //colors = kb.getcolors; maybe?
            KeyColors = new AvaloniaDictionary<Key, Color>();
            KeyColors.CollectionChanged += (a, b) => OnPropertyChanged(nameof(KeyColors));

            FillKeyColors();
        }

        #region Commands
        public async void Click(string name)
        {
            if (!Enum.TryParse<Key>(name, out var r))
                return;

            KeyColors[r] = await GetColorFromDialog(name);
        }

        public async void Apply()
        {
            //if all colors are equal
            if (KeyColors.All(o => o.Value.ToUint32() == KeyColors[0].ToUint32()))
            {
                keyboard.SetColor(KeyColors[0].ToDrawingClr());
            }
            else
            {
                var newColors = KeyColors.Select(kc => new Tuple<Key, System.Drawing.Color>(kc.Key, kc.Value.ToDrawingClr()));
                await Task.Run(() => keyboard.SetKey(newColors));
            }
        }

        public async void Fill()
        {
            SetAllKeys(await GetColorFromDialog("Fill"));
        }

        public void CheckForKeyboard()
        {
            KeyboardConnected = Ajazz.TryGetKeyboard(out keyboard);
        }
        #endregion

        #region Methods
        private void FillKeyColors()
        {
            //should probably get colors from the keyboard here
            Color c = Colors.Blue;
            foreach (var k in AllKeys)
            {
                KeyColors.Add(k, c);
                c = ColorUtils.ChangeHue(c, 1.2);
            }
        }

        private void SetAllKeys(Color clr)
        {
            foreach (var k in AllKeys)
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
            return (cp.DataContext as ColorPickerViewModel)?.Color ?? default;
        }
        #endregion

        #region inpc
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}

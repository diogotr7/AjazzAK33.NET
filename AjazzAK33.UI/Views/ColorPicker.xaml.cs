using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AjazzAK33.UI
{
    public class ColorPicker : Window
    {
        public ColorPicker()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

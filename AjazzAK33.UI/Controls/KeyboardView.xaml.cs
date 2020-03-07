using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AjazzAK33.UI
{
    public class KeyboardView : UserControl
    {
        public KeyboardView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

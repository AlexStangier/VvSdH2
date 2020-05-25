using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace AvaloniaUI.Views
{
    public class LoginView : UserControl
    {
        public LoginView()
        {
            this.InitializeComponent();
            this.DataContext =
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

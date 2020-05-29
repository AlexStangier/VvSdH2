using Application;
using ApplicationShared;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaUI.Models;
using System;

namespace AvaloniaUI.Views
{
    public class LoginView : UserControl
    {
        public LoginView()
        {
            this.InitializeComponent();
            this.DataContext = new LoginViewModel() { Username = "Benutzername", Password = "Passwort"};
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void OnLoginClicked(object sender, RoutedEventArgs args)
        {
            var context = this.DataContext as LoginViewModel;
            //context.Info = $"Hello {context.Username} with the Password {context.Password}";

            IUser _user = new UserController();
            var verified = await _user.Login(context.Username, context.Password);

            if(verified)
            {
                context.Info = "Sie werden eingeloggt";
            }
            else
            {
                context.Info = "Der Benutzername oder Passwort ist falsch!";
            }
        }
    }
}

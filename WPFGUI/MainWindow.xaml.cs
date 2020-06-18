﻿using ApplicationShared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFGUI.ViewModels;
using WPFGUI.Views;
using Application;
using Core;

namespace WPFGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new NavigationViewModel();
            viewModel.SelectedViewModel = new LoginViewModel(viewModel);
            this.DataContext = viewModel;
        }

        private async void WindowClosing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Wollen Sie wirklich das Fenster schließen?", "Abmelden", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                IUser _user = new UserController();
                var logout = await _user.Logout(gUser.username);
                if (!logout)
                {
                    MessageBox.Show("Beim Abmelden ist ein Fehler aufgetreten", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
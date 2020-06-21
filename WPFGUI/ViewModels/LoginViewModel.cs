using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WPFGUI.ViewModels;
using WPFGUI.Interface;
using ApplicationShared;
using Application;
using Core;
using System.IO.Packaging;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WPFGUI.ViewModels
{
    class LoginViewModel : INotifyPropertyChanged
    {
        //=================View_aendern=========================
        public ICommand LandCommand { get; set; }

        private readonly NavigationViewModel _navigationViewModel;
        public LoginViewModel(NavigationViewModel navigationViewModel)
        {
            _navigationViewModel = navigationViewModel;
            LandCommand = new BaseCommand(OpenLand);
        }

        public LoginViewModel(NavigationViewModel navigationViewModel, string info)
        {
            _navigationViewModel = navigationViewModel;
            LandCommand = new BaseCommand(OpenLand);
            _info = info;
        }

        private async void OpenLand(object obj)   //Aktiv wenn Button gedrückt wird
        {
            IUser _user = new UserController();
            var passwordBox = obj as PasswordBox;

            var verified = await _user.Login(_username, passwordBox.Password);
            if (verified)
            {
                User _newUser = new User();
                _newUser.Username = _username; 
                _navigationViewModel.SelectedViewModel = new LandingPageViewModel(_navigationViewModel, _newUser);

                // start logout check thread + logout time in min
                AutoLogOff.CreateLogoutThread(_navigationViewModel, _newUser, 1);
            }
            else
            {
                Info = "Passwort oder Benutzername ist falsch!";
            }
            
        }


        //===============Context======================


        private string _username;
        private string _password;
        private string _info;

        public string Username
        {
            get { return _username; }
            set
            {
                if (value != _username)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }

            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (value != _password)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public string Info
        {
            get { return _info; }
            set
            {
                if (value != _info)
                {
                    _info = value;
                    OnPropertyChanged(nameof(Info));
                }
            }
        }

        public string Test
        {
            get { return _username + " " + _password; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

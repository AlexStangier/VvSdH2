using System;
using System.Collections.Generic;
using System.Text;
using WPFGUI.ViewModels;
using WPFGUI.Interface;
using System.Windows.Input;
using Core;
using ApplicationShared;
using Application;

namespace WPFGUI.ViewModels
{

    class ReservationViewModel
    {
        public ICommand LandCommand { get; set; }
        public ICommand LoginCommand { get; set; }

        private readonly NavigationViewModel _navigationViewModel;
        private User user;
        private string _loginas = "Eingeloggt als, ";
        public ReservationViewModel(NavigationViewModel navigationViewModel, User newUser)
        {
            _navigationViewModel = navigationViewModel;
            user = newUser;
            LandCommand = new BaseCommand(OpenLand);
            LoginCommand = new BaseCommand(OpenLogin);
        }

        private void OpenLand(object obj)
        {
            _navigationViewModel.SelectedViewModel = new LandingPageViewModel(_navigationViewModel, user);
        }

        private async void OpenLogin(object obj)
        {
            IUser _user = new UserController();
            var logout = await _user.Logout(user.Username);
            if (logout)
            {
                string info = "Sie wurden erfolgreich Ausgeloggt.";
                _navigationViewModel.SelectedViewModel = new LoginViewModel(_navigationViewModel, info);
            }
        }

        public string LoginAs
        {
            get
            {
                return (_loginas + user.Username);
            }
        }
    }
}

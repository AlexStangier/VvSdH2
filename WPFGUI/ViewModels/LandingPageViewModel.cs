using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WPFGUI.ViewModels;
using WPFGUI.Interface;
using ApplicationShared;
using Application;
using System.IO.Packaging;
using System.Windows.Documents;
using Core;
using System.Threading.Tasks;
using System.Windows;
using Renci.SshNet.Messages;

namespace WPFGUI.ViewModels
{
    class LandingPageViewModel
    {
        public ICommand ResvCommand { get; set; }
        public ICommand LoginCommand { get; set; }

        public ICommand CancelReservationCommand { get; set; }

        private readonly NavigationViewModel _navigationViewModel;
        private User user;
        private string _loginas = "Eingeloggt als, ";
        public LandingPageViewModel(NavigationViewModel navigationViewModel, User newUser)
        {
            _navigationViewModel = navigationViewModel;
            user = newUser;
            ResvCommand = new BaseCommand(OpenResv);
            LoginCommand = new BaseCommand(OpenLogin);
            CancelReservationCommand = new BaseCommand(CancelReservation);

            var controller = new BookingController();
            var result = Task.Run(() => controller.GetUserReservations(newUser));
            Reservations = new ObservableCollection<Reservation>(result.Result);
        }

        private void OpenResv(object obj)
        {
            _navigationViewModel.SelectedViewModel = new ReservationViewModel(_navigationViewModel, user);
        }

        private async void OpenLogin(object obj)
        {
            IUser _user = new UserController();
            var logout = await _user.Logout(user.Username);
            if (logout)
            {
                string info = "Sie wurden erfolgreich Ausgeloggt.";
                _navigationViewModel.SelectedViewModel = new LoginViewModel(_navigationViewModel, info);

                // close logoutThread
                AutoLogOff.GetToken.Cancel();
            }
            
        }
        
        public string LoginAs
        {
            get {
                return (_loginas+user.Username); 
                }
        }



        //Tabelle
        public ObservableCollection<Reservation> Reservations { get; }

        public async void CancelReservation(object obj)
        {
            var reservation = obj as Reservation;
            var controller = new BookingController();
            MessageBoxResult result = MessageBox.Show("Wollen Sie die Reservierung wirklich stonieren?", "Info", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    if (await controller.CancelReservation(user, reservation.ReservationId))
                    {
                        Reservations.Remove(reservation);
                        MessageBox.Show("Reservierung wurde erfolgreich stoniert.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim löschen der Reservierung", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }
        }
    }
}

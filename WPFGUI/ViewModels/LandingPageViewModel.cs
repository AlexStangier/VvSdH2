﻿using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WPFGUI.Interface;
using ApplicationShared;
using Application;
using Core;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace WPFGUI.ViewModels
{
    class LandingPageViewModel
    {
        public ICommand ResvCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand ListUpdateCommand { get; set; }
        public ICommand CancelReservationCommand { get; set; }
        public ICommand UpdateReservationCommand { get; set; }

        private readonly NavigationViewModel _navigationViewModel;
        private readonly User _user;
        private string _loginas = "Eingeloggt als, ";
        private List<Reservation> reservations;

        //Tabelle
        public ObservableCollection<Reservation> Reservations { get; }
        public bool OnlyShowOwnReservations { get; set; } = true;
        public bool DontShowPastReservations { get; set; } = true;

        public LandingPageViewModel(NavigationViewModel navigationViewModel, User newUser)
        {
            using var context = new ReservationContext();
            if (context.Database.CanConnect())
            {
                _navigationViewModel = navigationViewModel;
                _user = newUser;
                ResvCommand = new BaseCommand(OpenResv);
                LoginCommand = new BaseCommand(OpenLogin);
                ListUpdateCommand = new BaseCommand(UpdateReservationsList);
                CancelReservationCommand = new BaseCommand(CancelReservation);
                UpdateReservationCommand = new BaseCommand(UpdateReservation);
                Reservations = new ObservableCollection<Reservation>();
                UpdateReservationsList(null);
            }
            else
            {
                MessageBox.Show("Fehler beim Laden der Reservirungen", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                _user.Username = gUser.username;
            }
        }

        private void OpenResv(object obj)
        {
            _navigationViewModel.SelectedViewModel = new ReservationViewModel(_navigationViewModel, _user);
        }

        private async void OpenLogin(object obj)
        {
            IUser _user = new UserController();
            var logout = await _user.Logout(this._user.Username);
            if (logout)
            {
                string info = "Sie wurden erfolgreich Ausgeloggt.";
                _navigationViewModel.SelectedViewModel = new LoginViewModel(_navigationViewModel, info);

                // close logoutThread
                AutoLogOff.GetToken.Cancel();
            }
            else
            {
                MessageBox.Show("Fehler beim Abmelden.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }
        
        public string LoginAs
        {
            get => _loginas+_user.Username;
        }

        public async void CancelReservation(object obj)
        {
            var reservation = obj as Reservation;
            var controller = new BookingController();
            MessageBoxResult result = MessageBox.Show("Wollen Sie die Reservierung wirklich stonieren?", "Info", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    if (await controller.CancelReservation(_user, reservation.ReservationId))
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

        public async void UpdateReservation(object obj)
        {
            gReservation reservation = new gReservation(obj as Reservation, reservations);
            await using var context = new ReservationContext();
            var concreteUserA = context.Users
                .Where(x => x.Username.Equals(gUser.username))
                .Include(y => y.Rights)
                .FirstOrDefault();
            if (gUser.username.Equals(gReservation.reservation.User.Username) || concreteUserA.Rights.PrivilegeLevel > 3)
            {
                UpdateWindow updateWindow = new UpdateWindow();
                updateWindow.ShowDialog();
                UpdateReservationsList(this);
            }
            else
            {
                MessageBox.Show("Sie haben keine Berechtigung diese Reservierung zu ändern", "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
                
        }

        /// <summary>
        /// Sets the list source according to the checkbox parameters
        /// </summary>
        /// <param name="obj">ignored</param>
        public void UpdateReservationsList(object obj)
        {
            var controller = new BookingController();

            if (OnlyShowOwnReservations)
            {
                reservations = Task.Run(() => controller.GetUserReservations(_user)).Result;
            }
            else
            {
                reservations = Task.Run(() => controller.GetAllReservations()).Result;
            }

            if(DontShowPastReservations)
            {
                reservations = controller.RemovePastReservations(reservations);
            }

            Reservations.Clear();
            foreach(var reservation in reservations)
            {
                Reservations.Add(reservation);
            }
        }
    }
}

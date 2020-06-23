using Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Packaging;
using System.Text;
using System.Windows.Input;
using WPFGUI.Interface;
using Application;
using System.Windows;

namespace WPFGUI.ViewModels
{
    class UpdateWindowViewModel
    {
        private DateTime _selectedDate;
        private string _selectedTimeSlot;
        private Reservation _reservation;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
            }
        }

        public string SelectedTimeSlot
        {
            get => _selectedTimeSlot;
            set
            {
                _selectedTimeSlot = value;  
            }
        }

        public ICommand updateResCommand { get; set; }
        public ICommand cancelCommand { get; set; }
     
        public UpdateWindowViewModel()
        {
            _reservation = gReservation.reservation;
            updateResCommand = new BaseCommand(UpdateReservation);
            cancelCommand = new BaseCommand(Cancel);
            SelectedDate = gReservation.reservation.StartTime.Date;
        }


        public async void UpdateReservation(object obj)
        {
            var controller = new BookingController();
            var window = obj as Window;

            int timeslot = SelectedTimeSlot switch
            {
                "slot1" => 1,
                "slot2" => 2,
                "slot3" => 3,
                "slot4" => 4,
                "slot5" => 5,
                _ => -1
            };
            if(timeslot != -1) 
            {
                if (await controller.UpdateReservation(_reservation, SelectedDate, timeslot))
                {
                    MessageBox.Show("Reservierung erfolgreich geändert.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    window.Close();
                }
                else
                {
                    MessageBox.Show("Reservierung konnte nicht geändert werden.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Sie müssen ein Timeslot auswählen", "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        } 

        public void Cancel(object obj)
        {
            var window = obj as Window;
            window.Close();
        }
    }
}

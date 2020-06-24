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
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
                "slot6" => 6,
                _ => -1
            };
            if(timeslot != -1) 
            {
                await using var context = new ReservationContext();
                var reservations = context.Reservations.Where(x => x.ReservationId != 0).Include(y => y.Room);
                if (reservations.Any(x => x.Room.RoomId == _reservation.Room.RoomId && x.StartTime == getTimestampsFromTimeslot(timeslot, SelectedDate)))
                {
                    MessageBox.Show("Reservierung konnte nicht geändert werden, da sie mit einer anderen Reservierung kollidiert.","Fehler",MessageBoxButton.OK,MessageBoxImage.Error);
                }
                else
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

        private DateTime getTimestampsFromTimeslot(int slot, DateTime selectedDay)
        {
            var toReturn = new List<DateTime>();
            var newStartDate = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day);
            var newEndDate = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day);

            switch (slot)
            {
                case 1:
                    newStartDate = newStartDate.AddHours(8).AddMinutes(0);
                    break;
                case 2:
                    newStartDate = newStartDate.AddHours(9).AddMinutes(45);
                    break;
                case 3:
                    newStartDate = newStartDate.AddHours(11).AddMinutes(35);
                    break;
                case 4:
                    newStartDate = newStartDate.AddHours(14).AddMinutes(0);
                    break;
                case 5:
                    newStartDate = newStartDate.AddHours(15).AddMinutes(45);
                    break;
                case 6:
                    newStartDate.AddHours(17).AddMinutes(30);
                    break;
                default:
                    throw new Exception("Slot index was out of Range");
            }

            return newStartDate;
        }
    }
}

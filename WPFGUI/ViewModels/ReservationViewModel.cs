using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WPFGUI.ViewModels;
using WPFGUI.Interface;
using System.Windows.Input;
using Core;
using ApplicationShared;
using Application;
using System.Windows.Ink;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using Attribute = Core.Attribute;
using System.Globalization;

namespace WPFGUI.ViewModels
{
    enum RoomColor
    {
        Initial,
        Free,
        Filtered,
        Booked
    }

    class RoomStruct : INotifyPropertyChanged
    {
        public const string Initial = "#FFFFFF";
        public const string Available = "#FF5CB85C";
        public const string Filtered = "#FFF0AD4E";
        public const string Booked = "#FFD9534F";

        public event PropertyChangedEventHandler PropertyChanged;

        private string _background;
        private string _number;
        private string _title;
        private string _email;

        public string Background {
            get => _background;
            set
            {
                _background = value;
                NotifyPropertyChanged("Background");
            } 
        }

        public string Number {
            get => _number;
            set
            {
                _number = value;
                NotifyPropertyChanged("Number");
            }
        }

        public string Title { 
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public string Email { 
            get => _email;
            set 
            {
                _email = value;
                NotifyPropertyChanged("Email");
            }
        }

        public RoomStruct(string background, string number, string title, string email)
        {
            Background = background;
            Number = number;
            Title = title;
            Email = email;
        }

        public RoomStruct(string number)
        {
            Background = Available;
            Number = number;
            Title = "";
            Email = "";
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class ReservationViewModel
    {
        private string _selectedBuilding;
        private int _selectedFloor;
        private string _selectedTimeSlot;
        private DateTime _selectedDate;
        private int _selectedRoomSize;

        private bool _isAirConditioningChecked;
        private bool _isComputersChecked;
        private bool _isPowerOutletsChecked;
        private bool _isPresenterChecked;

        public string SelectedBuilding {
            get => _selectedBuilding;
            set 
            {
                if (value.Contains("Gebäude A"))
                {
                    _selectedBuilding = "A";
                }
                else if (value.Contains("Gebäude B"))
                {
                    _selectedBuilding = "B";
                }
                UpdateRoomStatus();
            } 
        }

        public string GetFloor
        {
            get => "Stockwerk: " + _selectedFloor;
        }

        public int IncFloor
        {
            set => _selectedFloor++;
        }

        public ICommand DecFloor
        {
            set => _selectedFloor--;
        }

        public string SelectedTimeSlot
        {
            get => _selectedTimeSlot;
            set
            {
                _selectedTimeSlot = value;
                UpdateRoomStatus();
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                UpdateRoomStatus();
            }
        }

        public string SelectedRoomSize
        {
            get => "";
            set
            {
                switch (value)
                {
                    case "roomSize1":
                        _selectedRoomSize = 30;
                        break;
                    case "roomSize2":
                        _selectedRoomSize = 50;
                        break;
                    case "roomSize3":
                        _selectedRoomSize = 100;
                        break;
                }
                UpdateRoomStatus();
            }
        }

        public RoomStruct[] Rooms { get; set; }


        public bool IsAirConditioningChecked
        {
            get => _isAirConditioningChecked;
            set
            {
                _isAirConditioningChecked = value;
                UpdateRoomStatus();
            }
        }

        public bool IsComputersChecked
        {
            get => _isComputersChecked;
            set
            {
                _isComputersChecked = value;
                UpdateRoomStatus();
            }
        }

        public bool IsPowerOutletsChecked
        {
            get => _isPowerOutletsChecked;
            set
            {
                _isPowerOutletsChecked = value;
                UpdateRoomStatus();
            }
        }

        public bool IsPresenterChecked
        {
            get => _isPresenterChecked;
            set
            {
                _isPresenterChecked = value;
                UpdateRoomStatus();
            }
        }

        public ICommand LandCommand { get; set; }
        public ICommand LoginCommand { get; set; }

        private readonly NavigationViewModel _navigationViewModel;
        private User user;
        private string _loginas = "Eingeloggt als, ";
        public ReservationViewModel(NavigationViewModel navigationViewModel, User newUser)
        {
            Rooms = new[] {
                new RoomStruct("A 100"),
                new RoomStruct("A 101"),
                new RoomStruct("A 102"),
                new RoomStruct("A 103"),
                new RoomStruct("A 104"),
                new RoomStruct("A 105"),
                new RoomStruct("A 106"),
                new RoomStruct("A 107"),
                new RoomStruct("A 108"),
                new RoomStruct("A 109"),
                new RoomStruct("A 110"),
                new RoomStruct("A 111"),
                new RoomStruct("A 112"),
                new RoomStruct("A 113"),
            };

            _navigationViewModel = navigationViewModel;
            _selectedBuilding = "A";
            _selectedFloor = 1;
            user = newUser;
            LandCommand = new BaseCommand(OpenLand);
            LoginCommand = new BaseCommand(OpenLogin);

            SelectedDate = DateTime.Now;
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

                // close logoutThread
                AutoLogOff.GetToken.Cancel();
            }
        }

        public string LoginAs
        {
            get => _loginas + user.Username;
        }

        private async void UpdateRoomStatus()
        {
            IRoom _room = new RoomController();
            List<Room> rooms = await _room.GetCurrentStatusForFloor(_selectedBuilding, _selectedFloor);

            Attribute attr = new Attribute
            {
                AirConditioning = _isAirConditioningChecked,
                Computers = _isComputersChecked,
                PowerOutlets = _isPowerOutletsChecked,
                Presenter = _isPresenterChecked
            };

            List<Room> filteredRooms = await _room.Filter(rooms, _selectedRoomSize, attr);

            for (int i = 0; i < Rooms.Length; i++)
            {
                if (i >= rooms.Count)
                {
                    // Floor has not enough rooms
                    Rooms[i].Background = RoomStruct.Initial;
                    Rooms[i].Number = "";
                    Rooms[i].Email = "";
                    Rooms[i].Title = "";
                    continue;
                }

                Room r = rooms[i];
                Rooms[i].Number = $"{r.Building} {r.RoomNr}";
                if (!filteredRooms.Contains(r))
                {
                    Rooms[i].Background = RoomStruct.Filtered;
                }
                else
                {
                    Rooms[i].Background = RoomStruct.Available;
                }

                if (SelectedTimeSlot != null && SelectedDate != null)
                {
                    DateTime timestamp = SelectedDate;

                    TimeSpan ts;
                    switch (SelectedTimeSlot)
                    {
                        case "slot1":
                            ts = new TimeSpan(8, 0, 0);
                            timestamp = timestamp.Date + ts;
                            break;
                        case "slot2":
                            ts = new TimeSpan(9, 45, 0);
                            timestamp = timestamp.Date + ts;
                            break;
                        case "slot3":
                            ts = new TimeSpan(11, 35, 0);
                            timestamp = timestamp.Date + ts;
                            break;
                        case "slot4":
                            ts = new TimeSpan(14, 0, 0);
                            timestamp = timestamp.Date + ts;
                            break;
                        case "slot5":
                            ts = new TimeSpan(15, 45, 0);
                            timestamp = timestamp.Date + ts;
                            break;
                    }

                    (string Username, string RightsName) = await _room.GetReservationForRoom(r, timestamp);

                    if (Username.Length > 0)
                    {
                        Rooms[i].Background = RoomStruct.Booked;
                        Rooms[i].Email = Username;
                        Rooms[i].Title = RightsName;
                    }
                    else
                    {
                        Rooms[i].Email = "";
                        Rooms[i].Title = "";
                    }
                }
            }


            // TODO: Räume die für den gewünschten Zeitraum belegt sind rot einfärben. (Geht erst nach Auswahl des Slots)
            // TODO: Zusätzlich noch die Daten des Reservierers anzeigen.
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Drawing;

namespace AvaloniaUI.Models
{
    class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _info;

        public string Username
        {
            get { return _username; }
            set 
            { 
                if(value != _username)
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
                if(value != _password)
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
                if(value != _info)
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

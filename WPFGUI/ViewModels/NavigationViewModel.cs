using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WPFGUI.ViewModels
{
    class NavigationViewModel : INotifyPropertyChanged
    {
        private object selectedViewModel;

        public object SelectedViewModel
        {
            get
            {
                return selectedViewModel;
            }
            set
            {
                selectedViewModel = value;
                OnPropertyChanged("SelectedViewModel");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

    }
}

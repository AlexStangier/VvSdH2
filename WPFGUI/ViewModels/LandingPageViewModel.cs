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

namespace WPFGUI.ViewModels
{
    class LandingPageViewModel
    {
        public ICommand ResvCommand { get; set; }

        private readonly NavigationViewModel _navigationViewModel;
        public LandingPageViewModel(NavigationViewModel navigationViewModel)
        {
            _navigationViewModel = navigationViewModel;
            ResvCommand = new BaseCommand(OpenResv);
        }

        private void OpenResv(object obj)
        {
            _navigationViewModel.SelectedViewModel = new ReservationViewModel();
        }
    }
}

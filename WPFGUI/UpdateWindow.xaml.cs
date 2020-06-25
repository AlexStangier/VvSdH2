using System.Windows;
using WPFGUI.ViewModels;

namespace WPFGUI
{
    /// <summary>
    /// Interaktionslogik für UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private UpdateWindowViewModel updateWindowViewModel;
        public UpdateWindow()
        {
            InitializeComponent();
            this.updateWindowViewModel = new UpdateWindowViewModel();
            this.DataContext = updateWindowViewModel;
        }
    }
}

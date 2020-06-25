using ApplicationShared;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using WPFGUI.ViewModels;
using Application;
using Core;

namespace WPFGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new NavigationViewModel();
            viewModel.SelectedViewModel = new LoginViewModel(viewModel);
            this.DataContext = viewModel;
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Wollen Sie wirklich das Fenster schließen?", "Abmelden", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                using var context = new ReservationContext();
                if (context.Database.CanConnect())
                {
                    IUser _user = new UserController();
                    var logout = Task.Run(async () => await _user.Logout(gUser.username)).Result;

                    if (!logout && (gUser.username != null))
                    {
                        MessageBox.Show("Beim Abmelden ist ein Fehler aufgetreten", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                    }
                }
               
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}

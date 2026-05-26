using Strela_Sanatorii.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Strela_Sanatorii.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();
            LoadDefaultPage();
        }

        private void LoadDefaultPage()
        {
            MainFrame.Navigate(new BookingGridPage());
        }

        private void Nav_Dashboard(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BookingGridPage());
        }

        private void Nav_Clients(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GuestsPage());  // БЫЛО: ClientsPage
        }

        private void Nav_Rooms(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RoomsPage());
        }

        private void Nav_Shifts(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ShiftsPage());
        }

        private void Nav_Reports(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReportsPage());
        }

        private void Nav_Prescriptions(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PrescriptionsPage());
        }

        private void Nav_NurseSchedule(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new NurseSchedulePage());
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти из системы?",
                "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                new LoginWindow().Show();
                this.Close();
            }
        }
    }
}

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
    /// Логика взаимодействия для SuperuserMainWindow.xaml
    /// </summary>
    public partial class SuperuserMainWindow : Window
    {
        public SuperuserMainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new BookingGridPage());
        }

        // Администрирование
        private void Nav_Users(object sender, RoutedEventArgs e) => MainFrame.Navigate(new UsersPage());
        private void Nav_Rooms(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RoomsPage());
        private void Nav_Categories(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RoomCategoriesPage());
        private void Nav_Shifts(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ShiftsPage());

        // Гости
        private void Nav_Dashboard(object sender, RoutedEventArgs e) => MainFrame.Navigate(new BookingGridPage());
        private void Nav_Guests(object sender, RoutedEventArgs e) => MainFrame.Navigate(new GuestsPage());
        private void Nav_Reports(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ReportsPage());

        // Услуги и лечение
        private void Nav_Services(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ServicesListPage());
        private void Nav_ServiceJournal(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ServiceJournalPage());
        private void Nav_Prescriptions(object sender, RoutedEventArgs e) => MainFrame.Navigate(new PrescriptionsPage());
        private void Nav_NurseSchedule(object sender, RoutedEventArgs e) => MainFrame.Navigate(new NurseSchedulePage());

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выйти из системы?", "Выход",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                new LoginWindow().Show();
                this.Close();
            }
        }
    }
}

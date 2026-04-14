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
    /// Логика взаимодействия для ServicesMainWindow.xaml
    /// </summary>
    public partial class ServicesMainWindow : Window
    {
        public ServicesMainWindow()
        {
            InitializeComponent();

            // При открытии сразу открываем страницу "Запись на услуги"
            MainFrame.Navigate(new ServiceRecordsPage());
        }

        private void Nav_Records(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ServiceRecordsPage());
        }

        private void Nav_Journal(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ServiceJournalPage());
        }

        private void Nav_Services(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ServicesListPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из системы?",
                                       "Выход",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                new LoginWindow().Show();
                this.Close();
            }
        }
    }
}

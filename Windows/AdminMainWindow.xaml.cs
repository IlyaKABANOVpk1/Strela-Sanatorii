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

            // Загружаем "Шахматку" при открытии окна
            LoadDefaultPage();
        }

        /// <summary>
        /// Страница по умолчанию при запуске
        /// </summary>
        private void LoadDefaultPage()
        {
            MainFrame.Navigate(new BookingGridPage());
        }

        // ====================== НАВИГАЦИЯ ======================

        private void Nav_Dashboard(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BookingGridPage());
        }

        private void Nav_Clients(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void Nav_Shifts(object sender, RoutedEventArgs e)
        {
            // Если у тебя ещё нет страницы ShiftsPage — можно пока показывать заглушку
            // Или создай пустую страницу ShiftsPage.xaml
            MessageBox.Show("Страница «Смены» находится в разработке",
                          "Информация",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);

            // Пример, если страница уже есть:
            // MainFrame.Navigate(new ShiftsPage());
        }

        private void Nav_Reports(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел «Отчёты» находится в разработке.\n\n" +
                          "Здесь будут отчёты по загрузке номеров, оплате услуг и т.д.",
                          "Отчёты",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);

            // Когда сделаешь страницу:
            // MainFrame.Navigate(new ReportsPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти из системы?",
                                       "Выход",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Возвращаемся на окно логина
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                this.Close();
            }
        }

       
    }
}

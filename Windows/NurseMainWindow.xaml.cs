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
    /// Логика взаимодействия для NurseMainWindow.xaml
    /// </summary>
    public partial class NurseMainWindow : Window
    {
        public NurseMainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new NurseSchedulePage());
        }

        private void Nav_Schedule(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GuestServiceSchedulePage());
        }

        private void Nav_Execute(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new NurseSchedulePage());
        }

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

using Strela_Sanatorii.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Strela_Sanatorii.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            using (var db = new ApplicationContext())
            {
                ClientsGrid.ItemsSource = db.Clients.ToList();
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();

            using (var db = new ApplicationContext())
            {
                var result = db.Clients
                    .Where(c =>
                        (c.FullName != null && c.FullName.ToLower().Contains(search)) ||
                        (c.PersonnelNumber != null && c.PersonnelNumber.ToLower().Contains(search)))
                    .ToList();

                ClientsGrid.ItemsSource = result;
            }
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            var window = new BookingWindow(null, null); // используем как форму ввода
            window.ShowDialog();

            LoadClients();
        }
    }
}

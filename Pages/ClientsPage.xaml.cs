using Strela_Sanatorii.Models.Accommodation_tables;
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
            var window = new ClientEditWindow();
            if (window.ShowDialog() == true)
            {
                LoadClients();
            }
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client client)
            {
                var window = new ClientEditWindow(client);
                if (window.ShowDialog() == true)
                {
                    LoadClients();
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.");
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client client)
            {
                var result = MessageBox.Show($"Удалить клиента {client.FullName}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new ApplicationContext())
                    {
                        var c = db.Clients.Find(client.Id);
                        if (c != null)
                        {
                            // Проверка на связанные записи
                            bool hasBookings = db.Bookings.Any(b => b.ClientId == c.Id);
                            bool hasAppointments = db.ServiceAppointments.Any(a => a.ClientId == c.Id);

                            if (hasBookings || hasAppointments)
                            {
                                MessageBox.Show("Невозможно удалить клиента, так как есть связанные брони или записи на услуги.");
                                return;
                            }

                            db.Clients.Remove(c);
                            db.SaveChanges();
                        }
                    }
                    LoadClients();
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления.");
            }
        }
    }
}

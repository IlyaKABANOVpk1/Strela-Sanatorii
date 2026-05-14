using Strela_Sanatorii.Models.Accommodation_tables;
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
    /// Логика взаимодействия для BookingWindow.xaml
    /// </summary>
    public partial class BookingWindow : Window
    {
        private Room _room;
        private Shift _shift;

        public BookingWindow(Room room, Shift shift)
        {
            InitializeComponent();

            if (room == null || shift == null)
            {
                MessageBox.Show("Ошибка: номер или смена не переданы в окно заселения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            _room = room;
            _shift = shift;

            txtRoom.Text = $"Номер: {room.RoomNumber} | {room.Category}";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClients.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента!");
                return;
            }

            var selectedClient = cmbClients.SelectedItem as Client;

            using (var db = new ApplicationContext())
            {
                var existingBooking = db.Bookings
                    .FirstOrDefault(b => b.RoomId == _room.Id && b.ShiftId == _shift.Id);

                if (existingBooking != null)
                {
                    MessageBox.Show($"Номер {_room.RoomNumber} уже занят для выбранной смены.");
                    return;
                }

                var booking = new Booking
                {
                    ClientId = selectedClient.Id,
                    RoomId = _room.Id,
                    ShiftId = _shift.Id,
                    CreatedAt = DateTime.UtcNow
                };

                db.Bookings.Add(booking);
                db.SaveChanges();
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtClientSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtClientSearch.Text?.Trim().ToLower() ?? "";

            using (var db = new ApplicationContext())
            {
                var filtered = db.Clients
                    .Where(c => string.IsNullOrEmpty(search) ||
                                c.FullName.ToLower().Contains(search) ||
                                (c.PersonnelNumber != null && c.PersonnelNumber.ToLower().Contains(search)))
                    .OrderBy(c => c.FullName)
                    .ToList();

                cmbClients.ItemsSource = filtered;
                cmbClients.IsDropDownOpen = filtered.Count > 0 && !string.IsNullOrEmpty(search);
            }
        }

        private void cmbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbClients.SelectedItem is Client client)
            {
                txtPhone.Text = client.Phone ?? "";
                txtTabNumber.Text = client.PersonnelNumber ?? "";
            }
        }
    }
}

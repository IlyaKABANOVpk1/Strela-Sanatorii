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
                // Создаём новую бронь
                var booking = new Booking
                {
                    ClientId = selectedClient.Id,
                    RoomId = _room.Id,    // _room передаётся через конструктор окна
                    ShiftId = _shift.Id,  // _shift передаётся через конструктор окна
                    CreatedAt = DateTime.Now
                };

                db.Bookings.Add(booking);
                db.SaveChanges();
            }

            this.DialogResult = true; // чтобы окно вернуло результат
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmbClients_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new ApplicationContext())
            {
                var clients = db.Clients
                                .OrderBy(c => c.FullName)
                                .ToList();

                cmbClients.ItemsSource = clients;
                cmbClients.DisplayMemberPath = "FullName"; // что показываем
                cmbClients.SelectedValuePath = "Id";       // что возвращаем
            }
        }

        private void cmbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbClients.SelectedItem is Client client)
            {
                txtPhone.Text = client.Phone;
                txtTabNumber.Text = client.PersonnelNumber;
            }
        }

        private void cmbClients_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = cmbClients.Text.ToLower();
            using (var db = new ApplicationContext())
            {
                var clients = db.Clients
                                .Where(c => c.FullName.ToLower().Contains(search))
                                .OrderBy(c => c.FullName)
                                .ToList();

                cmbClients.ItemsSource = clients;
                cmbClients.IsDropDownOpen = true; // чтобы список показывался при вводе
            }
        }
    }
}

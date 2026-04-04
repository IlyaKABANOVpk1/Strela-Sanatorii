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
            string fio = txtFio.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string pers = txtTabNumber.Text.Trim();

            if (string.IsNullOrEmpty(fio))
            {
                MessageBox.Show("Введите ФИО");
                return;
            }

            using (var db = new ApplicationContext())
            {
                // создаём клиента
                var client = new Client
                {
                    FullName = fio,
                    Phone = phone,
                    PersonnelNumber = pers
                };

                db.Clients.Add(client);
                db.SaveChanges();

                // создаём бронь
                var booking = new Booking
                {
                    ClientId = client.Id,
                    RoomId = _room.Id,
                    ShiftId = _shift.Id,
                    CreatedAt = DateTime.Now
                };

                db.Bookings.Add(booking);
                db.SaveChanges();
            }

            MessageBox.Show("Гость успешно заселён");

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

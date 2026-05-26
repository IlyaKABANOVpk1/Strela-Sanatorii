using Microsoft.EntityFrameworkCore;
using Strela_Sanatorii.Models.Additional_service_tables;
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
    /// Логика взаимодействия для BookingProgressWindow.xaml
    /// </summary>
    public partial class BookingProgressWindow : Window
    {
        public BookingProgressWindow(int bookingId)
        {
            InitializeComponent();
            LoadProgress(bookingId);
        }

        private void LoadProgress(int bookingId)
        {
            using (var db = new ApplicationContext())
            {
                var booking = db.Bookings
                    .Include(b => b.Guest)
                    .Include(b => b.Room)
                    .Include(b => b.Shift)
                    .Include(b => b.ServicePackage)
                    .FirstOrDefault(b => b.Id == bookingId);

                if (booking == null) return;

                txtGuestName.Text = booking.Guest.FullName;
                txtRoomInfo.Text = $"Номер: {booking.Room.RoomNumber}";
                txtShiftInfo.Text = $"Смена: {booking.Shift.Name}";

                // Считаем прогресс
                var totalServices = db.GuestServiceSchedules
                    .Count(s => s.BookingId == bookingId);

                var completedServices = db.GuestServiceSchedules
                    .Count(s => s.BookingId == bookingId && s.Status == ScheduleStatus.Completed);

                var percent = totalServices > 0 ? (completedServices * 100 / totalServices) : 0;

                progressServices.Value = percent;
                txtProgressPercent.Text = $"{percent}%";
                txtCompletedCount.Text = $"{completedServices} из {totalServices}";
                txtRemainingCount.Text = $"{totalServices - completedServices}";
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

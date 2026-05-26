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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Strela_Sanatorii.Pages
{
    /// <summary>
    /// Логика взаимодействия для GuestServiceSchedulePage.xaml
    /// </summary>
    public partial class GuestServiceSchedulePage : Page
    {
        public GuestServiceSchedulePage()
        {
            InitializeComponent();
            LoadGuests();
            LoadShifts();
        }

        private void LoadGuests()
        {
            using (var db = new ApplicationContext())
            {
                cmbGuest.ItemsSource = db.Guests.OrderBy(g => g.LastName).ToList();
            }
        }

        private void LoadShifts()
        {
            using (var db = new ApplicationContext())
            {
                cmbShift.ItemsSource = db.Shifts.OrderBy(s => s.StartDate).ToList();
            }
        }

        private void Guest_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadSchedule();
        }

        private void Shift_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadSchedule();
        }

        private void LoadSchedule()
        {
            if (cmbGuest.SelectedItem == null) return;

            var guestId = (int)cmbGuest.SelectedValue;

            using (var db = new ApplicationContext())
            {
                var query = db.GuestServiceSchedules
                    .Include(s => s.Service)
                    .Where(s => s.GuestId == guestId)
                    .AsQueryable();

                if (cmbShift.SelectedItem != null)
                {
                    var shiftId = (int)cmbShift.SelectedValue;
                    query = query.Where(s => s.BookingId != null && db.Bookings.Any(b => b.Id == s.BookingId && b.ShiftId == shiftId));
                }

                ScheduleGrid.ItemsSource = query
                    .OrderBy(s => s.ScheduledDate)
                    .ThenBy(s => s.ScheduledTime)
                    .ToList();
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var schedule = (sender as Button).DataContext as GuestServiceSchedule;
            if (schedule == null || schedule.Status != ScheduleStatus.Assigned) return;

            using (var db = new ApplicationContext())
            {
                var s = db.GuestServiceSchedules.Find(schedule.Id);
                if (s != null)
                {
                    s.Status = ScheduleStatus.Confirmed;
                    s.ConfirmedAt = DateTime.Now;
                    db.SaveChanges();
                }
            }
            LoadSchedule();
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            var schedule = (sender as Button).DataContext as GuestServiceSchedule;
            if (schedule == null || schedule.Status != ScheduleStatus.Confirmed) return;

            using (var db = new ApplicationContext())
            {
                var s = db.GuestServiceSchedules.Find(schedule.Id);
                if (s != null)
                {
                    s.Status = ScheduleStatus.Completed;
                    s.Notes = "Процедура выполнена"; // Можно открыть диалог для ввода
                    db.SaveChanges();
                }
            }
            LoadSchedule();
        }
    }
}

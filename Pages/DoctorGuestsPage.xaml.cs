using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для DoctorGuestsPage.xaml
    /// </summary>
    public partial class DoctorGuestsPage : Page
    {
        private List<GuestViewModel> _allGuests;

        public DoctorGuestsPage()
        {
            InitializeComponent();
            LoadShifts();
            LoadGuests();
        }

        private void LoadShifts()
        {
            using (var db = new ApplicationContext())
            {
                cmbShift.ItemsSource = db.Shifts.OrderBy(s => s.StartDate).ToList();
            }
        }

        private void LoadGuests()
        {
            using (var db = new ApplicationContext())
            {
                _allGuests = db.Bookings
                    .Include(b => b.Guest)
                    .Include(b => b.Room)
                    .Include(b => b.Shift)
                    .Where(b => b.Status != BookingStatus.CheckedOut)
                    .Select(b => new GuestViewModel
                    {
                        GuestId = b.GuestId,
                        LastName = b.Guest.LastName,
                        FirstName = b.Guest.FirstName,
                        MiddleName = b.Guest.MiddleName,
                        RoomNumber = b.Room.RoomNumber,
                        PersonnelNumber = b.Guest.PersonnelNumber,
                        BirthDate = b.Guest.BirthDate,
                        ShiftId = b.ShiftId,
                        BookingId = b.Id
                    })
                    .OrderBy(g => g.LastName)
                    .ToList();

                GuestsGrid.ItemsSource = _allGuests;
            }
        }

        private void Shift_Changed(object sender, SelectionChangedEventArgs e)
        {
            FilterGuests();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterGuests();
        }

        private void FilterGuests()
        {
            var filtered = _allGuests.AsEnumerable();

            if (cmbShift.SelectedItem is Shift shift)
            {
                filtered = filtered.Where(g => g.ShiftId == shift.Id);
            }

            string search = txtSearch.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(g =>
                    (g.LastName != null && g.LastName.ToLower().Contains(search)) ||
                    (g.FirstName != null && g.FirstName.ToLower().Contains(search)) ||
                    (g.PersonnelNumber != null && g.PersonnelNumber.ToLower().Contains(search)));
            }

            GuestsGrid.ItemsSource = filtered.ToList();
        }

        private void Prescribe_Click(object sender, RoutedEventArgs e)
        {
            var guest = (sender as Button).DataContext as GuestViewModel;
            if (guest == null) return;

            var window = new PrescriptionEditWindow(guest.GuestId, guest.BookingId);
            if (window.ShowDialog() == true)
            {
                MessageBox.Show("Назначение создано.");
            }
        }
    }

    public class GuestViewModel
    {
        public int GuestId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string RoomNumber { get; set; }
        public string PersonnelNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public int ShiftId { get; set; }
        public int BookingId { get; set; }
    }
}

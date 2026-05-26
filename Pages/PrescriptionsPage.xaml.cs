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
    /// Логика взаимодействия для PrescriptionsPage.xaml
    /// </summary>
    public partial class PrescriptionsPage : Page
    {
        private List<PrescriptionViewModel> _allPrescriptions;

        public PrescriptionsPage()
        {
            InitializeComponent();
            LoadGuests();
            LoadPrescriptions();
        }

        private void LoadGuests()
        {
            using (var db = new ApplicationContext())
            {
                cmbGuest.ItemsSource = db.Guests
                    .OrderBy(g => g.LastName)
                    .ToList();
            }
        }

        private void LoadPrescriptions()
        {
            using (var db = new ApplicationContext())
            {
                _allPrescriptions = db.MedicalPrescriptions
                    .Include(p => p.Guest)
                    .Include(p => p.Doctor)
                    .Select(p => new PrescriptionViewModel
                    {
                        Id = p.Id,
                        GuestName = p.Guest.FullName,
                        Diagnosis = p.Diagnosis,
                        Medications = p.Medications,
                        Procedures = p.Procedures,
                        Recommendations = p.Recommendations,
                        CreatedAt = p.CreatedAt,
                        Status = p.Status.ToString(),
                        DoctorName = p.Doctor.FullName,
                        GuestId = p.GuestId,
                        BookingId = p.BookingId
                    })
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();

                PrescriptionsGrid.ItemsSource = _allPrescriptions;
            }
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            var filtered = _allPrescriptions.AsEnumerable();

            if (cmbGuest.SelectedItem is Guest guest)
            {
                filtered = filtered.Where(p => p.GuestId == guest.Id);
            }

            if (cmbStatus.SelectedItem is ComboBoxItem item && item.Content.ToString() != "Все")
            {
                string status = item.Content.ToString();
                filtered = filtered.Where(p => p.Status == status);
            }

            PrescriptionsGrid.ItemsSource = filtered.ToList();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var prescription = (sender as Button).DataContext as PrescriptionViewModel;
            if (prescription == null) return;

            var window = new PrescriptionEditWindow(prescription.GuestId, prescription.BookingId, prescription.Id);
            if (window.ShowDialog() == true)
            {
                LoadPrescriptions();
            }
        }
    }

    public class PrescriptionViewModel
    {
        public int Id { get; set; }
        public string GuestName { get; set; }
        public string Diagnosis { get; set; }
        public string Medications { get; set; }
        public string Procedures { get; set; }
        public string Recommendations { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string DoctorName { get; set; }
        public int GuestId { get; set; }
        public int BookingId { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using Strela_Sanatorii.Models.Accommodation_tables;
using Strela_Sanatorii.Models.Medical_tables;
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
    /// Логика взаимодействия для PrescriptionEditWindow.xaml
    /// </summary>
    public partial class PrescriptionEditWindow : Window
    {
        private int _guestId;
        private int _bookingId;
        private int? _prescriptionId;
        private MedicalPrescription _prescription;
        private Guest _selectedGuest;

        // Создание нового назначения
        public PrescriptionEditWindow(int guestId, int bookingId)
        {
            InitializeComponent();
            _guestId = guestId;
            _bookingId = bookingId;
            _prescriptionId = null;
            LoadGuestInfo();
        }

        // Редактирование существующего
        public PrescriptionEditWindow(int guestId, int bookingId, int prescriptionId)
        {
            InitializeComponent();
            _guestId = guestId;
            _bookingId = bookingId;
            _prescriptionId = prescriptionId;
            LoadGuestInfo();
            LoadPrescription();
        }

        private void LoadGuestInfo()
        {
            using (var db = new ApplicationContext())
            {
                var guest = db.Guests.Find(_guestId);
                var booking = db.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefault(b => b.Id == _bookingId);

                if (guest != null && booking != null)
                {
                    txtGuestInfo.Text = $"{guest.FullName} | Номер: {booking.Room.RoomNumber} | Табельный: {guest.PersonnelNumber}";
                    _selectedGuest = guest;  // <-- ДОБАВЛЕНО
                }
            }
        }

        private void LoadPrescription()
        {
            if (!_prescriptionId.HasValue) return;

            using (var db = new ApplicationContext())
            {
                _prescription = db.MedicalPrescriptions.Find(_prescriptionId.Value);
                if (_prescription != null)
                {
                    txtTitle.Text = "Редактирование назначения";
                    txtDiagnosis.Text = _prescription.Diagnosis;
                    txtMedications.Text = _prescription.Medications;
                    txtProcedures.Text = _prescription.Procedures;
                    txtRecommendations.Text = _prescription.Recommendations;
                    btnChangeGuest.Visibility = Visibility.Visible;

                    cmbStatus.Visibility = Visibility.Visible;
                    cmbStatus.SelectedIndex = (int)_prescription.Status;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiagnosis.Text))
            {
                MessageBox.Show("Введите диагноз.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                // Получаем текущего пользователя (врача) — упрощённо, в реальности через сессию
                int doctorId = GetCurrentDoctorId();

                if (_prescriptionId.HasValue)
                {
                    // Редактирование
                    var existing = db.MedicalPrescriptions.Find(_prescriptionId.Value);
                    if (existing != null)
                    {
                        existing.Diagnosis = txtDiagnosis.Text.Trim();
                        existing.Medications = string.IsNullOrWhiteSpace(txtMedications.Text) ? null : txtMedications.Text.Trim();
                        existing.Procedures = string.IsNullOrWhiteSpace(txtProcedures.Text) ? null : txtProcedures.Text.Trim();
                        existing.Recommendations = string.IsNullOrWhiteSpace(txtRecommendations.Text) ? null : txtRecommendations.Text.Trim();

                        if (cmbStatus.SelectedItem != null)
                        {
                            existing.Status = (PrescriptionStatus)cmbStatus.SelectedIndex;
                            if (existing.Status == PrescriptionStatus.Completed)
                                existing.CompletedAt = DateTime.Now;
                        }
                    }
                }
                else
                {
                    // Создание нового
                    var prescription = new MedicalPrescription
                    {
                        GuestId = _guestId,
                        BookingId = _bookingId,
                        Diagnosis = txtDiagnosis.Text.Trim(),
                        Medications = string.IsNullOrWhiteSpace(txtMedications.Text) ? null : txtMedications.Text.Trim(),
                        Procedures = string.IsNullOrWhiteSpace(txtProcedures.Text) ? null : txtProcedures.Text.Trim(),
                        Recommendations = string.IsNullOrWhiteSpace(txtRecommendations.Text) ? null : txtRecommendations.Text.Trim(),
                        DoctorId = doctorId,
                        CreatedAt = DateTime.Now,
                        Status = PrescriptionStatus.Assigned
                    };

                    db.MedicalPrescriptions.Add(prescription);

                    // Автоматически создаём записи в графике услуг для процедур
                    CreateServiceSchedules(db, prescription);
                }

                db.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void CreateServiceSchedules(ApplicationContext db, MedicalPrescription prescription)
        {
            // Если в процедурах указаны услуги из справочника — создаём график
            // Упрощённая логика: ищем услуги по названию в тексте процедур
            if (string.IsNullOrWhiteSpace(prescription.Procedures)) return;

            var services = db.AdditionalServices.ToList();
            var words = prescription.Procedures.Split(new[] { ' ', ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var service in services)
            {
                if (words.Any(w => w.ToLower().Contains(service.Name.ToLower()) ||
                                   service.Name.ToLower().Contains(w.ToLower())))
                {
                    // Создаём назначение в графике на ближайшие дни
                    for (int day = 0; day < 3; day++)
                    {
                        db.GuestServiceSchedules.Add(new Models.Additional_service_tables.GuestServiceSchedule
                        {
                            GuestId = prescription.GuestId,
                            BookingId = prescription.BookingId,
                            ServiceId = service.Id,
                            ScheduledDate = DateTime.Now.AddDays(day),
                            ScheduledTime = service.WorkStart,
                            Status = Models.Additional_service_tables.ScheduleStatus.Assigned,
                            Notes = $"Назначено врачом: {prescription.Diagnosis}"
                        });
                    }
                }
            }
        }

        private int GetCurrentDoctorId()
        {
            // Упрощённо: берём первого пользователя с ролью "Врач"
            // В реальности здесь должна быть проверка текущей сессии
            using (var db = new ApplicationContext())
            {
                var doctor = db.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Role.Name == "Врач");

                return doctor?.Id ?? 1; // fallback на ID 1
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChangeGuest_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new GuestSearchWindow();
            if (searchWindow.ShowDialog() == true)
            {
                _selectedGuest = searchWindow.SelectedGuest;
                _guestId = _selectedGuest.Id;
                LoadGuestInfo();
            }
        }
    }
}

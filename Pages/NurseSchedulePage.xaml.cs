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
    /// Логика взаимодействия для NurseSchedulePage.xaml
    /// </summary>
    public partial class NurseSchedulePage : Page
    {
        private List<NurseScheduleViewModel> _allItems;

        public NurseSchedulePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new ApplicationContext())
            {
                // Получаем график услуг + связанные назначения врача
                _allItems = db.GuestServiceSchedules
                    .Include(s => s.Service)
                    .Include(s => s.Guest)
                    .Where(s => s.Status != ScheduleStatus.Cancelled)
                    .Select(s => new NurseScheduleViewModel
                    {
                        ScheduleId = s.Id,
                        GuestName = s.Guest.FullName,
                        ServiceName = s.Service.Name,
                        ScheduledDate = s.ScheduledDate,
                        ScheduledTime = s.ScheduledTime,
                        Status = s.Status.ToString(),
                        StatusEnum = s.Status,
                        Notes = s.Notes,
                        // Получаем диагноз из последнего назначения врача для этого гостя
                        Diagnosis = db.MedicalPrescriptions
                            .Where(p => p.GuestId == s.GuestId && p.BookingId == s.BookingId)
                            .OrderByDescending(p => p.CreatedAt)
                            .Select(p => p.Diagnosis)
                            .FirstOrDefault()
                    })
                    .OrderBy(s => s.ScheduledDate)
                    .ThenBy(s => s.ScheduledTime)
                    .ToList();

                NurseGrid.ItemsSource = _allItems;
            }
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            var filtered = _allItems.AsEnumerable();

            if (cmbStatus.SelectedItem is ComboBoxItem item && item.Content.ToString() != "Все")
            {
                string status = item.Content.ToString();
                filtered = filtered.Where(s => s.Status == status);
            }

            NurseGrid.ItemsSource = filtered.ToList();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as NurseScheduleViewModel;
            if (item == null || item.StatusEnum != ScheduleStatus.Assigned) return;

            UpdateScheduleStatus(item.ScheduleId, ScheduleStatus.Confirmed, "Подтверждено медработником");
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as NurseScheduleViewModel;
            if (item == null || item.StatusEnum != ScheduleStatus.Confirmed) return;

            // Открываем диалог для ввода результата
            var result = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите результат выполнения процедуры:",
                "Выполнение назначения",
                "Процедура выполнена успешно");

            if (!string.IsNullOrWhiteSpace(result))
            {
                UpdateScheduleStatus(item.ScheduleId, ScheduleStatus.Completed, result);
            }
        }

        private void UpdateScheduleStatus(int scheduleId, ScheduleStatus newStatus, string notes)
        {
            using (var db = new ApplicationContext())
            {
                var schedule = db.GuestServiceSchedules.Find(scheduleId);
                if (schedule != null)
                {
                    schedule.Status = newStatus;
                    schedule.Notes = notes;
                    schedule.ConfirmedAt = DateTime.Now;
                    schedule.ConfirmedByUserId = GetCurrentNurseId();
                    db.SaveChanges();
                }
            }

            LoadData();
        }

        private int GetCurrentNurseId()
        {
            using (var db = new ApplicationContext())
            {
                var nurse = db.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Role.Name == "Медработник");

                return nurse?.Id ?? 1;
            }
        }
    }

    public class NurseScheduleViewModel
    {
        public int ScheduleId { get; set; }
        public string GuestName { get; set; }
        public string ServiceName { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public string Status { get; set; }
        public ScheduleStatus StatusEnum { get; set; }
        public string Notes { get; set; }
        public string Diagnosis { get; set; }
    }
}

using Strela_Sanatorii.Models.Accommodation_tables;
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
    /// Логика взаимодействия для ServiceRecordsPage.xaml
    /// </summary>
    public partial class ServiceRecordsPage : Page
    {
        private List<TimeSpan> _timeSlots;

        public ServiceRecordsPage()
        {
            InitializeComponent();
            LoadData();
            GenerateTimeSlots();
        }

        private void LoadData()
        {
            using (var db = new ApplicationContext())
            {
                cmbClients.ItemsSource = db.Clients.ToList();
                cmbServices.ItemsSource = db.AdditionalServices.ToList();
            }
        }

        private void GenerateTimeSlots()
        {
            _timeSlots = new List<TimeSpan>();
            // создаём слоты с 10:00 до 18:00 каждые 30 минут
            for (int h = 10; h <= 17; h++)
            {
                _timeSlots.Add(new TimeSpan(h, 0, 0));
                _timeSlots.Add(new TimeSpan(h, 30, 0));
            }
            cmbTimeSlots.ItemsSource = _timeSlots;
        }

        private void RefreshTimeSlots()
        {
            if (cmbServices.SelectedItem == null || dpDate.SelectedDate == null)
                return;

            var service = cmbServices.SelectedItem as AdditionalService;
            var date = dpDate.SelectedDate.Value;

            using (var db = new ApplicationContext())
            {
                // выбираем занятые слоты
                var busySlots = db.ServiceAppointments
                                  .Where(a => a.ServiceId == service.Id && a.AppointmentDate == date)
                                  .Select(a => a.StartTime)
                                  .ToList();

                // фильтруем свободные слоты
                cmbTimeSlots.ItemsSource = _timeSlots
                                            .Where(ts => !busySlots.Contains(ts))
                                            .ToList();
            }
        }

        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTimeSlots();
        }

        private void cmbServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTimeSlots();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClients.SelectedItem == null ||
                cmbServices.SelectedItem == null ||
                dpDate.SelectedDate == null ||
                cmbTimeSlots.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            var client = cmbClients.SelectedItem as Client;
            var service = cmbServices.SelectedItem as AdditionalService;
            var date = dpDate.SelectedDate.Value;
            var time = (TimeSpan)cmbTimeSlots.SelectedItem;

            using (var db = new ApplicationContext())
            {
                // проверка на занятость на случай, если два сотрудника одновременно работают
                var exists = db.ServiceAppointments
                               .Any(a => a.ServiceId == service.Id &&
                                         a.AppointmentDate == date &&
                                         a.StartTime == time);

                if (exists)
                {
                    MessageBox.Show("Слот уже занят, выберите другой");
                    RefreshTimeSlots();
                    return;
                }

                var appointment = new ServiceAppointment
                {
                    ClientId = client.Id,
                    ServiceId = service.Id,
                    AppointmentDate = date,
                    StartTime = time
                };

                db.ServiceAppointments.Add(appointment);
                db.SaveChanges();
            }

            MessageBox.Show("Запись создана");
            RefreshTimeSlots();
        }
    }
}

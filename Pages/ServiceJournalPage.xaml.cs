using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для ServiceJournalPage.xaml
    /// </summary>
    public partial class ServiceJournalPage : Page
    {
        private List<ServiceAppointment> _appointments;

        public ServiceJournalPage()
        {
            InitializeComponent();
            LoadFilters();
            LoadData();
        }

        private void LoadFilters()
        {
            using (var db = new ApplicationContext())
            {
                cmbFilterClient.ItemsSource = db.Clients.OrderBy(c => c.FullName).ToList();
                cmbFilterService.ItemsSource = db.AdditionalServices.OrderBy(s => s.Name).ToList();
            }
        }

        private void LoadData()
        {
            using (var db = new ApplicationContext())
            {
                _appointments = db.ServiceAppointments
                                  .Include(a => a.Client)
                                  .Include(a => a.Service)
                                  .ToList();
                JournalGrid.ItemsSource = _appointments;
            }
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            var filtered = _appointments.AsEnumerable();

            if (cmbFilterClient.SelectedItem is Client client)
                filtered = filtered.Where(a => a.ClientId == client.Id);

            if (cmbFilterService.SelectedItem is AdditionalService service)
                filtered = filtered.Where(a => a.ServiceId == service.Id);

            if (dpFilterDate.SelectedDate != null)
                filtered = filtered.Where(a => a.AppointmentDate.Date == dpFilterDate.SelectedDate.Value.Date);

            JournalGrid.ItemsSource = filtered.ToList();
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
           
            
            if (JournalGrid.SelectedItem is ServiceAppointment appointment)
            {
                if (appointment.IsPaid)
                {
                    MessageBox.Show("Эта запись уже оплачена.");
                    return;
                }
                using (var db = new ApplicationContext())
                {
                    var record = db.ServiceAppointments.Find(appointment.Id);
                    if (record != null)
                    {
                        record.IsPaid = true;
                        db.SaveChanges();
                        MessageBox.Show("Оплата отмечена.");
                    }
                }

                LoadData();
            }
            else
            {
                MessageBox.Show("Выберите запись для оплаты.");
            }
        }
    }
}

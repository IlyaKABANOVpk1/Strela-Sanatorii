using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            LoadAll();
        }

        private void LoadAll()
        {
            LoadLoad();
            LoadIncome();
            LoadVisits();
        }

        private void LoadLoad()
        {
            using (var db = new ApplicationContext())
            {
                int totalRooms = db.Rooms.Count();

                var data = db.Shifts
                    .OrderBy(s => s.StartDate)
                    .Select(s => new
                    {
                        ShiftName = s.Name,
                        Total = totalRooms,
                        Booked = db.Bookings.Count(b => b.ShiftId == s.Id),
                        Free = totalRooms - db.Bookings.Count(b => b.ShiftId == s.Id),
                        Percent = totalRooms > 0
                            ? (int)(((double)db.Bookings.Count(b => b.ShiftId == s.Id) / totalRooms) * 100)
                            : 0
                    })
                    .ToList();

                dgLoad.ItemsSource = data;
            }
        }

        private void LoadIncome()
        {
            using (var db = new ApplicationContext())
            {
                var data = db.ServiceAppointments
                    .Include(a => a.Service)
                    .Where(a => a.IsPaid)
                    .GroupBy(a => new { a.ServiceId, a.Service.Name, a.Service.Price })
                    .Select(g => new
                    {
                        Service = g.Key.Name,
                        Count = g.Count(),
                        Total = g.Count() * g.Key.Price
                    })
                    .ToList();

                dgIncome.ItemsSource = data;
            }
        }

        private void LoadVisits()
        {
            using (var db = new ApplicationContext())
            {
                var data = db.ServiceAppointments
                    .Include(a => a.Service)
                    .GroupBy(a => a.Service.Name)
                    .Select(g => new
                    {
                        Service = g.Key,
                        Total = g.Count(),
                        Paid = g.Count(x => x.IsPaid),
                        NotPaid = g.Count(x => !x.IsPaid)
                    })
                    .ToList();

                dgVisits.ItemsSource = data;
            }
        }
    }
}

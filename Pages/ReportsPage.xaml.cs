using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
            LoadShifts();
        }

        private void LoadShifts()
        {
            using (var db = new ApplicationContext())
            {
                cmbReportShift.ItemsSource = db.Shifts
                    .OrderBy(s => s.StartDate)
                    .ToList();
            }
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
                int totalCapacity = db.Rooms.Sum(r => r.Capacity);

                var data = db.Shifts
                    .OrderBy(s => s.StartDate)
                    .Select(s => new
                    {
                        ShiftName = s.Name,
                        TotalCapacity = totalCapacity,
                        OccupiedPlaces = db.Bookings
                            .Where(b => b.ShiftId == s.Id)
                            .Sum(b => (int?)b.GuestCount) ?? 0,
                        FreePlaces = totalCapacity - (db.Bookings
                            .Where(b => b.ShiftId == s.Id)
                            .Sum(b => (int?)b.GuestCount) ?? 0),
                        Percent = totalCapacity > 0
                            ? (int)(((double)(db.Bookings
                                .Where(b => b.ShiftId == s.Id)
                                .Sum(b => (int?)b.GuestCount) ?? 0) / totalCapacity) * 100)
                            : 0
                    })
                    .ToList();

                dgLoad.ItemsSource = data.Select(d => new
                {
                    d.ShiftName,
                    Total = d.TotalCapacity,
                    Booked = d.OccupiedPlaces,
                    Free = d.FreePlaces,
                    Percent = d.Percent
                }).ToList();
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

        private void ReportShift_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (cmbReportShift.SelectedItem == null) return;

            var shiftId = (int)cmbReportShift.SelectedValue;

            using (var db = new ApplicationContext())
            {
                var rooms = db.Rooms
                    .Include(r => r.RoomCategory)
                    .OrderBy(r => r.RoomNumber)
                    .ToList();

                var bookings = db.Bookings
                    .Where(b => b.ShiftId == shiftId)
                    .Include(b => b.Guest)
                    .Include(b => b.ServicePackage)
                    .ToList();

                var reportData = new List<ShiftReportItem> ();

                foreach (var room in rooms)
                {
                    var roomBookings = bookings.Where(b => b.RoomId == room.Id).ToList();
                    var occupied = roomBookings.Sum(b => b.GuestCount);

                    reportData.Add(new ShiftReportItem
                    {
                        RoomNumber = room.RoomNumber,
                        Category = room.RoomCategory?.Name ?? "Без категории",
                        Capacity = room.Capacity,
                        Occupied = occupied,
                        GuestNames = roomBookings.Any()
                            ? string.Join(", ", roomBookings.Select(b => b.Guest.FullName))
                            : "—",
                        PackageName = roomBookings.FirstOrDefault()?.ServicePackage?.Name ?? "—"
                    });
                }

                dgShiftReport.ItemsSource = reportData;
            }
        }

        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            if (dgShiftReport.ItemsSource == null)
            {
                MessageBox.Show("Сначала выберите смену для формирования отчёта.");
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"Отчет_по_смене_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                var items = dgShiftReport.ItemsSource as List <ShiftReportItem>;
                if (items == null) return;

                var lines = new List<string>
                {
                    "Номер;Категория;Вместимость;Занято мест;Гости;Пакет услуг"
                };

                foreach (var item in items)
                {
                    lines.Add($"{item.RoomNumber};{item.Category};{item.Capacity};{item.Occupied};{item.GuestNames};{item.PackageName}");
                }

                File.WriteAllLines(dialog.FileName, lines, Encoding.UTF8);
                MessageBox.Show("Отчёт экспортирован успешно.");
            }
        }
    }

    public class ShiftReportItem
    {
        public string RoomNumber { get; set; }
        public string Category { get; set; }
        public int Capacity { get; set; }
        public int Occupied { get; set; }
        public string GuestNames { get; set; }
        public string PackageName { get; set; }
    }
}

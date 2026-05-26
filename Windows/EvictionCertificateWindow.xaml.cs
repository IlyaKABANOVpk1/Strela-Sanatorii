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
using System.Windows.Shapes;

namespace Strela_Sanatorii.Windows
{
    /// <summary>
    /// Логика взаимодействия для EvictionCertificateWindow.xaml
    /// </summary>
    public partial class EvictionCertificateWindow : Window
    {
        public EvictionCertificateWindow(int bookingId)
        {
            InitializeComponent();
            LoadData(bookingId);
        }

        private void LoadData(int bookingId)
        {
            using (var db = new ApplicationContext())
            {
                var booking = db.Bookings
                    .Include(b => b.Guest)
                    .Include(b => b.Room)
                    .ThenInclude(r => r.RoomCategory)
                    .Include(b => b.Shift)
                    .Include(b => b.ServicePackage)
                    .FirstOrDefault(b => b.Id == bookingId);

                if (booking == null)
                {
                    MessageBox.Show("Бронь не найдена.");
                    Close();
                    return;
                }

                // Основные данные
                txtFullName.Text = booking.Guest.FullName;
                txtPersonnelNumber.Text = booking.Guest.PersonnelNumber ?? "—";
                txtRoomNumber.Text = $"{booking.Room.RoomNumber} ({booking.Room.RoomCategory?.Name ?? "Без категории"})";
                txtShiftName.Text = booking.Shift.Name;
                txtCheckInDate.Text = booking.CreatedAt.ToString("dd.MM.yyyy");
                txtCheckOutDate.Text = booking.CheckOutDate?.ToString("dd.MM.yyyy") ?? DateTime.Now.ToString("dd.MM.yyyy");

                // Услуги из пакета
                var services = new List<ServiceReportItem>();

                if (booking.ServicePackageId.HasValue)
                {
                    var packageItems = db.PackageItems
                        .Where(pi => pi.ServicePackageId == booking.ServicePackageId.Value)
                        .Include(pi => pi.AdditionalService)
                        .ToList();

                    foreach (var item in packageItems)
                    {
                        services.Add(new ServiceReportItem
                        {
                            Name = item.AdditionalService.Name,
                            Quantity = item.Quantity,
                            Price = item.AdditionalService.Price,
                            Total = item.AdditionalService.Price * item.Quantity
                        });
                    }
                }

                // Дополнительные услуги
                var addons = db.BookingAddons
                    .Where(ba => ba.BookingId == bookingId)
                    .Include(ba => ba.AdditionalService)
                    .ToList();

                foreach (var addon in addons)
                {
                    services.Add(new ServiceReportItem
                    {
                        Name = addon.AdditionalService.Name + " (доп.)",
                        Quantity = addon.Quantity,
                        Price = addon.AdditionalService.Price,
                        Total = addon.AdditionalService.Price * addon.Quantity
                    });
                }

                // Проживание
                services.Insert(0, new ServiceReportItem
                {
                    Name = $"Проживание: {booking.Room.RoomCategory?.Name}",
                    Quantity = 1,
                    Price = booking.Room.RoomCategory?.BasePrice ?? 0,
                    Total = booking.Room.RoomCategory?.BasePrice ?? 0
                });

                dgServices.ItemsSource = services;

                var total = services.Sum(s => s.Total);
                txtTotalAmount.Text = total.ToString("C");
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Создаём копию панели для печати
                var printDocument = new FlowDocument();
                var section = new Section();

                // Клонируем содержимое
                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {
                    var brush = new VisualBrush(PrintPanel);
                    context.DrawRectangle(brush, null, new Rect(new Point(), new Size(PrintPanel.ActualWidth, PrintPanel.ActualHeight)));
                }

                var image = new DrawingVisual();

                printDialog.PrintVisual(PrintPanel, "Справка о выселении");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ServiceReportItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}

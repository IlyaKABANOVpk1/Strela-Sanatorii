using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Strela_Sanatorii.Models;
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
    public partial class BookingGridPage : Page
    {
        public BookingGridPage()
        {
            InitializeComponent();
            LoadShifts();
        }

        private void LoadShifts()
        {
            using (var db = new ApplicationContext())
            {
                cmbShifts.ItemsSource = db.Shifts.ToList();
                cmbShifts.DisplayMemberPath = "Name";
                cmbShifts.SelectedIndex = 0;
            }

            RefreshGrid(null, null);
        }

        private void RefreshGrid(object sender, RoutedEventArgs e)
        {
            if (cmbShifts.SelectedItem == null) return;

            var selectedShift = cmbShifts.SelectedItem as Shift;

            using (var db = new ApplicationContext())
            {
                var rooms = db.Rooms
                    .Include(r => r.RoomCategory)
                    .OrderBy(r => r.RoomNumber)
                    .ToList();

                var bookings = db.Bookings
                    .Where(b => b.ShiftId == selectedShift.Id)
                    .Include(b => b.Guest)
                    .ToList();

                RoomsGrid.Children.Clear();

                foreach (var room in rooms)
                {
                    var roomBookings = bookings.Where(b => b.RoomId == room.Id).ToList();
                    var occupiedPlaces = roomBookings.Sum(b => b.GuestCount);
                    var isFull = occupiedPlaces >= room.Capacity;
                    var hasBookings = roomBookings.Any();

                    var card = new Card
                    {
                        Width = 160,
                        Height = 140,
                        Margin = new Thickness(10),
                        Padding = new Thickness(10),
                        Background = isFull
                            ? new SolidColorBrush(Color.FromRgb(239, 83, 80))      // полностью занят
                            : (hasBookings
                                ? new SolidColorBrush(Color.FromRgb(255, 167, 38))  // частично занят
                                : new SolidColorBrush(Color.FromRgb(102, 187, 106))), // свободен
                        Cursor = System.Windows.Input.Cursors.Hand,
                        ToolTip = isFull ? "Занят" : (hasBookings ? $"Частично занят ({occupiedPlaces}/{room.Capacity})" : "Свободен")
                    };

                    var stack = new StackPanel();

                    // Номер
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"№ {room.RoomNumber}",
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        HorizontalAlignment = HorizontalAlignment.Center
                    });

                    // Категория
                    stack.Children.Add(new TextBlock
                    {
                        Text = room.RoomCategory?.Name ?? "Без категории",
                        FontSize = 11,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Opacity = 0.7
                    });

                    // Занятость
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"{occupiedPlaces}/{room.Capacity}",
                        FontSize = 13,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 5, 0, 0),
                        Foreground = Brushes.White
                    });

                    // ФИО или "Свободен"
                    if (hasBookings)
                    {
                        var names = string.Join(", ", roomBookings.Select(b => b.Guest.FullName).Take(2));
                        if (roomBookings.Count > 2) names += "...";

                        stack.Children.Add(new TextBlock
                        {
                            Text = names,
                            FontSize = 11,
                            TextWrapping = TextWrapping.Wrap,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(0, 5, 0, 0)
                        });
                    }
                    else
                    {
                        stack.Children.Add(new TextBlock
                        {
                            Text = "Свободен",
                            FontSize = 12,
                            Opacity = 0.8,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(0, 5, 0, 0)
                        });
                    }

                    card.Content = stack;

                    card.MouseLeftButtonUp += (s, ev) =>
                    {
                        if (isFull)
                        {
                            // Показываем окно действий с существующими бронями
                            var mainBooking = roomBookings.First();
                            var dialog = new BookingActionsWindow(mainBooking.Guest.FullName, room.RoomNumber, mainBooking.Id, mainBooking.ServicePackage?.Name);

                            if (dialog.ShowDialog() == true)
                            {
                                HandleBookingAction(dialog.ActionResult, room, selectedShift, mainBooking);
                            }
                        }
                        else
                        {
                            // Заселение (свободный или частично занят)
                            BookingWindow bookingWindow = new BookingWindow(room, selectedShift);
                            bookingWindow.ShowDialog();
                            RefreshGrid(null, null);
                        }
                    };

                    RoomsGrid.Children.Add(card);
                }
            }
        }

        private void HandleBookingAction(string action, Room room, Shift shift, Booking booking)
        {
            switch (action)
            {
                case "Evict":
                    using (var db = new ApplicationContext())
                    {
                        var b = db.Bookings.Find(booking.Id);
                        if (b != null)
                        {
                            b.Status = BookingStatus.CheckedOut;
                            b.CheckOutDate = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                    RefreshGrid(null, null);
                    break;

                case "Rebook":
                    using (var db = new ApplicationContext())
                    {
                        var b = db.Bookings.Find(booking.Id);
                        if (b != null)
                        {
                            db.Bookings.Remove(b);
                            db.SaveChanges();
                        }
                    }
                    BookingWindow bw = new BookingWindow(room, shift);
                    bw.ShowDialog();
                    RefreshGrid(null, null);
                    break;

                case "Progress":
                    var progressWindow = new BookingProgressWindow(booking.Id);
                    progressWindow.ShowDialog();
                    break;

                case "Addon":
                    var addonWindow = new PackageAddonWindow(booking.Id, booking.ServicePackage?.Name);
                    if (addonWindow.ShowDialog() == true)
                        RefreshGrid(null, null);
                    break;
                case "Certificate":
                    var certWindow = new EvictionCertificateWindow(booking.Id);
                    certWindow.ShowDialog();
                    break;
            }
        }
    }
}

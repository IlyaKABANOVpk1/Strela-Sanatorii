using Strela_Sanatorii.Models.Accommodation_tables;
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
using Strela_Sanatorii.Models;
using MaterialDesignThemes.Wpf;
using Strela_Sanatorii.Windows;





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
                              .OrderBy(r => r.RoomNumber)
                              .ToList();

                var bookings = db.Bookings
                    .Where(b => b.ShiftId == selectedShift.Id)
                    .Select(b => new
                    {
                        b.RoomId,
                        ClientName = b.Client.FullName
                    })
                    .ToList();

                RoomsGrid.Children.Clear();

                foreach (var room in rooms)
                {
                    
                    var booking = bookings.FirstOrDefault(b => b.RoomId == room.Id);

                    var card = new Card
                    {
                        Width = 140,
                        Height = 120,
                        Margin = new Thickness(10),
                        Padding = new Thickness(10),

                        Background = booking != null
                            ? new SolidColorBrush(Color.FromRgb(239, 83, 80))   // занят
                            : new SolidColorBrush(Color.FromRgb(102, 187, 106)), // свободен

                        Cursor = System.Windows.Input.Cursors.Hand,
                        ToolTip = booking != null ? "Занят" : "Свободен"
                    };

                    var stack = new StackPanel();

                    // Номер
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"Номер {room.RoomNumber}",
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        HorizontalAlignment = HorizontalAlignment.Center
                    });

                    // Категория
                    stack.Children.Add(new TextBlock
                    {
                        Text = room.Category,
                        FontSize = 12,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Opacity = 0.7
                    });

                    // ФИО или "Свободен"
                    if (booking != null)
                    {
                        stack.Children.Add(new TextBlock
                        {
                            Text = booking.ClientName,
                            FontSize = 13,
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
                            Opacity = 0.6,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(0, 5, 0, 0)
                        });
                    }

                    card.Content = stack;

                    card.MouseLeftButtonUp += (s, ev) =>
                    {
                        if (booking != null)
                        {
                            var dialog = new BookingActionsWindow(booking.ClientName, room.RoomNumber);

                            if (dialog.ShowDialog() == true)
                            {
                                // ВЫСЕЛЕНИЕ
                                if (dialog.ActionResult == "Evict")
                                {
                                    using (var db = new ApplicationContext())
                                    {
                                        var bookingToDelete = db.Bookings
                                            .FirstOrDefault(b => b.RoomId == room.Id && b.ShiftId == selectedShift.Id);

                                        if (bookingToDelete != null)
                                        {
                                            db.Bookings.Remove(bookingToDelete);
                                            db.SaveChanges();
                                        }
                                    }

                                    RefreshGrid(null, null);
                                }

                                // ПЕРЕСЕЛЕНИЕ
                                else if (dialog.ActionResult == "Rebook")
                                {
                                    using (var db = new ApplicationContext())
                                    {
                                        var bookingToDelete = db.Bookings
                                            .FirstOrDefault(b => b.RoomId == room.Id && b.ShiftId == selectedShift.Id);

                                        if (bookingToDelete != null)
                                        {
                                            db.Bookings.Remove(bookingToDelete);
                                            db.SaveChanges();
                                        }
                                    }

                                    BookingWindow bookingWindow = new BookingWindow(room, selectedShift);
                                    bookingWindow.ShowDialog();

                                    RefreshGrid(null, null);
                                }
                            }
                        }
                        else
                        {
                            BookingWindow bookingWindow = new BookingWindow(room, selectedShift);
                            bookingWindow.ShowDialog();

                            RefreshGrid(null, null);
                        }
                    };

                    RoomsGrid.Children.Add(card);
                }
            }
        }
    }
}

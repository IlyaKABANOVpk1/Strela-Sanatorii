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
                                 .Select(b => b.RoomId)
                                 .ToList();

                RoomsGrid.Children.Clear();

                foreach (var room in rooms)
                {
                    var card = new Card
                    {
                        Width = 140,
                        Height = 120,
                        Margin = new Thickness(10),
                        Padding = new Thickness(10),
                        
                        Background = bookings.Contains(room.Id) ? Brushes.LightCoral : Brushes.LightGreen,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        ToolTip = bookings.Contains(room.Id) ? "Занят" : "Свободен"
                    };

                    var stack = new StackPanel();

                    stack.Children.Add(new TextBlock
                    {
                        Text = $"Номер {room.RoomNumber}",
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        HorizontalAlignment = HorizontalAlignment.Center
                    });

                    stack.Children.Add(new TextBlock
                    {
                        Text = room.Category,
                        FontSize = 12,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Opacity = 0.7
                    });

                    card.Content = stack;

                    card.MouseLeftButtonUp += (s, ev) =>
                    {
                        if (bookings.Contains(room.Id))
                        {
                            MessageBox.Show($"Номер {room.RoomNumber} уже занят на эту смену.");
                        }
                        else
                        {
                            // Открываем окно для брони
                            BookingWindow bookingWindow = new BookingWindow(room, selectedShift);
                            bookingWindow.ShowDialog();

                            // После закрытия обновляем сетку
                            RefreshGrid(null, null);
                        }
                    };

                    RoomsGrid.Children.Add(card);
                }
            }
        }
    }
}

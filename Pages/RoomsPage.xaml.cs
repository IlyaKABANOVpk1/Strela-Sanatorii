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
    /// <summary>
    /// Логика взаимодействия для RoomsPage.xaml
    /// </summary>
    public partial class RoomsPage : Page
    {
        public RoomsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new ApplicationContext())
            {
                RoomsGrid.ItemsSource = db.Rooms.OrderBy(r => r.RoomNumber).ToList();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new RoomEditWindow();
            if (window.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsGrid.SelectedItem is Room room)
            {
                var window = new RoomEditWindow(room);
                if (window.ShowDialog() == true)
                    LoadData();
            }
            else
            {
                MessageBox.Show("Выберите номер для редактирования.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsGrid.SelectedItem is Room room)
            {
                var result = MessageBox.Show($"Удалить номер {room.RoomNumber}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new ApplicationContext())
                    {
                        var r = db.Rooms.Find(room.Id);
                        if (r != null)
                        {
                            bool hasBookings = db.Bookings.Any(b => b.RoomId == r.Id);
                            if (hasBookings)
                            {
                                MessageBox.Show("Невозможно удалить номер с активными бронями.");
                                return;
                            }

                            db.Rooms.Remove(r);
                            db.SaveChanges();
                        }
                    }
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите номер для удаления.");
            }
        }
    }
}

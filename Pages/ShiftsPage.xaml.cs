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
    /// Логика взаимодействия для ShiftsPage.xaml
    /// </summary>
    public partial class ShiftsPage : Page
    {
        public ShiftsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new ApplicationContext())
            {
                ShiftsGrid.ItemsSource = db.Shifts.OrderBy(s => s.StartDate).ToList();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new ShiftEditWindow();
            if (window.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ShiftsGrid.SelectedItem is Shift shift)
            {
                var window = new ShiftEditWindow(shift);
                if (window.ShowDialog() == true)
                    LoadData();
            }
            else
            {
                MessageBox.Show("Выберите смену для редактирования.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ShiftsGrid.SelectedItem is Shift shift)
            {
                var result = MessageBox.Show($"Удалить смену {shift.Name}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new ApplicationContext())
                    {
                        var s = db.Shifts.Find(shift.Id);
                        if (s != null)
                        {
                            bool hasBookings = db.Bookings.Any(b => b.ShiftId == s.Id);
                            if (hasBookings)
                            {
                                MessageBox.Show("Невозможно удалить смену с активными бронями.");
                                return;
                            }

                            db.Shifts.Remove(s);
                            db.SaveChanges();
                        }
                    }
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите смену для удаления.");
            }
        }
    }
}

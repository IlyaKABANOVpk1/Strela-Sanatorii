using Strela_Sanatorii.Models.Accommodation_tables;
using Strela_Sanatorii.Models.Additional_service_tables;
using Strela_Sanatorii.Utils;
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
    /// Логика взаимодействия для BookingWindow.xaml
    /// </summary>
    public partial class BookingWindow : Window
    {
        private Room _room;
        private Shift _shift;
        private Guest _selectedGuest;

        public BookingWindow(Room room, Shift shift)
        {
            InitializeComponent();

            if (room == null || shift == null)
            {
                MessageBox.Show("Ошибка: номер или смена не переданы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            _room = room;
            _shift = shift;

            txtRoom.Text = $"Номер: {room.RoomNumber} | {room.RoomCategory?.Name ?? "Без категории"} | Вместимость: {room.Capacity} чел.";
            LoadPackages();
        }

        private void LoadPackages()
        {
            using (var db = new ApplicationContext())
            {
                cmbPackage.ItemsSource = db.ServicePackages.OrderBy(p => p.Name).ToList();
                cmbPackage.SelectedIndex = -1;
            }
        }

        private void SearchGuest_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new GuestSearchWindow();
            if (searchWindow.ShowDialog() == true)
            {
                _selectedGuest = searchWindow.SelectedGuest;
                txtGuestInfo.Text = _selectedGuest.FullName;
                txtPhone.Text = _selectedGuest.Phone ?? "";
                txtTabNumber.Text = _selectedGuest.PersonnelNumber ?? "";
            }
        }

        private void chkIsFamily_CheckedChanged(object sender, RoutedEventArgs e)
        {
            txtGuestCount.IsEnabled = chkIsFamily.IsChecked == true;
            if (chkIsFamily.IsChecked != true)
                txtGuestCount.Text = "1";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGuest == null)
            {
                MessageBox.Show("Выберите гостя!");
                return;
            }

            // Проверка количества человек
            int guestCount = 1;
            if (chkIsFamily.IsChecked == true)
            {
                if (!ValidationHelper.IsPositiveInt(txtGuestCount.Text, out guestCount))
                {
                    MessageBox.Show("Введите корректное количество человек.");
                    return;
                }
            }

            // Проверка вместимости
            if (guestCount > _room.Capacity)
            {
                MessageBox.Show($"Номер {_room.RoomNumber} рассчитан максимум на {_room.Capacity} человек.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                var occupiedPlaces = db.Bookings
                    .Where(b => b.RoomId == _room.Id && b.ShiftId == _shift.Id)
                    .Sum(b => (int?)b.GuestCount) ?? 0;

                var availablePlaces = _room.Capacity - occupiedPlaces;

                if (guestCount > availablePlaces)
                {
                    MessageBox.Show($"В номере осталось только {availablePlaces} свободных мест.");
                    return;
                }

                var existingBooking = db.Bookings
                    .FirstOrDefault(b => b.RoomId == _room.Id && b.ShiftId == _shift.Id && b.GuestId == _selectedGuest.Id);

                if (existingBooking != null)
                {
                    MessageBox.Show($"Гость {_selectedGuest.FullName} уже заселён в номер {_room.RoomNumber} на выбранную смену.");
                    return;
                }

                var booking = new Booking
                {
                    GuestId = _selectedGuest.Id,
                    RoomId = _room.Id,
                    ShiftId = _shift.Id,
                    IsFamily = chkIsFamily.IsChecked == true,
                    GuestCount = guestCount,
                    ServicePackageId = cmbPackage.SelectedValue as int?,
                    CreatedAt = DateTime.UtcNow
                };

                db.Bookings.Add(booking);
                db.SaveChanges();

                if (booking.ServicePackageId.HasValue)
                {
                    CreateScheduleFromPackage(db, booking.Id, booking.ServicePackageId.Value, _selectedGuest.Id);
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void CreateScheduleFromPackage(ApplicationContext db, int bookingId, int packageId, int guestId)
        {
            var items = db.PackageItems
                .Where(pi => pi.ServicePackageId == packageId)
                .ToList();

            foreach (var item in items)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    db.GuestServiceSchedules.Add(new GuestServiceSchedule
                    {
                        GuestId = guestId,
                        BookingId = bookingId,
                        ServiceId = item.AdditionalServiceId,
                        ScheduledDate = DateTime.Now.AddDays(i),
                        Status = ScheduleStatus.Assigned
                    });
                }
            }
            db.SaveChanges();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

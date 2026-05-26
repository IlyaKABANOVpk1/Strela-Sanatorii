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
    /// Логика взаимодействия для GuestsPage.xaml
    /// </summary>
    public partial class GuestsPage : Page
    {
        private List<Guest> _allGuests;
        private List<Guest> _filteredGuests;
        private int _currentPage = 1;
        private const int PageSize = 50;
        private int _totalPages = 1;

        public GuestsPage()
        {
            InitializeComponent();
            LoadGuests();
        }

        private void LoadGuests()
        {
            using (var db = new ApplicationContext())
            {
                _allGuests = db.Guests
                    .OrderBy(g => g.LastName)
                    .ThenBy(g => g.FirstName)
                    .ToList();

                _filteredGuests = new List<Guest>(_allGuests);
                _currentPage = 1;
                UpdatePagination();
            }
        }

        private void UpdatePagination()
        {
            _totalPages = (int)Math.Ceiling((double)_filteredGuests.Count / PageSize);
            if (_totalPages < 1) _totalPages = 1;
            if (_currentPage > _totalPages) _currentPage = _totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageData = _filteredGuests
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            GuestsGrid.ItemsSource = pageData;
            txtPageInfo.Text = $"{_currentPage} / {_totalPages} (всего: {_filteredGuests.Count})";

            btnPrev.IsEnabled = _currentPage > 1;
            btnNext.IsEnabled = _currentPage < _totalPages;
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                _filteredGuests = new List<Guest>(_allGuests);
            }
            else
            {
                _filteredGuests = _allGuests
                    .Where(g =>
                        (g.LastName != null && g.LastName.ToLower().Contains(search)) ||
                        (g.FirstName != null && g.FirstName.ToLower().Contains(search)) ||
                        (g.MiddleName != null && g.MiddleName.ToLower().Contains(search)) ||
                        (g.PersonnelNumber != null && g.PersonnelNumber.ToLower().Contains(search)))
                    .ToList();
            }

            _currentPage = 1;
            UpdatePagination();
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePagination();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                UpdatePagination();
            }
        }

        private void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            var window = new GuestEditWindow();
            if (window.ShowDialog() == true)
                LoadGuests();
        }

        private void EditGuest_Click(object sender, RoutedEventArgs e)
        {
            if (GuestsGrid.SelectedItem is Guest guest)
            {
                var window = new GuestEditWindow(guest);
                if (window.ShowDialog() == true)
                    LoadGuests();
            }
            else
            {
                MessageBox.Show("Выберите гостя для редактирования.");
            }
        }

        private void DeleteGuest_Click(object sender, RoutedEventArgs e)
        {
            if (GuestsGrid.SelectedItem is Guest guest)
            {
                var result = MessageBox.Show($"Удалить гостя {guest.FullName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new ApplicationContext())
                    {
                        var g = db.Guests.Find(guest.Id);
                        if (g != null)
                        {
                            bool hasBookings = db.Bookings.Any(b => b.GuestId == g.Id);
                            bool hasAppointments = db.ServiceAppointments.Any(a => a.GuestId == g.Id);

                            if (hasBookings || hasAppointments)
                            {
                                MessageBox.Show("Невозможно удалить гостя, так как есть связанные брони или записи на услуги.");
                                return;
                            }

                            db.Guests.Remove(g);
                            db.SaveChanges();
                        }
                    }
                    LoadGuests();
                }
            }
            else
            {
                MessageBox.Show("Выберите гостя для удаления.");
            }
        }
    }
}

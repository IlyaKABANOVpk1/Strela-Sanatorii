using Microsoft.EntityFrameworkCore;
using Strela_Sanatorii.Models.UserTables;
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
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        private List<User> _allUsers;
        private List<User> _filteredUsers;
        private int _currentPage = 1;
        private const int PageSize = 50;
        private int _totalPages = 1;

        private User _currentUser;

        // Новый конструктор с передачей текущего пользователя
        public UsersPage(User currentUser) : this()
        {
            _currentUser = currentUser;
        }
        public UsersPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var db = new ApplicationContext())
            {
                _allUsers = db.Users
                    .Include(u => u.Role)
                    .ToList();

                _filteredUsers = new List<User>(_allUsers);
                _currentPage = 1;
                UpdatePagination();
            }
        }

        private void UpdatePagination()
        {
            _totalPages = (int)Math.Ceiling((double)_filteredUsers.Count / PageSize);
            if (_totalPages < 1) _totalPages = 1;
            if (_currentPage > _totalPages) _currentPage = _totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var pageData = _filteredUsers
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            UsersGrid.ItemsSource = pageData;
            txtPageInfo.Text = $"{_currentPage} / {_totalPages} (всего: {_filteredUsers.Count})";

            btnPrev.IsEnabled = _currentPage > 1;
            btnNext.IsEnabled = _currentPage < _totalPages;
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                _filteredUsers = new List<User>(_allUsers);
            }
            else
            {
                _filteredUsers = _allUsers
                    .Where(u => u.Login.ToLower().Contains(search))
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

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;

            if (_currentUser != null && user.Id == _currentUser.Id)
            {
                MessageBox.Show("Нельзя удалить самого себя.");
                return;
            }

            if (user == null) return;

            // Проверка: нельзя удалить самого себя
            // TODO: заменить на реальную проверку текущего пользователя из сессии
            // Пока упрощённо — по логину или ID (нужно передать текущего пользователя в конструктор)
            if (user.Login == "super" || user.Login == "admin")  // Временная заглушка
            {
                MessageBox.Show("Нельзя удалить системного пользователя.");
                return;
            }

            if (MessageBox.Show($"Удалить пользователя {user.Login}?",
                "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new ApplicationContext())
                {
                    // Проверка: есть ли назначения врача
                    bool hasPrescriptions = db.MedicalPrescriptions.Any(p => p.DoctorId == user.Id);
                    if (hasPrescriptions)
                    {
                        MessageBox.Show("Невозможно удалить пользователя — он назначал лечение гостям.");
                        return;
                    }

                    // Проверка: есть ли подтверждённые процедуры медработником
                    bool hasConfirmedServices = db.GuestServiceSchedules.Any(s => s.ConfirmedByUserId == user.Id);
                    if (hasConfirmedServices)
                    {
                        MessageBox.Show("Невозможно удалить пользователя — он выполнял процедуры.");
                        return;
                    }

                    var u = db.Users.Find(user.Id);
                    if (u != null)
                    {
                        db.Users.Remove(u);
                        db.SaveChanges();
                    }
                }

                LoadUsers();
            }
        }
    }
}

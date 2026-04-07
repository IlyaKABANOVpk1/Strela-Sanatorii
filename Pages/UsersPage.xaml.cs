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
        public UsersPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var db = new ApplicationContext())
            {
                UsersGrid.ItemsSource = db.Users
                    .Include(u => u.Role)
                    .ToList();
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();

            using (var db = new ApplicationContext())
            {
                var result = db.Users
                    .Include(u => u.Role)
                    .Where(u => u.Login.ToLower().Contains(search))
                    .ToList();

                UsersGrid.ItemsSource = result;
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

            if (user == null) return;

            if (MessageBox.Show($"Удалить пользователя {user.Login}?",
                "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new ApplicationContext())
                {
                    var u = db.Users.Find(user.Id);
                    db.Users.Remove(u);
                    db.SaveChanges();
                }

                LoadUsers();
            }
        }
    }
}

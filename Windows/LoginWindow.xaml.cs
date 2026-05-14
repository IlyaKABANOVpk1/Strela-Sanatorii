using Microsoft.EntityFrameworkCore;
using Strela_Sanatorii.Models.UserTables;
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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Collapsed;

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Введите логин и пароль");
                return;
            }

            try
            {
                var user = GetUserByCredentials(login, password);

                if (user != null)
                {
                    OpenMainWindowByRole(user);
                    this.Close();
                }
                else
                {
                    ShowError("Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка подключения к базе данных");
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private User GetUserByCredentials(string login, string password)
        {
            using (var db = new ApplicationContext())
            {
                return db.Users
                         .Include(u => u.Role)
                         .FirstOrDefault(u => u.Login == login && u.PasswordHash == password);
            }
        }

        private void OpenMainWindowByRole(User user)
        {
            if (user.Role.Name == "Администратор")
            {
                new AdminMainWindow().Show();
            }
            else if (user.Role.Name == "Сотрудник доп. услуг")
            {
                new ServicesMainWindow().Show();
            }
            else if (user.Role.Name == "Системный администратор")
            {
                new SysAdminMainWindow().Show();
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }
    }
}

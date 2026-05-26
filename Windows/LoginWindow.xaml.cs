using Microsoft.EntityFrameworkCore;
using Strela_Sanatorii.Models.UserTables;
using Strela_Sanatorii.Services;
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
                var user = db.Users
                             .Include(u => u.Role)
                             .FirstOrDefault(u => u.Login == login);

                if (user == null) return null;

                // Проверка 1: пароль в BCrypt-формате ($2a$ или $2b$)
                if (IsBcryptHash(user.PasswordHash))
                {
                    if (PasswordService.VerifyPassword(password, user.PasswordHash))
                        return user;
                }
                // Проверка 2: обратная совместимость — пароль в открытом виде
                else if (user.PasswordHash == password)
                {
                    // Перехешировать в BCrypt
                    user.PasswordHash = PasswordService.HashPassword(password);
                    db.SaveChanges();
                    return user;
                }

                return null;
            }
        }

        // Проверяет, является ли строка BCrypt-хешем
        private bool IsBcryptHash(string hash)
        {
            if (string.IsNullOrEmpty(hash)) return false;
            return hash.StartsWith("$2a$") || hash.StartsWith("$2b$") || hash.StartsWith("$2y$");
        }

        private void OpenMainWindowByRole(User user)
        {
            switch (user.Role.Name)
            {
                case "Администратор":
                    new AdminMainWindow().Show();
                    break;
                case "Сотрудник доп. услуг":
                    new ServicesMainWindow().Show();
                    break;
                case "Врач":
                    new DoctorMainWindow().Show();
                    break;
                case "Медработник":
                    new NurseMainWindow().Show();
                    break;
                case "Суперпользователь":
                    new SuperuserMainWindow().Show();
                    break;
                default:
                    MessageBox.Show("Неизвестная роль пользователя.");
                    break;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }
    }
}

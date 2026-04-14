using Microsoft.EntityFrameworkCore;
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
                lblError.Text = "Введите логин и пароль";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var user = db.Users
                        .Include(u => u.Role)
                        .FirstOrDefault(u => u.Login == login && u.PasswordHash == password);

                    if (user != null)
                    {
                        if (user.Role.Name == "Администратор")
                        {
                            AdminMainWindow adminWindow = new AdminMainWindow();
                            adminWindow.Show();
                        }
                        else if (user.Role.Name == "Сотрудник доп. услуг")
                        {
                            ServicesMainWindow serviceWindow = new ServicesMainWindow();
                            serviceWindow.Show();
                        }
                        else if (user.Role.Name == "Системный администратор")
                        {
                            MessageBox.Show("Окно системного администратора пока не реализовано",
                                            "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Неизвестная роль пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                        this.Close();
                    }
                    else
                    {
                        lblError.Text = "Неверный логин или пароль";
                        lblError.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Ошибка подключения к базе данных";
                lblError.Visibility = Visibility.Visible;
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

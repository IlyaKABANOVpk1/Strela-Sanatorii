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
            // Скрываем ошибку при новом нажатии
            lblError.Visibility = Visibility.Collapsed;

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            // Проверка на пустые поля
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
                        // Переход по ролям
                        if (user.Role.Name == "Администратор")
                        {
                            AdminMainWindow adminWindow = new AdminMainWindow();
                            adminWindow.Show();
                        }
                        else if (user.Role.Name == "Сотрудник доп. услуг")
                        {
                            MessageBox.Show("Окно сотрудника услуг пока не реализовано");
                            // ServicesMainWindow serviceWindow = new ServicesMainWindow();
                            // serviceWindow.Show();
                        }
                        else if (user.Role.Name == "Системный администратор")
                        {
                            MessageBox.Show("Окно системного администратора пока не реализовано");
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

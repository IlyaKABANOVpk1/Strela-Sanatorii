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
            string login = txtLogin.Text;
            string password = txtPassword.Password;

            using (ApplicationContext db = new ApplicationContext())
            {
                // Ищем пользователя и подгружаем его роль (Include)
                var user = db.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Login == login && u.PasswordHash == password);

                if (user != null)
                {
                    // Логика перехода в зависимости от роли
                    if (user.Role.Name == "Администратор")
                    {
                        //AdminMainWindow adminWin = new AdminMainWindow();
                        //adminWin.Show();
                    }
                    else if (user.Role.Name == "Сотрудник доп. услуг")
                    {
                        //ServicesMainWindow serviceWin = new ServicesMainWindow();
                        //serviceWin.Show();
                    }

                    this.Close(); // Закрываем окно логина
                }
                else
                {
                    lblError.Text = "Неверный логин или пароль";
                    lblError.Visibility = Visibility.Visible;
                }
            }
        }
    }
}

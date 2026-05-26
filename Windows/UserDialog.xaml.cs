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
    /// Логика взаимодействия для UserDialog.xaml
    /// </summary>
    public partial class UserDialog : Window
    {
        public UserDialog()
        {
            InitializeComponent();

            using (var db = new ApplicationContext())
            {
                cmbRole.ItemsSource = db.Roles.ToList();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Введите логин.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО пользователя.");
                return;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов.");
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                // Проверка уникальности логина
                if (db.Users.Any(u => u.Login == txtLogin.Text.Trim()))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    return;
                }

                var user = new User
                {
                    Login = txtLogin.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    PasswordHash = PasswordService.HashPassword(txtPassword.Password),
                    RoleId = (cmbRole.SelectedItem as Role).Id
                };

                db.Users.Add(user);
                db.SaveChanges();
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

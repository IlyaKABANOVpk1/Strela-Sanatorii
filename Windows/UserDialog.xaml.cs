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
            using (var db = new ApplicationContext())
            {
                var user = new User
                {
                    Login = txtLogin.Text,
                    PasswordHash = txtPassword.Password,
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

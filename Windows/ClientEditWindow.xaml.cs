using Strela_Sanatorii.Models.Accommodation_tables;
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
    /// Логика взаимодействия для ClientEditWindow.xaml
    /// </summary>
    public partial class ClientEditWindow : Window
    {
        private Client _client;

        public ClientEditWindow(Client client = null)
        {
            InitializeComponent();
            _client = client;

            if (_client != null)
            {
                txtFullName.Text = _client.FullName;
                txtPhone.Text = _client.Phone;
                txtPersonnelNumber.Text = _client.PersonnelNumber;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО клиента.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                if (_client == null)
                {
                    var newClient = new Client
                    {
                        FullName = txtFullName.Text.Trim(),
                        Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                        PersonnelNumber = string.IsNullOrWhiteSpace(txtPersonnelNumber.Text) ? null : txtPersonnelNumber.Text.Trim()
                    };
                    db.Clients.Add(newClient);
                }
                else
                {
                    var existing = db.Clients.Find(_client.Id);
                    if (existing != null)
                    {
                        existing.FullName = txtFullName.Text.Trim();
                        existing.Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim();
                        existing.PersonnelNumber = string.IsNullOrWhiteSpace(txtPersonnelNumber.Text) ? null : txtPersonnelNumber.Text.Trim();
                    }
                }

                try
                {
                    db.SaveChanges();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                    return;
                }
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

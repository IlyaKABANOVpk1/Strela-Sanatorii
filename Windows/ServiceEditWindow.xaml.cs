using Strela_Sanatorii.Models.Additional_service_tables;
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
    /// Логика взаимодействия для ServiceEditWindow.xaml
    /// </summary>
    public partial class ServiceEditWindow : Window
    {
        private AdditionalService _service;

        public ServiceEditWindow(AdditionalService service = null)
        {
            InitializeComponent();
            _service = service;

            if (_service != null)
            {
                txtName.Text = _service.Name;
                txtPrice.Text = _service.Price.ToString("F2");
                txtWorkStart.Text = _service.WorkStart.ToString(@"hh\:mm");
                txtWorkEnd.Text = _service.WorkEnd.ToString(@"hh\:mm");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                !decimal.TryParse(txtPrice.Text, out decimal price) ||
                !TimeSpan.TryParse(txtWorkStart.Text, out TimeSpan workStart) ||
                !TimeSpan.TryParse(txtWorkEnd.Text, out TimeSpan workEnd))
            {
                MessageBox.Show("Введите корректные данные.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                if (_service == null)
                {
                    var newService = new AdditionalService
                    {
                        Name = txtName.Text,
                        Price = price,
                        WorkStart = workStart,
                        WorkEnd = workEnd
                    };
                    db.AdditionalServices.Add(newService);
                }
                else
                {
                    var existing = db.AdditionalServices.Find(_service.Id);
                    if (existing != null)
                    {
                        existing.Name = txtName.Text;
                        existing.Price = price;
                        existing.WorkStart = workStart;
                        existing.WorkEnd = workEnd;
                    }
                }
                db.SaveChanges();
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

using Strela_Sanatorii.Models.Additional_service_tables;
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
    /// Логика взаимодействия для ServicesListPage.xaml
    /// </summary>
    public partial class ServicesListPage : Page
    {
        private List<AdditionalService> _services;

        public ServicesListPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new ApplicationContext())
            {
                _services = db.AdditionalServices.ToList();
                ServicesGrid.ItemsSource = _services;
            }
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            var window = new ServiceEditWindow();
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesGrid.SelectedItem is AdditionalService service)
            {
                using (var db = new ApplicationContext())
                {
                    var item = db.AdditionalServices.Find(service.Id);
                    if (item != null)
                    {
                        db.AdditionalServices.Remove(item);
                        db.SaveChanges();
                    }
                }
                LoadData();
            }
            else
            {
                MessageBox.Show("Выберите услугу для удаления.");
            }
        }

        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesGrid.SelectedItem is AdditionalService service)
            {
                var window = new ServiceEditWindow(service);
                if (window.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу для редактирования.");
            }
        }
    }
}

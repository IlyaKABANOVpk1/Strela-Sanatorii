using Strela_Sanatorii.Models.Additional_service_tables;
using Strela_Sanatorii.Utils;
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
    /// Логика взаимодействия для PackageAddonWindow.xaml
    /// </summary>
    public partial class PackageAddonWindow : Window
    {
        private int _bookingId;
        private List<AddonItem> _addons = new List<AddonItem>();

        public PackageAddonWindow(int bookingId, string currentPackageName)
        {
            InitializeComponent();
            _bookingId = bookingId;
            txtCurrentPackage.Text = currentPackageName ?? "Базовый пакет не назначен";
            LoadServices();
        }

        private void LoadServices()
        {
            using (var db = new ApplicationContext())
            {
                // Показываем только те услуги, которых нет в текущем пакете
                var bookedServiceIds = db.BookingAddons
                    .Where(ba => ba.BookingId == _bookingId)
                    .Select(ba => ba.AdditionalServiceId)
                    .ToList();

                cmbService.ItemsSource = db.AdditionalServices
                    .Where(s => !bookedServiceIds.Contains(s.Id))
                    .OrderBy(s => s.Name)
                    .ToList();
            }
        }

        private void AddAddon_Click(object sender, RoutedEventArgs e)
        {
            if (cmbService.SelectedItem == null) return;
            if (!ValidationHelper.IsPositiveInt(txtQuantity.Text, out int qty)) return;

            var service = cmbService.SelectedItem as AdditionalService;
            _addons.Add(new AddonItem
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                Quantity = qty,
                Total = service.Price * qty
            });

            AddonsGrid.ItemsSource = null;
            AddonsGrid.ItemsSource = _addons;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!_addons.Any())
            {
                MessageBox.Show("Добавьте хотя бы одну услугу.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                foreach (var addon in _addons)
                {
                    db.BookingAddons.Add(new BookingAddon
                    {
                        BookingId = _bookingId,
                        AdditionalServiceId = addon.ServiceId,
                        Quantity = addon.Quantity,
                        AddedAt = DateTime.Now
                    });
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

    public class AddonItem
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}

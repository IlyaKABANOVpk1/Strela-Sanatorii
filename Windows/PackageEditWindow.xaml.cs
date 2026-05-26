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
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace Strela_Sanatorii.Windows
{
    /// <summary>
    /// Логика взаимодействия для PackageEditWindow.xaml
    /// </summary>
    public partial class PackageEditWindow : Window
    {
        private ServicePackage _package;
        private List<PackageItemViewModel> _items = new List<PackageItemViewModel>();

        public PackageEditWindow(ServicePackage package = null)
        {
            InitializeComponent();
            _package = package;
            LoadServices();

            if (_package != null)
            {
                txtName.Text = _package.Name;
                txtDescription.Text = _package.Description;

                using (var db = new ApplicationContext())
                {
                    _items = db.PackageItems
                        .Where(pi => pi.ServicePackageId == _package.Id)
                        .Select(pi => new PackageItemViewModel
                        {
                            Id = pi.Id,
                            ServiceId = pi.AdditionalServiceId,
                            ServiceName = pi.AdditionalService.Name,
                            UnitPrice = pi.AdditionalService.Price,
                            Quantity = pi.Quantity,
                            Total = pi.AdditionalService.Price * pi.Quantity
                        })
                        .ToList();
                }
                RefreshItemsGrid();
            }
        }

        private void LoadServices()
        {
            using (var db = new ApplicationContext())
            {
                cmbService.ItemsSource = db.AdditionalServices.OrderBy(s => s.Name).ToList();
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (cmbService.SelectedItem == null)
            {
                MessageBox.Show("Выберите услугу.");
                return;
            }

            if (!ValidationHelper.IsPositiveInt(txtQuantity.Text, out int qty))
            {
                MessageBox.Show("Введите корректное количество.");
                return;
            }

            var service = cmbService.SelectedItem as AdditionalService;

            // Проверка на дубли
            if (_items.Any(i => i.ServiceId == service.Id))
            {
                MessageBox.Show("Эта услуга уже добавлена в пакет.");
                return;
            }

            _items.Add(new PackageItemViewModel
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                UnitPrice = service.Price,
                Quantity = qty,
                Total = service.Price * qty
            });

            RefreshItemsGrid();
            cmbService.SelectedIndex = -1;
            txtQuantity.Text = "1";
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn.DataContext as PackageItemViewModel;
            _items.Remove(item);
            RefreshItemsGrid();
        }

        private void RefreshItemsGrid()
        {
            ItemsGrid.ItemsSource = null;
            ItemsGrid.ItemsSource = _items;
            var total = _items.Sum(i => i.Total);
            txtTotalPrice.Text = $"Итого: {total:C}";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название пакета.");
                return;
            }

            if (!_items.Any())
            {
                MessageBox.Show("Добавьте хотя бы одну услугу в пакет.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                if (_package == null)
                {
                    // Создание нового пакета
                    var newPackage = new ServicePackage
                    {
                        Name = txtName.Text.Trim(),
                        Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
                        TotalPrice = _items.Sum(i => i.Total)
                    };
                    db.ServicePackages.Add(newPackage);
                    db.SaveChanges();

                    // Добавляем элементы
                    foreach (var item in _items)
                    {
                        db.PackageItems.Add(new PackageItem
                        {
                            ServicePackageId = newPackage.Id,
                            AdditionalServiceId = item.ServiceId,
                            Quantity = item.Quantity
                        });
                    }
                }
                else
                {
                    // Редактирование
                    var existing = db.ServicePackages.Find(_package.Id);
                    if (existing != null)
                    {
                        existing.Name = txtName.Text.Trim();
                        existing.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim();
                        existing.TotalPrice = _items.Sum(i => i.Total);

                        // Удаляем старые элементы
                        var oldItems = db.PackageItems.Where(pi => pi.ServicePackageId == existing.Id);
                        db.PackageItems.RemoveRange(oldItems);

                        // Добавляем новые
                        foreach (var item in _items)
                        {
                            db.PackageItems.Add(new PackageItem
                            {
                                ServicePackageId = existing.Id,
                                AdditionalServiceId = item.ServiceId,
                                Quantity = item.Quantity
                            });
                        }
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

    // ViewModel для отображения в таблице
    public class PackageItemViewModel
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}

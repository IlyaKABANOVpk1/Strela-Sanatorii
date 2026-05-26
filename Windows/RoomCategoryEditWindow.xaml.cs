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
    /// Логика взаимодействия для RoomCategoryEditWindow.xaml
    /// </summary>
    public partial class RoomCategoryEditWindow : Window
    {
        private RoomCategory _category;

        public RoomCategoryEditWindow(RoomCategory category = null)
        {
            InitializeComponent();
            _category = category;

            if (_category != null)
            {
                txtName.Text = _category.Name;
                txtDescription.Text = _category.Description;
                txtBasePrice.Text = _category.BasePrice.ToString("F2");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название категории.");
                return;
            }

            if (!decimal.TryParse(txtBasePrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную положительную цену.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                if (_category == null)
                {
                    db.RoomCategories.Add(new RoomCategory
                    {
                        Name = txtName.Text.Trim(),
                        Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
                        BasePrice = price
                    });
                }
                else
                {
                    var existing = db.RoomCategories.Find(_category.Id);
                    if (existing != null)
                    {
                        existing.Name = txtName.Text.Trim();
                        existing.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim();
                        existing.BasePrice = price;
                    }
                }

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
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

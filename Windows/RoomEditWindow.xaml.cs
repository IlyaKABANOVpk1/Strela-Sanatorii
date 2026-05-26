using Strela_Sanatorii.Models.Accommodation_tables;
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
    /// Логика взаимодействия для RoomEditWindow.xaml
    /// </summary>
    public partial class RoomEditWindow : Window
    {
        private Room _room;

        public RoomEditWindow(Room room = null)
        {
            InitializeComponent();
            _room = room;

            LoadCategories();

            if (_room != null)
            {
                txtRoomNumber.Text = _room.RoomNumber;
                cmbCategory.SelectedValue = _room.RoomCategoryId;
                txtCapacity.Text = _room.Capacity.ToString();
            }
        }

        private void LoadCategories()
        {
            using (var db = new ApplicationContext())
            {
                cmbCategory.ItemsSource = db.RoomCategories.OrderBy(c => c.Name).ToList();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                MessageBox.Show("Введите номер комнаты.");
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.");
                return;
            }

            if (!ValidationHelper.IsPositiveInt(txtCapacity.Text, out int capacity))
            {
                MessageBox.Show("Вместимость должна быть положительным числом.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                if (_room == null)
                {
                    db.Rooms.Add(new Room
                    {
                        RoomNumber = txtRoomNumber.Text.Trim(),
                        RoomCategoryId = (int)cmbCategory.SelectedValue,
                        Capacity = capacity
                    });
                }
                else
                {
                    var existing = db.Rooms.Find(_room.Id);
                    if (existing != null)
                    {
                        existing.RoomNumber = txtRoomNumber.Text.Trim();
                        existing.RoomCategoryId = (int)cmbCategory.SelectedValue;
                        existing.Capacity = capacity;
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

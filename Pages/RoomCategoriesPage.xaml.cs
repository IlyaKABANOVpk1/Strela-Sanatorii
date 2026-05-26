using Strela_Sanatorii.Models.Accommodation_tables;
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
    /// Логика взаимодействия для RoomCategoriesPage.xaml
    /// </summary>
    public partial class RoomCategoriesPage : Page
    {
        public RoomCategoriesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new ApplicationContext())
            {
                CategoriesGrid.ItemsSource = db.RoomCategories.OrderBy(c => c.Name).ToList();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new RoomCategoryEditWindow();
            if (window.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is RoomCategory category)
            {
                var window = new RoomCategoryEditWindow(category);
                if (window.ShowDialog() == true)
                    LoadData();
            }
            else
            {
                MessageBox.Show("Выберите категорию для редактирования.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is RoomCategory category)
            {
                var result = MessageBox.Show($"Удалить категорию {category.Name}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new ApplicationContext())
                    {
                        var c = db.RoomCategories.Find(category.Id);
                        if (c != null)
                        {
                            bool hasRooms = db.Rooms.Any(r => r.RoomCategoryId == c.Id);
                            if (hasRooms)
                            {
                                MessageBox.Show("Невозможно удалить категорию, к которой привязаны номера.");
                                return;
                            }

                            db.RoomCategories.Remove(c);
                            db.SaveChanges();
                        }
                    }
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию для удаления.");
            }
        }
    }
}

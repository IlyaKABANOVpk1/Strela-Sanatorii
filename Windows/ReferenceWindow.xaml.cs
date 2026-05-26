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
    /// Логика взаимодействия для ReferenceWindow.xaml
    /// </summary>
    public partial class ReferenceWindow : Window
    {
        public ReferenceWindow()
        {
            InitializeComponent();
            cmbReferenceType.SelectedIndex = 0;
        }

        private void ReferenceType_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (cmbReferenceType.SelectedItem == null) return;

            var selected = (cmbReferenceType.SelectedItem as ComboBoxItem).Content.ToString();

            switch (selected)
            {
                case "Категории номеров":
                    LoadRoomCategories();
                    break;
                case "Дополнительные услуги":
                    LoadAdditionalServices();
                    break;
                case "Роли пользователей":
                    LoadRoles();
                    break;
            }
        }

        private void LoadRoomCategories()
        {
            using (var db = new ApplicationContext())
            {
                var data = db.RoomCategories
                    .Select(c => new { c.Id, c.Name, c.Description, c.BasePrice })
                    .ToList();

                ReferenceGrid.Columns.Clear();
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 50 });
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new System.Windows.Data.Binding("Name"), Width = 150 });
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "Описание", Binding = new System.Windows.Data.Binding("Description"), Width = 200 });
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "Базовая цена", Binding = new System.Windows.Data.Binding("BasePrice") { StringFormat = "C" }, Width = 120 });

                ReferenceGrid.ItemsSource = data;
            }
        }

        private void LoadAdditionalServices()
        {
            using (var db = new ApplicationContext())
            {
                var data = db.AdditionalServices
                    .Select(s => new { s.Id, s.Name, s.Price, s.WorkStart, s.WorkEnd })
                    .ToList();

                ReferenceGrid.Columns.Clear();
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 50 });
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new System.Windows.Data.Binding("Name"), Width = 200 });
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "Цена", Binding = new System.Windows.Data.Binding("Price") { StringFormat = "C" }, Width = 100 });
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "Начало", Binding = new System.Windows.Data.Binding("WorkStart"), Width = 80 });
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "Окончание", Binding = new System.Windows.Data.Binding("WorkEnd"), Width = 80 });

                ReferenceGrid.ItemsSource = data;
            }
        }

        private void LoadRoles()
        {
            using (var db = new ApplicationContext())
            {
                var data = db.Roles.ToList();

                ReferenceGrid.Columns.Clear();
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new System.Windows.Data.Binding("Id"), Width = 50 });
                ReferenceGrid.Columns.Add(new DataGridTextColumn { Header = "Название роли", Binding = new System.Windows.Data.Binding("Name"), Width = 300 });

                ReferenceGrid.ItemsSource = data;
            }
        }
    }
}

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
    /// Логика взаимодействия для GuestSearchWindow.xaml
    /// </summary>
    public partial class GuestSearchWindow : Window
    {
        public Guest SelectedGuest { get; private set; }

        public GuestSearchWindow()
        {
            InitializeComponent();
            LoadAllGuests();
        }

        private void LoadAllGuests()
        {
            using (var db = new ApplicationContext())
            {
                var guests = db.Guests
                    .OrderBy(g => g.LastName)
                    .ThenBy(g => g.FirstName)
                    .ToList();

                ResultsGrid.ItemsSource = guests;
            }
        }

        private void Search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string search = txtSearch.Text?.Trim().ToLower() ?? "";
            if (string.IsNullOrEmpty(search))
            {
                LoadAllGuests();
                return;
            }

            string filterType = (cmbFilter.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Всем полям";

            using (var db = new ApplicationContext())
            {
                var query = db.Guests.AsQueryable();

                switch (filterType)
                {
                    case "Фамилии":
                        query = query.Where(g => g.LastName.ToLower().Contains(search));
                        break;
                    case "Табельному":
                        query = query.Where(g => g.PersonnelNumber != null && g.PersonnelNumber.ToLower().Contains(search));
                        break;
                    case "Паспорту":
                        query = query.Where(g =>
                            (g.PassportSeries != null && g.PassportSeries.ToLower().Contains(search)) ||
                            (g.PassportNumber != null && g.PassportNumber.ToLower().Contains(search)));
                        break;
                    default: // Всем полям
                        query = query.Where(g =>
                            g.LastName.ToLower().Contains(search) ||
                            g.FirstName.ToLower().Contains(search) ||
                            (g.MiddleName != null && g.MiddleName.ToLower().Contains(search)) ||
                            (g.PersonnelNumber != null && g.PersonnelNumber.ToLower().Contains(search)) ||
                            (g.PassportSeries != null && g.PassportSeries.ToLower().Contains(search)) ||
                            (g.PassportNumber != null && g.PassportNumber.ToLower().Contains(search)));
                        break;
                }

                ResultsGrid.ItemsSource = query
                    .OrderBy(g => g.LastName)
                    .ThenBy(g => g.FirstName)
                    .ToList();
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            LoadAllGuests();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsGrid.SelectedItem is Guest guest)
            {
                SelectedGuest = guest;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Выберите гостя из списка.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ResultsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Select_Click(sender, e);
        }
    }
}

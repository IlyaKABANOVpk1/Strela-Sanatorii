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
    /// Логика взаимодействия для ShiftEditWindow.xaml
    /// </summary>
    public partial class ShiftEditWindow : Window
    {
        private Shift _shift;

        public ShiftEditWindow(Shift shift = null)
        {
            InitializeComponent();
            _shift = shift;

            if (_shift != null)
            {
                txtName.Text = _shift.Name;
                dpStart.SelectedDate = _shift.StartDate;
                dpEnd.SelectedDate = _shift.EndDate;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название смены.");
                return;
            }

            if (dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
            {
                MessageBox.Show("Выберите даты начала и окончания.");
                return;
            }

            DateTime start = dpStart.SelectedDate.Value;
            DateTime end = dpEnd.SelectedDate.Value;

            if (end < start)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала.");
                return;
            }

            using (var db = new ApplicationContext())
            {
                if (_shift == null)
                {
                    db.Shifts.Add(new Shift
                    {
                        Name = txtName.Text.Trim(),
                        StartDate = start,
                        EndDate = end
                    });
                }
                else
                {
                    var existing = db.Shifts.Find(_shift.Id);
                    if (existing != null)
                    {
                        existing.Name = txtName.Text.Trim();
                        existing.StartDate = start;
                        existing.EndDate = end;
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

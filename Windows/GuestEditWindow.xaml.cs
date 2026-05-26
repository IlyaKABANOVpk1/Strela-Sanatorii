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
    /// Логика взаимодействия для GuestEditWindow.xaml
    /// </summary>
    public partial class GuestEditWindow : Window
    {
        private Guest _guest;

        public GuestEditWindow(Guest guest = null)
        {
            InitializeComponent();
            _guest = guest;

            if (_guest != null)
            {
                // Основное
                txtLastName.Text = _guest.LastName;
                txtFirstName.Text = _guest.FirstName;
                txtMiddleName.Text = _guest.MiddleName;
                dpBirthDate.SelectedDate = _guest.BirthDate;
                cmbGender.Text = _guest.Gender;
                txtPhone.Text = _guest.Phone;
                txtPersonnelNumber.Text = _guest.PersonnelNumber;

                // Паспорт
                txtPassportSeries.Text = _guest.PassportSeries;
                txtPassportNumber.Text = _guest.PassportNumber;
                txtSnils.Text = _guest.Snils;

                // Контактное лицо
                txtEmergencyName.Text = _guest.EmergencyContactName;
                txtEmergencyPhone.Text = _guest.EmergencyContactPhone;

                // Медицинское
                txtAllergies.Text = _guest.Allergies;
                txtContraindications.Text = _guest.Contraindications;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // ВАЛИДАЦИЯ
            if (!Validate()) return;

            using (var db = new ApplicationContext())
            {
                if (_guest == null)
                {
                    var newGuest = new Guest
                    {
                        LastName = txtLastName.Text.Trim(),
                        FirstName = txtFirstName.Text.Trim(),
                        MiddleName = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? null : txtMiddleName.Text.Trim(),
                        BirthDate = dpBirthDate.SelectedDate,
                        Gender = (cmbGender.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                        Phone = ValidationHelper.FormatPhone(txtPhone.Text),
                        PersonnelNumber = txtPersonnelNumber.Text.Trim(),
                        PassportSeries = string.IsNullOrWhiteSpace(txtPassportSeries.Text) ? null : txtPassportSeries.Text.Trim(),
                        PassportNumber = string.IsNullOrWhiteSpace(txtPassportNumber.Text) ? null : txtPassportNumber.Text.Trim(),
                        Snils = string.IsNullOrWhiteSpace(txtSnils.Text) ? null : txtSnils.Text.Trim(),
                        EmergencyContactName = string.IsNullOrWhiteSpace(txtEmergencyName.Text) ? null : txtEmergencyName.Text.Trim(),
                        EmergencyContactPhone = ValidationHelper.FormatPhone(txtEmergencyPhone.Text),
                        Allergies = string.IsNullOrWhiteSpace(txtAllergies.Text) ? null : txtAllergies.Text.Trim(),
                        Contraindications = string.IsNullOrWhiteSpace(txtContraindications.Text) ? null : txtContraindications.Text.Trim()
                    };
                    db.Guests.Add(newGuest);
                }
                else
                {
                    var existing = db.Guests.Find(_guest.Id);
                    if (existing != null)
                    {
                        existing.LastName = txtLastName.Text.Trim();
                        existing.FirstName = txtFirstName.Text.Trim();
                        existing.MiddleName = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? null : txtMiddleName.Text.Trim();
                        existing.BirthDate = dpBirthDate.SelectedDate;
                        existing.Gender = (cmbGender.SelectedItem as ComboBoxItem)?.Content?.ToString();
                        existing.Phone = ValidationHelper.FormatPhone(txtPhone.Text);
                        existing.PersonnelNumber = txtPersonnelNumber.Text.Trim();
                        existing.PassportSeries = string.IsNullOrWhiteSpace(txtPassportSeries.Text) ? null : txtPassportSeries.Text.Trim();
                        existing.PassportNumber = string.IsNullOrWhiteSpace(txtPassportNumber.Text) ? null : txtPassportNumber.Text.Trim();
                        existing.Snils = string.IsNullOrWhiteSpace(txtSnils.Text) ? null : txtSnils.Text.Trim();
                        existing.EmergencyContactName = string.IsNullOrWhiteSpace(txtEmergencyName.Text) ? null : txtEmergencyName.Text.Trim();
                        existing.EmergencyContactPhone = ValidationHelper.FormatPhone(txtEmergencyPhone.Text);
                        existing.Allergies = string.IsNullOrWhiteSpace(txtAllergies.Text) ? null : txtAllergies.Text.Trim();
                        existing.Contraindications = string.IsNullOrWhiteSpace(txtContraindications.Text) ? null : txtContraindications.Text.Trim();
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

        private bool Validate()
        {
            // Фамилия
            if (!ValidationHelper.IsValidName(txtLastName.Text))
            {
                MessageBox.Show("Введите корректную фамилию (только буквы).");
                return false;
            }

            // Имя
            if (!ValidationHelper.IsValidName(txtFirstName.Text))
            {
                MessageBox.Show("Введите корректное имя (только буквы).");
                return false;
            }

            // Отчество (если введено)
            if (!string.IsNullOrWhiteSpace(txtMiddleName.Text) && !ValidationHelper.IsValidName(txtMiddleName.Text))
            {
                MessageBox.Show("Введите корректное отчество (только буквы).");
                return false;
            }

            // Табельный номер
            if (!ValidationHelper.IsValidPersonnelNumber(txtPersonnelNumber.Text))
            {
                MessageBox.Show("Табельный номер должен содержать от 3 до 10 цифр.");
                return false;
            }

            // Телефон (если введён)
            if (!string.IsNullOrWhiteSpace(txtPhone.Text) && !ValidationHelper.IsValidPhone(txtPhone.Text))
            {
                MessageBox.Show("Введите телефон в формате +7 (XXX) XXX-XX-XX.");
                return false;
            }

            // Дата рождения не из будущего
            if (dpBirthDate.SelectedDate.HasValue && dpBirthDate.SelectedDate.Value > DateTime.Now)
            {
                MessageBox.Show("Дата рождения не может быть в будущем.");
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

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
    /// Логика взаимодействия для BookingActionsWindow.xaml
    /// </summary>
    public partial class BookingActionsWindow : Window
    {
        public string ActionResult { get; private set; } = "";

        public BookingActionsWindow(string clientName, string roomNumber)
        {
            InitializeComponent();

            txtInfo.Text = $"Номер {roomNumber}\n{clientName}";
        }

        private void Evict_Click(object sender, RoutedEventArgs e)
        {
            ActionResult = "Evict";
            DialogResult = true;
            Close();
        }

        private void Rebook_Click(object sender, RoutedEventArgs e)
        {
            ActionResult = "Rebook";
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

using System.Configuration;
using System.Data;
using System.Windows;

namespace Strela_Sanatorii
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var db = new ApplicationContext())
                {
                    db.Database.EnsureCreated();   // только создаёт структуру таблиц
                    // SeedData() убрали — будем заполнять вручную
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе:\n\n{ex.Message}\n\nПроверь, что PostgreSQL запущен и пароль верный.",
                                "Ошибка запуска", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
    


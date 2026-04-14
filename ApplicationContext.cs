using Microsoft.EntityFrameworkCore;
using Strela_Sanatorii.Models.Accommodation_tables;
using Strela_Sanatorii.Models.Additional_service_tables;
using Strela_Sanatorii.Models.UserTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii
{
    public class ApplicationContext : DbContext
    {
        
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<AdditionalService> AdditionalServices { get; set; }
        public DbSet<ServiceAppointment> ServiceAppointments { get; set; }

        public ApplicationContext()
        {
           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=StrelaSanatoryDb;Username=postgres;Password=1");
        }


        //public void SeedData()
        //{
        //    // Если уже есть хотя бы один номер — значит данные уже были загружены
        //    if (Rooms.Any())
        //        return;

        //    // ====================== НОМЕРА ======================
        //    Rooms.AddRange(
        //        new Room { RoomNumber = "101", Category = "Одноместный", Capacity = 1 },
        //        new Room { RoomNumber = "102", Category = "Двухместный", Capacity = 2 },
        //        new Room { RoomNumber = "103", Category = "Люкс", Capacity = 2 },
        //        new Room { RoomNumber = "104", Category = "Одноместный", Capacity = 1 },
        //        new Room { RoomNumber = "105", Category = "Двухместный", Capacity = 2 },
        //        new Room { RoomNumber = "201", Category = "Люкс", Capacity = 2 }
        //    );

        //    // ====================== СМЕНЫ ======================
        //    Shifts.AddRange(
        //        new Shift
        //        {
        //            Name = "Смена №1 (Июнь 2026)",
        //            StartDate = new DateTime(2026, 6, 1),
        //            EndDate = new DateTime(2026, 6, 14)
        //        },
        //        new Shift
        //        {
        //            Name = "Смена №2 (Июль 2026)",
        //            StartDate = new DateTime(2026, 7, 1),
        //            EndDate = new DateTime(2026, 7, 14)
        //        }
        //    );

        //    // ====================== КЛИЕНТЫ ======================
        //    Clients.AddRange(
        //        new Client { FullName = "Иванов Иван Иванович", Phone = "+79001234567", PersonnelNumber = "10001" },
        //        new Client { FullName = "Петров Пётр Петрович", Phone = "+79007654321", PersonnelNumber = "10002" },
        //        new Client { FullName = "Сидорова Анна Сергеевна", Phone = "+79009876543", PersonnelNumber = "10003" },
        //        new Client { FullName = "Кузнецов Дмитрий", Phone = "+79005551234", PersonnelNumber = "10004" }
        //    );

        //    // ====================== ДОПОЛНИТЕЛЬНЫЕ УСЛУГИ ======================
        //    AdditionalServices.AddRange(
        //        new AdditionalService
        //        {
        //            Name = "Массаж",
        //            Price = 1500,
        //            WorkStart = new TimeSpan(10, 0, 0),
        //            WorkEnd = new TimeSpan(17, 0, 0)
        //        },
        //        new AdditionalService
        //        {
        //            Name = "ЛФК (лечебная физкультура)",
        //            Price = 800,
        //            WorkStart = new TimeSpan(9, 0, 0),
        //            WorkEnd = new TimeSpan(15, 0, 0)
        //        },
        //        new AdditionalService
        //        {
        //            Name = "Посещение бассейна",
        //            Price = 500,
        //            WorkStart = new TimeSpan(8, 0, 0),
        //            WorkEnd = new TimeSpan(20, 0, 0)
        //        }
        //    );

        //    // ====================== ПОЛЬЗОВАТЕЛИ ======================
        //    Users.AddRange(
        //        new User
        //        {
        //            Login = "admin",
        //            PasswordHash = "123",
        //            FullName = "Главный администратор",
        //            RoleId = 1
        //        },
        //        new User
        //        {
        //            Login = "service",
        //            PasswordHash = "123",
        //            FullName = "Сотрудник доп. услуг",
        //            RoleId = 2
        //        }
        //    );

        //    SaveChanges();

        //    // Можно добавить тестовые брони и записи на услуги (в дальнейшем)
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

           
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.PersonnelNumber)
                .IsUnique();

        
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Администратор" },
                new Role { Id = 2, Name = "Сотрудник доп. услуг" },
                new Role { Id = 3, Name = "Системный администратор"}
            );
        }
    }
}

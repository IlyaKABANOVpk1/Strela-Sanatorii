using Microsoft.EntityFrameworkCore;
using Strela_Sanatorii.Models.Accommodation_tables;
using Strela_Sanatorii.Models.Additional_service_tables;
using Strela_Sanatorii.Models.Medical_tables;
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
        // Пользователи и роли
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        // Размещение
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomCategory> RoomCategories { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        // Дополнительные услуги
        public DbSet<AdditionalService> AdditionalServices { get; set; }
        public DbSet<ServiceAppointment> ServiceAppointments { get; set; }
        public DbSet<ServicePackage> ServicePackages { get; set; }
        public DbSet<PackageItem> PackageItems { get; set; }
        public DbSet<BookingAddon> BookingAddons { get; set; }
        public DbSet<GuestServiceSchedule> GuestServiceSchedules { get; set; }

        // Медицинские назначения
        public DbSet<MedicalPrescription> MedicalPrescriptions { get; set; }

        static ApplicationContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=StrelaSanatoryDb;Username=postgres;Password=1");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальные индексы
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Guest>()
                .HasIndex(g => g.PersonnelNumber)
                .IsUnique();

            // Связь Booking -> GuestServiceSchedule
            modelBuilder.Entity<GuestServiceSchedule>()
                .HasOne(s => s.Booking)
                .WithMany(b => b.ServiceSchedules)
                .HasForeignKey(s => s.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь GuestServiceSchedule -> Guest
            modelBuilder.Entity<GuestServiceSchedule>()
                .HasOne(s => s.Guest)
                .WithMany()
                .HasForeignKey(s => s.GuestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь MedicalPrescription -> Guest
            modelBuilder.Entity<MedicalPrescription>()
                .HasOne(p => p.Guest)
                .WithMany()
                .HasForeignKey(p => p.GuestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь MedicalPrescription -> Booking
            modelBuilder.Entity<MedicalPrescription>()
                .HasOne(p => p.Booking)
                .WithMany()
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь MedicalPrescription -> Doctor (User)
            modelBuilder.Entity<MedicalPrescription>()
                .HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Начальные данные ролей
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Администратор" },
                new Role { Id = 2, Name = "Сотрудник доп. услуг" },
                new Role { Id = 3, Name = "Врач" },
                new Role { Id = 4, Name = "Медработник" },
                new Role { Id = 5, Name = "Суперпользователь" }
            );
        }
    }
}

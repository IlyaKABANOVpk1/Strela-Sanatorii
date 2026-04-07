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

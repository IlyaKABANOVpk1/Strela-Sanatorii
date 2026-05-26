using Strela_Sanatorii.Models.Additional_service_tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Accommodation_tables
{
    public enum BookingStatus
    {
        CheckedIn,      // Заселён
        InProgress,     // В процессе
        CheckedOut      // Выселен
    }

    public class Booking
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public virtual Guest Guest { get; set; }

        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        public int ShiftId { get; set; }
        public virtual Shift Shift { get; set; }

        // === ПАКЕТ УСЛУГ ===
        public int? ServicePackageId { get; set; }
        public virtual ServicePackage ServicePackage { get; set; }

        // === СЕМЬЯ ===
        public bool IsFamily { get; set; } = false;           // С семьёй
        public int GuestCount { get; set; } = 1;              // Количество человек (1 по умолчанию)
        // ===================

        public BookingStatus Status { get; set; } = BookingStatus.CheckedIn;
        public DateTime? CheckOutDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<GuestServiceSchedule> ServiceSchedules { get; set; }
    }
}

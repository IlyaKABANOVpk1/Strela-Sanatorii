using Strela_Sanatorii.Models.Accommodation_tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Additional_service_tables
{
    public enum ScheduleStatus
    {
        Assigned,      // Назначено
        Confirmed,     // Подтверждено гостем
        Completed,     // Выполнено
        Cancelled      // Отменено
    }

    public class GuestServiceSchedule
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public virtual Guest Guest { get; set; }  // <-- ДОБАВЛЕНО

        public int? BookingId { get; set; }
        public virtual Booking Booking { get; set; }  // <-- ДОБАВЛЕНО

        public int ServiceId { get; set; }
        public virtual AdditionalService Service { get; set; }

        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }

        public ScheduleStatus Status { get; set; } = ScheduleStatus.Assigned;
        public string Notes { get; set; }

        public int? ConfirmedByUserId { get; set; }
        public DateTime? ConfirmedAt { get; set; }
    }
}

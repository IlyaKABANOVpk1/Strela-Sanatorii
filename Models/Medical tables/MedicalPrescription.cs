using Strela_Sanatorii.Models.Accommodation_tables;
using Strela_Sanatorii.Models.UserTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Medical_tables
{
    public enum PrescriptionStatus
    {
        Assigned,      // Назначено
        InProgress,    // В процессе выполнения
        Completed,     // Выполнено
        Cancelled      // Отменено
    }

    public class MedicalPrescription
    {
        public int Id { get; set; }

        public int GuestId { get; set; }
        public virtual Guest Guest { get; set; }

        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }

        // Диагноз
        public string Diagnosis { get; set; }

        // Назначенные лекарства
        public string Medications { get; set; }

        // Назначенные процедуры (текстовое описание + связь с услугами через отдельную таблицу если нужно)
        public string Procedures { get; set; }

        // Дополнительные рекомендации
        public string Recommendations { get; set; }

        // Кто назначил
        public int DoctorId { get; set; }
        public virtual User Doctor { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Assigned;

        // Дата завершения курса
        public DateTime? CompletedAt { get; set; }
    }
}

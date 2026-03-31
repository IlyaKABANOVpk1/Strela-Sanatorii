using Strela_Sanatorii.Models.Accommodation_tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Additional_service_tables
{
    public class ServiceAppointment
    {
        public int Id { get; set; }

        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        public int ServiceId { get; set; }
        public virtual AdditionalService Service { get; set; }

        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; } // Время слота (например, 11:00)

        public bool IsPaid { get; set; } = false;
    }
}

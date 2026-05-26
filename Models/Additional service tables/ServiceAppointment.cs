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

        public int GuestId { get; set; }
        public virtual Guest Guest { get; set; }

        public int ServiceId { get; set; }
        public virtual AdditionalService Service { get; set; }

        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }

        public bool IsPaid { get; set; } = false;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Accommodation_tables
{
    public class Booking
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        public int ShiftId { get; set; }
        public virtual Shift Shift { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

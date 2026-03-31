using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Accommodation_tables
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string Category { get; set; } // Одноместный, Люкс и т.д.
        public int Capacity { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}

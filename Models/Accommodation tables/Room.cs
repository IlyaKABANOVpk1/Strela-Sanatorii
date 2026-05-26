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

        public int RoomCategoryId { get; set; }           // ВМЕСТО строки Category
        public virtual RoomCategory RoomCategory { get; set; }

        public int Capacity { get; set; }                  // Вместимость (1, 2, 4 и т.д.)

        public virtual ICollection<Booking> Bookings { get; set; }
}
}

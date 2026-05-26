using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Accommodation_tables
{
    public class RoomCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }           // Одноместный, Двухместный, Люкс, Семейный
        public string Description { get; set; }    // Описание категории
        public decimal BasePrice { get; set; }     // Базовая цена за смену

        public virtual ICollection<Room> Rooms { get; set; }
    }
}

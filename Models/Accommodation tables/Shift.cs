using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Accommodation_tables
{
    public class Shift
    {
        public int Id { get; set; }
        public string Name { get; set; } // Смена №1 Август
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

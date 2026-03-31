using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Additional_service_tables
{
    public class AdditionalService
    {
        public int Id { get; set; }
        public string Name { get; set; } // Массаж, ЛФК
        public decimal Price { get; set; }
        public TimeSpan WorkStart { get; set; } // 10:00
        public TimeSpan WorkEnd { get; set; }   // 17:00
    }
}

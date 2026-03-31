using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Accommodation_tables
{
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PersonnelNumber { get; set; } // Табельный номер АО "Стрела"
        public string Phone { get; set; }
    }
}

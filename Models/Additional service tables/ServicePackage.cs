using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Additional_service_tables
{
    public class ServicePackage
    {
        public int Id { get; set; }
        public string Name { get; set; }           // Название пакета
        public string Description { get; set; }    // Описание
        public decimal TotalPrice { get; set; }    // Общая стоимость пакета

        public virtual ICollection<PackageItem> Items { get; set; }
}
}

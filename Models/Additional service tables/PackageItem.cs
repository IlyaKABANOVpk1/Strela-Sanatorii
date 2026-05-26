using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Additional_service_tables
{
    public class PackageItem
    {
        public int Id { get; set; }

        public int ServicePackageId { get; set; }
        public virtual ServicePackage ServicePackage { get; set; }

        public int AdditionalServiceId { get; set; }
        public virtual AdditionalService AdditionalService { get; set; }

        public int Quantity { get; set; } = 1;    // Количество процедур в пакете
    }
}

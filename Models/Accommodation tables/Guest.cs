using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.Accommodation_tables
{
    public class Guest
    {
        public int Id { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("middle_name")]
        public string MiddleName { get; set; }

        [NotMapped]
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        [Column("personnel_number")]
        public string PersonnelNumber { get; set; }

        // === АНКЕТА (все nullable) ===
        [Column("birth_date")]
        public DateTime? BirthDate { get; set; }

        [Column("gender")]
        public string Gender { get; set; }

        [Column("passport_series")]
        public string PassportSeries { get; set; }

        [Column("passport_number")]
        public string PassportNumber { get; set; }

        [Column("snils")]
        public string Snils { get; set; }

        [Column("emergency_contact_name")]
        public string EmergencyContactName { get; set; }

        [Column("emergency_contact_phone")]
        public string EmergencyContactPhone { get; set; }

        [Column("allergies")]
        public string Allergies { get; set; }

        [Column("contraindications")]
        public string Contraindications { get; set; }
        // ===================

        [Column("phone")]
        public string Phone { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
}
}

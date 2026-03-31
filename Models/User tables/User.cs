using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.UserTables
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }

        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}

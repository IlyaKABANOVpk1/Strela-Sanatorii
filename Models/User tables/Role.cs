using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Models.UserTables
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Администратор", "Сотрудник доп. услуг"

        // Навигационное свойство для связи
        public virtual ICollection<User> Users { get; set; }
    }
}

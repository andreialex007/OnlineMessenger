using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineMessenger.Domain.Entities
{
    public class Role
    {
        public const string OperatorRoleName = "Operator";
        public const string AdministratorRoleName = "Administrator";

        public Role()
        {
            Users = new List<User>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Role name cannot be empty")]
        public string Name { get; set; }

        public virtual List<User> Users { get; set; }
    }
}
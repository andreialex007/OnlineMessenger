using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineMessenger.Domain.Entities
{
    public class User
    {
        public User()
        {
            CreatedDate = DateTime.Now;
            FromMessages = new List<Message>();
            ToMessages = new List<Message>();
            Roles = new List<Role>();
            Clients = new List<User>();
            VisibleToOperators = new List<User>();
            VisibleClients = new List<User>();
            AudioNotificationsEnabled = true;
            VisualNotificationsEnabled = true;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [MinLength(3, ErrorMessage = "Min length must be 3")]
        [MaxLength(250, ErrorMessage = "Min length must be 250")]
        public string Name { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastLogout { get; set; }

        public UserData UserData { get; set; }

        public bool? AudioNotificationsEnabled { get; set; }
        public bool? VisualNotificationsEnabled { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [MinLength(3, ErrorMessage = "Min length must be 3")]
        [MaxLength(250, ErrorMessage = "Min length must be 250")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Not valid email address")]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public bool? IsConnected { get; set; }

        public virtual List<Message> FromMessages { get; set; }
        public virtual List<Message> ToMessages { get; set; }
        public virtual List<Role> Roles { get; set; }

        // consultant property
        public virtual List<User> Clients { get; set; } 
        public virtual User Operator { get; set; }
        public virtual int? OperatorId { get; set; }

        //connected disconnected state visibility
        public virtual List<User> VisibleToOperators { get; set; }
        public virtual List<User> VisibleClients { get; set; }
    }
}
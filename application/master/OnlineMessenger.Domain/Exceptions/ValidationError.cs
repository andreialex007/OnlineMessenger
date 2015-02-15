using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMessenger.Domain.Exceptions
{
    public class ValidationError
    {
        public ValidationError(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}

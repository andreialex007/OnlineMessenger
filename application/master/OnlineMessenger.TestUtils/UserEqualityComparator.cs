using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.TestUtils
{
    public class UserEqualityComparator : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            return x.Id == y.Id
                   && x.CreatedDate == y.CreatedDate
                   && x.Name == y.Name
                   && x.PasswordHash == y.PasswordHash
                   && x.Email == y.Email;
        }

        public int GetHashCode(User obj)
        {
            throw new NotImplementedException();
        }
    }
}

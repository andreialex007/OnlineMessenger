using System.Collections.Generic;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Domain.Tests.Unit
{
    public class UserEqualityComparator : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            return x.Email == y.Email && x.Id == y.Id && x.Name == y.Name;
        }

        public int GetHashCode(User obj)
        {
            return 0;
        }
    }
}

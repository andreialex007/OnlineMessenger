using System;
using System.Collections.Generic;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Domain.Infrastructure
{
    public interface IUsersRepository : IRepository<User>
    {
        Dictionary<DateTime, int> UsersPerDay();
    }
}
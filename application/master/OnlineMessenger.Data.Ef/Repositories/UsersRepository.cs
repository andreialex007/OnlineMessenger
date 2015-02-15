using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.Data.Ef.Repositories
{
    public class UsersRepository : RepositoryBase<User>, IUsersRepository
    {
        public UsersRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

        public Dictionary<DateTime, int> UsersPerDay()
        {
            return Query(x => x.Roles)
                .Where(x => !x.Roles.Any())
                .GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate.Value), x => x)
                .ToDictionary(x => x.Key.Value, x => x.Count());
        }
    }
}

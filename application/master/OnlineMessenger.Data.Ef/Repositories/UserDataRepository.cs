using System.Data.Entity;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.Data.Ef.Repositories
{
    public class UserDataRepository : RepositoryBase<UserData>, IUserDataRepository
    {
        public UserDataRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

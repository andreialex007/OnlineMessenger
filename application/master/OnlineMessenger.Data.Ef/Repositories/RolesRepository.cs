using System.Data.Entity;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.Data.Ef.Repositories
{
    public class RolesRepository : RepositoryBase<Role>, IRolesRepository
    {
        public RolesRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}

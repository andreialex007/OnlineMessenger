using System.Linq;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.Domain.Extensions
{
    public static class RolesRepositoryExtensions
    {
        public static IQueryable<Role> GetRolesByIds(this IRolesRepository repository, params int[] ids)
        {
            return repository.Query().Where(x => ids.Contains(x.Id));
        }

        public static Role GetRoleByIds(this IRolesRepository repository, int roleId)
        {
            return repository.Query().Single(x => x.Id == roleId);
        }


        public static Role GetRoleByName(this IRolesRepository repository, string name)
        {
            return repository.Query().SingleOrDefault(x => x.Name == name);
        } 
    }
}

using System.Linq;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Infrastructure;

namespace OnlineMessenger.Domain.Extensions
{
    public static class UserRepositoryExtensions
    {

        public static User GetUserByName(this IUsersRepository repository, string name)
        {
            return repository.Query().SingleOrDefault(x => x.Name == name);
        }

        public static User GetUserByEmail(this IUsersRepository repository, string email)
        {
            return repository.Query().SingleOrDefault(x => x.Email == email);
        }

        public static User GetUserWithDataByName(this IUsersRepository repository, string userName)
        {
            return repository.Query(x => x.UserData).SingleOrDefault(x => x.Name == userName);
        }

        public static User GetUserByNameWithRoles(this IUsersRepository repository, string name)
        {
            return repository.Query(x => x.Roles).SingleOrDefault(x => x.Name == name);
        }

        public static User GetUserById(this IUsersRepository repository, int id)
        {
            return repository.Query().SingleOrDefault(x => x.Id == id);
        }
        public static User GetUserByNameWithVisibleUsersAndRoles(this IUsersRepository repository, string userName)
        {
            return repository.Query(x => x.VisibleToOperators, x => x.Roles).SingleOrDefault(x => x.Name == userName);
        }

        public static IQueryable<User> GetUsersByIds(this IUsersRepository repository, params int[] ids)
        {
            return repository.Query().Where(x => ids.Contains(x.Id));
        }

        public static IQueryable<User> GetUsersByIdsWithRoles(this IUsersRepository repository, params int[] ids)
        {
            return repository.Query(x => x.Roles).Where(x => ids.Contains(x.Id));
        }

        public static IQueryable<User> AllUsers(this IUsersRepository repository)
        {
            return repository.Query();
        }

        public static IQueryable<User> SearchUsersByName(this IUsersRepository repository, string name)
        {
            return repository.Query().Where(x => x.Name.ToLower().Contains(name));
        }

        public static IQueryable<User> GetUsersByNames(this IUsersRepository repository, string[] names)
        {
            return repository.Query().Where(x => names.Contains(x.Name.ToLower()));
        }

        public static IQueryable<User> GetUsersByRole(this IUsersRepository repository, string roleName)
        {
            return repository.Query(x => x.Roles).Where(x => x.Roles.Any(r => r.Name == roleName));
        }
    }
}

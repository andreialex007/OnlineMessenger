using System.Data.Entity;
using OnlineMessenger.Data.Ef.Models;

namespace OnlineMessenger.Data.Ef.Utils
{
    public static class DbContextExtensions
    {
        public static void ClearData(this MessengerDbContext dbContext)
        {
            foreach (var item in new DbSet[] { dbContext.Roles, dbContext.Messages, dbContext.Users, dbContext.UserDatas })
                item.ClearTable();
        }

        private static void ClearTable(this DbSet dbSet)
        {
            foreach (var item in dbSet)
                dbSet.Remove(item);
        }
    }
}

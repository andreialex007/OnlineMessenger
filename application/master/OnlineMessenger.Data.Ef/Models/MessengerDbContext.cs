using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using OnlineMessenger.Data.Ef.Models.Mapping;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Data.Ef.Models
{
    public class MessengerDbContext : DbContext
    {
        static MessengerDbContext()
        {
            Database.SetInitializer<MessengerDbContext>(null);
        }

        public void InitializeDb()
        {
            new AppDatabaseInitializer().InitializeDatabase(this);
        }

        public MessengerDbContext()
            : base("Name=MessengerDbContext")
        {
            Configuration.ProxyCreationEnabled = false;
            if (!Users.Any())
            {
                new AppDatabaseInitializer().InitializeDatabase(this);
            }
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserData> UserDatas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MessageMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}

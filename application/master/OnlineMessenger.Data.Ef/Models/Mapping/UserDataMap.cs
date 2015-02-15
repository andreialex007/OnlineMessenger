using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Data.Ef.Models.Mapping
{
    public class UserDataMap : EntityTypeConfiguration<UserData>
    {

        public UserDataMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            this.ToTable("UserData");
            this.Property(t => t.AvatarImage).HasColumnName("AvatarImage");
            this.Property(t => t.UserId).HasColumnName("UserId");

            this.HasRequired(x => x.User)
                .WithRequiredPrincipal(x => x.UserData)
                .WillCascadeOnDelete(true);
        }
    }
}

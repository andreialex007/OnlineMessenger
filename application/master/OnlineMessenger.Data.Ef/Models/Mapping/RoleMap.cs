using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Data.Ef.Models.Mapping
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            this.Property(t => t.Name)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Roles");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");

            // Relationships
            this.HasMany(t => t.Users)
                .WithMany(t => t.Roles)
                .Map(m =>
                     {
                         m.ToTable("UserRole");
                         m.MapLeftKey("Roles_Id");
                         m.MapRightKey("Users_Id");
                     });


        }
    }
}

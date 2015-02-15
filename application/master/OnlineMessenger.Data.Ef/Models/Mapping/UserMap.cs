using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Data.Ef.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            this.Property(t => t.Name)
                .IsRequired();

            this.Property(t => t.Email)
                .IsRequired();

            this.Property(t => t.PasswordHash)
                .IsRequired();

            this.Map(m => m.MapInheritedProperties());

            // Table & Column Mappings
            this.ToTable("Users");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OperatorId).HasColumnName("OperatorId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.LastLogout).HasColumnName("LastLogout");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            this.Property(t => t.AudioNotificationsEnabled).HasColumnName("AudioNotificationsEnabled");
            this.Property(t => t.VisualNotificationsEnabled).HasColumnName("VisualNotificationsEnabled");

            HasOptional(t => t.Operator)
                .WithMany(t => t.Clients)
                .HasForeignKey(d => d.OperatorId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.UserData)
                .WithRequired(x => x.User)
                .WillCascadeOnDelete(true);

            this.HasMany(t => t.VisibleToOperators)
                .WithMany(t => t.VisibleClients)
                .Map(m =>
                {
                    m.ToTable("ClientVisibleToOperators");
                    m.MapLeftKey("Client_Id");
                    m.MapRightKey("Operator_Id");
                });


        }
    }
}

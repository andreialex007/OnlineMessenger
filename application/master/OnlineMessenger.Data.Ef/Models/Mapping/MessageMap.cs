using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.Data.Ef.Models.Mapping
{
    public class MessageMap : EntityTypeConfiguration<Message>
    {
        public MessageMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            this.Property(t => t.Text)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Messages");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Text).HasColumnName("Text");
            this.Property(t => t.FromId).HasColumnName("FromId");

            // Relationships
            this.HasMany(t => t.To)
                .WithMany(t => t.ToMessages)
                .Map(m =>
                     {
                         m.ToTable("UserMessage");
                         m.MapLeftKey("ToMessages_Id");
                         m.MapRightKey("To_Id");
                     });

            HasRequired(t => t.From)
                .WithMany(t => t.FromMessages)
                .HasForeignKey(d => d.FromId)
                .WillCascadeOnDelete(false);

        }
    }
}

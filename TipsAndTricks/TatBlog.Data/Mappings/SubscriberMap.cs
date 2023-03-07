using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings;

//3b. Tạo lớp SubscriberMap để cấu hình bảng và các cột được ánh xạ từ lớp Subscriber

// * ef migration --> failed???

public class SubscriberMap : IEntityTypeConfiguration<Subscriber>
{
    public void Configure(EntityTypeBuilder<Subscriber> builder)
    {
        // Table name
        builder.ToTable("Subscribers");

        // Primary key
        builder.HasKey(p => p.Id);

        // Fields
        builder.Property(p => p.SubEmail)
               .IsRequired()
               .HasMaxLength(500);
        builder.Property(p => p.SubDated)
               .IsRequired()
               .HasColumnType("datetime");
        builder.Property(p => p.UnSubDated)
               .HasColumnType("datetime");
        builder.Property(p => p.Cancel)
               .IsRequired()
               .HasMaxLength(5000);
        builder.Property(p => p.Block)
               .IsRequired()
               .HasDefaultValue(false);
        builder.Property(p => p.AdNotes)
               .IsRequired()
               .HasMaxLength(5000);
    }
}
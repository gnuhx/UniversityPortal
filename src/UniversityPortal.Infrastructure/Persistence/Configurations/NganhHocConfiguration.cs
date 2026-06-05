using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class NganhHocConfiguration : IEntityTypeConfiguration<NganhHoc>
{
    public void Configure(EntityTypeBuilder<NganhHoc> builder)
    {
        builder.ToTable("nganh_hoc");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MaNganh).IsRequired().HasMaxLength(20).HasColumnName("ma_nganh");
        builder.HasIndex(x => x.MaNganh).IsUnique();
        builder.Property(x => x.TenNganh).IsRequired().HasMaxLength(150).HasColumnName("ten_nganh");
        builder.Property(x => x.NganhChaId).HasColumnName("nganh_cha_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.NganhCha).WithMany(n => n.NganhCons).HasForeignKey(x => x.NganhChaId).OnDelete(DeleteBehavior.Restrict);
    }
}

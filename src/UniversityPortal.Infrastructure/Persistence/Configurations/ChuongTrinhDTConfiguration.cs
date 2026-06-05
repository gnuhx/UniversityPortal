using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class ChuongTrinhDTConfiguration : IEntityTypeConfiguration<ChuongTrinhDT>
{
    public void Configure(EntityTypeBuilder<ChuongTrinhDT> builder)
    {
        builder.ToTable("chuong_trinh_dt");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MaCtdt).IsRequired().HasMaxLength(20).HasColumnName("ma_ctdt");
        builder.HasIndex(x => x.MaCtdt).IsUnique();
        builder.Property(x => x.NganhId).HasColumnName("nganh_id");
        builder.Property(x => x.KhoaHoc).IsRequired().HasMaxLength(20).HasColumnName("khoa_hoc");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Nganh).WithMany(n => n.ChuongTrinhDTs).HasForeignKey(x => x.NganhId).OnDelete(DeleteBehavior.Restrict);
    }
}

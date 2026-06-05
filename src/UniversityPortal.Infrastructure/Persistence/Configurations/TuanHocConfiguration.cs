using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class TuanHocConfiguration : IEntityTypeConfiguration<TuanHoc>
{
    public void Configure(EntityTypeBuilder<TuanHoc> builder)
    {
        builder.ToTable("tuan_hoc");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.NamHocId).HasColumnName("nam_hoc_id");
        builder.Property(x => x.MaTuan).IsRequired().HasMaxLength(20).HasColumnName("ma_tuan");
        builder.HasIndex(x => x.MaTuan).IsUnique();
        builder.Property(x => x.SoThuTuTuan).HasColumnName("so_thu_tu_tuan");
        builder.Property(x => x.NgayBatDau).HasColumnName("ngay_bat_dau");
        builder.Property(x => x.NgayKetThuc).HasColumnName("ngay_ket_thuc");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.NamHoc).WithMany(n => n.TuanHocs).HasForeignKey(x => x.NamHocId).OnDelete(DeleteBehavior.Cascade);
    }
}

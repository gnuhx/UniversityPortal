using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class ThoiKhoaBieuConfiguration : IEntityTypeConfiguration<ThoiKhoaBieu>
{
    public void Configure(EntityTypeBuilder<ThoiKhoaBieu> builder)
    {
        builder.ToTable("thoi_khoa_bieu");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LopHpId).HasColumnName("lop_hp_id");
        builder.Property(x => x.TuanHocId).HasColumnName("tuan_hoc_id");
        builder.Property(x => x.Thu).HasColumnName("thu");
        builder.Property(x => x.TietBatDau).HasColumnName("tiet_bat_dau");
        builder.Property(x => x.TietKetThuc).HasColumnName("tiet_ket_thuc");
        builder.Property(x => x.PhongHoc).IsRequired().HasMaxLength(20).HasColumnName("phong_hoc");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.LopHocPhan).WithMany(l => l.ThoiKhoaBieus).HasForeignKey(x => x.LopHpId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.TuanHoc).WithMany(t => t.ThoiKhoaBieus).HasForeignKey(x => x.TuanHocId).OnDelete(DeleteBehavior.Cascade);
    }
}

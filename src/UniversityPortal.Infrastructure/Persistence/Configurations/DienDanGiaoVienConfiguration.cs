using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class DienDanGiaoVienConfiguration : IEntityTypeConfiguration<DienDanGiaoVien>
{
    public void Configure(EntityTypeBuilder<DienDanGiaoVien> builder)
    {
        builder.ToTable("dien_dan_giao_vien");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LopId).HasColumnName("lop_id");
        builder.Property(x => x.GiaoVienId).HasColumnName("giao_vien_id");
        builder.Property(x => x.NoiDung).IsRequired().HasColumnType("nvarchar(max)").HasColumnName("noi_dung");
        builder.Property(x => x.NgayGui).HasColumnName("ngay_gui");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Lop).WithMany(l => l.DienDanGiaoViens).HasForeignKey(x => x.LopId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.GiaoVien).WithMany(g => g.DienDanGiaoViens).HasForeignKey(x => x.GiaoVienId).OnDelete(DeleteBehavior.Cascade);
    }
}

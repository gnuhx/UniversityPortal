using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class DatPhongThucHanhConfiguration : IEntityTypeConfiguration<DatPhongThucHanh>
{
    public void Configure(EntityTypeBuilder<DatPhongThucHanh> builder)
    {
        builder.ToTable("dat_phong_thuc_hanh");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.GiaoVienId).HasColumnName("giao_vien_id");
        builder.Property(x => x.PhongHoc).IsRequired().HasMaxLength(20).HasColumnName("phong_hoc");
        builder.Property(x => x.NgayDat).HasColumnName("ngay_dat");
        builder.Property(x => x.CaHoc).HasColumnName("ca_hoc");
        builder.Property(x => x.LyDo).HasColumnType("nvarchar(max)").HasColumnName("ly_do");
        builder.Property(x => x.TrangThai).IsRequired().HasMaxLength(20).HasColumnName("trang_thai");
        builder.Property(x => x.NguoiDuyetId).HasColumnName("nguoi_duyet_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.GiaoVien).WithMany(g => g.DatPhongThucHanhs).HasForeignKey(x => x.GiaoVienId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.NguoiDuyet).WithMany().HasForeignKey(x => x.NguoiDuyetId).OnDelete(DeleteBehavior.NoAction);
    }
}

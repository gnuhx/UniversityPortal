using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class YeuCauHanhChinhConfiguration : IEntityTypeConfiguration<YeuCauHanhChinh>
{
    public void Configure(EntityTypeBuilder<YeuCauHanhChinh> builder)
    {
        builder.ToTable("yeu_cau_hanh_chinh");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SinhVienId).HasColumnName("sinh_vien_id");
        builder.Property(x => x.LoaiYeuCau).IsRequired().HasMaxLength(100).HasColumnName("loai_yeu_cau");
        builder.Property(x => x.NoiDung).IsRequired().HasColumnType("text").HasColumnName("noi_dung");
        builder.Property(x => x.FileDinhKem).HasMaxLength(500).HasColumnName("file_dinh_kem");
        builder.Property(x => x.TrangThai).IsRequired().HasMaxLength(20).HasColumnName("trang_thai");
        builder.Property(x => x.NguoiDuyetId).HasColumnName("nguoi_duyet_id");
        builder.Property(x => x.NgayTao).HasColumnName("ngay_tao");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.SinhVien).WithMany(s => s.YeuCauHanhChinhs).HasForeignKey(x => x.SinhVienId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.NguoiDuyet).WithMany().HasForeignKey(x => x.NguoiDuyetId).OnDelete(DeleteBehavior.SetNull);
    }
}

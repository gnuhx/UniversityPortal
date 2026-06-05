using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class YeuCauSuaDiemConfiguration : IEntityTypeConfiguration<YeuCauSuaDiem>
{
    public void Configure(EntityTypeBuilder<YeuCauSuaDiem> builder)
    {
        builder.ToTable("yeu_cau_sua_diem");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LopHpId).HasColumnName("lop_hp_id");
        builder.Property(x => x.GiaoVienId).HasColumnName("giao_vien_id");
        builder.Property(x => x.LyDo).IsRequired().HasColumnType("text").HasColumnName("ly_do");
        builder.Property(x => x.TrangThai).IsRequired().HasMaxLength(20).HasColumnName("trang_thai");
        builder.Property(x => x.NguoiDuyetId).HasColumnName("nguoi_duyet_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.LopHocPhan).WithMany(l => l.YeuCauSuaDiems).HasForeignKey(x => x.LopHpId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.GiaoVien).WithMany(g => g.YeuCauSuaDiems).HasForeignKey(x => x.GiaoVienId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.NguoiDuyet).WithMany().HasForeignKey(x => x.NguoiDuyetId).OnDelete(DeleteBehavior.SetNull);
    }
}

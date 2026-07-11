using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class ThongBaoConfiguration : IEntityTypeConfiguration<ThongBao>
{
    public void Configure(EntityTypeBuilder<ThongBao> builder)
    {
        builder.ToTable("thong_bao");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LoaiThongBao).IsRequired().HasMaxLength(30).HasColumnName("loai_thong_bao");
        builder.Property(x => x.MucDo).IsRequired().HasMaxLength(20).HasColumnName("muc_do");
        builder.Property(x => x.TieuDe).IsRequired().HasMaxLength(255).HasColumnName("tieu_de");
        builder.Property(x => x.NoiDung).IsRequired().HasColumnType("nvarchar(max)").HasColumnName("noi_dung");
        builder.Property(x => x.NguoiTaoId).HasColumnName("nguoi_tao_id");
        builder.Property(x => x.LopNhanId).HasColumnName("lop_nhan_id");
        builder.Property(x => x.NgayTao).HasColumnName("ngay_tao");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.NguoiTao).WithMany().HasForeignKey(x => x.NguoiTaoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.LopNhan).WithMany(l => l.ThongBaos).HasForeignKey(x => x.LopNhanId).OnDelete(DeleteBehavior.SetNull);
    }
}

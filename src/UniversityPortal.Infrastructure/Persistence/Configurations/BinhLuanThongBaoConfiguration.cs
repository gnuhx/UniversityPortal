using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class BinhLuanThongBaoConfiguration : IEntityTypeConfiguration<BinhLuanThongBao>
{
    public void Configure(EntityTypeBuilder<BinhLuanThongBao> builder)
    {
        builder.ToTable("binh_luan_thong_bao");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ThongBaoId).HasColumnName("thong_bao_id");
        builder.Property(x => x.TaiKhoanId).HasColumnName("tai_khoan_id");
        builder.Property(x => x.NoiDung).IsRequired().HasColumnType("nvarchar(max)").HasColumnName("noi_dung");
        builder.Property(x => x.NgayBinhLuan).HasColumnName("ngay_binh_luan");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.ThongBao).WithMany(t => t.BinhLuans).HasForeignKey(x => x.ThongBaoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.TaiKhoan).WithMany().HasForeignKey(x => x.TaiKhoanId).OnDelete(DeleteBehavior.Cascade);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class BienBanSHCNConfiguration : IEntityTypeConfiguration<BienBanSHCN>
{
    public void Configure(EntityTypeBuilder<BienBanSHCN> builder)
    {
        builder.ToTable("bien_ban_shcn");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LopId).HasColumnName("lop_id");
        builder.Property(x => x.TuanHocId).HasColumnName("tuan_hoc_id");
        builder.Property(x => x.ThoiGian).HasColumnName("thoi_gian");
        builder.Property(x => x.DiaDiem).IsRequired().HasMaxLength(150).HasColumnName("dia_diem");
        builder.Property(x => x.GvcnId).HasColumnName("gvcn_id");
        builder.Property(x => x.ThuKyId).HasColumnName("thu_ky_id");
        builder.Property(x => x.NoiDung).IsRequired().HasColumnType("nvarchar(max)").HasColumnName("noi_dung");
        builder.Property(x => x.PhanHoiGvcn).HasColumnType("nvarchar(max)").HasColumnName("phan_hoi_gvcn");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Lop).WithMany(l => l.BienBanSHCNs).HasForeignKey(x => x.LopId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.TuanHoc).WithMany(t => t.BienBanSHCNs).HasForeignKey(x => x.TuanHocId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Gvcn).WithMany(g => g.BienBanSHCNs).HasForeignKey(x => x.GvcnId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ThuKy).WithMany(s => s.BienBanSHCNThuKy).HasForeignKey(x => x.ThuKyId).OnDelete(DeleteBehavior.Restrict);
    }
}

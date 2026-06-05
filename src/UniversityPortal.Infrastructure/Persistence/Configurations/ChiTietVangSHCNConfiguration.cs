using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class ChiTietVangSHCNConfiguration : IEntityTypeConfiguration<ChiTietVangSHCN>
{
    public void Configure(EntityTypeBuilder<ChiTietVangSHCN> builder)
    {
        builder.ToTable("chi_tiet_vang_shcn");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BienBanId).HasColumnName("bien_ban_id");
        builder.Property(x => x.SinhVienId).HasColumnName("sinh_vien_id");
        builder.Property(x => x.LyDo).HasMaxLength(255).HasColumnName("ly_do");
        builder.Property(x => x.CoPhep).HasColumnName("co_phep");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.BienBan).WithMany(b => b.ChiTietVangSHCNs).HasForeignKey(x => x.BienBanId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SinhVien).WithMany(s => s.ChiTietVangSHCNs).HasForeignKey(x => x.SinhVienId).OnDelete(DeleteBehavior.Cascade);
    }
}

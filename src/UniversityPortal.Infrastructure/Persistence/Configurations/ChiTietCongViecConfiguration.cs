using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class ChiTietCongViecConfiguration : IEntityTypeConfiguration<ChiTietCongViec>
{
    public void Configure(EntityTypeBuilder<ChiTietCongViec> builder)
    {
        builder.ToTable("chi_tiet_cong_viec");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BienBanId).HasColumnName("bien_ban_id");
        builder.Property(x => x.TenCongViec).IsRequired().HasMaxLength(255).HasColumnName("ten_cong_viec");
        builder.Property(x => x.TrangThaiViec).IsRequired().HasMaxLength(30).HasColumnName("trang_thai_viec");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.BienBan).WithMany(b => b.ChiTietCongViecs).HasForeignKey(x => x.BienBanId).OnDelete(DeleteBehavior.Cascade);
    }
}

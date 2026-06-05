using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Configurations;

public class TaiKhoanConfiguration : IEntityTypeConfiguration<TaiKhoan>
{
    public void Configure(EntityTypeBuilder<TaiKhoan> builder)
    {
        builder.ToTable("tai_khoan");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TenDangNhap).IsRequired().HasMaxLength(100).HasColumnName("ten_dang_nhap");
        builder.HasIndex(x => x.TenDangNhap).IsUnique();
        builder.Property(x => x.MatKhau).IsRequired().HasMaxLength(255).HasColumnName("mat_khau");
        builder.Property(x => x.VaiTroId).HasColumnName("vai_tro_id");
        builder.Property(x => x.PhongBanId).HasColumnName("phong_ban_id");
        builder.Property(x => x.HoTen).IsRequired().HasMaxLength(150).HasColumnName("ho_ten");
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150).HasColumnName("email");
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.AnhDaiDien).HasMaxLength(500).HasColumnName("anh_dai_dien");
        builder.Property(x => x.TrangThai).HasDefaultValue(true).HasColumnName("trang_thai");
        builder.Property(x => x.RefreshToken).HasMaxLength(500).HasColumnName("refresh_token");
        builder.Property(x => x.RefreshTokenExpiry).HasColumnName("refresh_token_expiry");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.VaiTro).WithMany(v => v.TaiKhoans).HasForeignKey(x => x.VaiTroId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PhongBan).WithMany(p => p.TaiKhoans).HasForeignKey(x => x.PhongBanId).OnDelete(DeleteBehavior.SetNull);
    }
}

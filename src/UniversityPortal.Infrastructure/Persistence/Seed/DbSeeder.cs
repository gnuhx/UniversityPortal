using Microsoft.EntityFrameworkCore;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Vai tro
        if (!await db.VaiTros.AnyAsync())
        {
            db.VaiTros.AddRange(
                new VaiTro { Id = 1, TenVaiTro = "Admin" },
                new VaiTro { Id = 2, TenVaiTro = "Giao vien" },
                new VaiTro { Id = 3, TenVaiTro = "Sinh vien" },
                new VaiTro { Id = 4, TenVaiTro = "Giao vu" }
            );
            await db.SaveChangesAsync();
        }

        // Phong ban
        if (!await db.PhongBans.AnyAsync())
        {
            db.PhongBans.AddRange(
                new PhongBan { Id = 1, TenPhongBan = "Phong Dao tao" },
                new PhongBan { Id = 2, TenPhongBan = "Phong Cong tac Sinh vien" },
                new PhongBan { Id = 3, TenPhongBan = "Khoa Cong nghe Thong tin" }
            );
            await db.SaveChangesAsync();
        }

        // Tai khoan admin
        if (!await db.TaiKhoans.AnyAsync(t => t.TenDangNhap == "admin"))
        {
            db.TaiKhoans.Add(new TaiKhoan
            {
                TenDangNhap = "admin",
                MatKhau     = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                VaiTroId    = 1,
                PhongBanId  = 1,
                HoTen       = "Quản trị viên",
                Email       = "admin@uni.edu.vn",
                TrangThai   = true
            });
            await db.SaveChangesAsync();
        }
    }
}

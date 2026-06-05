using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class TaiKhoanRepository(AppDbContext context) : BaseRepository<TaiKhoan>(context), ITaiKhoanRepository
{
    public async Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap)
        => await DbSet.Include(x => x.VaiTro).FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);

    public async Task<TaiKhoan?> GetByRefreshTokenAsync(string refreshToken)
        => await DbSet.Include(x => x.VaiTro).FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
}

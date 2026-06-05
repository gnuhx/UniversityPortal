using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository tài khoản — bổ sung truy vấn theo tên đăng nhập, refresh token và bộ lọc phân trang.
/// </summary>
public class TaiKhoanRepository(AppDbContext context) : BaseRepository<TaiKhoan>(context), ITaiKhoanRepository
{
    /// <summary>Tìm tài khoản theo tên đăng nhập, bao gồm thông tin vai trò.</summary>
    public async Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap)
        => await DbSet
            .Include(x => x.VaiTro)
            .Include(x => x.PhongBan)
            .FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);

    /// <summary>Tìm tài khoản theo refresh token (dùng khi làm mới access token).</summary>
    public async Task<TaiKhoan?> GetByRefreshTokenAsync(string refreshToken)
        => await DbSet
            .Include(x => x.VaiTro)
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

    /// <summary>
    /// Lấy danh sách tài khoản có phân trang.
    /// Tìm kiếm không phân biệt hoa thường theo họ tên, email hoặc tên đăng nhập.
    /// </summary>
    public async Task<PagedResultDto<TaiKhoan>> GetPagedFilterAsync(
        int page, int pageSize, string? keyword, int? vaiTroId, bool? trangThai)
    {
        var query = DbSet
            .Include(x => x.VaiTro)
            .Include(x => x.PhongBan)
            .AsQueryable();

        // Lọc theo từ khoá tìm kiếm trên họ tên, email và tên đăng nhập
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.ToLower();
            query = query.Where(x =>
                x.HoTen.ToLower().Contains(kw) ||
                x.Email.ToLower().Contains(kw) ||
                x.TenDangNhap.ToLower().Contains(kw));
        }

        if (vaiTroId.HasValue)
            query = query.Where(x => x.VaiTroId == vaiTroId.Value);

        if (trangThai.HasValue)
            query = query.Where(x => x.TrangThai == trangThai.Value);

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.HoTen)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<TaiKhoan> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }
}

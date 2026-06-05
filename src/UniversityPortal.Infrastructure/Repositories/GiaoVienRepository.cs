using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository giáo viên — bổ sung truy vấn chi tiết và bộ lọc phân trang.
/// </summary>
public class GiaoVienRepository(AppDbContext context) : BaseRepository<GiaoVien>(context), IGiaoVienRepository
{
    /// <summary>Tìm giáo viên theo tài khoản liên kết, bao gồm thông tin tài khoản.</summary>
    public async Task<GiaoVien?> GetByTaiKhoanIdAsync(int taiKhoanId)
        => await DbSet
            .Include(x => x.TaiKhoan).ThenInclude(t => t.PhongBan)
            .FirstOrDefaultAsync(x => x.TaiKhoanId == taiKhoanId);

    /// <summary>Tìm giáo viên theo mã GV để kiểm tra trùng.</summary>
    public async Task<GiaoVien?> GetByMaGvAsync(string maGv)
        => await DbSet.FirstOrDefaultAsync(x => x.MaGv == maGv);

    /// <summary>Lấy danh sách giáo viên phân trang, lọc theo họ tên hoặc mã GV.</summary>
    public async Task<PagedResultDto<GiaoVien>> GetPagedFilterAsync(int page, int pageSize, string? keyword)
    {
        var query = DbSet
            .Include(x => x.TaiKhoan).ThenInclude(t => t.PhongBan)
            .AsQueryable();

        // Lọc không phân biệt hoa thường theo họ tên trong TaiKhoan hoặc mã GV
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.ToLower();
            query = query.Where(x =>
                x.TaiKhoan.HoTen.ToLower().Contains(kw) ||
                x.MaGv.ToLower().Contains(kw));
        }

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.TaiKhoan.HoTen)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<GiaoVien> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Lấy thông tin chi tiết giáo viên theo id, bao gồm tài khoản và phòng ban.</summary>
    public async Task<GiaoVien?> GetDetailAsync(int id)
        => await DbSet
            .Include(x => x.TaiKhoan).ThenInclude(t => t.PhongBan)
            .Include(x => x.TaiKhoan).ThenInclude(t => t.VaiTro)
            .FirstOrDefaultAsync(x => x.Id == id);
}

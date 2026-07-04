using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository sinh viên — bổ sung truy vấn chi tiết và bộ lọc phân trang.
/// </summary>
public class SinhVienRepository(AppDbContext context) : BaseRepository<SinhVien>(context), ISinhVienRepository
{
    /// <summary>Tìm sinh viên theo MSSV, bao gồm thông tin tài khoản.</summary>
    public async Task<SinhVien?> GetByMssvAsync(string mssv)
        => await DbSet
            .Include(x => x.TaiKhoan)
            .FirstOrDefaultAsync(x => x.Mssv == mssv);

    /// <summary>Tìm sinh viên theo tài khoản liên kết.</summary>
    public async Task<SinhVien?> GetByTaiKhoanIdAsync(int taiKhoanId)
        => await DbSet.FirstOrDefaultAsync(x => x.TaiKhoanId == taiKhoanId);

    /// <summary>Lấy danh sách sinh viên theo lớp có phân trang (dùng cho GVCN xem danh sách lớp mình).</summary>
    public async Task<PagedResultDto<SinhVien>> GetPagedByLopAsync(int lopId, int page, int pageSize)
    {
        var query = DbSet
            .Include(x => x.TaiKhoan)
            .Where(x => x.LopId == lopId);

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.TaiKhoan.HoTen)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<SinhVien> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Lấy danh sách sinh viên phân trang, lọc theo họ tên / MSSV và lớp sinh hoạt.</summary>
    public async Task<PagedResultDto<SinhVien>> GetPagedFilterAsync(
        int page, int pageSize, string? keyword, int? lopId)
    {
        var query = DbSet
            .Include(x => x.TaiKhoan)
            .Include(x => x.Lop)
            .AsQueryable();

        // Lọc không phân biệt hoa thường theo họ tên hoặc MSSV
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.ToLower();
            query = query.Where(x =>
                x.TaiKhoan.HoTen.ToLower().Contains(tuKhoa) ||
                x.Mssv.ToLower().Contains(tuKhoa));
        }

        if (lopId.HasValue)
            query = query.Where(x => x.LopId == lopId.Value);

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.Mssv)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<SinhVien> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Lấy thông tin chi tiết sinh viên theo id, bao gồm tài khoản và lớp sinh hoạt.</summary>
    public async Task<SinhVien?> GetDetailAsync(int id)
        => await DbSet
            .Include(x => x.TaiKhoan)
            .Include(x => x.Lop)
            .FirstOrDefaultAsync(x => x.Id == id);

    /// <summary>Lấy sinh viên theo id kèm Lop sinh hoạt và Chương trình đào tạo của lớp (dùng để xác định CTDT hiện tại).</summary>
    public async Task<SinhVien?> GetByIdWithLopCtdtAsync(int id)
        => await DbSet
            .Include(x => x.Lop)
                .ThenInclude(l => l!.ChuongTrinhDT)
            .FirstOrDefaultAsync(x => x.Id == id);
}

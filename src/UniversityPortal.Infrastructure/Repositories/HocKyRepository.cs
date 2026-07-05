using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class HocKyRepository(AppDbContext context) : BaseRepository<HocKy>(context), IHocKyRepository
{
    public async Task<IEnumerable<HocKy>> GetAllAsync()
        => await DbSet
            .Include(x => x.NamHoc)
            .OrderByDescending(x => x.NgayBatDau)
            .ToListAsync();

    /// <summary>Lấy danh sách học kỳ phân trang, lọc theo năm học — dùng cho trang quản lý học kỳ của 1 năm học.</summary>
    public async Task<PagedResultDto<HocKy>> GetPagedFilterAsync(int page, int pageSize, int? namHocId)
    {
        var query = DbSet.Include(x => x.NamHoc).AsQueryable();

        if (namHocId.HasValue)
            query = query.Where(x => x.NamHocId == namHocId.Value);

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.NgayBatDau)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<HocKy> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task<HocKy?> GetDetailAsync(int id)
        => await DbSet.Include(x => x.NamHoc).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> ExistsByNamHocAsync(int namHocId)
        => await DbSet.AnyAsync(x => x.NamHocId == namHocId);

    /// <summary>Học kỳ mà sinh viên có ít nhất 1 lớp học phần đã đăng ký (dùng cho dropdown TKB của sinh viên).</summary>
    public async Task<IEnumerable<HocKy>> GetForSinhVienAsync(int sinhVienId)
        => await DbSet
            .Include(x => x.NamHoc)
            .Where(hk => hk.LopHocPhans.Any(lhp => lhp.DanhSachLopHPs.Any(d => d.SinhVienId == sinhVienId)))
            .OrderByDescending(x => x.NgayBatDau)
            .ToListAsync();

    /// <summary>Học kỳ mà giáo viên có ít nhất 1 lớp học phần đang dạy (dùng cho dropdown TKB của giáo viên).</summary>
    public async Task<IEnumerable<HocKy>> GetForGiaoVienAsync(int giaoVienId)
        => await DbSet
            .Include(x => x.NamHoc)
            .Where(hk => hk.LopHocPhans.Any(lhp => lhp.GiaoVienId == giaoVienId))
            .OrderByDescending(x => x.NgayBatDau)
            .ToListAsync();
}

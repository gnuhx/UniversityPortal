using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class YeuCauHanhChinhRepository(AppDbContext context)
    : BaseRepository<YeuCauHanhChinh>(context), IYeuCauHanhChinhRepository
{
    public async Task<IEnumerable<YeuCauHanhChinh>> GetBySinhVienAsync(int sinhVienId)
        => await DbSet
            .Where(x => x.SinhVienId == sinhVienId)
            .Include(x => x.SinhVien).ThenInclude(sv => sv.TaiKhoan)
            .Include(x => x.NguoiDuyet)
            .OrderByDescending(x => x.NgayTao)
            .ToListAsync();

    public async Task<PagedResultDto<YeuCauHanhChinh>> GetAllPagedAsync(int page, int pageSize, string? trangThai)
    {
        var query = DbSet
            .Include(x => x.SinhVien).ThenInclude(sv => sv.TaiKhoan)
            .Include(x => x.NguoiDuyet)
            .AsQueryable();

        if (!string.IsNullOrEmpty(trangThai))
            query = query.Where(x => x.TrangThai == trangThai);

        var total = await query.CountAsync();
        var data = await query
            .OrderByDescending(x => x.NgayTao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<YeuCauHanhChinh> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task<YeuCauHanhChinh?> GetDetailAsync(int id)
        => await DbSet
            .Where(x => x.Id == id)
            .Include(x => x.SinhVien).ThenInclude(sv => sv.TaiKhoan)
            .Include(x => x.NguoiDuyet)
            .FirstOrDefaultAsync();
}

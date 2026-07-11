using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class YeuCauSuaDiemRepository(AppDbContext context)
    : BaseRepository<YeuCauSuaDiem>(context), IYeuCauSuaDiemRepository
{
    public async Task<IEnumerable<YeuCauSuaDiem>> GetByGiaoVienAsync(int giaoVienId)
        => await DbSet
            .Where(x => x.GiaoVienId == giaoVienId)
            .Include(x => x.LopHocPhan).ThenInclude(lhp => lhp.ChiTietCTDT).ThenInclude(ct => ct.MonHoc)
            .Include(x => x.LopHocPhan).ThenInclude(lhp => lhp.HocKy)
            .Include(x => x.GiaoVien).ThenInclude(gv => gv.TaiKhoan)
            .Include(x => x.NguoiDuyet)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<PagedResultDto<YeuCauSuaDiem>> GetAllPagedAsync(int page, int pageSize, string? trangThai)
    {
        var query = DbSet
            .Include(x => x.LopHocPhan).ThenInclude(lhp => lhp.ChiTietCTDT).ThenInclude(ct => ct.MonHoc)
            .Include(x => x.LopHocPhan).ThenInclude(lhp => lhp.HocKy)
            .Include(x => x.GiaoVien).ThenInclude(gv => gv.TaiKhoan)
            .Include(x => x.NguoiDuyet)
            .AsQueryable();

        if (!string.IsNullOrEmpty(trangThai))
            query = query.Where(x => x.TrangThai == trangThai);

        var total = await query.CountAsync();
        var data = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<YeuCauSuaDiem> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task<YeuCauSuaDiem?> GetDetailAsync(int id)
        => await DbSet
            .Where(x => x.Id == id)
            .Include(x => x.LopHocPhan).ThenInclude(lhp => lhp.ChiTietCTDT).ThenInclude(ct => ct.MonHoc)
            .Include(x => x.LopHocPhan).ThenInclude(lhp => lhp.HocKy)
            .Include(x => x.GiaoVien).ThenInclude(gv => gv.TaiKhoan)
            .Include(x => x.NguoiDuyet)
            .FirstOrDefaultAsync();
}

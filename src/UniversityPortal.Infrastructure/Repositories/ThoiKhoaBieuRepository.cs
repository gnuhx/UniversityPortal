using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository thời khoá biểu — hỗ trợ lọc theo lớp HP, tra lịch theo
/// sinh viên/giáo viên, và kiểm tra trùng phòng học.
/// </summary>
public class ThoiKhoaBieuRepository(AppDbContext context) : BaseRepository<ThoiKhoaBieu>(context), IThoiKhoaBieuRepository
{
    private IQueryable<ThoiKhoaBieu> QueryWithDetails() => DbSet
        .Include(x => x.LopHocPhan).ThenInclude(l => l.ChiTietCTDT).ThenInclude(ct => ct.MonHoc)
        .Include(x => x.LopHocPhan).ThenInclude(l => l.HocKy)
        .Include(x => x.LopHocPhan).ThenInclude(l => l.GiaoVien).ThenInclude(gv => gv.TaiKhoan)
        .Include(x => x.TuanHoc);

    public async Task<PagedResultDto<ThoiKhoaBieu>> GetPagedFilterAsync(int page, int pageSize, int? lopHpId)
    {
        var query = QueryWithDetails();

        if (lopHpId.HasValue)
            query = query.Where(x => x.LopHpId == lopHpId.Value);

        var total = await query.CountAsync();
        var data = await query
            .OrderBy(x => x.TuanHoc.SoThuTuTuan).ThenBy(x => x.Thu).ThenBy(x => x.TietBatDau)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<ThoiKhoaBieu> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task<ThoiKhoaBieu?> GetDetailAsync(int id)
        => await QueryWithDetails().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<ThoiKhoaBieu>> GetForSinhVienAsync(int sinhVienId, int? hocKyId)
        => await QueryWithDetails()
            .Where(x => x.LopHocPhan.DanhSachLopHPs.Any(d => d.SinhVienId == sinhVienId))
            .Where(x => !hocKyId.HasValue || x.LopHocPhan.HocKyId == hocKyId.Value)
            .ToListAsync();

    public async Task<IEnumerable<ThoiKhoaBieu>> GetForGiaoVienAsync(int giaoVienId, int? hocKyId)
        => await QueryWithDetails()
            .Where(x => x.LopHocPhan.GiaoVienId == giaoVienId)
            .Where(x => !hocKyId.HasValue || x.LopHocPhan.HocKyId == hocKyId.Value)
            .ToListAsync();

    public async Task<bool> ExistsConflictAsync(int tuanHocId, int thu, string phongHoc, int tietBatDau, int tietKetThuc, int? excludeId)
        => await DbSet.AnyAsync(x =>
            x.TuanHocId == tuanHocId &&
            x.Thu == thu &&
            x.PhongHoc == phongHoc &&
            x.TietBatDau <= tietKetThuc &&
            x.TietKetThuc >= tietBatDau &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
}

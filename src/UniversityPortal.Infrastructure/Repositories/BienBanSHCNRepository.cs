using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository biên bản sinh hoạt chủ nhiệm — luôn include đủ quan hệ cần cho DTO
/// (lớp, tuần học, GVCN, thư ký, công việc, điểm danh vắng kèm thông tin sinh viên).
/// </summary>
public class BienBanSHCNRepository(AppDbContext context) : BaseRepository<BienBanSHCN>(context), IBienBanSHCNRepository
{
    private IQueryable<BienBanSHCN> QueryWithIncludes() =>
        DbSet
            .Include(x => x.Lop)
            .Include(x => x.TuanHoc)
            .Include(x => x.Gvcn).ThenInclude(g => g.TaiKhoan)
            .Include(x => x.ThuKy).ThenInclude(s => s.TaiKhoan)
            .Include(x => x.ChiTietCongViecs)
            .Include(x => x.ChiTietVangSHCNs).ThenInclude(v => v.SinhVien).ThenInclude(sv => sv.TaiKhoan);

    /// <summary>Lấy danh sách biên bản phân trang, lọc theo lớp (bỏ trống = tất cả lớp).</summary>
    public async Task<PagedResultDto<BienBanSHCN>> GetPagedFilterAsync(int? lopId, int page, int pageSize)
    {
        var query = QueryWithIncludes();

        if (lopId.HasValue)
            query = query.Where(x => x.LopId == lopId.Value);

        var total = await query.CountAsync();
        var data  = await query
            .OrderByDescending(x => x.ThoiGian)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<BienBanSHCN> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Lấy chi tiết biên bản theo id, kèm công việc và điểm danh vắng.</summary>
    public async Task<BienBanSHCN?> GetDetailAsync(int id)
        => await QueryWithIncludes().FirstOrDefaultAsync(x => x.Id == id);

    /// <summary>Lấy toàn bộ biên bản của 1 lớp, mới nhất trước — dùng cho sinh viên xem lịch sử lớp mình.</summary>
    public async Task<IEnumerable<BienBanSHCN>> GetByLopAsync(int lopId)
        => await QueryWithIncludes()
            .Where(x => x.LopId == lopId)
            .OrderByDescending(x => x.ThoiGian)
            .ToListAsync();
}

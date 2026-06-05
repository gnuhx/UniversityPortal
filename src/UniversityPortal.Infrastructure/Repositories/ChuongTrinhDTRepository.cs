using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository chương trình đào tạo — hỗ trợ phân trang và lọc theo ngành.
/// </summary>
public class ChuongTrinhDTRepository(AppDbContext context) : BaseRepository<ChuongTrinhDT>(context), IChuongTrinhDTRepository
{
    /// <summary>Lấy danh sách CTDT phân trang, lọc theo mã CTDT và ngành.</summary>
    public async Task<PagedResultDto<ChuongTrinhDT>> GetPagedFilterAsync(
        int page, int pageSize, string? keyword, int? nganhId)
    {
        var query = DbSet
            .Include(x => x.Nganh)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.ToLower();
            query = query.Where(x =>
                x.MaCtdt.ToLower().Contains(kw) ||
                x.KhoaHoc.ToLower().Contains(kw));
        }

        if (nganhId.HasValue)
            query = query.Where(x => x.NganhId == nganhId.Value);

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.MaCtdt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<ChuongTrinhDT> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Tìm CTDT theo mã để kiểm tra trùng khi tạo mới.</summary>
    public async Task<ChuongTrinhDT?> GetByMaCtdtAsync(string maCtdt)
        => await DbSet.FirstOrDefaultAsync(x => x.MaCtdt == maCtdt);

    /// <summary>Lấy chi tiết CTDT theo id, bao gồm ngành học.</summary>
    public async Task<ChuongTrinhDT?> GetDetailAsync(int id)
        => await DbSet
            .Include(x => x.Nganh)
            .FirstOrDefaultAsync(x => x.Id == id);
}

using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository ngành học — hỗ trợ phân trang, lọc và tra cứu ngành cha.
/// </summary>
public class NganhHocRepository(AppDbContext context) : BaseRepository<NganhHoc>(context), INganhHocRepository
{
    /// <summary>Lấy danh sách ngành học phân trang, lọc theo mã ngành hoặc tên ngành.</summary>
    public async Task<PagedResultDto<NganhHoc>> GetPagedFilterAsync(int page, int pageSize, string? keyword)
    {
        var query = DbSet
            .Include(x => x.NganhCha)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.ToLower();
            query = query.Where(x =>
                x.MaNganh.ToLower().Contains(kw) ||
                x.TenNganh.ToLower().Contains(kw));
        }

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.MaNganh)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<NganhHoc> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Tìm ngành theo mã để kiểm tra trùng khi tạo mới.</summary>
    public async Task<NganhHoc?> GetByMaNganhAsync(string maNganh)
        => await DbSet.FirstOrDefaultAsync(x => x.MaNganh == maNganh);

    /// <summary>Lấy chi tiết ngành theo id, bao gồm ngành cha.</summary>
    public async Task<NganhHoc?> GetDetailAsync(int id)
        => await DbSet
            .Include(x => x.NganhCha)
            .FirstOrDefaultAsync(x => x.Id == id);
}

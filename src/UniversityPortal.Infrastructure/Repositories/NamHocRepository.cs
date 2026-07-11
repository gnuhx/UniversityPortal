using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository năm học — hỗ trợ phân trang, lọc theo keyword.
/// </summary>
public class NamHocRepository(AppDbContext context) : BaseRepository<NamHoc>(context), INamHocRepository
{
    /// <summary>Lấy danh sách năm học phân trang, lọc theo tên năm học.</summary>
    public async Task<PagedResultDto<NamHoc>> GetPagedFilterAsync(int page, int pageSize, string? keyword)
    {
        var query = DbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.ToLower();
            query = query.Where(x => x.TenNamHoc.ToLower().Contains(kw));
        }

        var total = await query.CountAsync();
        var data  = await query
            .OrderByDescending(x => x.TenNamHoc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<NamHoc> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Lấy tất cả năm học, sắp giảm dần theo tên (năm học mới nhất trước).</summary>
    public async Task<IEnumerable<NamHoc>> GetAllAsync()
        => await DbSet.OrderByDescending(x => x.TenNamHoc).ToListAsync();

    /// <summary>Tìm năm học theo tên để kiểm tra trùng khi tạo mới.</summary>
    public async Task<NamHoc?> GetByTenNamHocAsync(string tenNamHoc)
        => await DbSet.FirstOrDefaultAsync(x => x.TenNamHoc == tenNamHoc);
}

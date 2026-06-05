using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository môn học — hỗ trợ phân trang và lọc theo mã / tên môn.
/// </summary>
public class MonHocRepository(AppDbContext context) : BaseRepository<MonHoc>(context), IMonHocRepository
{
    /// <summary>Lấy danh sách môn học phân trang, lọc theo mã môn hoặc tên môn.</summary>
    public async Task<PagedResultDto<MonHoc>> GetPagedFilterAsync(int page, int pageSize, string? keyword)
    {
        var query = DbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.ToLower();
            query = query.Where(x =>
                x.MaMon.ToLower().Contains(kw) ||
                x.TenMon.ToLower().Contains(kw));
        }

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.MaMon)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<MonHoc> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Tìm môn học theo mã môn để kiểm tra trùng khi tạo mới.</summary>
    public async Task<MonHoc?> GetByMaMonAsync(string maMon)
        => await DbSet.FirstOrDefaultAsync(x => x.MaMon == maMon);
}

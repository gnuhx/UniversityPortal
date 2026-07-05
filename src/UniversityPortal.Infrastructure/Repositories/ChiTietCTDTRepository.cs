using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Cài đặt repository chi tiết chương trình đào tạo — hỗ trợ lọc theo CTDT và kiểm tra trùng.
/// </summary>
public class ChiTietCTDTRepository(AppDbContext context) : BaseRepository<ChiTietCTDT>(context), IChiTietCTDTRepository
{
    /// <summary>Lấy danh sách chi tiết CTDT phân trang, có thể lọc theo CTDT cụ thể.</summary>
    public async Task<PagedResultDto<ChiTietCTDT>> GetPagedFilterAsync(int page, int pageSize, int? ctdtId)
    {
        var query = DbSet
            .Include(x => x.ChuongTrinhDT)
            .Include(x => x.MonHoc)
            .Include(x => x.HocKy)
            .AsQueryable();

        if (ctdtId.HasValue)
            query = query.Where(x => x.CtdtId == ctdtId.Value);

        var total = await query.CountAsync();
        var data  = await query
            .OrderBy(x => x.HocKyId).ThenBy(x => x.MonHoc.MaMon)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<ChiTietCTDT> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Lấy chi tiết một dòng theo id, bao gồm CTDT, môn học và học kỳ.</summary>
    public async Task<ChiTietCTDT?> GetDetailAsync(int id)
        => await DbSet
            .Include(x => x.ChuongTrinhDT)
            .Include(x => x.MonHoc)
            .Include(x => x.HocKy)
            .FirstOrDefaultAsync(x => x.Id == id);

    /// <summary>Kiểm tra môn học đã tồn tại trong CTDT để tránh thêm trùng.</summary>
    public async Task<bool> ExistsAsync(int ctdtId, int monHocId)
        => await DbSet.AnyAsync(x => x.CtdtId == ctdtId && x.MonHocId == monHocId);

    /// <summary>Lấy toàn bộ danh sách môn học bắt buộc của một CTDT (dùng để đối chiếu điều kiện tốt nghiệp).</summary>
    public async Task<IEnumerable<ChiTietCTDT>> GetByCtdtIdAsync(int ctdtId)
        => await DbSet
            .Where(x => x.CtdtId == ctdtId)
            .Include(x => x.MonHoc)
            .Include(x => x.HocKy)
            .OrderBy(x => x.HocKyId).ThenBy(x => x.MonHoc.MaMon)
            .ToListAsync();

    /// <summary>Đếm số chi tiết CTDT đang gắn với học kỳ này để chặn xoá học kỳ.</summary>
    public async Task<int> CountByHocKyAsync(int hocKyId)
        => await DbSet.CountAsync(x => x.HocKyId == hocKyId);
}

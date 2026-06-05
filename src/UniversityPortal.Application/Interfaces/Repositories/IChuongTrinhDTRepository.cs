using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository chương trình đào tạo — hỗ trợ truy vấn phân trang và lọc theo ngành.
/// </summary>
public interface IChuongTrinhDTRepository : IRepository<ChuongTrinhDT>
{
    /// <summary>Lấy danh sách CTDT có phân trang, lọc theo keyword và ngành.</summary>
    Task<PagedResultDto<ChuongTrinhDT>> GetPagedFilterAsync(int page, int pageSize, string? keyword, int? nganhId);

    /// <summary>Tìm CTDT theo mã CTDT.</summary>
    Task<ChuongTrinhDT?> GetByMaCtdtAsync(string maCtdt);

    /// <summary>Lấy CTDT theo id, include Nganh.</summary>
    Task<ChuongTrinhDT?> GetDetailAsync(int id);
}

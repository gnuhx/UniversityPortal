using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository ngành học — hỗ trợ truy vấn phân trang, lọc và kiểm tra trùng mã.
/// </summary>
public interface INganhHocRepository : IRepository<NganhHoc>
{
    /// <summary>Lấy danh sách ngành có phân trang, lọc theo keyword (mã, tên ngành), ngành (id) và khoá học (qua CTĐT).</summary>
    Task<PagedResultDto<NganhHoc>> GetPagedFilterAsync(int page, int pageSize, string? keyword, int? nganhId = null, string? khoaHoc = null);

    /// <summary>Tìm ngành theo mã ngành.</summary>
    Task<NganhHoc?> GetByMaNganhAsync(string maNganh);

    /// <summary>Lấy ngành theo id, include NganhCha.</summary>
    Task<NganhHoc?> GetDetailAsync(int id);
}

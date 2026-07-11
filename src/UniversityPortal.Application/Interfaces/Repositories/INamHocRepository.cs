using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository năm học — hỗ trợ truy vấn phân trang và lọc theo keyword (tên năm học).
/// </summary>
public interface INamHocRepository : IRepository<NamHoc>
{
    /// <summary>Lấy danh sách năm học có phân trang, lọc theo keyword (tên năm học), sắp giảm dần.</summary>
    Task<PagedResultDto<NamHoc>> GetPagedFilterAsync(int page, int pageSize, string? keyword);

    /// <summary>Lấy tất cả năm học, sắp giảm dần theo tên (năm học mới nhất trước).</summary>
    Task<IEnumerable<NamHoc>> GetAllAsync();

    /// <summary>Tìm năm học theo tên để kiểm tra trùng khi tạo mới.</summary>
    Task<NamHoc?> GetByTenNamHocAsync(string tenNamHoc);
}

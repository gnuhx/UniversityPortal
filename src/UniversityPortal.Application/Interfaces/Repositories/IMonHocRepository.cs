using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository môn học — hỗ trợ truy vấn phân trang và kiểm tra trùng mã môn.
/// </summary>
public interface IMonHocRepository : IRepository<MonHoc>
{
    /// <summary>Lấy danh sách môn học có phân trang và lọc theo keyword (mã, tên môn).</summary>
    Task<PagedResultDto<MonHoc>> GetPagedFilterAsync(int page, int pageSize, string? keyword);

    /// <summary>Tìm môn học theo mã môn.</summary>
    Task<MonHoc?> GetByMaMonAsync(string maMon);
}

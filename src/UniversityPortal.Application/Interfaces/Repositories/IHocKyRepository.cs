using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IHocKyRepository : IRepository<HocKy>
{
    Task<IEnumerable<HocKy>> GetAllAsync();

    /// <summary>Lấy danh sách học kỳ có phân trang, lọc theo năm học.</summary>
    Task<PagedResultDto<HocKy>> GetPagedFilterAsync(int page, int pageSize, int? namHocId);

    /// <summary>Lấy chi tiết học kỳ theo id, include năm học.</summary>
    Task<HocKy?> GetDetailAsync(int id);

    /// <summary>Kiểm tra còn học kỳ nào thuộc năm học này không (dùng khi xoá năm học).</summary>
    Task<bool> ExistsByNamHocAsync(int namHocId);
}

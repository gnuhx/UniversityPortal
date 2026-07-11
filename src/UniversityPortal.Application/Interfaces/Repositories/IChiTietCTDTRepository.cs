using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository chi tiết chương trình đào tạo — hỗ trợ lọc theo CTDT và kiểm tra trùng.
/// </summary>
public interface IChiTietCTDTRepository : IRepository<ChiTietCTDT>
{
    /// <summary>Lấy danh sách chi tiết CTDT có phân trang, lọc theo CTDT và môn học.</summary>
    Task<PagedResultDto<ChiTietCTDT>> GetPagedFilterAsync(int page, int pageSize, int? ctdtId);

    /// <summary>Lấy chi tiết CTDT theo id, include CTDT, MonHoc, HocKy.</summary>
    Task<ChiTietCTDT?> GetDetailAsync(int id);

    /// <summary>Kiểm tra môn học đã tồn tại trong CTDT chưa (tránh thêm trùng).</summary>
    Task<bool> ExistsAsync(int ctdtId, int monHocId);

    /// <summary>Lấy toàn bộ danh sách môn học bắt buộc của một CTDT, include MonHoc và HocKy.</summary>
    Task<IEnumerable<ChiTietCTDT>> GetByCtdtIdAsync(int ctdtId);

    /// <summary>Đếm số chi tiết CTDT đang gắn với học kỳ này (dùng khi xoá học kỳ).</summary>
    Task<int> CountByHocKyAsync(int hocKyId);
}

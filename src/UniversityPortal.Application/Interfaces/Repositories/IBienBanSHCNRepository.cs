using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository biên bản sinh hoạt chủ nhiệm — hỗ trợ lọc theo lớp và lấy chi tiết kèm công việc/điểm danh vắng.
/// </summary>
public interface IBienBanSHCNRepository : IRepository<BienBanSHCN>
{
    /// <summary>Lấy danh sách biên bản có phân trang, lọc theo lớp (bỏ trống = tất cả lớp).</summary>
    Task<PagedResultDto<BienBanSHCN>> GetPagedFilterAsync(int? lopId, int page, int pageSize);

    /// <summary>Lấy chi tiết biên bản theo id, kèm công việc và điểm danh vắng.</summary>
    Task<BienBanSHCN?> GetDetailAsync(int id);

    /// <summary>Lấy toàn bộ biên bản của 1 lớp (không phân trang, dùng cho sinh viên xem lịch sử lớp mình).</summary>
    Task<IEnumerable<BienBanSHCN>> GetByLopAsync(int lopId);
}

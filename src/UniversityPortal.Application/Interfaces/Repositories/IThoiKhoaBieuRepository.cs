using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IThoiKhoaBieuRepository : IRepository<ThoiKhoaBieu>
{
    /// <summary>Danh sách buổi học phân trang, có thể lọc theo lớp học phần.</summary>
    Task<PagedResultDto<ThoiKhoaBieu>> GetPagedFilterAsync(int page, int pageSize, int? lopHpId);

    /// <summary>Chi tiết một buổi học kèm đầy đủ thông tin lớp HP / môn học / tuần.</summary>
    Task<ThoiKhoaBieu?> GetDetailAsync(int id);

    /// <summary>Lịch học của một sinh viên (qua các lớp HP đã đăng ký), lọc theo học kỳ nếu có.</summary>
    Task<IEnumerable<ThoiKhoaBieu>> GetForSinhVienAsync(int sinhVienId, int? hocKyId);

    /// <summary>Lịch dạy của một giáo viên, lọc theo học kỳ nếu có.</summary>
    Task<IEnumerable<ThoiKhoaBieu>> GetForGiaoVienAsync(int giaoVienId, int? hocKyId);

    /// <summary>
    /// Kiểm tra buổi học mới/sửa có trùng phòng trong cùng tuần + thứ + khung tiết
    /// với một buổi học khác không (dùng để chặn double-booking phòng học).
    /// </summary>
    Task<bool> ExistsConflictAsync(int tuanHocId, int thu, string phongHoc, int tietBatDau, int tietKetThuc, int? excludeId);
}

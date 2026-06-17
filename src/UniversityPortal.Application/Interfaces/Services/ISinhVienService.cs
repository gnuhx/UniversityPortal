// Giao diện dịch vụ quản lý sinh viên — CRUD kèm tài khoản liên kết
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.SinhVien;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý sinh viên.
/// Mỗi thao tác tạo/xoá sinh viên đồng thời tác động lên tài khoản liên kết.
/// </summary>
public interface ISinhVienService
{
    /// <summary>Lấy danh sách sinh viên có phân trang, lọc theo họ tên / MSSV và lớp.</summary>
    Task<PagedResultDto<SinhVienDto>> GetPagedAsync(int page, int pageSize, string? keyword, int? lopId);

    /// <summary>Lấy thông tin chi tiết sinh viên theo id.</summary>
    Task<SinhVienDto> GetByIdAsync(int id);

    /// <summary>Tạo mới sinh viên kèm tài khoản đăng nhập với vai trò Sinh viên.</summary>
    Task<SinhVienDto> CreateAsync(CreateSinhVienDto dto);

    /// <summary>Cập nhật thông tin sinh viên và tài khoản liên kết.</summary>
    Task<SinhVienDto> UpdateAsync(int id, UpdateSinhVienDto dto);

    /// <summary>Xoá sinh viên; không xoá tài khoản, chỉ khoá tài khoản liên kết.</summary>
    Task DeleteAsync(int id);

    /// <summary>Lấy thông tin sinh viên theo tài khoản đăng nhập hiện tại (dùng cho sinh viên xem hồ sơ).</summary>
    Task<SinhVienDto> GetMeAsync(int taiKhoanId);
}

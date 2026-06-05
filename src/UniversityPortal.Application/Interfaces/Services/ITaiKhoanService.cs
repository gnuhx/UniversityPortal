// Giao diện dịch vụ quản lý tài khoản — CRUD, đổi mật khẩu, khoá/mở, upload avatar
using Microsoft.AspNetCore.Http;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.TaiKhoan;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý tài khoản người dùng hệ thống.
/// Bao gồm CRUD, đổi mật khẩu, khoá/mở tài khoản và cập nhật ảnh đại diện.
/// </summary>
public interface ITaiKhoanService
{
    /// <summary>Lấy danh sách tài khoản có phân trang và bộ lọc.</summary>
    Task<PagedResultDto<TaiKhoanDto>> GetPagedAsync(
        int page, int pageSize, string? keyword, int? vaiTroId, bool? trangThai);

    /// <summary>Lấy thông tin chi tiết một tài khoản theo id.</summary>
    Task<TaiKhoanDto> GetByIdAsync(int id);

    /// <summary>Tạo mới tài khoản; ném BadRequestException nếu tên đăng nhập đã tồn tại.</summary>
    Task<TaiKhoanDto> CreateAsync(CreateTaiKhoanDto dto);

    /// <summary>Cập nhật thông tin tài khoản theo id.</summary>
    Task<TaiKhoanDto> UpdateAsync(int id, UpdateTaiKhoanDto dto);

    /// <summary>Đổi mật khẩu; xác thực mật khẩu cũ trước khi thay đổi.</summary>
    Task DoiMatKhauAsync(int id, DoiMatKhauDto dto);

    /// <summary>Bật/tắt trạng thái hoạt động của tài khoản.</summary>
    Task<TaiKhoanDto> ToggleTrangThaiAsync(int id);

    /// <summary>Cập nhật ảnh đại diện; xoá ảnh cũ nếu có và lưu ảnh mới vào /uploads/avatars.</summary>
    Task<string> CapNhatAnhDaiDienAsync(int id, IFormFile file);
}

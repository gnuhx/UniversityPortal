// Giao diện dịch vụ xử lý file — lưu và xoá ảnh đại diện trên filesystem
using Microsoft.AspNetCore.Http;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ xử lý file upload / delete trên hệ thống lưu trữ cục bộ.
/// Thư mục gốc: /uploads (được mount vào Docker volume).
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Lưu ảnh đại diện vào thư mục /uploads/avatars.
    /// Trả về đường dẫn tương đối (vd: /uploads/avatars/123_abc.jpg) để lưu vào DB.
    /// </summary>
    Task<string> SaveAvatarAsync(IFormFile file, int taiKhoanId);

    /// <summary>
    /// Xoá file theo đường dẫn tương đối nếu tồn tại trên disk.
    /// Không ném exception nếu file không tồn tại.
    /// </summary>
    void DeleteFile(string? relativePath);
}

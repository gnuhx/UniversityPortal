// Dịch vụ xử lý file — lưu ảnh đại diện vào volume /uploads, xoá ảnh cũ khi cập nhật
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.Infrastructure.Services;

/// <summary>
/// Cài đặt dịch vụ file upload/delete trên filesystem cục bộ.
/// Ảnh đại diện được lưu tại {contentRoot}/uploads/avatars/ và phục vụ qua đường dẫn tĩnh /uploads.
/// </summary>
public class FileService(IHostEnvironment env) : IFileService
{
    // Thư mục gốc để lưu file — được mount vào Docker volume /app/uploads
    private readonly string _uploadRoot = Path.Combine(env.ContentRootPath, "uploads");

    /// <summary>
    /// Lưu ảnh đại diện vào thư mục uploads/avatars.
    /// Tên file = {taiKhoanId}_{GUID}{ext} để tránh trùng và ghi đè nhầm.
    /// </summary>
    public async Task<string> SaveAvatarAsync(IFormFile file, int taiKhoanId)
    {
        // Tạo thư mục avatars nếu chưa tồn tại
        var avatarDir = Path.Combine(_uploadRoot, "avatars");
        Directory.CreateDirectory(avatarDir);

        var ext      = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{taiKhoanId}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(avatarDir, fileName);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        // Trả về đường dẫn tương đối để lưu vào DB và phục vụ qua static files
        return $"/uploads/avatars/{fileName}";
    }

    /// <summary>
    /// Xoá file theo đường dẫn tương đối.
    /// Bỏ qua nếu đường dẫn null hoặc file không tồn tại.
    /// </summary>
    public void DeleteFile(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        // Chuyển đường dẫn tương đối (/uploads/avatars/...) sang đường dẫn tuyệt đối
        var fullPath = Path.Combine(env.ContentRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}

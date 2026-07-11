// DTO dùng chung cho tạo mới và cập nhật ngành học
namespace UniversityPortal.Application.DTOs.NganhHoc;

/// <summary>
/// Dữ liệu đầu vào khi tạo mới hoặc cập nhật ngành học.
/// NganhChaId = null nghĩa là ngành cấp cao nhất.
/// </summary>
public class UpsertNganhHocDto
{
    public string MaNganh { get; set; } = string.Empty;
    public string TenNganh { get; set; } = string.Empty;
    public int? NganhChaId { get; set; }
    public int? PhongBanId { get; set; }
}

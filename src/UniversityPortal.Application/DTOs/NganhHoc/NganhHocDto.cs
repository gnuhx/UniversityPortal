// DTO phản hồi thông tin ngành học — hỗ trợ cấu trúc ngành cha–con
namespace UniversityPortal.Application.DTOs.NganhHoc;

/// <summary>
/// Thông tin ngành học trả về client, bao gồm tên ngành cha nếu có.
/// </summary>
public class NganhHocDto
{
    public int Id { get; set; }
    public string MaNganh { get; set; } = string.Empty;
    public string TenNganh { get; set; } = string.Empty;
    public int? NganhChaId { get; set; }
    public string? TenNganhCha { get; set; }
    public DateTime CreatedAt { get; set; }
}

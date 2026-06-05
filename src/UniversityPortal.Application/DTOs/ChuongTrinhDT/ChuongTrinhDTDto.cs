// DTO phản hồi thông tin chương trình đào tạo kèm tên ngành
namespace UniversityPortal.Application.DTOs.ChuongTrinhDT;

/// <summary>
/// Thông tin chương trình đào tạo trả về client, bao gồm tên ngành học liên kết.
/// </summary>
public class ChuongTrinhDTDto
{
    public int Id { get; set; }
    public string MaCtdt { get; set; } = string.Empty;
    public int NganhId { get; set; }
    public string TenNganh { get; set; } = string.Empty;
    public string KhoaHoc { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

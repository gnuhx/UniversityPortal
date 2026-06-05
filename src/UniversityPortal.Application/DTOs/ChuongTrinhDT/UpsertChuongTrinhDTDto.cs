// DTO dùng chung cho tạo mới và cập nhật chương trình đào tạo
namespace UniversityPortal.Application.DTOs.ChuongTrinhDT;

/// <summary>
/// Dữ liệu đầu vào khi tạo mới hoặc cập nhật chương trình đào tạo.
/// </summary>
public class UpsertChuongTrinhDTDto
{
    public string MaCtdt { get; set; } = string.Empty;
    public int NganhId { get; set; }
    public string KhoaHoc { get; set; } = string.Empty;
}

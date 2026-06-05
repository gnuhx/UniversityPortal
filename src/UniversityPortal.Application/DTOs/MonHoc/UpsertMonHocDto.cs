// DTO dùng chung cho tạo mới và cập nhật môn học
namespace UniversityPortal.Application.DTOs.MonHoc;

/// <summary>
/// Dữ liệu đầu vào khi tạo mới hoặc cập nhật môn học.
/// </summary>
public class UpsertMonHocDto
{
    public string MaMon { get; set; } = string.Empty;
    public string TenMon { get; set; } = string.Empty;
}

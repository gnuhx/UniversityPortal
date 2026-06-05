// DTO phản hồi thông tin môn học
namespace UniversityPortal.Application.DTOs.MonHoc;

/// <summary>
/// Thông tin môn học trả về client.
/// </summary>
public class MonHocDto
{
    public int Id { get; set; }
    public string MaMon { get; set; } = string.Empty;
    public string TenMon { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

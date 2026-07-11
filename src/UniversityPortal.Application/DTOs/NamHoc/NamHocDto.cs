// DTO phản hồi thông tin năm học
namespace UniversityPortal.Application.DTOs.NamHoc;

public class NamHocDto
{
    public int Id { get; set; }
    public string TenNamHoc { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

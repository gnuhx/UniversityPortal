// DTO phản hồi thông tin phòng ban / khoa
namespace UniversityPortal.Application.DTOs.PhongBan;

/// <summary>
/// Thông tin phòng ban (phòng chức năng hoặc khoa) trả về client.
/// </summary>
public class PhongBanDto
{
    public int Id { get; set; }
    public string TenPhongBan { get; set; } = string.Empty;
}

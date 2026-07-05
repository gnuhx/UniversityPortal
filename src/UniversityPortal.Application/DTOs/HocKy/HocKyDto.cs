// DTO phản hồi thông tin học kỳ
namespace UniversityPortal.Application.DTOs.HocKy;

public class HocKyDto
{
    public int Id { get; set; }
    public string TenHocKy { get; set; } = string.Empty;
    public int NamHocId { get; set; }
    public string TenNamHoc { get; set; } = string.Empty;
    public DateOnly NgayBatDau { get; set; }
}

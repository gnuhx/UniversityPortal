// DTO dùng chung cho tạo mới và cập nhật học kỳ
namespace UniversityPortal.Application.DTOs.HocKy;

public class UpsertHocKyDto
{
    public string TenHocKy { get; set; } = string.Empty;
    public int NamHocId { get; set; }
    public DateOnly NgayBatDau { get; set; }
}

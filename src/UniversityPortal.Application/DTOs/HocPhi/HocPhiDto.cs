namespace UniversityPortal.Application.DTOs.HocPhi;

public class HocPhiDto
{
    public int Id { get; set; }
    public int SinhVienId { get; set; }
    public string TenSinhVien { get; set; } = string.Empty;
    public string Mssv { get; set; } = string.Empty;
    public int HocKyId { get; set; }
    public string TenHocKy { get; set; } = string.Empty;
    public decimal SoTien { get; set; }
    public string TrangThaiDong { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

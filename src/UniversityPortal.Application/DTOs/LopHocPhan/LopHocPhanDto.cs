namespace UniversityPortal.Application.DTOs.LopHocPhan;

public class LopHocPhanDto
{
    public int Id { get; set; }
    public string MaLopHp { get; set; } = string.Empty;
    public int ChiTietCtdtId { get; set; }
    public string MaMon { get; set; } = string.Empty;
    public string TenMon { get; set; } = string.Empty;
    public int HocKyId { get; set; }
    public string TenHocKy { get; set; } = string.Empty;
    public int GiaoVienId { get; set; }
    public string TenGiaoVien { get; set; } = string.Empty;
    public int SoSinhVien { get; set; }
    public bool KhoaBangDiem { get; set; }
    public bool TrangThaiKetThuc { get; set; }
}

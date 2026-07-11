namespace UniversityPortal.Application.DTOs.DanhSachLopHP;

public class DanhSachLopHPDto
{
    public int Id { get; set; }
    public int LopHpId { get; set; }
    public string MaLopHp { get; set; } = string.Empty;
    public string MaMon { get; set; } = string.Empty;
    public string TenMon { get; set; } = string.Empty;
    public int HocKyId { get; set; }
    public string TenHocKy { get; set; } = string.Empty;
    public int NamHocId { get; set; }
    public string TenNamHoc { get; set; } = string.Empty;
    public string TenGiaoVien { get; set; } = string.Empty;
    public string LoaiDangKy { get; set; } = string.Empty;
    public string TrangThaiDuyet { get; set; } = string.Empty;
    public float? DiemQt1 { get; set; }
    public float? DiemQt2 { get; set; }
    public float? DiemThi { get; set; }
    public float? DiemTongKet { get; set; }
    public decimal? SoTienPhaiDong { get; set; }
    public string? TrangThaiDongTien { get; set; }
    public bool KhoaBangDiem { get; set; }
    public string? TenSinhVien { get; set; }
    public string? Mssv { get; set; }
}

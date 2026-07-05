// DTO phản hồi một buổi học trong thời khoá biểu
namespace UniversityPortal.Application.DTOs.ThoiKhoaBieu;

public class ThoiKhoaBieuDto
{
    public int Id { get; set; }
    public int LopHpId { get; set; }
    public string MaLopHp { get; set; } = string.Empty;
    public string MaMon { get; set; } = string.Empty;
    public string TenMon { get; set; } = string.Empty;
    public string TenGiaoVien { get; set; } = string.Empty;
    public int HocKyId { get; set; }
    public string TenHocKy { get; set; } = string.Empty;
    public int TuanHocId { get; set; }
    public string MaTuan { get; set; } = string.Empty;
    public int SoThuTuTuan { get; set; }
    public int Thu { get; set; }
    public int TietBatDau { get; set; }
    public int TietKetThuc { get; set; }
    public string PhongHoc { get; set; } = string.Empty;

    /// <summary>Ngày học thực tế = TuanHoc.NgayBatDau (thứ Hai) + (Thu - 2) ngày.</summary>
    public DateOnly NgayHoc { get; set; }
}

using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class DanhSachLopHP : AuditableEntity
{
    public int SinhVienId { get; set; }
    public int LopHpId { get; set; }
    public string LoaiDangKy { get; set; } = string.Empty;
    public string TrangThaiDuyet { get; set; } = string.Empty;
    public int? NguoiDuyetId { get; set; }
    public float? DiemQt1 { get; set; }
    public float? DiemQt2 { get; set; }
    public float? DiemThi { get; set; }
    public float? DiemTongKet { get; set; }
    public decimal? SoTienPhaiDong { get; set; }
    public string? TrangThaiDongTien { get; set; }

    public SinhVien SinhVien { get; set; } = null!;
    public LopHocPhan LopHocPhan { get; set; } = null!;
    public TaiKhoan? NguoiDuyet { get; set; }
}

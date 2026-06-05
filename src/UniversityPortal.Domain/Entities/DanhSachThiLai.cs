using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class DanhSachThiLai : AuditableEntity
{
    public int SinhVienId { get; set; }
    public int LopHpId { get; set; }
    public float? DiemThiLai { get; set; }
    public decimal SoTienPhaiDong { get; set; }
    public string TrangThaiDongTien { get; set; } = string.Empty;
    public int? NguoiDuyetId { get; set; }

    public SinhVien SinhVien { get; set; } = null!;
    public LopHocPhan LopHocPhan { get; set; } = null!;
    public TaiKhoan? NguoiDuyet { get; set; }
}

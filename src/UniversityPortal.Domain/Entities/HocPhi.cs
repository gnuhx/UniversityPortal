using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class HocPhi : AuditableEntity
{
    public int SinhVienId { get; set; }
    public int HocKyId { get; set; }
    public decimal SoTien { get; set; }
    public string TrangThaiDong { get; set; } = string.Empty;

    public SinhVien SinhVien { get; set; } = null!;
    public HocKy HocKy { get; set; } = null!;
}

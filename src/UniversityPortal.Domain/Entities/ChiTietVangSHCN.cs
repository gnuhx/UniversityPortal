using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class ChiTietVangSHCN : AuditableEntity
{
    public int BienBanId { get; set; }
    public int SinhVienId { get; set; }
    public string? LyDo { get; set; }
    public bool CoPhep { get; set; }

    public BienBanSHCN BienBan { get; set; } = null!;
    public SinhVien SinhVien { get; set; } = null!;
}

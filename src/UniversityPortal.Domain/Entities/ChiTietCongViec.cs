using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class ChiTietCongViec : AuditableEntity
{
    public int BienBanId { get; set; }
    public string TenCongViec { get; set; } = string.Empty;
    public string TrangThaiViec { get; set; } = string.Empty;

    public BienBanSHCN BienBan { get; set; } = null!;
}

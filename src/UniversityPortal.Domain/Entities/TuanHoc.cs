using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class TuanHoc : AuditableEntity
{
    public int NamHocId { get; set; }
    public string MaTuan { get; set; } = string.Empty;
    public int SoThuTuTuan { get; set; }
    public DateOnly NgayBatDau { get; set; }
    public DateOnly NgayKetThuc { get; set; }

    public NamHoc NamHoc { get; set; } = null!;
    public ICollection<ThoiKhoaBieu> ThoiKhoaBieus { get; set; } = [];
    public ICollection<BienBanSHCN> BienBanSHCNs { get; set; } = [];
}

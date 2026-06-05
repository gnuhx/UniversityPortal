using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class ThoiKhoaBieu : AuditableEntity
{
    public int LopHpId { get; set; }
    public int TuanHocId { get; set; }
    public int Thu { get; set; }
    public int TietBatDau { get; set; }
    public int TietKetThuc { get; set; }
    public string PhongHoc { get; set; } = string.Empty;

    public LopHocPhan LopHocPhan { get; set; } = null!;
    public TuanHoc TuanHoc { get; set; } = null!;
}

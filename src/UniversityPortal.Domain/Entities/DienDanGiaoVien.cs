using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class DienDanGiaoVien : AuditableEntity
{
    public int LopId { get; set; }
    public int GiaoVienId { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public DateTime NgayGui { get; set; }

    public LopSinhHoat Lop { get; set; } = null!;
    public GiaoVien GiaoVien { get; set; } = null!;
}

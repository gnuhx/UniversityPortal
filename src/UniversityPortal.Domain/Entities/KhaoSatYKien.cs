using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class KhaoSatYKien : AuditableEntity
{
    public int SinhVienId { get; set; }
    public int LopHpId { get; set; }
    public int DiemDanhGia { get; set; }
    public string? GopY { get; set; }

    public SinhVien SinhVien { get; set; } = null!;
    public LopHocPhan LopHocPhan { get; set; } = null!;
}

using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class HocBa : AuditableEntity
{
    public int SinhVienId { get; set; }
    public int CtdtId { get; set; }
    public float DiemTbcTichLuy { get; set; }
    public int SoTinChiTichLuy { get; set; }

    public SinhVien SinhVien { get; set; } = null!;
    public ChuongTrinhDT ChuongTrinhDT { get; set; } = null!;
}

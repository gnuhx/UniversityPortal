using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class ChiTietCTDT : AuditableEntity
{
    public int CtdtId { get; set; }
    public int MonHocId { get; set; }
    public int HocKyId { get; set; }
    public int SoTinChi { get; set; }
    public bool TinhDiemTb { get; set; } = true;

    public ChuongTrinhDT ChuongTrinhDT { get; set; } = null!;
    public MonHoc MonHoc { get; set; } = null!;
    public HocKy HocKy { get; set; } = null!;
    public ICollection<LopHocPhan> LopHocPhans { get; set; } = [];
}

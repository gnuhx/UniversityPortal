using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class ChuongTrinhDT : AuditableEntity
{
    public string MaCtdt { get; set; } = string.Empty;
    public int NganhId { get; set; }
    public string KhoaHoc { get; set; } = string.Empty;

    public NganhHoc Nganh { get; set; } = null!;
    public ICollection<LopSinhHoat> LopSinhHoats { get; set; } = [];
    public ICollection<ChiTietCTDT> ChiTietCTDTs { get; set; } = [];
    public ICollection<HocBa> HocBas { get; set; } = [];
}

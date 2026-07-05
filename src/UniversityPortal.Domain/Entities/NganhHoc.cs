using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class NganhHoc : AuditableEntity
{
    public string MaNganh { get; set; } = string.Empty;
    public string TenNganh { get; set; } = string.Empty;
    public int? NganhChaId { get; set; }
    public int? PhongBanId { get; set; }

    public NganhHoc? NganhCha { get; set; }
    public ICollection<NganhHoc> NganhCons { get; set; } = [];
    public PhongBan? PhongBan { get; set; }
    public ICollection<ChuongTrinhDT> ChuongTrinhDTs { get; set; } = [];
}

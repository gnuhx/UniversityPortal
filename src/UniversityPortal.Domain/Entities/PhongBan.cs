using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class PhongBan : AuditableEntity
{
    public string TenPhongBan { get; set; } = string.Empty;

    public ICollection<TaiKhoan> TaiKhoans { get; set; } = [];
}

using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class VaiTro : AuditableEntity
{
    public string TenVaiTro { get; set; } = string.Empty;

    public ICollection<TaiKhoan> TaiKhoans { get; set; } = [];
}

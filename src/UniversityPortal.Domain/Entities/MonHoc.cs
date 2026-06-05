using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class MonHoc : AuditableEntity
{
    public string MaMon { get; set; } = string.Empty;
    public string TenMon { get; set; } = string.Empty;

    public ICollection<ChiTietCTDT> ChiTietCTDTs { get; set; } = [];
}

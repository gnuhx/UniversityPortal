using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class NamHoc : AuditableEntity
{
    public string TenNamHoc { get; set; } = string.Empty;

    public ICollection<HocKy> HocKys { get; set; } = [];
    public ICollection<TuanHoc> TuanHocs { get; set; } = [];
}

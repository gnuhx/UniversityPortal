using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class HocKy : AuditableEntity
{
    public string TenHocKy { get; set; } = string.Empty;
    public int NamHocId { get; set; }
    public DateOnly NgayBatDau { get; set; }

    public NamHoc NamHoc { get; set; } = null!;
    public ICollection<ChiTietCTDT> ChiTietCTDTs { get; set; } = [];
    public ICollection<LopHocPhan> LopHocPhans { get; set; } = [];
    public ICollection<DiemRenLuyen> DiemRenLuyens { get; set; } = [];
    public ICollection<HocPhi> HocPhis { get; set; } = [];
}

using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class DatPhongThucHanh : AuditableEntity
{
    public int GiaoVienId { get; set; }
    public string PhongHoc { get; set; } = string.Empty;
    public DateOnly NgayDat { get; set; }
    public int CaHoc { get; set; }
    public string? LyDo { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public int? NguoiDuyetId { get; set; }

    public GiaoVien GiaoVien { get; set; } = null!;
    public TaiKhoan? NguoiDuyet { get; set; }
}

using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class YeuCauSuaDiem : AuditableEntity
{
    public int LopHpId { get; set; }
    public int GiaoVienId { get; set; }
    public string LyDo { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
    public int? NguoiDuyetId { get; set; }

    public LopHocPhan LopHocPhan { get; set; } = null!;
    public GiaoVien GiaoVien { get; set; } = null!;
    public TaiKhoan? NguoiDuyet { get; set; }
}

using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Domain.Entities;

public class YeuCauHanhChinh : AuditableEntity
{
    public int SinhVienId { get; set; }
    public string LoaiYeuCau { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string? FileDinhKem { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public int? NguoiDuyetId { get; set; }
    public DateTime NgayTao { get; set; }

    public SinhVien SinhVien { get; set; } = null!;
    public TaiKhoan? NguoiDuyet { get; set; }
}

namespace UniversityPortal.Application.DTOs.SinhVien;

/// <summary>Kết quả kiểm tra điều kiện tốt nghiệp của sinh viên, đối chiếu với CTDT.</summary>
public class TotNghiepDto
{
    public bool DuDieuKienTotNghiep { get; set; }
    public int TongSoTinChiYeuCau { get; set; }
    public int TongSoTinChiDaTichLuy { get; set; }
    public List<MonHocConThieuDto> MonHocConThieu { get; set; } = [];
}

/// <summary>Một môn học bắt buộc của CTDT mà sinh viên chưa hoàn thành (chưa học hoặc chưa đạt).</summary>
public class MonHocConThieuDto
{
    public string MaMon { get; set; } = string.Empty;
    public string TenMon { get; set; } = string.Empty;
    public int SoTinChi { get; set; }
    public string TenHocKy { get; set; } = string.Empty;
}

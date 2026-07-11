namespace UniversityPortal.Application.DTOs.HocPhi;

public class GenerateHocPhiDto
{
    public int HocKyId { get; set; }
    public decimal TienMotTinChi { get; set; }
}

public class GenerateHocPhiResultDto
{
    public int Created { get; set; }
    public int Skipped { get; set; }
    public string Message => $"Tạo mới {Created} học phí, bỏ qua {Skipped} (đã tồn tại).";
}

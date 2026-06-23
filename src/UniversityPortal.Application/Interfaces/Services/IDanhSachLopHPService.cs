using UniversityPortal.Application.DTOs.DanhSachLopHP;

namespace UniversityPortal.Application.Interfaces.Services;

public interface IDanhSachLopHPService
{
    /// <summary>Lấy danh sách môn học + điểm của sinh viên đang đăng nhập.</summary>
    Task<IEnumerable<DanhSachLopHPDto>> GetBySinhVienMeAsync(int taiKhoanId);

    /// <summary>Lấy danh sách sinh viên trong một lớp học phần (dùng cho GV, Admin).</summary>
    Task<IEnumerable<DanhSachLopHPDto>> GetByLopHocPhanAsync(int lopHpId);

    /// <summary>Giáo viên nhập điểm cho một dòng DanhSachLopHP.</summary>
    Task<DanhSachLopHPDto> NhapDiemAsync(int danhSachLopHpId, int taiKhoanId, NhapDiemDto dto);
}

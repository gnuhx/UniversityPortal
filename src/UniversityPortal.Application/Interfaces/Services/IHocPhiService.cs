using UniversityPortal.Application.DTOs.HocPhi;

namespace UniversityPortal.Application.Interfaces.Services;

public interface IHocPhiService
{
    /// <summary>Sinh viên xem học phí của mình.</summary>
    Task<IEnumerable<HocPhiDto>> GetByMeAsync(int taiKhoanId);

    /// <summary>Admin tạo học phí cho sinh viên trong học kỳ.</summary>
    Task<HocPhiDto> CreateAsync(CreateHocPhiDto dto);

    /// <summary>Admin xem học phí theo học kỳ.</summary>
    Task<IEnumerable<HocPhiDto>> GetByHocKyAsync(int hocKyId);

    /// <summary>Admin cập nhật trạng thái đóng tiền.</summary>
    Task<HocPhiDto> UpdateTrangThaiAsync(int id, string trangThai);
}

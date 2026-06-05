// Giao diện dịch vụ quản lý chi tiết chương trình đào tạo — thêm/sửa/xoá môn trong CTDT
using UniversityPortal.Application.DTOs.ChiTietCTDT;
using UniversityPortal.Application.DTOs.Common;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý chi tiết chương trình đào tạo.
/// Cho phép thêm, sửa, xoá môn học trong một CTDT cụ thể.
/// </summary>
public interface IChiTietCTDTService
{
    /// <summary>Lấy danh sách chi tiết CTDT có phân trang, lọc theo CTDT.</summary>
    Task<PagedResultDto<ChiTietCTDTDto>> GetPagedAsync(int page, int pageSize, int? ctdtId);

    /// <summary>Lấy thông tin chi tiết một dòng theo id.</summary>
    Task<ChiTietCTDTDto> GetByIdAsync(int id);

    /// <summary>Thêm môn học vào CTDT; ném BadRequestException nếu môn đã có trong CTDT đó.</summary>
    Task<ChiTietCTDTDto> CreateAsync(CreateChiTietCTDTDto dto);

    /// <summary>Cập nhật số tín chỉ và cờ tính điểm TB của một dòng chi tiết.</summary>
    Task<ChiTietCTDTDto> UpdateAsync(int id, UpdateChiTietCTDTDto dto);

    /// <summary>Xoá một dòng chi tiết khỏi CTDT.</summary>
    Task DeleteAsync(int id);
}

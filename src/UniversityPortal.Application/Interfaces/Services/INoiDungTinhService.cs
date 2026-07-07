using UniversityPortal.Application.DTOs.NoiDungTinh;

namespace UniversityPortal.Application.Interfaces.Services;

/// <summary>
/// Dịch vụ quản lý nội dung tĩnh (Thư viện, Học Vụ,...) — mỗi khu vực gồm nhiều mục/tab có thứ tự.
/// </summary>
public interface INoiDungTinhService
{
    /// <summary>Lấy các mục nội dung của 1 khu vực, sắp theo Thứ tự — dùng cho trang public.</summary>
    Task<IEnumerable<NoiDungTinhDto>> GetByKhuVucAsync(string khuVuc);

    /// <summary>Lấy chi tiết 1 mục theo id.</summary>
    Task<NoiDungTinhDto> GetByIdAsync(int id);

    /// <summary>Tạo mới 1 mục nội dung; ném BadRequestException nếu MaMuc đã tồn tại trong cùng khu vực.</summary>
    Task<NoiDungTinhDto> CreateAsync(UpsertNoiDungTinhDto dto);

    /// <summary>Cập nhật 1 mục nội dung theo id.</summary>
    Task<NoiDungTinhDto> UpdateAsync(int id, UpsertNoiDungTinhDto dto);

    /// <summary>Xoá 1 mục nội dung theo id.</summary>
    Task DeleteAsync(int id);
}

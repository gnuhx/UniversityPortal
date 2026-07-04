using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository sinh viên — mở rộng IRepository với các truy vấn đặc thù.
/// </summary>
public interface ISinhVienRepository : IRepository<SinhVien>
{
    /// <summary>Tìm sinh viên theo MSSV, include TaiKhoan.</summary>
    Task<SinhVien?> GetByMssvAsync(string mssv);

    /// <summary>Tìm sinh viên theo tài khoản liên kết.</summary>
    Task<SinhVien?> GetByTaiKhoanIdAsync(int taiKhoanId);

    /// <summary>Lấy danh sách sinh viên theo lớp có phân trang (dùng cho GVCN xem lớp).</summary>
    Task<PagedResultDto<SinhVien>> GetPagedByLopAsync(int lopId, int page, int pageSize);

    /// <summary>Lấy danh sách sinh viên có phân trang và bộ lọc keyword + lớp.</summary>
    Task<PagedResultDto<SinhVien>> GetPagedFilterAsync(
        int page, int pageSize, string? keyword, int? lopId);

    /// <summary>Lấy thông tin sinh viên theo id, include TaiKhoan và Lop.</summary>
    Task<SinhVien?> GetDetailAsync(int id);

    /// <summary>Lấy sinh viên theo id kèm Lop sinh hoạt và Chương trình đào tạo của lớp.</summary>
    Task<SinhVien?> GetByIdWithLopCtdtAsync(int id);
}

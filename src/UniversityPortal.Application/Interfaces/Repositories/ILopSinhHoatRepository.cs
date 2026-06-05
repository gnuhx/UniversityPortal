using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository lớp sinh hoạt — hỗ trợ lọc theo GVCN, CTDT và kiểm tra mã lớp trùng.
/// </summary>
public interface ILopSinhHoatRepository : IRepository<LopSinhHoat>
{
    /// <summary>Lấy danh sách lớp có phân trang và lọc theo keyword (mã lớp).</summary>
    Task<PagedResultDto<LopSinhHoat>> GetPagedFilterAsync(int page, int pageSize, string? keyword, int? gvcnId);

    /// <summary>Tìm lớp theo mã lớp.</summary>
    Task<LopSinhHoat?> GetByMaLopAsync(string maLop);

    /// <summary>Lấy lớp theo id, include Gvcn, ThuKy, ChuongTrinhDT và đếm SinhViens.</summary>
    Task<LopSinhHoat?> GetDetailAsync(int id);
}

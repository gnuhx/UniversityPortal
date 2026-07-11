using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface ILopHocPhanRepository : IRepository<LopHocPhan>
{
    Task<PagedResultDto<LopHocPhan>> GetPagedByHocKyAsync(int hocKyId, int page, int pageSize);
    Task<PagedResultDto<LopHocPhan>> GetPagedByGiaoVienAsync(int giaoVienId, int page, int pageSize);
    Task<IEnumerable<LopHocPhan>> GetByGiaoVienWithDetailsAsync(int giaoVienId);
    Task<LopHocPhan?> GetDetailAsync(int id);

    /// <summary>Danh sách lớp học phần phân trang, có đầy đủ thông tin môn/giáo viên, lọc theo học kỳ và/hoặc từ khoá mã lớp.</summary>
    Task<PagedResultDto<LopHocPhan>> GetPagedFilterAsync(int page, int pageSize, int? hocKyId, string? keyword);
}

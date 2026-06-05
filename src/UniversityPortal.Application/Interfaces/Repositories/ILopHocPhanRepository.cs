using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface ILopHocPhanRepository : IRepository<LopHocPhan>
{
    Task<PagedResultDto<LopHocPhan>> GetPagedByHocKyAsync(int hocKyId, int page, int pageSize);
    Task<PagedResultDto<LopHocPhan>> GetPagedByGiaoVienAsync(int giaoVienId, int page, int pageSize);
}

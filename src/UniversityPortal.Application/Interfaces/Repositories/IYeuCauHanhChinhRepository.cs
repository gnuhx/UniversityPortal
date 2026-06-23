using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IYeuCauHanhChinhRepository : IRepository<YeuCauHanhChinh>
{
    Task<IEnumerable<YeuCauHanhChinh>> GetBySinhVienAsync(int sinhVienId);
    Task<PagedResultDto<YeuCauHanhChinh>> GetAllPagedAsync(int page, int pageSize, string? trangThai);
    Task<YeuCauHanhChinh?> GetDetailAsync(int id);
}

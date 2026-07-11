using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IYeuCauSuaDiemRepository : IRepository<YeuCauSuaDiem>
{
    Task<IEnumerable<YeuCauSuaDiem>> GetByGiaoVienAsync(int giaoVienId);
    Task<PagedResultDto<YeuCauSuaDiem>> GetAllPagedAsync(int page, int pageSize, string? trangThai);
    Task<YeuCauSuaDiem?> GetDetailAsync(int id);
}

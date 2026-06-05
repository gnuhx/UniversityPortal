using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface ISinhVienRepository : IRepository<SinhVien>
{
    Task<SinhVien?> GetByMssvAsync(string mssv);
    Task<SinhVien?> GetByTaiKhoanIdAsync(int taiKhoanId);
    Task<PagedResultDto<SinhVien>> GetPagedByLopAsync(int lopId, int page, int pageSize);
}

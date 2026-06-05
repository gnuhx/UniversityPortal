using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IGiaoVienRepository : IRepository<GiaoVien>
{
    Task<GiaoVien?> GetByTaiKhoanIdAsync(int taiKhoanId);
    Task<GiaoVien?> GetByMaGvAsync(string maGv);
}

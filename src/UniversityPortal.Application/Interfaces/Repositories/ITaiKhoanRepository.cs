using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface ITaiKhoanRepository : IRepository<TaiKhoan>
{
    Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap);
    Task<TaiKhoan?> GetByRefreshTokenAsync(string refreshToken);
}

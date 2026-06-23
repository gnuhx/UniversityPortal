using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IDanhSachLopHPRepository : IRepository<DanhSachLopHP>
{
    Task<IEnumerable<DanhSachLopHP>> GetBySinhVienAsync(int sinhVienId);
    Task<IEnumerable<DanhSachLopHP>> GetByLopHocPhanAsync(int lopHpId);
    Task<DanhSachLopHP?> GetBySinhVienAndLopAsync(int sinhVienId, int lopHpId);
    Task<DanhSachLopHP?> GetByIdWithLopHocPhanAsync(int id);
}

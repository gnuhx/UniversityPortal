using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IHocPhiRepository : IRepository<HocPhi>
{
    Task<IEnumerable<HocPhi>> GetBySinhVienAsync(int sinhVienId);
    Task<IEnumerable<HocPhi>> GetByHocKyAsync(int hocKyId);
    Task<HocPhi?> GetBySinhVienAndHocKyAsync(int sinhVienId, int hocKyId);
    Task<IEnumerable<HocPhi>> GetAllWithDetailsAsync();

    /// <summary>Kiểm tra còn khoản học phí nào gắn với học kỳ này không (dùng khi xoá học kỳ).</summary>
    Task<bool> ExistsByHocKyAsync(int hocKyId);
}

using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IHocPhiRepository : IRepository<HocPhi>
{
    Task<IEnumerable<HocPhi>> GetBySinhVienAsync(int sinhVienId);
    Task<IEnumerable<HocPhi>> GetByHocKyAsync(int hocKyId);
    Task<HocPhi?> GetBySinhVienAndHocKyAsync(int sinhVienId, int hocKyId);
    Task<IEnumerable<HocPhi>> GetAllWithDetailsAsync();

    /// <summary>Đếm số khoản học phí đang gắn với học kỳ này (dùng khi xoá học kỳ).</summary>
    Task<int> CountByHocKyAsync(int hocKyId);
}

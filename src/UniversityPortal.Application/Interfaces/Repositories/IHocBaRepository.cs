using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

/// <summary>
/// Repository học bạ — dùng để tra cứu chương trình đào tạo gần nhất của sinh viên
/// khi sinh viên không còn thuộc lớp sinh hoạt nào (VD: đã tốt nghiệp).
/// </summary>
public interface IHocBaRepository : IRepository<HocBa>
{
    /// <summary>Lấy bản ghi học bạ mới nhất của sinh viên (theo Id giảm dần).</summary>
    Task<HocBa?> GetLatestBySinhVienAsync(int sinhVienId);
}

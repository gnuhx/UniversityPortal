using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IHocKyRepository : IRepository<HocKy>
{
    Task<IEnumerable<HocKy>> GetAllAsync();
}

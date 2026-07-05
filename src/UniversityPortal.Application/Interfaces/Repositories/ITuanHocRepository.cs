using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface ITuanHocRepository : IRepository<TuanHoc>
{
    Task<IEnumerable<TuanHoc>> GetAllAsync();
}

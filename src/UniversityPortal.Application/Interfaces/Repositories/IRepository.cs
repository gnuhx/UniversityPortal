using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Domain.Entities.Common;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IRepository<T> where T : AuditableEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PagedResultDto<T>> GetPagedAsync(int page, int pageSize);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}

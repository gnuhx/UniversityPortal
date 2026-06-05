using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities.Common;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class BaseRepository<T>(AppDbContext context) : IRepository<T> where T : AuditableEntity
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<PagedResultDto<T>> GetPagedAsync(int page, int pageSize)
    {
        var total = await DbSet.CountAsync();
        var data = await DbSet.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResultDto<T> { Data = data, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

    public void Update(T entity) => DbSet.Update(entity);

    public void Delete(T entity) => DbSet.Remove(entity);
}

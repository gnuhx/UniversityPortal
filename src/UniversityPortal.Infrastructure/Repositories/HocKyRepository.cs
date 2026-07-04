using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class HocKyRepository(AppDbContext context) : BaseRepository<HocKy>(context), IHocKyRepository
{
    public async Task<IEnumerable<HocKy>> GetAllAsync()
        => await DbSet
            .Include(x => x.NamHoc)
            .OrderByDescending(x => x.NgayBatDau)
            .ToListAsync();
}

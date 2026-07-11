using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class HocBaRepository(AppDbContext context) : BaseRepository<HocBa>(context), IHocBaRepository
{
    public async Task<HocBa?> GetLatestBySinhVienAsync(int sinhVienId)
        => await DbSet
            .Where(x => x.SinhVienId == sinhVienId)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
}

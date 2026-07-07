using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class NoiDungTinhRepository(AppDbContext context) : BaseRepository<NoiDungTinh>(context), INoiDungTinhRepository
{
    public async Task<IEnumerable<NoiDungTinh>> GetByKhuVucAsync(string khuVuc)
        => await DbSet
            .Where(x => x.KhuVuc == khuVuc)
            .OrderBy(x => x.ThuTu)
            .ToListAsync();
}

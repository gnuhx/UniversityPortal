using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class ThongBaoRepository(AppDbContext context) : BaseRepository<ThongBao>(context), IThongBaoRepository
{
    public async Task<IEnumerable<ThongBao>> GetBySinhVienAsync(int lopId)
        => await DbSet
            .Where(x => x.LopNhanId == null || x.LopNhanId == lopId)
            .Include(x => x.NguoiTao)
            .Include(x => x.LopNhan)
            .Include(x => x.ThongBaoDaDocs)
            .OrderByDescending(x => x.NgayTao)
            .ToListAsync();

    public async Task<IEnumerable<ThongBao>> GetAllWithDetailsAsync()
        => await DbSet
            .Include(x => x.NguoiTao)
            .Include(x => x.LopNhan)
            .OrderByDescending(x => x.NgayTao)
            .ToListAsync();

    public async Task<ThongBao?> GetDetailAsync(int id)
        => await DbSet
            .Where(x => x.Id == id)
            .Include(x => x.NguoiTao)
            .Include(x => x.LopNhan)
            .Include(x => x.ThongBaoDaDocs)
            .FirstOrDefaultAsync();
}

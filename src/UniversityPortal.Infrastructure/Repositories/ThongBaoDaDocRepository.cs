using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class ThongBaoDaDocRepository(AppDbContext context) : BaseRepository<ThongBaoDaDoc>(context), IThongBaoDaDocRepository
{
    public async Task<ThongBaoDaDoc?> GetAsync(int thongBaoId, int taiKhoanId)
        => await DbSet.FirstOrDefaultAsync(x => x.ThongBaoId == thongBaoId && x.TaiKhoanId == taiKhoanId);
}

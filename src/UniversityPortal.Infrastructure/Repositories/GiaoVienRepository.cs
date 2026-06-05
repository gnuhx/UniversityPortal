using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class GiaoVienRepository(AppDbContext context) : BaseRepository<GiaoVien>(context), IGiaoVienRepository
{
    public async Task<GiaoVien?> GetByTaiKhoanIdAsync(int taiKhoanId)
        => await DbSet.Include(x => x.TaiKhoan).FirstOrDefaultAsync(x => x.TaiKhoanId == taiKhoanId);

    public async Task<GiaoVien?> GetByMaGvAsync(string maGv)
        => await DbSet.FirstOrDefaultAsync(x => x.MaGv == maGv);
}

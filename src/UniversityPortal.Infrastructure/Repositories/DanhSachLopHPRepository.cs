using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class DanhSachLopHPRepository(AppDbContext context) : BaseRepository<DanhSachLopHP>(context), IDanhSachLopHPRepository
{
    public async Task<IEnumerable<DanhSachLopHP>> GetBySinhVienAsync(int sinhVienId)
        => await DbSet.Where(x => x.SinhVienId == sinhVienId).Include(x => x.LopHocPhan).ToListAsync();

    public async Task<IEnumerable<DanhSachLopHP>> GetByLopHocPhanAsync(int lopHpId)
        => await DbSet.Where(x => x.LopHpId == lopHpId).Include(x => x.SinhVien).ToListAsync();

    public async Task<DanhSachLopHP?> GetBySinhVienAndLopAsync(int sinhVienId, int lopHpId)
        => await DbSet.FirstOrDefaultAsync(x => x.SinhVienId == sinhVienId && x.LopHpId == lopHpId);
}

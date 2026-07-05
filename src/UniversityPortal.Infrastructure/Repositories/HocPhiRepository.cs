using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class HocPhiRepository(AppDbContext context) : BaseRepository<HocPhi>(context), IHocPhiRepository
{
    public async Task<IEnumerable<HocPhi>> GetBySinhVienAsync(int sinhVienId)
        => await DbSet
            .Where(x => x.SinhVienId == sinhVienId)
            .Include(x => x.HocKy)
            .Include(x => x.SinhVien).ThenInclude(sv => sv.TaiKhoan)
            .OrderByDescending(x => x.HocKy.NgayBatDau)
            .ToListAsync();

    public async Task<IEnumerable<HocPhi>> GetByHocKyAsync(int hocKyId)
        => await DbSet
            .Where(x => x.HocKyId == hocKyId)
            .Include(x => x.SinhVien).ThenInclude(sv => sv.TaiKhoan)
            .Include(x => x.HocKy)
            .ToListAsync();

    public async Task<HocPhi?> GetBySinhVienAndHocKyAsync(int sinhVienId, int hocKyId)
        => await DbSet.FirstOrDefaultAsync(x => x.SinhVienId == sinhVienId && x.HocKyId == hocKyId);

    public async Task<IEnumerable<HocPhi>> GetAllWithDetailsAsync()
        => await DbSet
            .Include(x => x.SinhVien).ThenInclude(sv => sv.TaiKhoan)
            .Include(x => x.HocKy)
            .OrderByDescending(x => x.HocKy.NgayBatDau)
            .ThenBy(x => x.SinhVien.Mssv)
            .ToListAsync();

    public async Task<int> CountByHocKyAsync(int hocKyId)
        => await DbSet.CountAsync(x => x.HocKyId == hocKyId);
}

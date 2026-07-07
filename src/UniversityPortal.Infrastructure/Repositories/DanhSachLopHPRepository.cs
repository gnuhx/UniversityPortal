using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

public class DanhSachLopHPRepository(AppDbContext context) : BaseRepository<DanhSachLopHP>(context), IDanhSachLopHPRepository
{
    public async Task<IEnumerable<DanhSachLopHP>> GetBySinhVienAsync(int sinhVienId)
        => await DbSet
            .Where(x => x.SinhVienId == sinhVienId)
            .Include(x => x.LopHocPhan)
                .ThenInclude(lhp => lhp.ChiTietCTDT)
                    .ThenInclude(ct => ct.MonHoc)
            .Include(x => x.LopHocPhan)
                .ThenInclude(lhp => lhp.HocKy)
                    .ThenInclude(hk => hk.NamHoc)
            .Include(x => x.LopHocPhan)
                .ThenInclude(lhp => lhp.GiaoVien)
                    .ThenInclude(gv => gv.TaiKhoan)
            .ToListAsync();

    public async Task<IEnumerable<DanhSachLopHP>> GetByLopHocPhanAsync(int lopHpId)
        => await DbSet
            .Where(x => x.LopHpId == lopHpId)
            .Include(x => x.SinhVien).ThenInclude(sv => sv.TaiKhoan)
            .ToListAsync();

    public async Task<DanhSachLopHP?> GetBySinhVienAndLopAsync(int sinhVienId, int lopHpId)
        => await DbSet.FirstOrDefaultAsync(x => x.SinhVienId == sinhVienId && x.LopHpId == lopHpId);

    public async Task<IEnumerable<DanhSachLopHP>> GetByHocKyWithDetailsAsync(int hocKyId)
        => await DbSet
            .Where(x => x.LopHocPhan.HocKyId == hocKyId)
            .Include(x => x.SinhVien).ThenInclude(sv => sv.TaiKhoan)
            .Include(x => x.LopHocPhan)
                .ThenInclude(lhp => lhp.ChiTietCTDT)
            .ToListAsync();

    public async Task<DanhSachLopHP?> GetByIdWithLopHocPhanAsync(int id)
        => await DbSet
            .Where(x => x.Id == id)
            .Include(x => x.LopHocPhan)
                .ThenInclude(lhp => lhp.ChiTietCTDT)
                    .ThenInclude(ct => ct.MonHoc)
            .Include(x => x.LopHocPhan)
                .ThenInclude(lhp => lhp.HocKy)
            .Include(x => x.LopHocPhan)
                .ThenInclude(lhp => lhp.GiaoVien)
                    .ThenInclude(gv => gv.TaiKhoan)
            .FirstOrDefaultAsync();
}

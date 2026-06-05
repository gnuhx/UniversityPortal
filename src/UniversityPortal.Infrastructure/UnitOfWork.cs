using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Infrastructure.Persistence;
using UniversityPortal.Infrastructure.Repositories;

namespace UniversityPortal.Infrastructure;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private ITaiKhoanRepository? _taiKhoans;
    private ISinhVienRepository? _sinhViens;
    private IGiaoVienRepository? _giaoViens;
    private ILopHocPhanRepository? _lopHocPhans;
    private IDanhSachLopHPRepository? _danhSachLopHPs;

    public ITaiKhoanRepository TaiKhoans => _taiKhoans ??= new TaiKhoanRepository(context);
    public ISinhVienRepository SinhViens => _sinhViens ??= new SinhVienRepository(context);
    public IGiaoVienRepository GiaoViens => _giaoViens ??= new GiaoVienRepository(context);
    public ILopHocPhanRepository LopHocPhans => _lopHocPhans ??= new LopHocPhanRepository(context);
    public IDanhSachLopHPRepository DanhSachLopHPs => _danhSachLopHPs ??= new DanhSachLopHPRepository(context);

    public Task<int> CommitAsync() => context.SaveChangesAsync();

    public void Dispose() => context.Dispose();
}

using UniversityPortal.Application.Interfaces.Repositories;

namespace UniversityPortal.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITaiKhoanRepository TaiKhoans { get; }
    ISinhVienRepository SinhViens { get; }
    IGiaoVienRepository GiaoViens { get; }
    ILopHocPhanRepository LopHocPhans { get; }
    IDanhSachLopHPRepository DanhSachLopHPs { get; }

    Task<int> CommitAsync();
}

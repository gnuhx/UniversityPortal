using AutoMapper;
using UniversityPortal.Application.DTOs.DanhSachLopHP;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

public class DanhSachLopHPService(IUnitOfWork uow, IMapper mapper) : IDanhSachLopHPService
{
    public async Task<IEnumerable<DanhSachLopHPDto>> GetBySinhVienMeAsync(int taiKhoanId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        var list = await uow.DanhSachLopHPs.GetBySinhVienAsync(sv.Id);
        return mapper.Map<IEnumerable<DanhSachLopHPDto>>(list);
    }

    public async Task<IEnumerable<DanhSachLopHPDto>> GetByLopHocPhanAsync(int lopHpId)
    {
        var list = await uow.DanhSachLopHPs.GetByLopHocPhanAsync(lopHpId);
        return mapper.Map<IEnumerable<DanhSachLopHPDto>>(list);
    }
}

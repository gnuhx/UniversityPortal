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

    public async Task<DanhSachLopHPDto> NhapDiemAsync(int danhSachLopHpId, int taiKhoanId, NhapDiemDto dto)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        var ds = await uow.DanhSachLopHPs.GetByIdWithLopHocPhanAsync(danhSachLopHpId)
            ?? throw new NotFoundException($"Không tìm thấy bản ghi ID = {danhSachLopHpId}.");

        if (ds.LopHocPhan.KhoaBangDiem)
            throw new BadRequestException("Bảng điểm đã bị khoá, không thể nhập điểm.");

        if (ds.LopHocPhan.GiaoVienId != gv.Id)
            throw new ForbiddenException("Bạn không có quyền nhập điểm cho lớp này.");

        if (dto.DiemQt1.HasValue) ds.DiemQt1 = dto.DiemQt1;
        if (dto.DiemQt2.HasValue) ds.DiemQt2 = dto.DiemQt2;
        if (dto.DiemThi.HasValue)  ds.DiemThi  = dto.DiemThi;

        if (ds.DiemQt1.HasValue && ds.DiemQt2.HasValue && ds.DiemThi.HasValue)
            ds.DiemTongKet = (float)Math.Round(ds.DiemQt1.Value * 0.15 + ds.DiemQt2.Value * 0.15 + ds.DiemThi.Value * 0.70, 2);

        await uow.CommitAsync();
        return mapper.Map<DanhSachLopHPDto>(ds);
    }
}

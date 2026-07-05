using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.LopHocPhan;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

public class LopHocPhanService(IUnitOfWork uow, IMapper mapper) : ILopHocPhanService
{
    public async Task<IEnumerable<LopHocPhanDto>> GetByGiaoVienMeAsync(int taiKhoanId)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        var list = await uow.LopHocPhans.GetByGiaoVienWithDetailsAsync(gv.Id);
        return mapper.Map<IEnumerable<LopHocPhanDto>>(list);
    }

    public async Task<PagedResultDto<LopHocPhanDto>> GetPagedAsync(int page, int pageSize, int? hocKyId, string? keyword)
    {
        var paged = await uow.LopHocPhans.GetPagedFilterAsync(page, pageSize, hocKyId, keyword);
        return new PagedResultDto<LopHocPhanDto>
        {
            Data     = mapper.Map<IEnumerable<LopHocPhanDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task KhoaBangDiemAsync(int lopHpId, int taiKhoanId)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        var lhp = await uow.LopHocPhans.GetByIdAsync(lopHpId)
            ?? throw new NotFoundException($"Không tìm thấy lớp học phần ID = {lopHpId}.");

        if (lhp.GiaoVienId != gv.Id)
            throw new ForbiddenException("Bạn không có quyền khoá bảng điểm của lớp này.");

        if (lhp.KhoaBangDiem)
            throw new BadRequestException("Bảng điểm đã được khoá.");

        lhp.KhoaBangDiem = true;
        await uow.CommitAsync();
    }

    public async Task MoBangDiemAsync(int lopHpId)
    {
        var lhp = await uow.LopHocPhans.GetByIdAsync(lopHpId)
            ?? throw new NotFoundException($"Không tìm thấy lớp học phần ID = {lopHpId}.");

        lhp.KhoaBangDiem = false;
        await uow.CommitAsync();
    }
}

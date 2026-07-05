// Dịch vụ quản lý thời khoá biểu — CRUD buổi học + tra lịch theo sinh viên/giáo viên
using AutoMapper;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.DTOs.ThoiKhoaBieu;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;
using UniversityPortal.Domain.Entities;
using UniversityPortal.Domain.Exceptions;

namespace UniversityPortal.Application.Services;

public class ThoiKhoaBieuService(IUnitOfWork uow, IMapper mapper) : IThoiKhoaBieuService
{
    public async Task<PagedResultDto<ThoiKhoaBieuDto>> GetPagedAsync(int page, int pageSize, int? lopHpId)
    {
        var paged = await uow.ThoiKhoaBieus.GetPagedFilterAsync(page, pageSize, lopHpId);
        return new PagedResultDto<ThoiKhoaBieuDto>
        {
            Data     = mapper.Map<IEnumerable<ThoiKhoaBieuDto>>(paged.Data),
            Total    = paged.Total,
            Page     = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<ThoiKhoaBieuDto> GetByIdAsync(int id)
    {
        var tkb = await uow.ThoiKhoaBieus.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy buổi học id = {id}.");
        return mapper.Map<ThoiKhoaBieuDto>(tkb);
    }

    public async Task<ThoiKhoaBieuDto> CreateAsync(CreateThoiKhoaBieuDto dto)
    {
        await ValidateAsync(dto.LopHpId, dto.TuanHocId, dto.Thu, dto.TietBatDau, dto.TietKetThuc, dto.PhongHoc, excludeId: null);

        var tkb = new ThoiKhoaBieu
        {
            LopHpId      = dto.LopHpId,
            TuanHocId    = dto.TuanHocId,
            Thu          = dto.Thu,
            TietBatDau   = dto.TietBatDau,
            TietKetThuc  = dto.TietKetThuc,
            PhongHoc     = dto.PhongHoc
        };

        await uow.ThoiKhoaBieus.AddAsync(tkb);
        await uow.CommitAsync();

        return mapper.Map<ThoiKhoaBieuDto>(await uow.ThoiKhoaBieus.GetDetailAsync(tkb.Id));
    }

    public async Task<ThoiKhoaBieuDto> UpdateAsync(int id, UpdateThoiKhoaBieuDto dto)
    {
        var tkb = await uow.ThoiKhoaBieus.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy buổi học id = {id}.");

        await ValidateAsync(tkb.LopHpId, dto.TuanHocId, dto.Thu, dto.TietBatDau, dto.TietKetThuc, dto.PhongHoc, excludeId: id);

        tkb.TuanHocId   = dto.TuanHocId;
        tkb.Thu         = dto.Thu;
        tkb.TietBatDau  = dto.TietBatDau;
        tkb.TietKetThuc = dto.TietKetThuc;
        tkb.PhongHoc    = dto.PhongHoc;

        uow.ThoiKhoaBieus.Update(tkb);
        await uow.CommitAsync();

        return mapper.Map<ThoiKhoaBieuDto>(await uow.ThoiKhoaBieus.GetDetailAsync(id));
    }

    public async Task DeleteAsync(int id)
    {
        var tkb = await uow.ThoiKhoaBieus.GetDetailAsync(id)
            ?? throw new NotFoundException($"Không tìm thấy buổi học id = {id}.");

        uow.ThoiKhoaBieus.Delete(tkb);
        await uow.CommitAsync();
    }

    public async Task<IEnumerable<ThoiKhoaBieuDto>> GetForSinhVienMeAsync(int taiKhoanId, int? hocKyId)
    {
        var sv = await uow.SinhViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ sinh viên.");

        var list = await uow.ThoiKhoaBieus.GetForSinhVienAsync(sv.Id, hocKyId);
        return mapper.Map<IEnumerable<ThoiKhoaBieuDto>>(list);
    }

    public async Task<IEnumerable<ThoiKhoaBieuDto>> GetForGiaoVienMeAsync(int taiKhoanId, int? hocKyId)
    {
        var gv = await uow.GiaoViens.GetByTaiKhoanIdAsync(taiKhoanId)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ giáo viên.");

        var list = await uow.ThoiKhoaBieus.GetForGiaoVienAsync(gv.Id, hocKyId);
        return mapper.Map<IEnumerable<ThoiKhoaBieuDto>>(list);
    }

    /// <summary>Kiểm tra lớp HP / tuần học tồn tại và buổi học không trùng phòng/giờ với buổi khác.</summary>
    private async Task ValidateAsync(int lopHpId, int tuanHocId, int thu, int tietBatDau, int tietKetThuc, string phongHoc, int? excludeId)
    {
        if (await uow.LopHocPhans.GetByIdAsync(lopHpId) is null)
            throw new NotFoundException($"Không tìm thấy lớp học phần id = {lopHpId}.");

        if (await uow.TuanHocs.GetByIdAsync(tuanHocId) is null)
            throw new NotFoundException($"Không tìm thấy tuần học id = {tuanHocId}.");

        if (thu is < 2 or > 8)
            throw new BadRequestException("Thứ trong tuần không hợp lệ (2 = Thứ Hai ... 8 = Chủ nhật).");

        if (tietBatDau > tietKetThuc)
            throw new BadRequestException("Tiết bắt đầu phải nhỏ hơn hoặc bằng tiết kết thúc.");

        if (await uow.ThoiKhoaBieus.ExistsConflictAsync(tuanHocId, thu, phongHoc, tietBatDau, tietKetThuc, excludeId))
            throw new BadRequestException("Phòng học đã được sử dụng cho khung giờ này.");
    }
}

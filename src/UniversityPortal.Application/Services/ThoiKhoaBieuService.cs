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

    /// <summary>
    /// Tạo hàng loạt buổi học lặp lại hàng tuần cho một lớp HP, từ tuần bắt đầu đến tuần kết thúc
    /// (cùng thứ/tiết/phòng mỗi tuần). Tuần nào bị trùng phòng/giờ với buổi học khác sẽ được bỏ qua
    /// thay vì làm hỏng toàn bộ thao tác — giúp giáo vụ không phải thêm từng tuần thủ công.
    /// </summary>
    public async Task<GenerateThoiKhoaBieuResultDto> GenerateAsync(GenerateThoiKhoaBieuDto dto)
    {
        if (await uow.LopHocPhans.GetByIdAsync(dto.LopHpId) is null)
            throw new NotFoundException($"Không tìm thấy lớp học phần id = {dto.LopHpId}.");

        var tuanBatDau = await uow.TuanHocs.GetByIdAsync(dto.TuanBatDauId)
            ?? throw new NotFoundException($"Không tìm thấy tuần học id = {dto.TuanBatDauId}.");
        var tuanKetThuc = await uow.TuanHocs.GetByIdAsync(dto.TuanKetThucId)
            ?? throw new NotFoundException($"Không tìm thấy tuần học id = {dto.TuanKetThucId}.");

        if (tuanBatDau.NamHocId != tuanKetThuc.NamHocId)
            throw new BadRequestException("Tuần bắt đầu và tuần kết thúc phải cùng một năm học.");

        if (tuanBatDau.SoThuTuTuan > tuanKetThuc.SoThuTuTuan)
            throw new BadRequestException("Tuần bắt đầu phải trước hoặc bằng tuần kết thúc.");

        if (dto.Thu is < 2 or > 8)
            throw new BadRequestException("Thứ trong tuần không hợp lệ (2 = Thứ Hai ... 8 = Chủ nhật).");

        if (dto.TietBatDau > dto.TietKetThuc)
            throw new BadRequestException("Tiết bắt đầu phải nhỏ hơn hoặc bằng tiết kết thúc.");

        var tuans = (await uow.TuanHocs.GetAllAsync())
            .Where(t => t.NamHocId == tuanBatDau.NamHocId
                     && t.SoThuTuTuan >= tuanBatDau.SoThuTuTuan
                     && t.SoThuTuTuan <= tuanKetThuc.SoThuTuTuan)
            .OrderBy(t => t.SoThuTuTuan);

        var result = new GenerateThoiKhoaBieuResultDto();

        foreach (var tuan in tuans)
        {
            if (await uow.ThoiKhoaBieus.ExistsConflictAsync(tuan.Id, dto.Thu, dto.PhongHoc, dto.TietBatDau, dto.TietKetThuc, excludeId: null))
            {
                result.TuanBiBoQua.Add(tuan.MaTuan);
                continue;
            }

            await uow.ThoiKhoaBieus.AddAsync(new ThoiKhoaBieu
            {
                LopHpId     = dto.LopHpId,
                TuanHocId   = tuan.Id,
                Thu         = dto.Thu,
                TietBatDau  = dto.TietBatDau,
                TietKetThuc = dto.TietKetThuc,
                PhongHoc    = dto.PhongHoc
            });
            result.SoBuoiDaTao++;
        }

        if (result.SoBuoiDaTao > 0)
            await uow.CommitAsync();

        return result;
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

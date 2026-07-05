// Dịch vụ tra cứu phòng ban / khoa — chỉ đọc
using AutoMapper;
using UniversityPortal.Application.DTOs.PhongBan;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.Application.Services;

/// <summary>
/// Cài đặt dịch vụ tra cứu phòng ban / khoa.
/// </summary>
public class PhongBanService(IUnitOfWork uow, IMapper mapper) : IPhongBanService
{
    /// <summary>Lấy tất cả phòng ban / khoa (dùng cho dropdown).</summary>
    public async Task<IEnumerable<PhongBanDto>> GetAllAsync()
        => mapper.Map<IEnumerable<PhongBanDto>>(await uow.PhongBans.GetAllAsync());
}

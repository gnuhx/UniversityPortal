// Dịch vụ tra cứu tuần học — chỉ đọc
using AutoMapper;
using UniversityPortal.Application.DTOs.TuanHoc;
using UniversityPortal.Application.Interfaces;
using UniversityPortal.Application.Interfaces.Services;

namespace UniversityPortal.Application.Services;

public class TuanHocService(IUnitOfWork uow, IMapper mapper) : ITuanHocService
{
    public async Task<IEnumerable<TuanHocDto>> GetAllAsync()
        => mapper.Map<IEnumerable<TuanHocDto>>(await uow.TuanHocs.GetAllAsync());
}

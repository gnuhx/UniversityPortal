using AutoMapper;
using UniversityPortal.Application.DTOs.Auth;
using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaiKhoan, UserInfoDto>()
            .ForMember(d => d.VaiTro, o => o.MapFrom(s => s.VaiTro.TenVaiTro));
    }
}

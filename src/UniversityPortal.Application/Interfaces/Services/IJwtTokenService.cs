using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(TaiKhoan taiKhoan);
    string GenerateRefreshToken();
}

using FluentValidation;
using UniversityPortal.Application.DTOs.Auth;

namespace UniversityPortal.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.TenDangNhap).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MatKhau).NotEmpty().MinimumLength(6);
    }
}

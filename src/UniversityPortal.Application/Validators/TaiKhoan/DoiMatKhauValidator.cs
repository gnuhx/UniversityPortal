// Validator đổi mật khẩu — kiểm tra mật khẩu mới khớp xác nhận và đủ mạnh
using FluentValidation;
using UniversityPortal.Application.DTOs.TaiKhoan;

namespace UniversityPortal.Application.Validators.TaiKhoan;

/// <summary>
/// Xác thực dữ liệu đầu vào khi người dùng tự đổi mật khẩu.
/// </summary>
public class DoiMatKhauValidator : AbstractValidator<DoiMatKhauDto>
{
    public DoiMatKhauValidator()
    {
        RuleFor(x => x.MatKhauCu)
            .NotEmpty().WithMessage("Mật khẩu cũ không được để trống.");

        RuleFor(x => x.MatKhauMoi)
            .NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự.")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu mới phải có ít nhất 1 chữ hoa.")
            .Matches(@"[a-z]").WithMessage("Mật khẩu mới phải có ít nhất 1 chữ thường.")
            .Matches(@"\d").WithMessage("Mật khẩu mới phải có ít nhất 1 chữ số.");

        RuleFor(x => x.XacNhanMatKhau)
            .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống.")
            .Equal(x => x.MatKhauMoi).WithMessage("Xác nhận mật khẩu không khớp với mật khẩu mới.");
    }
}

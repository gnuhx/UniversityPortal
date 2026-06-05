// Validator tạo tài khoản — kiểm tra định dạng tên đăng nhập, email và độ mạnh mật khẩu
using FluentValidation;
using UniversityPortal.Application.DTOs.TaiKhoan;

namespace UniversityPortal.Application.Validators.TaiKhoan;

/// <summary>
/// Xác thực dữ liệu đầu vào khi tạo mới tài khoản.
/// Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số.
/// </summary>
public class CreateTaiKhoanValidator : AbstractValidator<CreateTaiKhoanDto>
{
    public CreateTaiKhoanValidator()
    {
        RuleFor(x => x.TenDangNhap)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống.")
            .MinimumLength(4).WithMessage("Tên đăng nhập phải có ít nhất 4 ký tự.")
            .MaximumLength(50).WithMessage("Tên đăng nhập không quá 50 ký tự.")
            .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("Tên đăng nhập chỉ gồm chữ, số, dấu chấm và gạch dưới.");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ hoa.")
            .Matches(@"[a-z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ thường.")
            .Matches(@"\d").WithMessage("Mật khẩu phải có ít nhất 1 chữ số.");

        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống.")
            .MaximumLength(100).WithMessage("Họ tên không quá 100 ký tự.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không đúng định dạng.");

        RuleFor(x => x.VaiTroId)
            .GreaterThan(0).WithMessage("Vai trò không hợp lệ.");
    }
}

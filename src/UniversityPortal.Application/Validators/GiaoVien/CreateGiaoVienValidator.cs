// Validator tạo giáo viên — kiểm tra tên đăng nhập, mã GV và email
using FluentValidation;
using UniversityPortal.Application.DTOs.GiaoVien;

namespace UniversityPortal.Application.Validators.GiaoVien;

/// <summary>
/// Xác thực dữ liệu đầu vào khi tạo mới giáo viên.
/// </summary>
public class CreateGiaoVienValidator : AbstractValidator<CreateGiaoVienDto>
{
    public CreateGiaoVienValidator()
    {
        RuleFor(x => x.TenDangNhap)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống.")
            .MinimumLength(4).WithMessage("Tên đăng nhập phải có ít nhất 4 ký tự.")
            .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("Tên đăng nhập chỉ gồm chữ, số, dấu chấm và gạch dưới.");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.");

        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống.")
            .MaximumLength(100).WithMessage("Họ tên không quá 100 ký tự.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không đúng định dạng.");

        RuleFor(x => x.MaGv)
            .NotEmpty().WithMessage("Mã giáo viên không được để trống.")
            .MaximumLength(20).WithMessage("Mã giáo viên không quá 20 ký tự.");
    }
}

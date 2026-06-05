// Validator cập nhật tài khoản
using FluentValidation;
using UniversityPortal.Application.DTOs.TaiKhoan;

namespace UniversityPortal.Application.Validators.TaiKhoan;

/// <summary>
/// Xác thực dữ liệu đầu vào khi cập nhật thông tin tài khoản.
/// </summary>
public class UpdateTaiKhoanValidator : AbstractValidator<UpdateTaiKhoanDto>
{
    public UpdateTaiKhoanValidator()
    {
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

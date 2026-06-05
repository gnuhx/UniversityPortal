// Validator cập nhật giáo viên
using FluentValidation;
using UniversityPortal.Application.DTOs.GiaoVien;

namespace UniversityPortal.Application.Validators.GiaoVien;

/// <summary>
/// Xác thực dữ liệu đầu vào khi cập nhật thông tin giáo viên.
/// </summary>
public class UpdateGiaoVienValidator : AbstractValidator<UpdateGiaoVienDto>
{
    public UpdateGiaoVienValidator()
    {
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

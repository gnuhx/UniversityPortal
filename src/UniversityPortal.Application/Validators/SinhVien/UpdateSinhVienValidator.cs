// Validator cập nhật sinh viên
using FluentValidation;
using UniversityPortal.Application.DTOs.SinhVien;

namespace UniversityPortal.Application.Validators.SinhVien;

/// <summary>
/// Xác thực dữ liệu đầu vào khi cập nhật thông tin sinh viên.
/// </summary>
public class UpdateSinhVienValidator : AbstractValidator<UpdateSinhVienDto>
{
    public UpdateSinhVienValidator()
    {
        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống.")
            .MaximumLength(100).WithMessage("Họ tên không quá 100 ký tự.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không đúng định dạng.");
    }
}

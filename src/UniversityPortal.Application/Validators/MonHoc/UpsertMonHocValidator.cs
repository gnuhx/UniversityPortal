// Validator tạo/cập nhật môn học
using FluentValidation;
using UniversityPortal.Application.DTOs.MonHoc;

namespace UniversityPortal.Application.Validators.MonHoc;

/// <summary>
/// Xác thực dữ liệu đầu vào khi tạo mới hoặc cập nhật môn học.
/// </summary>
public class UpsertMonHocValidator : AbstractValidator<UpsertMonHocDto>
{
    public UpsertMonHocValidator()
    {
        RuleFor(x => x.MaMon)
            .NotEmpty().WithMessage("Mã môn không được để trống.")
            .MaximumLength(20).WithMessage("Mã môn không quá 20 ký tự.");

        RuleFor(x => x.TenMon)
            .NotEmpty().WithMessage("Tên môn không được để trống.")
            .MaximumLength(200).WithMessage("Tên môn không quá 200 ký tự.");
    }
}

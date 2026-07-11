// Validator tạo/cập nhật học kỳ
using FluentValidation;
using UniversityPortal.Application.DTOs.HocKy;

namespace UniversityPortal.Application.Validators.HocKy;

/// <summary>
/// Xác thực dữ liệu đầu vào khi tạo mới hoặc cập nhật học kỳ.
/// </summary>
public class UpsertHocKyValidator : AbstractValidator<UpsertHocKyDto>
{
    public UpsertHocKyValidator()
    {
        RuleFor(x => x.TenHocKy)
            .NotEmpty().WithMessage("Tên học kỳ không được để trống.")
            .MaximumLength(100).WithMessage("Tên học kỳ không quá 100 ký tự.");

        RuleFor(x => x.NamHocId)
            .GreaterThan(0).WithMessage("Năm học không hợp lệ.");

        RuleFor(x => x.NgayBatDau)
            .NotEqual(default(DateOnly)).WithMessage("Ngày bắt đầu không hợp lệ.");
    }
}

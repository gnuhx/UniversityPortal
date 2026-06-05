// Validator tạo/cập nhật ngành học
using FluentValidation;
using UniversityPortal.Application.DTOs.NganhHoc;

namespace UniversityPortal.Application.Validators.NganhHoc;

/// <summary>
/// Xác thực dữ liệu đầu vào khi tạo mới hoặc cập nhật ngành học.
/// </summary>
public class UpsertNganhHocValidator : AbstractValidator<UpsertNganhHocDto>
{
    public UpsertNganhHocValidator()
    {
        RuleFor(x => x.MaNganh)
            .NotEmpty().WithMessage("Mã ngành không được để trống.")
            .MaximumLength(20).WithMessage("Mã ngành không quá 20 ký tự.")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Mã ngành chỉ gồm chữ hoa và số.");

        RuleFor(x => x.TenNganh)
            .NotEmpty().WithMessage("Tên ngành không được để trống.")
            .MaximumLength(200).WithMessage("Tên ngành không quá 200 ký tự.");
    }
}

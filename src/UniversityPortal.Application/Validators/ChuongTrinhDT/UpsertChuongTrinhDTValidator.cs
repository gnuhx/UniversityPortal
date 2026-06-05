// Validator tạo/cập nhật chương trình đào tạo
using FluentValidation;
using UniversityPortal.Application.DTOs.ChuongTrinhDT;

namespace UniversityPortal.Application.Validators.ChuongTrinhDT;

/// <summary>
/// Xác thực dữ liệu đầu vào khi tạo mới hoặc cập nhật chương trình đào tạo.
/// </summary>
public class UpsertChuongTrinhDTValidator : AbstractValidator<UpsertChuongTrinhDTDto>
{
    public UpsertChuongTrinhDTValidator()
    {
        RuleFor(x => x.MaCtdt)
            .NotEmpty().WithMessage("Mã CTDT không được để trống.")
            .MaximumLength(20).WithMessage("Mã CTDT không quá 20 ký tự.");

        RuleFor(x => x.NganhId)
            .GreaterThan(0).WithMessage("Ngành học không hợp lệ.");

        RuleFor(x => x.KhoaHoc)
            .NotEmpty().WithMessage("Khoá học không được để trống.")
            .MaximumLength(20).WithMessage("Khoá học không quá 20 ký tự.");
    }
}

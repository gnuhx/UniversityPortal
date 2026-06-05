// Validator tạo lớp sinh hoạt mới
using FluentValidation;
using UniversityPortal.Application.DTOs.LopSinhHoat;

namespace UniversityPortal.Application.Validators.LopSinhHoat;

/// <summary>
/// Xác thực dữ liệu đầu vào khi tạo mới lớp sinh hoạt.
/// </summary>
public class CreateLopSinhHoatValidator : AbstractValidator<CreateLopSinhHoatDto>
{
    public CreateLopSinhHoatValidator()
    {
        RuleFor(x => x.MaLop)
            .NotEmpty().WithMessage("Mã lớp không được để trống.")
            .MaximumLength(20).WithMessage("Mã lớp không quá 20 ký tự.");

        RuleFor(x => x.GvcnId)
            .GreaterThan(0).WithMessage("Giáo viên chủ nhiệm không hợp lệ.");

        RuleFor(x => x.ChuongTrinhDtId)
            .GreaterThan(0).WithMessage("Chương trình đào tạo không hợp lệ.");
    }
}

// Validator cập nhật lớp sinh hoạt
using FluentValidation;
using UniversityPortal.Application.DTOs.LopSinhHoat;

namespace UniversityPortal.Application.Validators.LopSinhHoat;

/// <summary>
/// Xác thực dữ liệu đầu vào khi cập nhật lớp sinh hoạt.
/// </summary>
public class UpdateLopSinhHoatValidator : AbstractValidator<UpdateLopSinhHoatDto>
{
    public UpdateLopSinhHoatValidator()
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

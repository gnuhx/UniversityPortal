// Validator thêm môn học vào chương trình đào tạo
using FluentValidation;
using UniversityPortal.Application.DTOs.ChiTietCTDT;

namespace UniversityPortal.Application.Validators.ChiTietCTDT;

/// <summary>
/// Xác thực dữ liệu đầu vào khi thêm môn học vào chương trình đào tạo.
/// </summary>
public class CreateChiTietCTDTValidator : AbstractValidator<CreateChiTietCTDTDto>
{
    public CreateChiTietCTDTValidator()
    {
        RuleFor(x => x.CtdtId)
            .GreaterThan(0).WithMessage("Chương trình đào tạo không hợp lệ.");

        RuleFor(x => x.MonHocId)
            .GreaterThan(0).WithMessage("Môn học không hợp lệ.");

        RuleFor(x => x.HocKyId)
            .GreaterThan(0).WithMessage("Học kỳ không hợp lệ.");

        RuleFor(x => x.SoTinChi)
            .InclusiveBetween(1, 10).WithMessage("Số tín chỉ phải từ 1 đến 10.");
    }
}

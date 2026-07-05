// Validator tạo/cập nhật năm học
using FluentValidation;
using UniversityPortal.Application.DTOs.NamHoc;

namespace UniversityPortal.Application.Validators.NamHoc;

/// <summary>
/// Xác thực dữ liệu đầu vào khi tạo mới hoặc cập nhật năm học.
/// Bắt buộc đúng định dạng "YYYY-YYYY" (năm sau = năm trước + 1) để các trang khác
/// (vd. seed dữ liệu ở docs/Data) có thể tin cậy định dạng này khi tra cứu theo tên.
/// </summary>
public class UpsertNamHocValidator : AbstractValidator<UpsertNamHocDto>
{
    public UpsertNamHocValidator()
    {
        RuleFor(x => x.TenNamHoc)
            .NotEmpty().WithMessage("Tên năm học không được để trống.")
            .MaximumLength(20).WithMessage("Tên năm học không quá 20 ký tự.")
            .Matches(@"^\d{4}-\d{4}$").WithMessage("Tên năm học phải theo định dạng YYYY-YYYY, vd. 2026-2027.")
            .Must(HaveConsecutiveYears).WithMessage("Năm sau phải liền sau năm trước, vd. 2026-2027.");
    }

    private static bool HaveConsecutiveYears(string tenNamHoc)
    {
        var parts = tenNamHoc.Split('-');
        return parts.Length == 2
            && int.TryParse(parts[0], out var start)
            && int.TryParse(parts[1], out var end)
            && end == start + 1;
    }
}

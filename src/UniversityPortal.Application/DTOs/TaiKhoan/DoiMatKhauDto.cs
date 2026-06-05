// DTO đổi mật khẩu — yêu cầu nhập mật khẩu cũ để xác thực quyền sở hữu
namespace UniversityPortal.Application.DTOs.TaiKhoan;

/// <summary>
/// Dữ liệu đầu vào khi người dùng tự đổi mật khẩu của mình.
/// Bắt buộc nhập mật khẩu cũ để bảo mật.
/// </summary>
public class DoiMatKhauDto
{
    public string MatKhauCu { get; set; } = string.Empty;
    public string MatKhauMoi { get; set; } = string.Empty;
    public string XacNhanMatKhau { get; set; } = string.Empty;
}

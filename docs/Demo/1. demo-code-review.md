# University Portal — Báo Cáo Review Code

> **Mục đích:** Ghi lại kết quả review code, các điểm logic chưa rõ ràng, thay đổi đã thực hiện và lưu ý khi bảo vệ.

---

## 1. Những Chỗ Logic Chưa Rõ Ràng (Cần Nắm Để Trả Lời Giảng Viên)

### 1.1 Thứ tự kiểm tra khi đăng nhập (`AuthService.LoginAsync`)

```
Thứ tự: tài khoản tồn tại? → tài khoản bị khoá? → mật khẩu đúng?
```

**Vấn đề tiềm ẩn:** Nếu tài khoản tồn tại nhưng bị khoá, server trả về HTTP 403 ("Tài khoản đã bị khoá") trước khi kiểm tra mật khẩu. Điều này có thể tiết lộ rằng tên đăng nhập đó tồn tại trong hệ thống.

**Tại sao chấp nhận được:** Đây là portal nội bộ trường, không phải hệ thống công cộng. Thông báo "Tài khoản bị khoá" giúp người dùng hiểu lý do không đăng nhập được.

---

### 1.2 Magic number `VaiTroId = 3` trong `SinhVienService.CreateAsync`

```csharp
// src/UniversityPortal.Application/Services/SinhVienService.cs : dòng 57
VaiTroId = 3, // Sinh viên
```

Con số `3` là hard-coded. Nếu seed data thay đổi thứ tự vai trò, đây sẽ là bug. Để an toàn hơn, có thể tra cứu theo tên vai trò (`"Sinh viên"`) hoặc dùng `VaiTroEnum`.

---

### 1.3 Swagger luôn bật — không chỉ trong môi trường Development

```csharp
// src/UniversityPortal.API/Program.cs : dòng 91-92
app.UseSwagger();
app.UseSwaggerUI();
```

Thông thường Swagger chỉ bật trong môi trường `Development`. Hiện tại Swagger cũng hiển thị trên Production. Điều này tiện cho demo nhưng không phải best practice cho sản phẩm thật.

---

### 1.4 Phân trang trong `BaseRepository.GetPagedAsync` không có `OrderBy`

```csharp
// src/UniversityPortal.Infrastructure/Repositories/BaseRepository.cs
var danhSach = await DbSet.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
```

Không có `OrderBy` nghĩa là thứ tự kết quả phụ thuộc vào database — không đảm bảo nhất quán giữa các trang. Repository cụ thể (như `SinhVienRepository.GetPagedFilterAsync`) có `OrderBy(x => x.Mssv)` nên không bị ảnh hưởng, nhưng phương thức base sẽ có vấn đề nếu entity khác dùng trực tiếp.

---

### 1.5 Xoay vòng Refresh Token — logic đúng nhưng không atomic

```csharp
// AuthService.RefreshTokenAsync: ghi token mới vào DB
taiKhoan.RefreshToken = refreshTokenMoi;
await uow.CommitAsync();
// Sau đó mới trả về cho client
```

Nếu server crash sau `CommitAsync` nhưng trước khi trả về response, client không nhận được token mới nhưng token cũ đã bị vô hiệu hoá — người dùng phải đăng nhập lại. Đây là tradeoff phổ biến của stateless auth.

---

## 2. Thay Đổi Đã Thực Hiện

### 2.1 Đổi tên biến viết tắt khó hiểu

| File | Tên cũ | Tên mới | Lý do |
|------|--------|---------|-------|
| [AuthService.cs](../src/UniversityPortal.Application/Services/AuthService.cs) | `jwt` (constructor param) | `jwtTokenService` | Rõ ràng hơn về kiểu dữ liệu |
| [AuthService.cs](../src/UniversityPortal.Application/Services/AuthService.cs) | `refreshDays` | `soNgayRefresh` | Nhất quán tên tiếng Việt |
| [AuthService.cs](../src/UniversityPortal.Application/Services/AuthService.cs) | `newAccessToken` / `newRefreshToken` | `accessTokenMoi` / `refreshTokenMoi` | Nhất quán tên tiếng Việt |
| [JwtTokenService.cs](../src/UniversityPortal.Infrastructure/Services/JwtTokenService.cs) | `secret` / `issuer` / `audience` / `expiry` | `chuoiBiMat` / `nhaCapToken` / `doiTuongNhan` / `soPhutHetHan` | Rõ nghĩa hơn |
| [JwtTokenService.cs](../src/UniversityPortal.Infrastructure/Services/JwtTokenService.cs) | `key` / `creds` / `claims` / `bytes` | `khoaKy` / `thongTinKy` / `danhSachClaim` / `byteNgauNhien` | Giải thích rõ vai trò từng biến |
| [SinhVienRepository.cs](../src/UniversityPortal.Infrastructure/Repositories/SinhVienRepository.cs) | `kw` | `tuKhoa` | Viết tắt không rõ ràng |
| [TaiKhoanRepository.cs](../src/UniversityPortal.Infrastructure/Repositories/TaiKhoanRepository.cs) | `kw` | `tuKhoa` | Viết tắt không rõ ràng |
| [BaseRepository.cs](../src/UniversityPortal.Infrastructure/Repositories/BaseRepository.cs) | `total` / `data` | `tongSoBanGhi` / `danhSach` | Rõ nghĩa hơn |
| [AuthController.cs](../src/UniversityPortal.API/Controllers/AuthController.cs) | `idClaim` / `id` | `claimId` / `taiKhoanId` | Tránh nhầm với `Id` entity |

### 2.2 Thêm comment tiếng Việt

| File | Nội dung đã thêm |
|------|-----------------|
| [AuthService.cs](../src/UniversityPortal.Application/Services/AuthService.cs) | Comment class + 3 method: `LoginAsync`, `RefreshTokenAsync`, `RevokeTokenAsync` |
| [GlobalExceptionMiddleware.cs](../src/UniversityPortal.API/Middleware/GlobalExceptionMiddleware.cs) | Comment class + 2 method: `InvokeAsync`, `HandleExceptionAsync` |
| [BaseRepository.cs](../src/UniversityPortal.Infrastructure/Repositories/BaseRepository.cs) | Comment class + tất cả 6 method CRUD |
| [JwtTokenService.cs](../src/UniversityPortal.Infrastructure/Services/JwtTokenService.cs) | Comment class + 2 method: `GenerateAccessToken`, `GenerateRefreshToken` |
| [AppDbContext.cs](../src/UniversityPortal.Infrastructure/Persistence/AppDbContext.cs) | Comment `SaveChangesAsync` giải thích auto-timestamp |
| [AuthController.cs](../src/UniversityPortal.API/Controllers/AuthController.cs) | Comment `Logout` giải thích cách đọc claim từ JWT |

### 2.3 Sửa lỗi nhỏ

| File | Thay đổi | Lý do |
|------|----------|-------|
| [GlobalExceptionMiddleware.cs](../src/UniversityPortal.API/Middleware/GlobalExceptionMiddleware.cs) | `"An unexpected error occurred."` → `"Đã xảy ra lỗi không mong đợi."` | Thống nhất ngôn ngữ tiếng Việt trong toàn bộ response |
| [UnitOfWork.cs](../src/UniversityPortal.Infrastructure/UnitOfWork.cs) | Thay comment `// Tuần 1` / `// Tuần 2` bằng nhóm nghiệp vụ thực tế | Comment timeline phát triển không có giá trị với người đọc code |

---

## 3. Kiến Trúc Tổng Quan (Nhanh)

```
Controller (AuthController, SinhVienController, ...)
    │  Nhận HTTP request, gọi Service
    ▼
Service (AuthService, SinhVienService, ...)
    │  Business logic, kiểm tra nghiệp vụ, ném Domain Exception
    ▼
IUnitOfWork → Repository (TaiKhoanRepository, SinhVienRepository, ...)
    │  Truy vấn EF Core, LINQ
    ▼
AppDbContext → SQL Server
    │  SaveChangesAsync tự set CreatedAt / UpdatedAt
    ▼
GlobalExceptionMiddleware  ←  bắt Domain Exception ở bất kỳ tầng nào
    │  Ánh xạ sang HTTP status + JSON chuẩn ApiResponseDto
    ▼
Client (React / Swagger)
```

---

## 4. Các File Quan Trọng Cần Demo

| File | Nội dung demo |
|------|--------------|
| [AuthService.cs](../src/UniversityPortal.Application/Services/AuthService.cs) | Luồng login, xoay vòng refresh token |
| [SinhVienService.cs](../src/UniversityPortal.Application/Services/SinhVienService.cs) | Tạo sinh viên kèm tài khoản, xoá mềm |
| [UnitOfWork.cs](../src/UniversityPortal.Infrastructure/UnitOfWork.cs) | Lazy-init repository, dùng chung DbContext |
| [BaseRepository.cs](../src/UniversityPortal.Infrastructure/Repositories/BaseRepository.cs) | Generic CRUD pattern |
| [GlobalExceptionMiddleware.cs](../src/UniversityPortal.API/Middleware/GlobalExceptionMiddleware.cs) | Xử lý lỗi tập trung |
| [AppDbContext.cs](../src/UniversityPortal.Infrastructure/Persistence/AppDbContext.cs) | Auto-timestamp, apply configurations |
| [MappingProfile.cs](../src/UniversityPortal.Application/Mappings/MappingProfile.cs) | AutoMapper Entity → DTO |
| [JwtTokenService.cs](../src/UniversityPortal.Infrastructure/Services/JwtTokenService.cs) | Tạo JWT và refresh token |

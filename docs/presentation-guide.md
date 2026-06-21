# University Portal — Tài Liệu Hướng Dẫn Bảo Vệ Đồ Án Tốt Nghiệp

> **Stack:** C# ASP.NET Core 10 · SQL Server · React + TypeScript + Vite · Docker · AWS EC2  
> **Kiến trúc:** Clean Architecture · Repository Pattern · Unit of Work · JWT Auth · EF Core Code First

---

## Mục Lục

1. [Cách mở demo khi bảo vệ](#1-cách-mở-demo-khi-bảo-vệ)
2. [Giới thiệu tổng quan đề tài](#2-giới-thiệu-tổng-quan-đề-tài)
3. [Các khái niệm cần thuộc](#3-các-khái-niệm-cần-thuộc)
4. [Kiến trúc hệ thống](#4-kiến-trúc-hệ-thống)
5. [Các Design Pattern đã dùng](#5-các-design-pattern-đã-dùng)
6. [Luồng xác thực JWT](#6-luồng-xác-thực-jwt)
7. [Nghiệp vụ chính được giải thích](#7-nghiệp-vụ-chính-được-giải-thích)
8. [Test Cases](#8-test-cases)
9. [Triển khai hệ thống](#9-triển-khai-hệ-thống)
10. [Câu hỏi thường gặp của giảng viên & trả lời](#10-câu-hỏi-thường-gặp-của-giảng-viên--trả-lời)
11. [Quy tắc trình bày](#11-quy-tắc-trình-bày)

---

## 1. Cách Mở Demo Khi Bảo Vệ

**Thực hiện theo thứ tự này:**

| Bước | Nội dung | Địa chỉ |
|------|----------|---------|
| 1 | Mở Swagger — danh sách toàn bộ API | `http://localhost:5000/swagger` |
| 2 | Đăng nhập bằng tài khoản admin, copy token | POST `/api/auth/login` |
| 3 | Authorize trong Swagger | Nhấn nút "Authorize", dán token |
| 4 | Gọi một API có phân quyền | GET `/api/sinh-vien` |
| 5 | Mở giao diện Frontend | `http://localhost:3000` |
| 6 | Mở cấu trúc solution trong IDE | Mở file `.slnx` |

---

## 2. Giới Thiệu Tổng Quan Đề Tài

### Đề tài giải quyết vấn đề gì?

**University Portal** — Cổng Thông Tin Sinh Viên — là ứng dụng web số hoá các nghiệp vụ của trường đại học, thay thế các quy trình thủ công bằng giấy tờ.

Hệ thống phục vụ **3 nhóm người dùng:**

| Vai trò | Quyền hạn chính |
|---------|----------------|
| Admin / Giáo vụ | Quản lý tài khoản, học phần, điểm, học phí, phê duyệt yêu cầu |
| Giáo viên | Nhập điểm, khoá bảng điểm, xem thời khoá biểu, tạo biên bản sinh hoạt chi nhánh |
| Sinh viên | Đăng ký học phần, xem điểm, đóng học phí, gửi yêu cầu hành chính |

### Các module đã xây dựng

- Quản lý tài khoản (TaiKhoan, VaiTro, PhongBan)
- Hồ sơ sinh viên & giáo viên (SinhVien, GiaoVien)
- Chương trình đào tạo (NganhHoc, ChuongTrinhDT, MonHoc, ChiTietCTDT)
- Đăng ký học phần (LopHocPhan, DanhSachLopHP)
- Nhập điểm & khoá bảng điểm (Diem, HocBa, DiemRenLuyen)
- Đăng ký thi lại (DanhSachThiLai)
- Thời khoá biểu (ThoiKhoaBieu)
- Học phí (HocPhi)
- Thông báo & bình luận (ThongBao, BinhLuanThongBao)
- Biên bản sinh hoạt chi nhánh (BienBanSHCN, ChiTietCongViec, ChiTietVangSHCN)
- Yêu cầu hành chính & sửa điểm (YeuCauHanhChinh, YeuCauSuaDiem)
- Đặt phòng thực hành (DatPhongThucHanh)
- Khảo sát ý kiến (KhaoSatYKien)
- Diễn đàn giáo viên (DienDanGiaoVien)

---

## 3. Các Khái Niệm Cần Thuộc

### 3.1 Clean Architecture (Kiến trúc sạch)

Clean Architecture chia hệ thống thành các lớp đồng tâm. **Lớp bên trong không bao giờ biết về lớp bên ngoài.**

```
┌─────────────────────────────────────────────┐
│                    API                       │  ← HTTP, Controllers, Middleware
│  ┌───────────────────────────────────────┐  │
│  │             Infrastructure            │  │  ← EF Core, Repositories, DB
│  │  ┌─────────────────────────────────┐  │  │
│  │  │           Application           │  │  │  ← Services, DTOs, Interfaces
│  │  │  ┌───────────────────────────┐  │  │  │
│  │  │  │          Domain           │  │  │  │  ← Entities, Exceptions, Enums
│  │  │  └───────────────────────────┘  │  │  │
│  │  └─────────────────────────────────┘  │  │
│  └───────────────────────────────────────┘  │
└─────────────────────────────────────────────┘
```

**Quy tắc phụ thuộc:** Mũi tên chỉ hướng vào trong.

```
API  →  Application  →  Domain
         ↑
   Infrastructure
```

- `Domain` — không tham chiếu bất kỳ project nào khác
- `Application` — chỉ tham chiếu `Domain`
- `Infrastructure` — tham chiếu `Application` + `Domain`
- `API` — tham chiếu `Application` + `Infrastructure` (chỉ để đăng ký DI)

**Tại sao quan trọng?** Nếu ngày mai chuyển từ SQL Server sang PostgreSQL, chỉ thay đổi `Infrastructure`. Lớp `Domain` và `Application` không bị ảnh hưởng.

---

### 3.2 Entity Framework Core — Code First

Thay vì viết câu lệnh SQL `CREATE TABLE` bằng tay, ta định nghĩa các class C# (entity) và EF Core tự tạo schema database.

**Base class mà tất cả entity đều kế thừa:**

```csharp
// Domain/Entities/Common/AuditableEntity.cs
public abstract class AuditableEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**AppDbContext tự cập nhật timestamp mỗi khi lưu:**

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
    {
        if (entry.State == EntityState.Added)
            entry.Entity.CreatedAt = DateTime.UtcNow;
        entry.Entity.UpdatedAt = DateTime.UtcNow;
    }
    return await base.SaveChangesAsync(ct);
}
```

**IEntityTypeConfiguration** ánh xạ từng entity vào bảng SQL một cách tường minh:

```csharp
// Ví dụ: DanhSachThiLaiConfiguration.cs
builder.ToTable("danh_sach_thi_lai");
builder.Property(x => x.SoTienPhaiDong)
       .HasColumnType("decimal(12,2)")
       .HasColumnName("so_tien_phai_dong");
builder.HasOne(x => x.SinhVien)
       .WithMany(s => s.DanhSachThiLais)
       .HasForeignKey(x => x.SinhVienId)
       .OnDelete(DeleteBehavior.Cascade);
```

---

### 3.3 Repository Pattern

Repository là lớp ẩn đi logic truy vấn database phía sau một interface. Tầng service chỉ gọi interface — không quan tâm SQL được viết như thế nào.

```csharp
// IRepository<T> — CRUD chung cho mọi entity
public interface IRepository<T> where T : AuditableEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PagedResultDto<T>> GetPagedAsync(int page, int pageSize);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

Mỗi entity cần query đặc thù thì có interface riêng:

```csharp
// ISinhVienRepository — thêm các method nghiệp vụ
public interface ISinhVienRepository : IRepository<SinhVien>
{
    Task<SinhVien?> GetByMssvAsync(string mssv);
    Task<SinhVien?> GetDetailAsync(int id);  // load kèm navigation properties
    Task<PagedResultDto<SinhVien>> GetPagedFilterAsync(
        int page, int pageSize, string? keyword, int? lopId);
}
```

---

### 3.4 Unit of Work

Unit of Work gom tất cả repository lại dưới một kết nối database. Mọi thay đổi được commit trong một transaction duy nhất qua `CommitAsync()`.

```csharp
// IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    ITaiKhoanRepository TaiKhoans { get; }
    ISinhVienRepository SinhViens { get; }
    IGiaoVienRepository GiaoViens { get; }
    // ... tất cả repositories
    Task<int> CommitAsync();
}
```

**Lazy initialization** — repository chỉ được tạo khi lần đầu tiên được truy cập:

```csharp
// UnitOfWork.cs
public ITaiKhoanRepository TaiKhoans
    => _taiKhoans ??= new TaiKhoanRepository(context);
```

**Tại sao không inject từng repository riêng lẻ?**
Nếu inject 3 repository riêng, mỗi cái có `DbContext` của riêng mình, chúng không thể tham gia cùng một transaction. Unit of Work dùng chung một `AppDbContext` cho toàn bộ request.

---

### 3.5 Service Layer (Tầng dịch vụ)

Service chứa business logic. Nhận DTO từ controller, gọi repository qua UnitOfWork, trả về DTO.

```
Controller  →  Service  →  UnitOfWork  →  Repository  →  DbContext  →  SQL Server
                  ↑ business logic nằm ở đây
```

Ví dụ — tạo sinh viên đồng thời tạo tài khoản đăng nhập:

```csharp
public async Task<SinhVienDto> CreateAsync(CreateSinhVienDto dto)
{
    // 1. Kiểm tra tên đăng nhập & MSSV chưa tồn tại
    if (await uow.TaiKhoans.GetByTenDangNhapAsync(dto.TenDangNhap) is not null)
        throw new BadRequestException($"Tên đăng nhập '{dto.TenDangNhap}' đã tồn tại.");

    // 2. Tạo TaiKhoan trước để lấy Id
    var taiKhoan = new TaiKhoan { VaiTroId = 3 /* SinhVien */, ... };
    await uow.TaiKhoans.AddAsync(taiKhoan);
    await uow.CommitAsync();

    // 3. Tạo SinhVien liên kết với TaiKhoan vừa tạo
    var sinhVien = new SinhVien { TaiKhoanId = taiKhoan.Id, Mssv = dto.Mssv };
    await uow.SinhViens.AddAsync(sinhVien);
    await uow.CommitAsync();

    return mapper.Map<SinhVienDto>(await uow.SinhViens.GetDetailAsync(sinhVien.Id));
}
```

---

### 3.6 AutoMapper

AutoMapper tự động chuyển đổi giữa Entity và DTO, tránh phải copy từng property thủ công.

```csharp
// Không có AutoMapper (tệ)
var dto = new SinhVienDto { Id = sv.Id, HoTen = sv.TaiKhoan.HoTen, ... };

// Có AutoMapper (tốt)
var dto = mapper.Map<SinhVienDto>(sv);
```

Các quy tắc ánh xạ được định nghĩa một lần trong `MappingProfile.cs`.

---

### 3.7 FluentValidation

Quy tắc kiểm tra dữ liệu đầu vào được viết thành class riêng, thay vì dùng Data Annotation trên DTO.

```csharp
public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.TenDangNhap).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MatKhau).NotEmpty().MinimumLength(6);
    }
}
```

Nếu validation thất bại, ASP.NET Core tự động trả về HTTP 400 kèm chi tiết lỗi trước khi phương thức controller được gọi.

---

### 3.8 JWT (JSON Web Token)

JWT là token xác thực không lưu trạng thái (stateless). Server ký token bằng secret key; client gửi token trong header mỗi request.

```
Client gửi:   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Server xác thực: chữ ký + hạn sử dụng + issuer + audience
```

**2 loại token trong project:**

| Token | Hạn sử dụng | Mục đích |
|-------|------------|---------|
| Access Token | 60 phút | Xác thực mỗi API call |
| Refresh Token | 7 ngày | Lấy access token mới mà không cần đăng nhập lại |

---

## 4. Kiến Trúc Hệ Thống

### 4.1 Cấu trúc Solution

```
UniversityPortal.sln
├── src/
│   ├── UniversityPortal.Domain/          ← Không phụ thuộc gì
│   │   ├── Entities/                     ← AuditableEntity + 30 entity
│   │   ├── Enums/                        ← VaiTroEnum, TrangThaiDuyetEnum, ...
│   │   └── Exceptions/                   ← NotFoundException, BadRequestException, ForbiddenException
│   │
│   ├── UniversityPortal.Application/     ← Tham chiếu: Domain
│   │   ├── Interfaces/
│   │   │   ├── Repositories/             ← IRepository<T> + 10 interface đặc thù
│   │   │   ├── Services/                 ← IAuthService, ISinhVienService, ...
│   │   │   └── IUnitOfWork.cs
│   │   ├── DTOs/                         ← Đối tượng Request/Response
│   │   ├── Services/                     ← Cài đặt business logic
│   │   ├── Mappings/MappingProfile.cs    ← Cấu hình AutoMapper
│   │   └── Validators/                   ← Cài đặt FluentValidation
│   │
│   ├── UniversityPortal.Infrastructure/  ← Tham chiếu: Application + Domain
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/           ← IEntityTypeConfiguration cho từng entity
│   │   │   └── Migrations/               ← EF Core tự sinh migration
│   │   ├── Repositories/                 ← Cài đặt cụ thể từng repository
│   │   ├── UnitOfWork.cs
│   │   └── DependencyInjection.cs        ← AddInfrastructure()
│   │
│   └── UniversityPortal.API/             ← Tham chiếu: Application + Infrastructure
│       ├── Controllers/                  ← Các endpoint HTTP
│       ├── Middleware/GlobalExceptionMiddleware.cs
│       └── Program.cs
│
├── tests/
│   └── UniversityPortal.Tests/
│
└── frontend/                             ← React + TypeScript + Vite
```

---

### 4.2 Vòng đời của một HTTP Request

Một request đi qua các tầng này:

```
HTTP Request  ──→  JWT Middleware (xác thực token)
                        ↓
               GlobalExceptionMiddleware (bắt lỗi toàn cục)
                        ↓
               Controller (parse request, gọi service)
                        ↓
               Service (business logic, ném domain exception nếu lỗi)
                        ↓
               UnitOfWork → Repository (truy vấn EF Core)
                        ↓
               SQL Server
                        ↓
               Repository trả Entity về Service
                        ↓
               Service map sang DTO (AutoMapper)
                        ↓
               Controller đóng gói vào ApiResponseDto<T>
                        ↓
HTTP Response  ←  JSON { success: true, data: {...} }
```

---

### 4.3 Chuẩn hoá Response API

Mọi endpoint đều trả về cùng một cấu trúc JSON:

```json
// Thành công
{
  "success": true,
  "data": { "id": 1, "hoTen": "Nguyễn Thị Hoa", "mssv": "2100001" },
  "message": null,
  "errors": null
}

// Thất bại
{
  "success": false,
  "data": null,
  "message": "Tên đăng nhập hoặc mật khẩu không đúng.",
  "errors": null
}
```

`GlobalExceptionMiddleware` ánh xạ domain exception sang HTTP status code:

| Exception | HTTP Status |
|-----------|------------|
| `NotFoundException` | 404 Not Found |
| `BadRequestException` | 400 Bad Request |
| `ForbiddenException` | 403 Forbidden |
| `UnauthorizedAccessException` | 401 Unauthorized |
| Mọi exception khác | 500 Internal Server Error |

---

## 5. Các Design Pattern Đã Dùng

| Pattern | Vị trí trong code | Mục đích |
|---------|------------------|---------|
| Clean Architecture | Toàn bộ backend | Tách biệt mối quan tâm, dễ test, dễ thay DB |
| Repository Pattern | `Infrastructure/Repositories` | Ẩn truy vấn DB, mock được trong test |
| Unit of Work | `UnitOfWork.cs` | Một transaction duy nhất cho mỗi request |
| Service Layer | `Application/Services` | Business logic tập trung một chỗ |
| DTO Pattern | `Application/DTOs` | Không bao giờ expose entity trực tiếp ra client |
| AutoMapper | `MappingProfile.cs` | Tự động chuyển đổi Entity ↔ DTO |
| FluentValidation | `Application/Validators` | Validation dữ liệu đầu vào dạng khai báo |
| Middleware Pipeline | `GlobalExceptionMiddleware` | Xử lý lỗi tập trung |
| Dependency Injection | `Program.cs` qua `AddApplication()` / `AddInfrastructure()` | Loose coupling |
| Static Factory | `ApiResponseDto.Ok()` / `ApiResponseDto.Fail()` | Tạo response nhất quán |

---

## 6. Luồng Xác Thực JWT

### 6.1 Đăng nhập

```
POST /api/auth/login
Body: { "tenDangNhap": "sv001", "matKhau": "Password@123" }

Luồng xử lý:
1. AuthService.LoginAsync() được gọi
2. Tìm TaiKhoan theo tên đăng nhập
3. Kiểm tra tài khoản đang hoạt động (TrangThai = true)
4. BCrypt.Verify(mật khẩu nhập, hash trong DB)
5. Nếu hợp lệ → JwtTokenService.GenerateAccessToken(taiKhoan)
6. Sinh refresh token (GUID ngẫu nhiên)
7. Lưu refresh token + hạn sử dụng vào database
8. Trả về: { accessToken, refreshToken, userInfo }
```

### 6.2 Làm mới token

```
POST /api/auth/refresh
Body: { "refreshToken": "..." }

Luồng xử lý:
1. Tìm TaiKhoan theo refresh token
2. Kiểm tra RefreshTokenExpiry > DateTime.UtcNow
3. Sinh access token mới + refresh token mới
4. Xoay vòng refresh token trong DB (token cũ bị vô hiệu hoá)
5. Trả về token mới
```

### 6.3 Phân quyền theo vai trò

Controller dùng attribute `[Authorize(Roles = "...")]`:

```csharp
[Authorize(Roles = "Admin,GiaoVu")]
[HttpPost]
public async Task<IActionResult> Create(CreateSinhVienDto dto) { ... }

[Authorize(Roles = "Admin,GiaoVu,SinhVien")]
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id) { ... }
```

---

## 7. Nghiệp Vụ Chính Được Giải Thích

### 7.1 Công thức tính điểm

`DiemTongKet = DiemQT1 × 0.15 + DiemQT2 × 0.15 + DiemThi × 0.70`

Ví dụ từ dữ liệu seed:
- Sinh viên 1 (đạt): `8.0 × 0.15 + 7.5 × 0.15 + 8.5 × 0.70 = 1.20 + 1.125 + 5.95 = 8.275 ≈ 8.3`
- Sinh viên 3 (rớt môn): `4.5 × 0.15 + 5.0 × 0.15 + 3.0 × 0.70 = 0.675 + 0.75 + 2.10 = 3.525 ≈ 3.6`

### 7.2 Khoá bảng điểm

Khi giáo viên khoá bảng điểm (`khoa_bang_diem = true`), điểm không thể sửa nữa.  
Muốn sửa, giáo viên phải tạo `YeuCauSuaDiem` → Admin phê duyệt → bảng điểm được mở lại.

### 7.3 Xoá mềm (Soft Delete)

Sinh viên và giáo viên **không bao giờ bị xoá vật lý**. Thay vào đó, trường `TaiKhoan.TrangThai` được đặt thành `false`. Cách này giữ lại toàn bộ lịch sử điểm, học phí và biên bản.

### 7.4 Khoá ngoại vòng tròn (LopSinhHoat)

`LopSinhHoat` có `ThuKyId` trỏ vào `SinhVien`.  
`SinhVien` có `LopId` trỏ vào `LopSinhHoat`.  
Vòng tròn này được giải quyết bằng cách insert 2 bước:
1. Insert `SinhVien` với `LopId = NULL`
2. Insert `LopSinhHoat` với `ThuKyId = NULL`
3. Update `SinhVien.LopId` trỏ vào lớp
4. Update `LopSinhHoat.ThuKyId` trỏ vào sinh viên

### 7.5 Thông báo

- `lop_nhan_id = NULL` → gửi toàn trường
- `lop_nhan_id = {id}` → chỉ gửi cho lớp đó
- `ThongBaoDaDoc` lưu trạng thái đã đọc / chưa đọc theo từng người dùng

---

## 8. Test Cases

> Tất cả test dùng **xUnit** + **Moq** + **FluentAssertions**  
> Mẫu viết test: **AAA — Arrange (Chuẩn bị) / Act (Thực hiện) / Assert (Kiểm tra)**

---

### TC-01: Đăng nhập — Đúng thông tin

**Module:** Auth  
**Kịch bản:** Người dùng đăng nhập với tên đăng nhập và mật khẩu đúng  
**Điều kiện:** Tài khoản `sv001` tồn tại với `TrangThai = true`

```
Arrange:
  - Mock ITaiKhoanRepository.GetByTenDangNhapAsync("sv001")
    → trả về TaiKhoan { MatKhau = BCrypt.HashPassword("Password@123"), TrangThai = true }
  - Mock IJwtTokenService.GenerateAccessToken() → "fake-access-token"
  - Mock IJwtTokenService.GenerateRefreshToken() → "fake-refresh-token"

Act:
  - await authService.LoginAsync(new LoginRequestDto
      { TenDangNhap = "sv001", MatKhau = "Password@123" })

Assert:
  - result.AccessToken == "fake-access-token"
  - result.RefreshToken == "fake-refresh-token"
  - result.UserInfo.HoTen khác null
```

**Kết quả mong đợi:** HTTP 200, trả về cả 2 token

---

### TC-02: Đăng nhập — Sai mật khẩu

**Module:** Auth  
**Kịch bản:** Người dùng nhập mật khẩu sai

```
Arrange:
  - Mock GetByTenDangNhapAsync trả về tài khoản với hash khác

Act:
  - await authService.LoginAsync({ TenDangNhap = "sv001", MatKhau = "SaiMatKhau" })

Assert:
  - ném BadRequestException("Tên đăng nhập hoặc mật khẩu không đúng.")
```

**Kết quả mong đợi:** HTTP 400

---

### TC-03: Đăng nhập — Tài khoản bị khoá

**Module:** Auth  
**Kịch bản:** Tài khoản tồn tại nhưng đang bị vô hiệu hoá

```
Arrange:
  - Mock GetByTenDangNhapAsync trả về TaiKhoan { TrangThai = false }

Act:
  - await authService.LoginAsync(...)

Assert:
  - ném ForbiddenException("Tài khoản đã bị khoá.")
```

**Kết quả mong đợi:** HTTP 403

---

### TC-04: Tạo sinh viên — Thành công

**Module:** SinhVien  
**Kịch bản:** Admin tạo sinh viên mới với thông tin hợp lệ, chưa trùng

```
Arrange:
  - Mock GetByTenDangNhapAsync("sv_new") → null   (tên đăng nhập chưa có)
  - Mock GetByMssvAsync("2100099") → null           (MSSV chưa có)
  - Mock TaiKhoans.AddAsync() → hoàn thành
  - Mock SinhViens.AddAsync() → hoàn thành
  - Mock CommitAsync() → 1
  - Mock GetDetailAsync() → SinhVien kèm TaiKhoan

Act:
  - await sinhVienService.CreateAsync(new CreateSinhVienDto
      { TenDangNhap = "sv_new", Mssv = "2100099", ... })

Assert:
  - result.Mssv == "2100099"
  - uow.CommitAsync() được gọi 2 lần (1 cho TaiKhoan, 1 cho SinhVien)
```

**Kết quả mong đợi:** HTTP 201, trả về SinhVienDto với Id mới

---

### TC-05: Tạo sinh viên — Tên đăng nhập đã tồn tại

**Module:** SinhVien

```
Arrange:
  - Mock GetByTenDangNhapAsync("sv001") → trả về TaiKhoan đang có

Act:
  - await sinhVienService.CreateAsync({ TenDangNhap = "sv001", ... })

Assert:
  - ném BadRequestException("Tên đăng nhập 'sv001' đã tồn tại.")
```

**Kết quả mong đợi:** HTTP 400

---

### TC-06: Tạo sinh viên — MSSV đã tồn tại

**Module:** SinhVien

```
Arrange:
  - Mock GetByTenDangNhapAsync("sv_new") → null
  - Mock GetByMssvAsync("2100001") → trả về SinhVien đang có

Assert:
  - ném BadRequestException("MSSV '2100001' đã tồn tại.")
```

**Kết quả mong đợi:** HTTP 400

---

### TC-07: Lấy sinh viên theo Id — Không tìm thấy

**Module:** SinhVien

```
Arrange:
  - Mock GetDetailAsync(999) → null

Act:
  - await sinhVienService.GetByIdAsync(999)

Assert:
  - ném NotFoundException("Không tìm thấy sinh viên id = 999.")
```

**Kết quả mong đợi:** HTTP 404

---

### TC-08: Xoá sinh viên — Xoá mềm

**Module:** SinhVien  
**Kịch bản:** Xoá sinh viên chỉ vô hiệu hoá tài khoản, không xoá bản ghi.

```
Arrange:
  - Mock GetDetailAsync(1) → SinhVien { TaiKhoan.TrangThai = true }

Act:
  - await sinhVienService.DeleteAsync(1)

Assert:
  - sinhVien.TaiKhoan.TrangThai == false  (tài khoản bị khoá)
  - uow.SinhViens.Update() đã được gọi
  - uow.CommitAsync() đã được gọi
  - Bản ghi KHÔNG bị xoá khỏi database
```

**Kết quả mong đợi:** HTTP 200, tài khoản bị vô hiệu hoá

---

### TC-09: Làm mới token — Refresh token đã hết hạn

**Module:** Auth

```
Arrange:
  - Mock GetByRefreshTokenAsync trả về TaiKhoan {
      RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1)  // đã quá hạn
    }

Act:
  - await authService.RefreshTokenAsync("token-cu")

Assert:
  - ném BadRequestException("Refresh token đã hết hạn. Vui lòng đăng nhập lại.")
```

**Kết quả mong đợi:** HTTP 400, bắt buộc đăng nhập lại

---

### TC-10: Làm mới token — Token không hợp lệ

**Module:** Auth

```
Arrange:
  - Mock GetByRefreshTokenAsync("token-rac") → null

Assert:
  - ném BadRequestException("Refresh token không hợp lệ.")
```

**Kết quả mong đợi:** HTTP 400

---

### TC-11: Thu hồi token — Tài khoản không tồn tại

**Module:** Auth

```
Arrange:
  - Mock GetByIdAsync(999) → null

Act:
  - await authService.RevokeTokenAsync(999)

Assert:
  - ném NotFoundException("Tài khoản không tồn tại.")
```

**Kết quả mong đợi:** HTTP 404

---

### TC-12: Lấy danh sách sinh viên — Lọc theo từ khoá

**Module:** SinhVien

```
Arrange:
  - Mock GetPagedFilterAsync(page=1, pageSize=10, keyword="Hoa", lopId=null)
    → trả về { Data: [sinh viên có chứa "Hoa"], Total: 1 }

Act:
  - await sinhVienService.GetPagedAsync(1, 10, "Hoa", null)

Assert:
  - result.Data.Count() == 1
  - result.Total == 1
  - result.Page == 1
  - result.PageSize == 10
```

---

### TC-13: GlobalExceptionMiddleware — Ánh xạ exception sang HTTP status

**Module:** Middleware

```
Arrange:
  - Middleware tiếp theo ném NotFoundException("Không tìm thấy")

Act:
  - middleware.InvokeAsync(httpContext)

Assert:
  - httpContext.Response.StatusCode == 404
  - Body JSON chứa { success: false, message: "Không tìm thấy" }
```

---

### TC-14: Tính điểm — Công thức đúng

**Module:** DanhSachLopHP

```
Arrange:
  - DiemQT1 = 8.0, DiemQT2 = 7.5, DiemThi = 8.5

Act:
  - Tính DiemTongKet theo công thức 15% + 15% + 70%

Assert:
  - DiemTongKet == 8.0 × 0.15 + 7.5 × 0.15 + 8.5 × 0.70
  - DiemTongKet ≈ 8.3
```

---

### TC-15: Thông báo — Toàn trường vs riêng lớp

**Module:** ThongBao

```
Kịch bản A: Gửi toàn trường
  - thongBao.LopNhanId = null
  - Kiểm tra: Tất cả sinh viên đều nhận được

Kịch bản B: Gửi theo lớp
  - thongBao.LopNhanId = 1
  - Kiểm tra: Chỉ sinh viên thuộc LopSinhHoat.Id = 1 nhận được
  - Kiểm tra: Sinh viên lớp khác KHÔNG nhận được
```

---

## 9. Triển Khai Hệ Thống

### Môi trường local (Development)

```bash
# Khởi động toàn bộ hệ thống bằng Docker Compose
docker-compose up -d

# Các service được khởi động:
# - SQL Server  → localhost:1433
# - Backend API → localhost:5000
# - Frontend    → localhost:80
```

### Môi trường Production (AWS EC2)

```
Internet
   ↓
[EC2 Instance - t3.small]
   ↓
[Nginx (cổng 80/443)]
   ├── /       → Phục vụ React SPA (file tĩnh)
   └── /api/   → Proxy ngược về backend:5000
                     ↓
            [ASP.NET Core API - cổng 5000]
                     ↓
            [SQL Server - cổng 1433]
```

### CI/CD — GitHub Actions

```yaml
Trigger: git push lên nhánh main
Các bước:
  1. SSH vào máy chủ EC2
  2. git pull origin main
  3. docker-compose up -d --build
  → Cập nhật tự động không cần can thiệp thủ công
```

### Health Check

```
GET /health
→ { "status": "healthy", "timestamp": "2026-06-21T..." }
```

Docker dùng endpoint này để kiểm tra backend đã sẵn sàng trước khi frontend khởi động.

---

## 10. Câu Hỏi Thường Gặp Của Giảng Viên & Trả Lời

**H: Tại sao dùng Clean Architecture thay vì kiến trúc 3 tầng đơn giản (Controller → Service → DB)?**  
T: Clean Architecture giúp lớp Domain và Application hoàn toàn độc lập với infrastructure. Có thể unit test business logic mà không cần chạm đến database. Nếu cần đổi SQL Server sang PostgreSQL, chỉ thay đổi lớp Infrastructure, các lớp còn lại không bị ảnh hưởng.

**H: Sự khác nhau giữa Repository Pattern và dùng DbContext trực tiếp trong service là gì?**  
T: Inject DbContext trực tiếp vào service trộn lẫn các mối quan tâm và khó viết test. Repository Pattern đặt toàn bộ logic truy vấn vào một chỗ. Khi test, ta có thể mock `IRepository<T>` mà không cần database thật.

**H: Tại sao cần Unit of Work thay vì inject từng repository riêng?**  
T: Nếu inject 3 repository riêng, mỗi cái tạo DbContext riêng, chúng không thể dùng chung một transaction. Unit of Work đảm bảo tất cả repository trong một HTTP request dùng chung một `AppDbContext`, nên `CommitAsync()` commit mọi thứ một lúc, đảm bảo tính nguyên tử.

**H: JWT là gì và tại sao không dùng session?**  
T: JWT là stateless — server không lưu trạng thái session. Token mang sẵn vai trò và thông tin người dùng. Hệ thống có thể scale ngang vì bất kỳ server nào cũng xác thực được token mà không cần shared session store. Session yêu cầu sticky session hoặc Redis dùng chung.

**H: Nếu access token bị đánh cắp thì sao?**  
T: Access token hết hạn sau 60 phút. Kẻ tấn công chỉ dùng được trong khoảng thời gian đó. Refresh token được xoay vòng mỗi lần sử dụng — token cũ bị vô hiệu hoá. Nếu refresh token bị đánh cắp và dùng, lần làm mới tiếp theo của người dùng hợp lệ sẽ thất bại, phát hiện ra sự cố.

**H: Tại sao dùng BCrypt thay vì MD5 hay SHA256?**  
T: MD5 và SHA256 là hàm băm nhanh — dễ brute-force bằng GPU. BCrypt cố tình làm chậm quá trình băm bằng cost factor, khiến brute-force không khả thi. BCrypt cũng tự động thêm salt vào mỗi hash, nên hai mật khẩu giống hệt nhau sẽ cho ra hash khác nhau.

**H: Xoá mềm là gì và tại sao dùng ở đây?**  
T: Xoá mềm đặt `TrangThai = false` thay vì xoá bản ghi. Hồ sơ sinh viên được liên kết với điểm, học phí, và bảng điểm tích luỹ. Xoá vật lý sẽ phá vỡ các khoá ngoại và xoá mất lịch sử học tập.

**H: AutoMapper có ảnh hưởng đến hiệu suất không?**  
T: AutoMapper dùng reflection + compiled expression tree. Với kích thước dữ liệu thông thường của ứng dụng web, overhead là không đáng kể. Lợi ích — không phải viết copy thủ công — vượt trội so với chi phí hiệu suất nhỏ đó.

**H: Hệ thống ngăn chặn SQL Injection như thế nào?**  
T: Entity Framework Core dùng parameterized query cho mọi thao tác. Dữ liệu đầu vào của người dùng không bao giờ được nối trực tiếp vào chuỗi SQL. Project này không dùng `FromSqlRaw` với dữ liệu không tin cậy.

**H: Frontend giao tiếp với backend như thế nào?**  
T: React dùng Axios kèm interceptor. Khi nhận được phản hồi 401 (access token hết hạn), interceptor tự động gọi `POST /api/auth/refresh`, nhận access token mới và thử lại request ban đầu — người dùng không cần biết điều này.

---

## 11. Quy Tắc Trình Bày

### Thứ tự trình bày

1. **Vấn đề** — Hệ thống giải quyết bài toán gì? (Quy trình thủ công bằng giấy tờ)
2. **Giải pháp** — Hệ thống làm được những gì?
3. **Kiến trúc** — Code được tổ chức như thế nào? (Vẽ sơ đồ các lớp)
4. **Demo** — Mở Swagger → Đăng nhập → Gọi API có phân quyền → Mở Frontend
5. **Pattern chính** — Giải thích 2–3 pattern đã dùng và lý do chọn
6. **Testing** — Chỉ file test, giải thích chi tiết 1–2 test case
7. **Triển khai** — Docker Compose, EC2, CI/CD

### Nên làm

- Vẽ sơ đồ Clean Architecture lên bảng **trước** khi mở code
- Demo code đang chạy thật — demo trên Swagger thuyết phục hơn slide
- Luôn giải thích **lý do** chọn pattern, không chỉ nói **pattern đó là gì**
- Dùng các từ khoá: *tách biệt mối quan tâm*, *dễ kiểm thử*, *loose coupling*, *đảo ngược phụ thuộc*
- Khi mở một file, chỉ rõ nó nằm ở lớp nào trong sơ đồ kiến trúc

### Không nên làm

- Không đọc code từng dòng — tóm tắt class đó làm gì và tại sao
- Không nói "cái này chỉ là CRUD" — giải thích nghiệp vụ bên trong
- Không bỏ qua schema database — chỉ ERD và giải thích các mối quan hệ chính
- Không hoảng loạn khi bị hỏi về thứ chưa làm — nói "phần đó thuộc sprint tiếp theo, hiện tại nhóm ưu tiên X vì..."

### Câu trả lời mẫu cần thuộc

| Tình huống | Nói gì |
|-----------|--------|
| Giải thích Clean Architecture | "Quy tắc phụ thuộc quy định lớp ngoài phụ thuộc vào lớp trong, không bao giờ ngược lại. Domain không biết gì về Entity Framework hay HTTP." |
| Giải thích Unit of Work | "Tất cả repository trong một HTTP request dùng chung một DbContext. CommitAsync() giống như lệnh COMMIT trong transaction SQL — tất cả lưu hoặc không gì được lưu." |
| Giải thích xoá mềm | "Không bao giờ xoá vật lý hồ sơ sinh viên vì chúng được liên kết với điểm, học phí và bảng điểm tích luỹ. Vô hiệu hoá tài khoản giữ nguyên toàn bộ lịch sử." |
| Giải thích JWT | "Token tự chứa thông tin người dùng và vai trò. Server không cần tra cứu session trong database cho mỗi request, giúp hệ thống scale tốt hơn." |
| Khi có lỗi trong demo | "Đây là lỗi đã biết trong sprint hiện tại. Trong môi trường production, vấn đề này được xử lý bằng [giải thích cách sửa]. Tính năng chính hoạt động đúng như đã trình bày." |

---

## Tờ Tham Chiếu Nhanh (In Ra Mang Theo)

```
TÊN ĐỀ TÀI:  University Portal — Cổng Thông Tin Sinh Viên
CÔNG NGHỆ:   ASP.NET Core 10 · SQL Server · React + TypeScript · Docker · EC2
PATTERN:     Clean Architecture · Repository + Unit of Work · JWT
CÁC TẦNG:   Domain → Application → Infrastructure → API
XÁC THỰC:   BCrypt + JWT AccessToken (60 phút) + RefreshToken (7 ngày, xoay vòng)
XOÁ:        Xoá mềm (TrangThai = false), dữ liệu được giữ lại
ĐIỂM:       DiemTongKet = QT1×15% + QT2×15% + Thi×70%
TEST:       xUnit + Moq + FluentAssertions, mẫu AAA
TRIỂN KHAI: docker-compose up → EC2 → Nginx → GitHub Actions CI/CD
HEALTH:     GET /health
SWAGGER:    http://localhost:5000/swagger
```

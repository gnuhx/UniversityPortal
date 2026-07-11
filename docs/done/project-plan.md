# 🎓 University Portal — Project Plan (As-Built)

**Stack thực tế:** C# ASP.NET Core (.NET 10) · **SQL Server** · ReactJS 19 + Vite + **Ant Design** · Docker Compose · GHCR (GitHub Container Registry)
**Patterns:** Clean Architecture · Repository Pattern · Unit of Work · Service Layer · Entity Framework Core (Code First, auto-migrate on startup)

> Tài liệu này mô tả kiến trúc **hiện có trong source code**, không còn là kế hoạch dự kiến. Trạng thái tổng quan: xem mục [Tình trạng hiện tại](#-tình-trạng-hiện-tại) cuối file.

---

## 📁 Cấu trúc Solution (thực tế)

```
UniversityPortal/
├── src/
│   ├── UniversityPortal.Domain/
│   │   ├── Entities/
│   │   │   ├── Common/AuditableEntity.cs      # Id, CreatedAt, UpdatedAt
│   │   │   ├── TaiKhoan.cs (+ RefreshToken, RefreshTokenExpiry)
│   │   │   ├── VaiTro.cs, PhongBan.cs
│   │   │   ├── GiaoVien.cs, SinhVien.cs, KetQuaAnhVanDauVao.cs
│   │   │   ├── LopSinhHoat.cs, NganhHoc.cs, ChuongTrinhDT.cs
│   │   │   ├── MonHoc.cs, ChiTietCTDT.cs
│   │   │   ├── NamHoc.cs, HocKy.cs, TuanHoc.cs
│   │   │   ├── LopHocPhan.cs, DanhSachLopHP.cs, DanhSachThiLai.cs
│   │   │   ├── HocBa.cs, DiemRenLuyen.cs, ThoiKhoaBieu.cs, HocPhi.cs
│   │   │   ├── ThongBao.cs, ThongBaoDaDoc.cs, BinhLuanThongBao.cs
│   │   │   ├── BienBanSHCN.cs, ChiTietCongViec.cs, ChiTietVangSHCN.cs
│   │   │   ├── YeuCauHanhChinh.cs (+ GiayXacNhan, BienNhan, PhanHoi)
│   │   │   ├── YeuCauSuaDiem.cs
│   │   │   ├── DatPhongThucHanh.cs, KhaoSatYKien.cs, DienDanGiaoVien.cs
│   │   │   └── NoiDungTinh.cs                  # thêm mới — nội dung tĩnh (thư viện/học vụ)
│   │   ├── Enums/
│   │   │   ├── VaiTroEnum.cs              # Admin=1, GiaoVien=2, SinhVien=3, GiaoVu=4
│   │   │   ├── LoaiDangKyEnum.cs, TrangThaiDuyetEnum.cs
│   │   │   ├── TrangThaiDongTienEnum.cs
│   │   │   └── LoaiThongBaoEnum.cs, MucDoThongBaoEnum.cs
│   │   └── Exceptions/ (NotFoundException, BadRequestException, ForbiddenException)
│   │
│   ├── UniversityPortal.Application/
│   │   ├── Interfaces/Repositories/    # 1 interface/entity có query riêng + IUnitOfWork
│   │   ├── Interfaces/Services/        # 1 interface / module nghiệp vụ
│   │   ├── DTOs/                       # 1 folder / module (TaiKhoan, SinhVien, LopHocPhan, DanhSachLopHP, HocPhi, ThongBao, YeuCauHanhChinh, YeuCauSuaDiem, NoiDungTinh, ...) + Common (PagedResultDto, ApiResponseDto)
│   │   ├── Mappings/MappingProfile.cs  # AutoMapper
│   │   ├── Validators/                 # FluentValidation — theo từng module chính
│   │   ├── Services/                   # implementation của Interfaces/Services
│   │   └── DependencyInjection.cs      # AddApplication(services)
│   │
│   ├── UniversityPortal.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs                 # ~32 DbSet, override SaveChangesAsync cho audit
│   │   │   ├── Configurations/                 # IEntityTypeConfiguration cho từng entity
│   │   │   └── Migrations/                     # EF Core migrations, auto-apply lúc Program.cs khởi động
│   │   ├── Repositories/                       # BaseRepository<T> + repo riêng theo entity
│   │   ├── Services/                           # JwtTokenService, FileService (upload avatar/tài liệu)
│   │   ├── UnitOfWork.cs
│   │   └── DependencyInjection.cs              # AddInfrastructure(services, config) — UseSqlServer
│   │
│   └── UniversityPortal.API/
│       ├── Controllers/            # 21 controllers — xem bảng bên dưới
│       ├── Middleware/GlobalExceptionMiddleware.cs
│       ├── appsettings.json / appsettings.Development.json
│       ├── Program.cs              # JWT auth, Swagger + Bearer, CORS (AllowAny), auto-migrate, /health, static /uploads
│       └── Dockerfile
│
├── tests/
│   └── UniversityPortal.Tests/UnitTest1.cs   # ⚠️ placeholder — chưa có test nghiệp vụ thật
│
├── frontend/                        # React 19 + Vite + TypeScript + Ant Design (KHÔNG dùng Tailwind)
│   ├── src/
│   │   ├── api/            # client.ts (axios instance), auth.ts, crud.ts (generic CRUD factory), modules.ts
│   │   ├── components/     # shared UI
│   │   ├── constants/
│   │   ├── hooks/
│   │   ├── pages/          # 21 trang route-level — xem bảng bên dưới
│   │   ├── store/          # Zustand
│   │   └── types/
│   ├── Dockerfile, nginx.conf
│
├── docker-compose.yml     # 2 services: frontend (3100→80), api (8080) — SQL Server KHÔNG chạy trong compose (dùng instance có sẵn qua CONNECTION_STRING)
├── Dockerfile             # build API
├── .env.example
└── README.md
```

---

## 🏗️ Dependency Rule (Clean Architecture) — giữ nguyên như thiết kế ban đầu

```
API  ──→  Application  ──→  Domain
           ↑
    Infrastructure
```

---

## 🧱 Base Classes (khớp với code hiện tại)

### AuditableEntity
```csharp
public abstract class AuditableEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### AppDbContext.SaveChangesAsync — tự set CreatedAt/UpdatedAt
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

### IUnitOfWork — tập hợp toàn bộ repository theo module
```csharp
public interface IUnitOfWork : IDisposable
{
    ITaiKhoanRepository TaiKhoans { get; }
    ISinhVienRepository SinhViens { get; }
    IGiaoVienRepository GiaoViens { get; }
    ILopHocPhanRepository LopHocPhans { get; }
    IDanhSachLopHPRepository DanhSachLopHPs { get; }
    IHocBaRepository HocBas { get; }
    INamHocRepository NamHocs { get; }
    IHocKyRepository HocKys { get; }
    ITuanHocRepository TuanHocs { get; }
    IThoiKhoaBieuRepository ThoiKhoaBieus { get; }
    INganhHocRepository NganhHocs { get; }
    IPhongBanRepository PhongBans { get; }
    IChuongTrinhDTRepository ChuongTrinhDTs { get; }
    IMonHocRepository MonHocs { get; }
    IChiTietCTDTRepository ChiTietCTDTs { get; }
    ILopSinhHoatRepository LopSinhHoats { get; }
    IThongBaoRepository ThongBaos { get; }
    IThongBaoDaDocRepository ThongBaoDaDocs { get; }
    IHocPhiRepository HocPhis { get; }
    IYeuCauHanhChinhRepository YeuCauHanhChinhs { get; }
    IYeuCauSuaDiemRepository YeuCauSuaDiems { get; }
    INoiDungTinhRepository NoiDungTinhs { get; }
    Task<int> CommitAsync();
}
```
> Lưu ý: `HocBa`, `DiemRenLuyen`, `KetQuaAnhVanDauVao`, `DanhSachThiLai`, `BienBanSHCN`, `ChiTietCongViec`, `ChiTietVangSHCN`, `DatPhongThucHanh`, `KhaoSatYKien`, `DienDanGiaoVien` **đã có Entity + EF Configuration + DbSet** (schema tồn tại trong DB) nhưng **chưa có Repository/Service/Controller/trang UI riêng** — xem mục Tình trạng hiện tại.

---

## 📋 Controllers hiện có (21)

| Controller | Module |
|---|---|
| `AuthController` | Login, refresh token, logout |
| `TaiKhoanController` | Quản lý tài khoản |
| `SinhVienController` | Sinh viên |
| `GiaoVienController` | Giáo viên |
| `PhongBanController` | Phòng ban / khoa |
| `LopSinhHoatController` | Lớp sinh hoạt |
| `NganhHocController` | Ngành học |
| `ChuongTrinhDTController` | Chương trình đào tạo (CTDT) |
| `ChiTietCTDTController` | Chi tiết CTDT (môn học theo ngành/khoá/học kỳ) |
| `MonHocController` | Môn học |
| `NamHocController` | Năm học |
| `HocKyController` | Học kỳ |
| `TuanHocController` | Tuần học |
| `LopHocPhanController` | Lớp học phần — mở lớp, khoá/mở bảng điểm |
| `DanhSachLopHPController` | Đăng ký học phần, duyệt, **nhập điểm** (`diem_qt1/qt2/thi` → `diem_tong_ket` = 15%+15%+70%) |
| `ThoiKhoaBieuController` | Thời khoá biểu |
| `HocPhiController` | Học phí |
| `ThongBaoController` | Thông báo |
| `YeuCauHanhChinhController` | Yêu cầu hành chính (giấy xác nhận, biên nhận, phản hồi) |
| `YeuCauSuaDiemController` | Yêu cầu mở lại bảng điểm đã khoá |
| `NoiDungTinhController` | Nội dung tĩnh — khu vực thư viện/học vụ (public + `NoiDungTinhAdminPage` để quản trị) |

---

## 🖥️ Frontend — 21 trang đã build (`frontend/src/pages`)

`LoginPage`, `DashboardPage`, `TaiKhoanPage`, `SinhVienPage`, `GiaoVienPage`, `LopSinhHoatPage`, `NganhHocPage`, `ChuongTrinhDTPage`, `ChiTietCTDTPage`, `MonHocPage`, `NamHocPage`, `HocKyPage`, `LopHocPhanPage`, `BangDiemPage`, `ThoiKhoaBieuPage`, `HocPhiPage`, `ThongBaoPage`, `YeuCauHanhChinhPage`, `YeuCauSuaDiemPage`, `KhuVucNoiDungPage`, `NoiDungTinhAdminPage`, `HoSoPage`.

**Stack frontend thực tế** (khác với kế hoạch ban đầu):
- React 19 + Vite + TypeScript
- **Ant Design** (`antd`, `@ant-design/icons`) — không dùng TailwindCSS
- `@tanstack/react-query` cho data fetching
- `zustand` cho state (auth store)
- `axios` (client.ts) + `dayjs`
- `react-router-dom` v7
- **Không có** react-hook-form / zod / react-hot-toast / lucide-react như kế hoạch cũ (Ant Design tự cung cấp Form, message/notification, icon).

---

## 🔐 Auth & Middleware (đã triển khai)

- JWT access token + refresh token (lưu `RefreshToken`/`RefreshTokenExpiry` trên `TaiKhoan`, xoay vòng mỗi lần refresh)
- `AuthController`: `POST /api/auth/login`, `POST /api/auth/refresh`, `POST /api/auth/logout`
- `GlobalExceptionMiddleware` bắt `NotFoundException`, `BadRequestException`, `ForbiddenException` → response chuẩn hoá
- CORS: `AllowAnyOrigin/Method/Header` (chưa siết theo domain cụ thể)
- Swagger + Bearer auth button + XML comments (`IncludeXmlComments`)
- `GET /health` → healthcheck endpoint
- Auto-apply EF Core migrations lúc `Program.cs` khởi động (`db.Database.MigrateAsync()`), cảnh báo log nếu bảng `vai_tro` trống
- Static file serving `/uploads` cho avatar/tài liệu đính kèm (qua `IFileService`)
- **Chưa có**: rate limiting (`AspNetCoreRateLimit` không được cài)

---

## 🐳 Docker / Deployment thực tế

Khác với kế hoạch ban đầu (không có MySQL container, không dùng EC2 trực tiếp trong compose):

```yaml
# docker-compose.yml (rút gọn)
services:
  frontend:
    build: { context: ./frontend, dockerfile: Dockerfile }
    ports: ["3100:80"]
    depends_on: [api]

  api:
    build: { context: ., dockerfile: Dockerfile }
    environment:
      ConnectionStrings__Default: "${CONNECTION_STRING}"   # SQL Server có sẵn, ngoài compose
      JWT__Issuer: UniversityPortal
      JWT__Audience: UniversityPortal
      JWT__ExpiryMinutes: "60"
      JWT__RefreshTokenExpiryDays: "7"
      JWT__Secret: "..."
    ports: ["8080:8080"]
    volumes: ["uploads_data:/app/uploads"]

volumes:
  uploads_data:
```

- **CSDL**: SQL Server có sẵn (không chạy trong Docker Compose) — connection string trỏ ra ngoài qua `.env` (`CONNECTION_STRING`).
- **Build & publish**: image build từ Dockerfile gốc, publish lên **GHCR** (`ghcr.io/gnuhx/university-portal-api` / `-frontend`) — xem `docs/done/deploy-docker-registrry.md`.
- **Chạy production**: kéo image từ GHCR về EC2 thay vì build lại từ source — xem `docs/done/deploy-ec2.md`.
- Không có pipeline GitHub Actions tự động deploy trong repo hiện tại (chỉ có tài liệu thủ công).

---

## ✅ Tình trạng hiện tại

### Đã hoàn thiện end-to-end (Entity → Repository → Service → Controller → DTO/Validator → Trang UI)
Tài khoản, vai trò, phòng ban, sinh viên, giáo viên, lớp sinh hoạt, ngành học, chương trình đào tạo, môn học, chi tiết CTDT, năm học/học kỳ/tuần học, lớp học phần (mở/đóng, khoá bảng điểm), đăng ký + nhập điểm học phần, thời khoá biểu, học phí, thông báo, yêu cầu hành chính (giấy xác nhận/biên nhận/phản hồi), yêu cầu sửa điểm, nội dung tĩnh (khu vực thư viện/học vụ). Chi tiết từng feature: xem `docs/features/01`…`22` (đều đánh dấu `done` trừ **`14_todo_xoa_ctdt_gop_mon_hoc.md`** — còn TODO).

### Có schema (Entity + EF Configuration + DbSet) nhưng CHƯA có Service/Controller/UI riêng
- `DanhSachThiLai` (đăng ký thi lại)
- `HocBa` (bảng tổng hợp GPA/tín chỉ tích luỹ)
- `DiemRenLuyen`
- `KetQuaAnhVanDauVao`
- `ThongBaoDaDoc`, `BinhLuanThongBao`
- `BienBanSHCN`, `ChiTietCongViec`, `ChiTietVangSHCN`
- `DatPhongThucHanh`
- `KhaoSatYKien`
- `DienDanGiaoVien`

> Đây là phần chênh lệch chính so với kế hoạch ban đầu (Tuần 3–5 cũ) — cân nhắc đưa vào backlog nếu đồ án cần mở rộng, hoặc ghi rõ là "giới hạn phạm vi" trong báo cáo.

### Test
- `tests/UniversityPortal.Tests` chỉ có 1 file `UnitTest1.cs` rỗng (placeholder do template tạo ra) — **chưa có unit test nghiệp vụ thật** (khác với checklist "≥10 test case Moq + xUnit" trong kế hoạch cũ).

### Khác biệt hạ tầng/công nghệ so với kế hoạch ban đầu
| Mục | Kế hoạch ban đầu | Thực tế |
|---|---|---|
| CSDL | MySQL (Pomelo) | **SQL Server** (`Microsoft.EntityFrameworkCore.SqlServer`) |
| UI framework | TailwindCSS + component tự viết | **Ant Design** (`antd`) |
| Form/validation FE | React Hook Form + Zod | Ant Design `Form` built-in |
| Toast/notification | react-hot-toast | Ant Design `message`/`notification` |
| Icon | lucide-react | `@ant-design/icons` |
| Rate limiting | AspNetCoreRateLimit | Chưa triển khai |
| Deploy | EC2 build trực tiếp từ source + GitHub Actions | Build → push GHCR → pull image trên EC2 (thủ công) |
| DB trong Docker Compose | Có service `mysql` | Không — dùng SQL Server có sẵn ngoài compose |

---

## 📦 Packages thực tế

### NuGet
```
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
AutoMapper
FluentValidation.DependencyInjectionExtensions
Microsoft.AspNetCore.Authentication.JwtBearer
BCrypt.Net-Next
Swashbuckle.AspNetCore
```

### NPM (frontend)
```
react react-dom react-router-dom
antd @ant-design/icons
@tanstack/react-query
zustand
axios dayjs
vite typescript
```

# 🎓 University Portal — Project Plan

**Stack:** C# ASP.NET Core · MySQL · ReactJS · Docker Compose · AWS EC2
**Patterns:** Clean Architecture · Repository Pattern · Unit of Work · Service Layer · Entity Framework Core (Code First)

---

## 📁 Cấu trúc Solution

```
UniversityPortal/
├── src/
│   ├── UniversityPortal.Domain/
│   │   ├── Entities/
│   │   │   ├── Common/
│   │   │   │   └── AuditableEntity.cs        # base class: Id, CreatedAt, UpdatedAt
│   │   │   ├── TaiKhoan.cs
│   │   │   ├── VaiTro.cs
│   │   │   ├── PhongBan.cs
│   │   │   ├── GiaoVien.cs
│   │   │   ├── SinhVien.cs
│   │   │   ├── KetQuaAnhVanDauVao.cs
│   │   │   ├── LopSinhHoat.cs
│   │   │   ├── NganhHoc.cs
│   │   │   ├── ChuongTrinhDT.cs
│   │   │   ├── MonHoc.cs
│   │   │   ├── ChiTietCTDT.cs
│   │   │   ├── NamHoc.cs
│   │   │   ├── HocKy.cs
│   │   │   ├── TuanHoc.cs
│   │   │   ├── LopHocPhan.cs
│   │   │   ├── DanhSachLopHP.cs
│   │   │   ├── DanhSachThiLai.cs
│   │   │   ├── HocBa.cs
│   │   │   ├── DiemRenLuyen.cs
│   │   │   ├── ThoiKhoaBieu.cs
│   │   │   ├── HocPhi.cs
│   │   │   ├── ThongBao.cs
│   │   │   ├── ThongBaoDaDoc.cs
│   │   │   ├── BinhLuanThongBao.cs
│   │   │   ├── BienBanSHCN.cs
│   │   │   ├── ChiTietCongViec.cs
│   │   │   ├── ChiTietVangSHCN.cs
│   │   │   ├── YeuCauHanhChinh.cs
│   │   │   ├── YeuCauSuaDiem.cs
│   │   │   ├── DatPhongThucHanh.cs
│   │   │   ├── KhaoSatYKien.cs
│   │   │   └── DienDanGiaoVien.cs
│   │   ├── Enums/
│   │   │   ├── VaiTroEnum.cs              # Admin, GiaoVien, SinhVien, GiaoVu
│   │   │   ├── LoaiDangKyEnum.cs          # HocChinh, HocLai, HocGhep, CaiThien
│   │   │   ├── TrangThaiDuyetEnum.cs      # ChoDuyet, DaDuyet, TuChoi
│   │   │   ├── TrangThaiDongTienEnum.cs   # ChuaDong, DaDong
│   │   │   ├── LoaiThongBaoEnum.cs        # HocVu, DoanHoi, HocPhi
│   │   │   └── MucDoThongBaoEnum.cs       # BinhThuong, QuanTrong, KhanCap
│   │   └── Exceptions/
│   │       ├── NotFoundException.cs
│   │       ├── BadRequestException.cs
│   │       └── ForbiddenException.cs
│   │
│   ├── UniversityPortal.Application/
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   │   ├── IRepository.cs             # Generic CRUD interface
│   │   │   │   ├── ITaiKhoanRepository.cs
│   │   │   │   ├── ISinhVienRepository.cs
│   │   │   │   ├── IGiaoVienRepository.cs
│   │   │   │   ├── ILopHocPhanRepository.cs
│   │   │   │   ├── IDanhSachLopHPRepository.cs
│   │   │   │   └── ...                        # 1 interface / entity có query riêng
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── Services/
│   │   │       ├── IAuthService.cs
│   │   │       ├── ITaiKhoanService.cs
│   │   │       ├── ISinhVienService.cs
│   │   │       ├── IGiaoVienService.cs
│   │   │       ├── ILopHocPhanService.cs
│   │   │       ├── IDiemService.cs
│   │   │       ├── IHocPhiService.cs
│   │   │       ├── IThongBaoService.cs
│   │   │       ├── IBienBanSHCNService.cs
│   │   │       ├── IYeuCauService.cs
│   │   │       └── IThiLaiService.cs
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   │   ├── LoginRequestDto.cs
│   │   │   │   └── LoginResponseDto.cs        # AccessToken, RefreshToken, UserInfo
│   │   │   ├── TaiKhoan/
│   │   │   ├── SinhVien/
│   │   │   ├── LopHocPhan/
│   │   │   ├── Diem/
│   │   │   ├── ThongBao/
│   │   │   └── Common/
│   │   │       ├── PagedResultDto.cs          # Data, Total, Page, PageSize
│   │   │       └── ApiResponseDto.cs          # Success, Data, Message, Errors
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs              # AutoMapper profiles
│   │   └── Validators/                        # FluentValidation
│   │       ├── LoginRequestValidator.cs
│   │       └── ...
│   │
│   ├── UniversityPortal.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/                # IEntityTypeConfiguration<T> mỗi entity
│   │   │   │   ├── TaiKhoanConfiguration.cs
│   │   │   │   └── ...
│   │   │   └── Migrations/
│   │   ├── Repositories/
│   │   │   ├── BaseRepository.cs              # impl IRepository<T>
│   │   │   ├── TaiKhoanRepository.cs
│   │   │   ├── SinhVienRepository.cs
│   │   │   └── ...
│   │   ├── UnitOfWork.cs                      # impl IUnitOfWork
│   │   └── DependencyInjection.cs             # AddInfrastructure(services)
│   │
│   ├── UniversityPortal.Application/
│   │   └── Services/                          # impl IService
│   │       ├── AuthService.cs
│   │       ├── SinhVienService.cs
│   │       └── ...
│   │   └── DependencyInjection.cs             # AddApplication(services)
│   │
│   └── UniversityPortal.API/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── TaiKhoanController.cs
│       │   ├── SinhVienController.cs
│       │   ├── LopHocPhanController.cs
│       │   ├── DiemController.cs
│       │   ├── ThoiKhoaBieuController.cs
│       │   ├── HocPhiController.cs
│       │   ├── ThongBaoController.cs
│       │   ├── BienBanSHCNController.cs
│       │   ├── YeuCauController.cs
│       │   └── ThiLaiController.cs
│       ├── Middleware/
│       │   └── GlobalExceptionMiddleware.cs
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs # JWT, Swagger, CORS config
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Program.cs
│       └── Dockerfile
│
├── tests/
│   └── UniversityPortal.Tests/
│       ├── Services/
│       │   ├── AuthServiceTests.cs
│       │   ├── DiemServiceTests.cs
│       │   └── ...
│       └── Repositories/
│
├── frontend/
│   └── university-portal-ui/
│       ├── src/
│       │   ├── api/                   # axios instances + endpoint functions
│       │   ├── components/            # shared UI components
│       │   ├── features/              # feature-based folders (auth, diem, lophocphan...)
│       │   ├── hooks/                 # custom hooks
│       │   ├── layouts/               # AppLayout, AuthLayout
│       │   ├── pages/                 # route-level pages
│       │   ├── store/                 # Zustand stores
│       │   ├── types/                 # TypeScript types/interfaces
│       │   └── utils/
│       ├── Dockerfile
│       └── nginx.conf
│
├── docker-compose.yml
├── docker-compose.override.yml
├── .env.example
└── README.md
```

---

## 🏗️ Dependency Rule (Clean Architecture)

```
API  ──→  Application  ──→  Domain
           ↑
    Infrastructure
```

- `Domain` không reference project nào
- `Application` chỉ reference `Domain`
- `Infrastructure` reference `Application` + `Domain`
- `API` reference `Application` + `Infrastructure` (chỉ để DI registration)
- `Tests` reference `Application` + `Infrastructure`

---

## 🧱 Base Classes quan trọng

### AuditableEntity (Domain/Entities/Common)
```csharp
public abstract class AuditableEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```
> Tất cả entities đều kế thừa class này. EF tự set `CreatedAt`/`UpdatedAt` qua `SaveChangesAsync` override trong `AppDbContext`.

### IRepository (Application/Interfaces/Repositories)
```csharp
public interface IRepository<T> where T : AuditableEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

### IUnitOfWork (Application/Interfaces)
```csharp
public interface IUnitOfWork : IDisposable
{
    ITaiKhoanRepository TaiKhoans { get; }
    ISinhVienRepository SinhViens { get; }
    IGiaoVienRepository GiaoViens { get; }
    ILopHocPhanRepository LopHocPhans { get; }
    IDanhSachLopHPRepository DanhSachLopHPs { get; }
    // ... tất cả repositories
    Task<int> CommitAsync();
}
```

### AppDbContext override SaveChangesAsync
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

---

## 🗓️ PHASE 1 — BACKEND (Tuần 1–6)

### Tuần 1 — Setup & Auth
- [ ] Tạo solution với 5 projects, setup project references đúng dependency rule
- [ ] Cài NuGet: `Pomelo.EntityFrameworkCore.MySql`, `AutoMapper`, `FluentValidation`, `JwtBearer`, `BCrypt.Net-Next`, `Swashbuckle`
- [ ] Tạo toàn bộ Entities kế thừa `AuditableEntity`, thêm Enums
- [ ] Setup `AppDbContext` + `IEntityTypeConfiguration` cho từng entity
- [ ] Implement `BaseRepository<T>` + `UnitOfWork`
- [ ] `AddInfrastructure()` + `AddApplication()` extension methods
- [ ] Auth: `POST /api/auth/login` → JWT, `POST /api/auth/refresh`
- [ ] `GlobalExceptionMiddleware` bắt `NotFoundException`, `BadRequestException`, `ForbiddenException`
- [ ] Chuẩn hoá response: `ApiResponseDto<T>`
- [ ] Migration đầu tiên + seed vai_tro, phong_ban, tai_khoan admin

### Tuần 2 — Tài khoản & Danh mục
- [ ] CRUD `TaiKhoan`, `GiaoVien`, `SinhVien` (có phân trang, filter)
- [ ] CRUD `NganhHoc`, `ChuongTrinhDT`, `MonHoc`, `ChiTietCTDT`
- [ ] CRUD `LopSinhHoat` — xử lý `thu_ky_id` nullable (insert trước, update sau)
- [ ] Upload avatar — lưu vào volume `/uploads`
- [ ] Phân quyền `[Authorize(Roles = "Admin,GiaoVu")]` theo từng endpoint
- [ ] FluentValidation cho toàn bộ request DTO của module này
- [ ] Seed: nganh, CTDT, mon hoc, lop sinh hoat, giao vien, sinh vien mẫu

### Tuần 3 — Học vụ & Lớp học phần
- [ ] CRUD `NamHoc`, `HocKy`, `TuanHoc`
- [ ] CRUD `LopHocPhan` (mở/đóng lớp, khoá bảng điểm)
- [ ] `DanhSachLopHP`: đăng ký, duyệt, tính tiền học lại/cải thiện
- [ ] `DanhSachThiLai`: đăng ký thi lại, duyệt, trạng thái đóng tiền
- [ ] `ThoiKhoaBieu`: xếp TKB theo tuần, check trùng phòng/tiết
- [ ] Seed: năm học, học kỳ, tuần học, lớp học phần, TKB

### Tuần 4 — Điểm & Học bạ
- [ ] Nhập điểm `diem_qt1`, `diem_qt2`, `diem_thi` → tính `diem_tong_ket` theo công thức (30% + 70%)
- [ ] Khoá bảng điểm (`khoa_bang_diem = true`) — chặn edit sau khi khoá
- [ ] `YeuCauSuaDiem`: GV tạo yêu cầu mở lại, Admin duyệt
- [ ] `HocBa`: tổng hợp GPA tích luỹ, tín chỉ tích luỹ
- [ ] `DiemRenLuyen`: nhập/xem theo học kỳ
- [ ] `HocPhi`: tạo theo học kỳ, cập nhật trạng thái
- [ ] `KetQuaAnhVanDauVao`: nhập/xem
- [ ] Seed: điểm các môn, học bạ, học phí

### Tuần 5 — Thông báo, SHCN & Hành chính
- [ ] `ThongBao`: tạo, gửi theo lớp hoặc toàn trường (`lop_nhan_id` nullable)
- [ ] `ThongBaoDaDoc`: đánh dấu đã đọc, đếm unread
- [ ] `BinhLuanThongBao`: thêm/xem comment
- [ ] `BienBanSHCN` + `ChiTietCongViec` + `ChiTietVangSHCN`
- [ ] `YeuCauHanhChinh`: tạo, upload file, duyệt/từ chối
- [ ] `DatPhongThucHanh`: đặt phòng, check conflict lịch
- [ ] `KhaoSatYKien`: SV đánh giá sau khi khoá bảng điểm
- [ ] `DienDanGiaoVien`: GV gửi thông tin lớp
- [ ] Seed: thông báo, biên bản SHCN, yêu cầu hành chính mẫu

### Tuần 6 — Polish Backend
- [ ] Unit Test: `AuthServiceTests`, `DiemServiceTests`, `DanhSachLopHPServiceTests` (dùng Moq + xUnit)
- [ ] Swagger: group theo module, JWT auth button, XML comments
- [ ] Pagination + Filter + Sort chuẩn cho tất cả list API
- [ ] Rate limiting (`AspNetCoreRateLimit`)
- [ ] CORS config
- [ ] `docker-compose.yml` chạy backend + mysql
- [ ] Health check endpoint `/health`

---

## 🗓️ PHASE 2 — FRONTEND (Tuần 7–10)

### Tuần 7 — Setup & Auth
- [ ] Khởi tạo Vite + React + TypeScript
- [ ] Cài: TailwindCSS, React Router v6, Axios, TanStack Query, Zustand, React Hook Form + Zod, react-hot-toast, lucide-react
- [ ] Axios instance + interceptor tự động refresh token
- [ ] Zustand store: `useAuthStore` (user, token, role)
- [ ] Protected Route component theo role
- [ ] Layout: Sidebar (collapse), Topbar (avatar, unread badge), Breadcrumb
- [ ] Trang Login

### Tuần 8 — Danh mục & Tài khoản
- [ ] Shared components: `DataTable`, `Modal`, `ConfirmDialog`, `Badge`, `Pagination`, `FileUpload`
- [ ] Trang quản lý Tài khoản (CRUD, đổi mật khẩu, upload avatar)
- [ ] Trang Sinh viên, Giáo viên
- [ ] Trang Lớp sinh hoạt
- [ ] Trang Ngành, CTDT, Môn học, Chi tiết CTDT

### Tuần 9 — Học vụ & Điểm
- [ ] Trang Năm học / Học kỳ / Tuần học
- [ ] Trang Lớp học phần: danh sách, đăng ký, duyệt
- [ ] Trang nhập điểm (GV): inline edit table, khoá bảng điểm
- [ ] Trang xem điểm (SV): bảng điểm cá nhân, GPA, học bạ
- [ ] Trang Thi lại: đăng ký, trạng thái đóng tiền
- [ ] Trang TKB: calendar view theo tuần
- [ ] Trang Học phí: xem số tiền, trạng thái

### Tuần 10 — Thông báo, SHCN & Hành chính
- [ ] Trang Thông báo: list, đọc, comment, badge unread realtime
- [ ] Trang Biên bản SHCN: tạo biên bản, điểm danh vắng
- [ ] Trang Yêu cầu hành chính: tạo, upload file, timeline trạng thái
- [ ] Trang Đặt phòng thực hành
- [ ] Trang Khảo sát ý kiến
- [ ] Dashboard theo 3 role: Admin / GV / SV (widget thống kê)
- [ ] Responsive mobile, loading skeleton, error boundary
- [ ] Build production + Docker image nginx

---

## 🗓️ PHASE 3 — DEPLOYMENT (Tuần 11)

### Docker Compose
```yaml
# docker-compose.yml
services:
  mysql:
    image: mysql:8.0
    restart: always
    environment:
      MYSQL_DATABASE: ${DB_NAME}
      MYSQL_ROOT_PASSWORD: ${DB_PASSWORD}
    volumes:
      - mysql_data:/var/lib/mysql
      - ./init.sql:/docker-entrypoint-initdb.d/init.sql
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      interval: 10s
      retries: 5

  backend:
    build: ./src/UniversityPortal.API
    restart: always
    environment:
      ConnectionStrings__Default: "Server=mysql;Port=3306;Database=${DB_NAME};Uid=root;Pwd=${DB_PASSWORD};"
      JWT__Secret: ${JWT_SECRET}
      JWT__ExpiryMinutes: 60
      ASPNETCORE_ENVIRONMENT: Production
    depends_on:
      mysql:
        condition: service_healthy
    ports:
      - "5000:5000"
    volumes:
      - uploads_data:/app/uploads

  frontend:
    build: ./frontend/university-portal-ui
    restart: always
    ports:
      - "80:80"
      - "443:443"
    depends_on:
      - backend
    volumes:
      - ./nginx.conf:/etc/nginx/conf.d/default.conf
      - certbot_data:/etc/letsencrypt

volumes:
  mysql_data:
  uploads_data:
  certbot_data:
```

### nginx.conf
```nginx
server {
    listen 80;
    server_name yourdomain.com;

    location / {
        root /usr/share/nginx/html;
        index index.html;
        try_files $uri /index.html;   # SPA fallback
    }

    location /api/ {
        proxy_pass http://backend:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

### AWS EC2 Setup
```bash
# 1. Cài Docker
sudo apt update && sudo apt install -y docker.io docker-compose
sudo usermod -aG docker ubuntu

# 2. Clone repo
git clone https://github.com/yourrepo/university-portal.git
cd university-portal

# 3. Tạo .env từ example
cp .env.example .env
nano .env   # điền DB_PASSWORD, JWT_SECRET, domain

# 4. Chạy
docker-compose up -d --build

# 5. SSL (nếu có domain)
sudo apt install certbot
sudo certbot certonly --standalone -d yourdomain.com
```

### GitHub Actions CI/CD (`.github/workflows/deploy.yml`)
```yaml
on:
  push:
    branches: [main]
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Deploy to EC2
        uses: appleboy/ssh-action@v1
        with:
          host: ${{ secrets.EC2_HOST }}
          username: ubuntu
          key: ${{ secrets.EC2_SSH_KEY }}
          script: |
            cd university-portal
            git pull origin main
            docker-compose up -d --build
```

---

## 🌱 Sample Init Data (SQL)

```sql
-- =============================================
-- 1. VAI TRO & PHONG BAN
-- =============================================
INSERT INTO vai_tro (id, ten_vai_tro) VALUES
(1, 'Admin'),
(2, 'Giao vien'),
(3, 'Sinh vien'),
(4, 'Giao vu');

INSERT INTO phong_ban (id, ten_phong_ban) VALUES
(1, 'Phong Dao tao'),
(2, 'Phong Cong tac Sinh vien'),
(3, 'Khoa Cong nghe Thong tin');

-- =============================================
-- 2. TAI KHOAN
-- =============================================
-- mat_khau: bcrypt cua "Password@123"
INSERT INTO tai_khoan (id, ten_dang_nhap, mat_khau, vai_tro_id, phong_ban_id, ho_ten, email, trang_thai, created_at, updated_at) VALUES
(1, 'admin', '$2a$12$xxxHASHxxx', 1, 1, 'Nguyen Van Admin',  'admin@uni.edu.vn',      true, NOW(), NOW()),
(2, 'gv001', '$2a$12$xxxHASHxxx', 2, 3, 'Tran Thi Lan',      'lan.tt@uni.edu.vn',     true, NOW(), NOW()),
(3, 'gv002', '$2a$12$xxxHASHxxx', 2, 3, 'Le Van Minh',       'minh.lv@uni.edu.vn',    true, NOW(), NOW()),
(4, 'sv001', '$2a$12$xxxHASHxxx', 3, NULL,'Nguyen Thi Hoa',  'hoa.nt@sv.uni.edu.vn',  true, NOW(), NOW()),
(5, 'sv002', '$2a$12$xxxHASHxxx', 3, NULL,'Pham Van Duc',    'duc.pv@sv.uni.edu.vn',  true, NOW(), NOW()),
(6, 'sv003', '$2a$12$xxxHASHxxx', 3, NULL,'Vo Thi Mai',      'mai.vt@sv.uni.edu.vn',  true, NOW(), NOW());

-- =============================================
-- 3. GIAO VIEN & SINH VIEN
-- =============================================
INSERT INTO giao_vien (id, tai_khoan_id, ma_gv, created_at, updated_at) VALUES
(1, 2, 'GV2021001', NOW(), NOW()),
(2, 3, 'GV2021002', NOW(), NOW());

-- Sinh vien insert truoc, lop_id = NULL (xu ly circular ref)
INSERT INTO sinh_vien (id, tai_khoan_id, mssv, lop_id, created_at, updated_at) VALUES
(1, 4, '2100001', NULL, NOW(), NOW()),
(2, 5, '2100002', NULL, NOW(), NOW()),
(3, 6, '2100003', NULL, NOW(), NOW());

-- =============================================
-- 4. NGANH & CHUONG TRINH DAO TAO
-- =============================================
INSERT INTO nganh_hoc (id, ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at) VALUES
(1, 'CNTT', 'Cong nghe Thong tin', NULL, NOW(), NOW()),
(2, 'KTPM', 'Ky thuat Phan mem',   1,    NOW(), NOW()),
(3, 'HTTT', 'He thong Thong tin',  1,    NOW(), NOW());

INSERT INTO chuong_trinh_dt (id, ma_ctdt, nganh_id, khoa_hoc, created_at, updated_at) VALUES
(1, 'KTPM2021', 2, '2021-2025', NOW(), NOW()),
(2, 'HTTT2021', 3, '2021-2025', NOW(), NOW());

-- =============================================
-- 5. LOP SINH HOAT (thu_ky_id = NULL truoc)
-- =============================================
INSERT INTO lop_sinh_hoat (id, ma_lop, gvcn_id, thu_ky_id, chuong_trinh_dt_id, created_at, updated_at) VALUES
(1, 'KTPM2021A', 1, NULL, 1, NOW(), NOW()),
(2, 'HTTT2021A', 2, NULL, 2, NOW(), NOW());

-- Gan lop cho sinh vien
UPDATE sinh_vien SET lop_id = 1, updated_at = NOW() WHERE id IN (1, 2);
UPDATE sinh_vien SET lop_id = 2, updated_at = NOW() WHERE id = 3;

-- Gan thu ky sau khi sinh vien da co lop_id
UPDATE lop_sinh_hoat SET thu_ky_id = 1, updated_at = NOW() WHERE id = 1;
UPDATE lop_sinh_hoat SET thu_ky_id = 3, updated_at = NOW() WHERE id = 2;

-- =============================================
-- 6. MON HOC & CHI TIET CTDT
-- =============================================
INSERT INTO mon_hoc (id, ma_mon, ten_mon, created_at, updated_at) VALUES
(1, 'INT1001', 'Nhap mon Lap trinh', NOW(), NOW()),
(2, 'INT1002', 'Co so Du lieu',      NOW(), NOW()),
(3, 'INT2001', 'Lap trinh Web',      NOW(), NOW()),
(4, 'INT2002', 'Ky thuat Phan mem',  NOW(), NOW()),
(5, 'INT3001', 'Do an Tot nghiep',   NOW(), NOW());

INSERT INTO nam_hoc (id, ten_nam_hoc, created_at, updated_at) VALUES
(1, '2024-2025', NOW(), NOW());

INSERT INTO hoc_ky (id, ten_hoc_ky, nam_hoc_id, ngay_bat_dau, created_at, updated_at) VALUES
(1, 'Hoc ky 1 - 2024-2025', 1, '2024-09-02', NOW(), NOW()),
(2, 'Hoc ky 2 - 2024-2025', 1, '2025-02-03', NOW(), NOW());

INSERT INTO tuan_hoc (id, nam_hoc_id, ma_tuan, so_thu_tu_tuan, ngay_bat_dau, ngay_ket_thuc, created_at, updated_at) VALUES
(1, 1, 'T01_2425', 1, '2024-09-02', '2024-09-08', NOW(), NOW()),
(2, 1, 'T02_2425', 2, '2024-09-09', '2024-09-15', NOW(), NOW()),
(3, 1, 'T03_2425', 3, '2024-09-16', '2024-09-22', NOW(), NOW());

INSERT INTO chi_tiet_ctdt (id, ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at) VALUES
(1, 1, 1, 1, 3, true,  NOW(), NOW()),
(2, 1, 2, 1, 3, true,  NOW(), NOW()),
(3, 1, 3, 2, 3, true,  NOW(), NOW());

-- =============================================
-- 7. LOP HOC PHAN & DANH SACH
-- =============================================
INSERT INTO lop_hoc_phan (id, chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at) VALUES
(1, 1, 1, 1, 'INT1001_01', false, false, NOW(), NOW()),
(2, 2, 1, 2, 'INT1002_01', false, false, NOW(), NOW());

-- diem_tong_ket = qt1*0.15 + qt2*0.15 + thi*0.7
INSERT INTO danh_sach_lop_hp (id, sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet, nguoi_duyet_id, diem_qt1, diem_qt2, diem_thi, diem_tong_ket, created_at, updated_at) VALUES
(1, 1, 1, 'Hoc chinh', 'Da duyet', 1, 8.0, 7.5, 8.5, 8.3,  NOW(), NOW()),
(2, 2, 1, 'Hoc chinh', 'Da duyet', 1, 6.0, 7.0, 5.5, 6.0,  NOW(), NOW()),
(3, 3, 1, 'Hoc chinh', 'Da duyet', 1, 4.5, 5.0, 3.0, 3.6,  NOW(), NOW()),  -- rot mon
(4, 1, 2, 'Hoc chinh', 'Da duyet', 1, 9.0, 8.5, 9.0, 8.9,  NOW(), NOW()),
(5, 2, 2, 'Hoc chinh', 'Da duyet', 1, 7.0, 6.5, 7.5, 7.3,  NOW(), NOW());

-- sv id=3 rot mon -> dang ky thi lai
INSERT INTO danh_sach_thi_lai (id, sinh_vien_id, lop_hp_id, diem_thi_lai, so_tien_phai_dong, trang_thai_dong_tien, nguoi_duyet_id, created_at, updated_at) VALUES
(1, 3, 1, NULL, 150000, 'Chua dong', NULL, NOW(), NOW());

-- =============================================
-- 8. THOI KHOA BIEU
-- =============================================
INSERT INTO thoi_khoa_bieu (id, lop_hp_id, tuan_hoc_id, thu, tiet_bat_dau, tiet_ket_thuc, phong_hoc, created_at, updated_at) VALUES
(1, 1, 1, 2, 1, 3, 'A101', NOW(), NOW()),
(2, 1, 2, 2, 1, 3, 'A101', NOW(), NOW()),
(3, 2, 1, 4, 4, 6, 'B203', NOW(), NOW()),
(4, 2, 2, 4, 4, 6, 'B203', NOW(), NOW());

-- =============================================
-- 9. HOC PHI
-- =============================================
INSERT INTO hoc_phi (id, sinh_vien_id, hoc_ky_id, so_tien, trang_thai_dong, created_at, updated_at) VALUES
(1, 1, 1, 8500000, 'Da dong',   NOW(), NOW()),
(2, 2, 1, 8500000, 'Da dong',   NOW(), NOW()),
(3, 3, 1, 8500000, 'Chua dong', NOW(), NOW());

-- =============================================
-- 10. THONG BAO
-- =============================================
INSERT INTO thong_bao (id, loai_thong_bao, muc_do, tieu_de, noi_dung, nguoi_tao_id, lop_nhan_id, ngay_tao, created_at, updated_at) VALUES
(1, 'Hoc vu',  'Quan trong', 'Lich thi cuoi ky HK1 2024-2025',
   'Sinh vien xem lich thi tren cong thong tin. Mang theo the sinh vien khi thi.',
   1, 1, NOW(), NOW(), NOW()),
(2, 'Hoc phi', 'Khan cap', 'Thong bao dong hoc phi dot 2',
   'Han cuoi dong hoc phi dot 2 la ngay 30/10/2024. Tre han se bi khoa tai khoan.',
   1, NULL, NOW(), NOW(), NOW());   -- NULL = gui toan truong

INSERT INTO thong_bao_da_doc (id, thong_bao_id, tai_khoan_id, da_doc, ngay_doc, created_at, updated_at) VALUES
(1, 1, 4, true,  NOW(), NOW(), NOW()),
(2, 1, 5, false, NULL,  NOW(), NOW()),
(3, 1, 6, false, NULL,  NOW(), NOW());

-- =============================================
-- 11. BIEN BAN SHCN
-- =============================================
INSERT INTO bien_ban_shcn (id, lop_id, tuan_hoc_id, thoi_gian, dia_diem, gvcn_id, thu_ky_id, noi_dung, phan_hoi_gvcn, created_at, updated_at) VALUES
(1, 1, 1, '2024-09-05 07:30:00', 'Phong A101', 1, 1,
 'Sinh hoat dau nam hoc. Pho bien noi quy. Trinh bay ke hoach hoc tap HK1.',
 'Lop can co gang hon. Lop truong theo doi si so hang tuan.',
 NOW(), NOW());

INSERT INTO chi_tiet_cong_viec (id, bien_ban_id, ten_cong_viec, trang_thai_viec, created_at, updated_at) VALUES
(1, 1, 'Dong hoc phi dot 1',      'Hoan thanh',       NOW(), NOW()),
(2, 1, 'Dang ky hoc phan HK1',    'Hoan thanh',       NOW(), NOW()),
(3, 1, 'Nop anh the sinh vien',   'Dang thuc hien',   NOW(), NOW());

INSERT INTO chi_tiet_vang_shcn (id, bien_ban_id, sinh_vien_id, ly_do, co_phep, created_at, updated_at) VALUES
(1, 1, 2, 'Benh', true, NOW(), NOW());

-- =============================================
-- 12. HOC BA & DIEM REN LUYEN
-- =============================================
INSERT INTO hoc_ba (id, sinh_vien_id, ctdt_id, diem_tbc_tich_luy, so_tin_chi_tich_luy, created_at, updated_at) VALUES
(1, 1, 1, 8.60, 6, NOW(), NOW()),
(2, 2, 1, 6.65, 6, NOW(), NOW()),
(3, 3, 1, 3.60, 3, NOW(), NOW());

INSERT INTO diem_ren_luyen (id, sinh_vien_id, hoc_ky_id, diem_tong, xep_loai, created_at, updated_at) VALUES
(1, 1, 1, 88, 'Tot',       NOW(), NOW()),
(2, 2, 1, 72, 'Kha',       NOW(), NOW()),
(3, 3, 1, 55, 'Trung binh',NOW(), NOW());

-- =============================================
-- 13. KET QUA ANH VAN DAU VAO
-- =============================================
INSERT INTO ket_qua_anh_van_dau_vao (id, sinh_vien_id, hinh_thuc_xet, diem_thi, diem_ta1, diem_ta2, diem_ta3, ghi_chu, created_at, updated_at) VALUES
(1, 1, 'VSTEP',  7.5,  NULL, NULL, NULL, 'Dat chuan B2',    NOW(), NOW()),
(2, 2, 'TOEIC',  NULL, NULL, NULL, NULL, 'TOEIC 650',       NOW(), NOW()),
(3, 3, 'Noi bo', NULL, 5.5,  6.0,  NULL, 'Hoc lai TA1',    NOW(), NOW());

-- =============================================
-- 14. YEU CAU HANH CHINH
-- =============================================
INSERT INTO yeu_cau_hanh_chinh (id, sinh_vien_id, loai_yeu_cau, noi_dung, trang_thai, nguoi_duyet_id, ngay_tao, created_at, updated_at) VALUES
(1, 2, 'Xac nhan sinh vien', 'De nghi cap giay xac nhan la sinh vien de vay von.', 'Da duyet',  1,    NOW(), NOW(), NOW()),
(2, 3, 'Hoan hoc phi',       'Xin hoan hoc phi vi ly do suc khoe gia dinh.',       'Cho duyet', NULL, NOW(), NOW(), NOW());

-- =============================================
-- 15. KHAO SAT & DIEN DAN
-- =============================================
INSERT INTO khao_sat_y_kien (id, sinh_vien_id, lop_hp_id, diem_danh_gia, gop_y, created_at, updated_at) VALUES
(1, 1, 1, 5, 'Giao vien giang day nhiet tinh, de hieu.',       NOW(), NOW()),
(2, 2, 1, 4, 'Can them bai tap thuc hanh.',                    NOW(), NOW()),
(3, 1, 2, 5, 'Noi dung hay, co nhieu vi du thuc te.',          NOW(), NOW());

INSERT INTO dien_dan_giao_vien (id, lop_id, giao_vien_id, noi_dung, ngay_gui, created_at, updated_at) VALUES
(1, 1, 1, 'Lop chu y on tap chuong 3 va 4 cho ky thi cuoi ky.', NOW(), NOW(), NOW());
```

---

## ✅ Checklist tổng quát

### Backend
- [ ] 5-project solution đúng Clean Architecture dependency rule
- [ ] `AuditableEntity` với `CreatedAt`, `UpdatedAt` tự động
- [ ] EF Code First + Migration + `IEntityTypeConfiguration`
- [ ] JWT Auth + Refresh Token
- [ ] Repository + Unit of Work
- [ ] Service Layer + AutoMapper + FluentValidation
- [ ] `GlobalExceptionMiddleware` + chuẩn hoá response
- [ ] Swagger với JWT + XML comments
- [ ] Unit Test >= 10 test cases (Moq + xUnit)
- [ ] Docker image chạy được

### Frontend
- [ ] React + Vite + TypeScript
- [ ] Auth flow + Protected Route theo role
- [ ] Axios interceptor refresh token
- [ ] TanStack Query cho data fetching
- [ ] Dashboard theo 3 role: Admin / GV / SV
- [ ] Responsive, skeleton loading, error boundary
- [ ] Docker image nginx serve static + proxy `/api`

### Deployment
- [ ] `docker-compose.yml` chạy 3 services (mysql, backend, frontend)
- [ ] EC2 t3.small + Security Group đúng port
- [ ] Nginx reverse proxy + SSL
- [ ] GitHub Actions CI/CD

---

## 📦 NuGet Packages

```
Pomelo.EntityFrameworkCore.MySql
Microsoft.EntityFrameworkCore.Design
AutoMapper.Extensions.Microsoft.DependencyInjection
FluentValidation.AspNetCore
Microsoft.AspNetCore.Authentication.JwtBearer
BCrypt.Net-Next
Swashbuckle.AspNetCore
AspNetCoreRateLimit
xunit
Moq
FluentAssertions
```

## 📦 NPM Packages

```
vite react react-dom typescript
tailwindcss
react-router-dom
axios
@tanstack/react-query
zustand
react-hook-form zod @hookform/resolvers
react-hot-toast
lucide-react
```
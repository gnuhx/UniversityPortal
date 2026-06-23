# Phase 2 — Giáo viên Portal + Nhập điểm + Yêu cầu

> **Mục tiêu**: Hoàn thiện portal cho vai trò **Giáo viên** (hiện chưa có trang nào riêng), đồng thời xây dựng hai module Yêu cầu (hành chính cho SV, sửa điểm cho GV) để đóng vòng luồng dữ liệu điểm số.
>
> **Sau phase này**: sinh viên thấy điểm thật vì giáo viên đã có trang nhập điểm; yêu cầu hành chính có thể tạo và duyệt end-to-end.

---

## Hiện trạng sau Phase 1

| Module | Domain | Repository | Service | Controller | Frontend |
|---|:---:|:---:|:---:|:---:|:---:|
| LopHocPhan | ✅ | ✅ `GetPagedByGiaoVienAsync` | ❌ | ❌ | ❌ |
| Nhập điểm (PUT diem) | ✅ `DanhSachLopHP` | ✅ | ✅ (GET only) | ✅ (GET only) | ❌ |
| YeuCauSuaDiem | ✅ | ❌ | ❌ | ❌ | ❌ |
| YeuCauHanhChinh | ✅ | ❌ | ❌ | ❌ | ❌ |
| Giáo viên dashboard | — | — | — | — | ❌ |

---

## Module 1 — Lớp học phần (LopHocPhan)

### 1.1 Backend — Repository mở rộng

**File sửa:** `Infrastructure/Repositories/LopHocPhanRepository.cs`

Thêm query include đầy đủ navigation để trả về DTO:

```csharp
// Lấy LHP của giáo viên, include MonHoc + HocKy + đếm SV
Task<IEnumerable<LopHocPhan>> GetByGiaoVienWithDetailsAsync(int giaoVienId);

// Lấy LHP theo Id, include đầy đủ để xem danh sách SV + điểm
Task<LopHocPhan?> GetDetailAsync(int id);
```

Thêm vào `ILopHocPhanRepository.cs` và `UnitOfWork` (không cần thêm vào IUnitOfWork vì LopHocPhans đã có).

---

### 1.2 Backend — DTO

**Files mới:** `Application/DTOs/LopHocPhan/`

```
LopHocPhanDto.cs
  id, maLopHp, tenMon, maMon, hocKyId, tenHocKy,
  giaoVienId, tenGiaoVien, soSinhVien, khoaBangDiem, trangThaiKetThuc
```

Thêm mapping `LopHocPhan → LopHocPhanDto` vào `MappingProfile`:
- `TenMon` ← `ChiTietCTDT.MonHoc.TenMon`
- `TenHocKy` ← `HocKy.TenHocKy`
- `TenGiaoVien` ← `GiaoVien.TaiKhoan.HoTen`
- `SoSinhVien` ← `DanhSachLopHPs.Count`

---

### 1.3 Backend — Service + Controller

**Files mới:**
```
Application/Interfaces/Services/ILopHocPhanService.cs
Application/Services/LopHocPhanService.cs
API/Controllers/LopHocPhanController.cs
```

**Endpoints:**

```
GET  /api/lop-hoc-phan/me
     → Giáo viên: lấy tất cả LHP mình đang phụ trách (include MonHoc, HocKy, đếm SV)
     → Authorize: Giáo viên

GET  /api/lop-hoc-phan?hocKyId=&page=&pageSize=
     → Lấy tất cả LHP, filter theo học kỳ
     → Authorize: Admin, Giáo vụ

PUT  /api/lop-hoc-phan/{id}/khoa-bang-diem
     → Khoá bảng điểm (set KhoaBangDiem = true, không thể undo trực tiếp)
     → Authorize: Giáo viên (chỉ LHP của mình)
     → Trả về lỗi nếu đã bị khoá
```

**Đăng ký vào DependencyInjection.cs:**
```csharp
services.AddScoped<ILopHocPhanService, LopHocPhanService>();
```

---

## Module 2 — Nhập điểm

### 2.1 Backend — Endpoint nhập điểm

**File sửa:** `DanhSachLopHPController.cs` — thêm 1 endpoint PUT.

```
PUT /api/danh-sach-lop-hp/{id}/diem
    → Giáo viên nhập / cập nhật điểm cho 1 dòng DanhSachLopHP
    → Authorize: Giáo viên
    Body: { diemQt1?, diemQt2?, diemThi? }
    Logic:
      1. Kiểm tra LopHocPhan.KhoaBangDiem = false (nếu khoá → 400 BadRequest)
      2. Cập nhật DiemQt1, DiemQt2, DiemThi
      3. Tính DiemTongKet = Qt1*0.15 + Qt2*0.15 + Thi*0.70 (chỉ khi cả 3 có giá trị)
      4. Commit + trả về DanhSachLopHPDto đã cập nhật
```

**DTO mới:** `Application/DTOs/DanhSachLopHP/NhapDiemDto.cs`
```csharp
public class NhapDiemDto
{
    public float? DiemQt1 { get; set; }
    public float? DiemQt2 { get; set; }
    public float? DiemThi { get; set; }
}
```

**Service mới:** Thêm `NhapDiemAsync(int danhSachLopHpId, int taiKhoanId, NhapDiemDto dto)` vào `IDanhSachLopHPService`.
- Kiểm tra giáo viên có quyền nhập điểm lớp HP này không (GiaoVienId của LHP = GiaoVienId của user)

---

## Module 3 — Yêu cầu sửa điểm (YeuCauSuaDiem)

### 3.1 Backend

**Files mới:**

```
Application/DTOs/YeuCauSuaDiem/
  YeuCauSuaDiemDto.cs
    id, lopHpId, maLopHp, tenMon, giaoVienId, tenGiaoVien,
    lyDo, trangThai, nguoiDuyetId, tenNguoiDuyet, createdAt
  CreateYeuCauSuaDiemDto.cs
    lopHpId, lyDo
  DuyetYeuCauSuaDiemDto.cs
    trangThai  ("Đã duyệt" | "Từ chối")

Infrastructure/Repositories/
  YeuCauSuaDiemRepository.cs
    GetByGiaoVienAsync(giaoVienId)  — GV xem yêu cầu của mình
    GetAllPagedAsync(page, pageSize, trangThai?)  — Admin xem tất cả

Application/Interfaces/Services/IYeuCauSuaDiemService.cs
Application/Services/YeuCauSuaDiemService.cs
API/Controllers/YeuCauSuaDiemController.cs
```

**Cập nhật IUnitOfWork + UnitOfWork:** thêm `IYeuCauSuaDiemRepository YeuCauSuaDiems`.

**Endpoints:**

```
GET  /api/yeu-cau-sua-diem?trangThai=
     → Admin: tất cả yêu cầu, filter theo trạng thái
     → Giáo viên: chỉ yêu cầu của mình

POST /api/yeu-cau-sua-diem
     → Giáo viên tạo yêu cầu mở khoá bảng điểm
     → Authorize: Giáo viên
     Body: { lopHpId, lyDo }
     Validation: LHP này phải đang KhoaBangDiem = true

PUT  /api/yeu-cau-sua-diem/{id}/duyet
     → Admin duyệt hoặc từ chối
     → Authorize: Admin
     Body: { trangThai }
     Nếu "Đã duyệt": set LopHocPhan.KhoaBangDiem = false (mở lại để nhập điểm)
```

---

## Module 4 — Yêu cầu hành chính (YeuCauHanhChinh)

### 4.1 Backend

**Files mới:**

```
Application/DTOs/YeuCauHanhChinh/
  YeuCauHanhChinhDto.cs
    id, sinhVienId, tenSinhVien, mssv,
    loaiYeuCau, noiDung, fileDinhKem,
    trangThai, nguoiDuyetId, tenNguoiDuyet, ngayTao, createdAt
  CreateYeuCauHanhChinhDto.cs
    loaiYeuCau, noiDung, fileDinhKem?
  DuyetYeuCauHanhChinhDto.cs
    trangThai  ("Đã duyệt" | "Từ chối")
    ghiChu?

Infrastructure/Repositories/
  YeuCauHanhChinhRepository.cs
    GetBySinhVienAsync(sinhVienId)
    GetAllPagedAsync(page, pageSize, trangThai?)

Application/Interfaces/Services/IYeuCauHanhChinhService.cs
Application/Services/YeuCauHanhChinhService.cs
API/Controllers/YeuCauHanhChinhController.cs
```

**Cập nhật IUnitOfWork + UnitOfWork:** thêm `IYeuCauHanhChinhRepository YeuCauHanhChinhs`.

**Endpoints:**

```
GET  /api/yeu-cau-hanh-chinh?page=&pageSize=&trangThai=
     → Sinh viên: chỉ thấy yêu cầu của mình (filter tự động bằng JWT)
     → Admin/Giáo vụ: thấy tất cả

POST /api/yeu-cau-hanh-chinh
     → Sinh viên tạo yêu cầu mới
     → Authorize: Sinh viên
     Body: { loaiYeuCau, noiDung, fileDinhKem? }

PUT  /api/yeu-cau-hanh-chinh/{id}/duyet
     → Admin hoặc Giáo vụ duyệt hoặc từ chối
     → Authorize: Admin, Giáo vụ
     Body: { trangThai, ghiChu? }
```

**Loại yêu cầu (string enum):** `Xác nhận sinh viên`, `Hoãn học phí`, `Bảo lưu`, `Miễn giảm học phí`, `Khác`

---

## Frontend

### 5.1 Cập nhật Dashboard cho Giáo viên

**File sửa:** `DashboardPage.tsx`

Khi `user.vaiTro === ROLES.GIAO_VIEN`:
- Card: Số lớp HP đang phụ trách
- Card: Tổng số sinh viên phụ trách
- Card: Số bảng điểm chưa khoá
- Quick link: "Nhập điểm", "Yêu cầu sửa điểm"

API cần: `GET /api/lop-hoc-phan/me` (đếm từ response)

---

### 5.2 Trang Lớp học phần (`/lop-hoc-phan`)

**File mới:** `src/pages/LopHocPhanPage.tsx`

**Giáo viên view:**
- Table: Mã lớp HP | Môn học | Học kỳ | Số SV | Bảng điểm | Hành động
- Cột "Bảng điểm": `Tag color="red"` nếu đã khoá, `Tag color="green"` nếu chưa khoá
- Hành động: nút **"Nhập điểm"** → mở drawer/modal, nút **"Khoá bảng điểm"** (có confirm dialog)

**Admin/Giáo vụ view:** Table tất cả LHP, filter theo học kỳ

**Thêm vào sidebar:**
```ts
{ key: "/lop-hoc-phan", icon: <ScheduleOutlined />, label: "Lớp học phần",
  roles: [ROLES.GIAO_VIEN, ROLES.ADMIN, ROLES.GIAO_VU] }
```

---

### 5.3 Drawer nhập điểm

**Component mới:** `src/components/NhapDiemDrawer.tsx`

Props: `lopHpId: number, maLopHp: string, onClose: () => void`

Nội dung:
- Gọi `GET /api/danh-sach-lop-hp?lopHpId=` để lấy danh sách SV + điểm hiện tại
- Editable Table (Ant Design `EditableCell` pattern):

| MSSV | Họ tên | ĐQT1 | ĐQT2 | ĐThi | ĐTổng kết |
|---|---|---|---|---|---|
| 2100001 | Nguyễn Thị Hoa | [input] | [input] | [input] | auto |

- ĐTổng kết được tính ngay trên UI: `qt1*0.15 + qt2*0.15 + thi*0.70`
- Nút **"Lưu"** trên từng hàng → gọi `PUT /api/danh-sach-lop-hp/{id}/diem`
- Hiển thị badge khoá nếu `khoaBangDiem = true` (ẩn input, chỉ xem)

---

### 5.4 Trang Yêu cầu hành chính (`/yeu-cau-hanh-chinh`)

**File mới:** `src/pages/YeuCauHanhChinhPage.tsx`

**Sinh viên view:**
- Nút "Tạo yêu cầu" → Modal:
  - Select: Loại yêu cầu
  - TextArea: Nội dung
  - Upload: File đính kèm (optional)
- Table yêu cầu của mình: Loại | Nội dung | Ngày tạo | Trạng thái
- Trạng thái badge: `Chờ duyệt`→xám, `Đã duyệt`→xanh, `Từ chối`→đỏ

**Admin/Giáo vụ view:**
- Table tất cả yêu cầu, filter Select theo trạng thái
- Inline action: nút **"Duyệt"** và **"Từ chối"** → confirm dialog với ô ghi chú

**Thêm vào sidebar:**
```ts
{ key: "/yeu-cau-hanh-chinh", icon: <FileTextOutlined />, label: "Yêu cầu hành chính",
  roles: [ROLES.SINH_VIEN, ROLES.ADMIN, ROLES.GIAO_VU] }
```

> Route `/yeu-cau-hanh-chinh` đã có trong `App.tsx` (placeholder từ Phase 1 sidebar).
> Cần bỏ `ProtectedRoute` restrict chỉ SINH_VIEN và mở cho cả ADMIN + GIAO_VU.

---

### 5.5 Trang Yêu cầu sửa điểm (`/yeu-cau-sua-diem`)

**File mới:** `src/pages/YeuCauSuaDiemPage.tsx`

**Giáo viên view:**
- Nút "Tạo yêu cầu" → Modal:
  - Select: Lớp học phần (fetch `/api/lop-hoc-phan/me`, chỉ hiện LHP đã khoá)
  - TextArea: Lý do
- Table: Lớp HP | Môn học | Lý do | Ngày tạo | Trạng thái

**Admin view:**
- Table tất cả yêu cầu, filter theo trạng thái
- Nút **"Duyệt"** → confirm → mở khoá bảng điểm tương ứng

**Thêm vào sidebar:**
```ts
{ key: "/yeu-cau-sua-diem", icon: <EditOutlined />, label: "Yêu cầu sửa điểm",
  roles: [ROLES.GIAO_VIEN, ROLES.ADMIN] }
```

---

## Checklist tổng quát Phase 2

### Module 1 — LopHocPhan
- [ ] BE: Thêm `GetByGiaoVienWithDetailsAsync` + `GetDetailAsync` vào `LopHocPhanRepository`
- [ ] BE: Cập nhật `ILopHocPhanRepository`
- [ ] BE: `LopHocPhanDto.cs` + mapping (tenMon, tenHocKy, tenGiaoVien, soSinhVien)
- [ ] BE: `ILopHocPhanService` + `LopHocPhanService` (getMe, getPaged, khoaBangDiem)
- [ ] BE: `LopHocPhanController` (GET /me, GET ?, PUT /{id}/khoa-bang-diem)
- [ ] BE: Đăng ký `ILopHocPhanService` vào `DependencyInjection.cs`

### Module 2 — Nhập điểm
- [ ] BE: `NhapDiemDto.cs`
- [ ] BE: Thêm `NhapDiemAsync` vào `IDanhSachLopHPService` + `DanhSachLopHPService`
- [ ] BE: Thêm `PUT /{id}/diem` vào `DanhSachLopHPController`

### Module 3 — YeuCauSuaDiem
- [ ] BE: `YeuCauSuaDiemDto`, `CreateYeuCauSuaDiemDto`, `DuyetYeuCauSuaDiemDto`
- [ ] BE: `YeuCauSuaDiemRepository.cs` + interface
- [ ] BE: Cập nhật `IUnitOfWork` + `UnitOfWork` thêm `YeuCauSuaDiems`
- [ ] BE: `IYeuCauSuaDiemService` + `YeuCauSuaDiemService`
- [ ] BE: `YeuCauSuaDiemController` (GET, POST, PUT /duyet)
- [ ] BE: Đăng ký service vào `DependencyInjection.cs`

### Module 4 — YeuCauHanhChinh
- [ ] BE: `YeuCauHanhChinhDto`, `CreateYeuCauHanhChinhDto`, `DuyetYeuCauHanhChinhDto`
- [ ] BE: `YeuCauHanhChinhRepository.cs` + interface
- [ ] BE: Cập nhật `IUnitOfWork` + `UnitOfWork` thêm `YeuCauHanhChinhs`
- [ ] BE: `IYeuCauHanhChinhService` + `YeuCauHanhChinhService`
- [ ] BE: `YeuCauHanhChinhController` (GET, POST, PUT /duyet)
- [ ] BE: Đăng ký service vào `DependencyInjection.cs`

### Frontend
- [ ] FE: Types `LopHocPhanDto`, `NhapDiemDto`, `YeuCauSuaDiemDto`, `YeuCauHanhChinhDto`
- [ ] FE: API functions: `lopHocPhanApi`, `nhapDiemApi`, `yeuCauSuaDiemApi`, `yeuCauHanhChinhApi`
- [ ] FE: Cập nhật `DashboardPage.tsx` — thêm block cho Giáo viên
- [ ] FE: `LopHocPhanPage.tsx` (`/lop-hoc-phan`) — giáo viên xem lớp HP
- [ ] FE: `NhapDiemDrawer.tsx` — editable table nhập điểm + auto-tính ĐTổng kết
- [ ] FE: `YeuCauHanhChinhPage.tsx` (`/yeu-cau-hanh-chinh`)
- [ ] FE: `YeuCauSuaDiemPage.tsx` (`/yeu-cau-sua-diem`)
- [ ] FE: Cập nhật `App.tsx` — mở route `/yeu-cau-hanh-chinh` cho ADMIN + GIAO_VU, thêm `/lop-hoc-phan`, `/yeu-cau-sua-diem`
- [ ] FE: Cập nhật `AppLayout.tsx` sidebar — thêm menu items mới theo role

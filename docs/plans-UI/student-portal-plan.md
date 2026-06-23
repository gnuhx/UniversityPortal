# Kế hoạch UI Sinh Viên — University Portal

> Mục tiêu: xây dựng đầy đủ giao diện cho vai trò **Sinh viên**, bao gồm backend endpoints còn thiếu và các trang frontend tương ứng.

---

## Tổng quan hiện trạng

### Backend đã có (Domain / Infrastructure)
- Tất cả Entity và EF Configuration đã tồn tại cho mọi module bên dưới.
- `DanhSachLopHPRepository` đã có `GetBySinhVienAsync(sinhVienId)`.
- `UnitOfWork` đã expose `LopHocPhans`, `DanhSachLopHPs`.

### Backend còn thiếu (Service / Controller / DTO)
| Module | Repository | Service | Controller |
|---|:---:|:---:|:---:|
| `GET /api/sinh-vien/me` | ✅ | ✅ | ❌ endpoint thiếu |
| Bảng điểm (DanhSachLopHP) | ✅ | ❌ | ❌ |
| Yêu cầu hành chính | ❌ | ❌ | ❌ |
| Yêu cầu sửa điểm | ❌ | ❌ | ❌ |
| Học phí | ❌ | ❌ | ❌ |
| Thông báo | ❌ | ❌ | ❌ |
| Thời khóa biểu | ❌ | ❌ | ❌ |

### Frontend đang có cho Sinh viên
- Sidebar: Tổng quan, Ngành học, Chương trình ĐT, Môn học (đều là read-only public)
- Dashboard: chỉ hiện thống kê chung, không có nội dung cá nhân

---

## Phase 1 — Hồ sơ & Bảng điểm

> **Mức độ**: nhỏ — repository đã sẵn, chỉ cần thêm endpoint + service + 2 trang FE.

### 1.1 Backend — `GET /api/sinh-vien/me`

**File cần sửa:** `SinhVienController.cs`

Thêm 1 endpoint mới, dùng JWT claim `nameidentifier` (taiKhoanId) để lookup SinhVien:

```
GET /api/sinh-vien/me
Authorization: Bearer <token>   (role: Sinh viên)
Response: SinhVienDto
  {
    id, mssv, hoTen, email, anhDaiDien,
    lopId, tenLop, trangThai, createdAt
  }
```

**Các bước:**
1. Thêm phương thức `GetByTaiKhoanIdAsync(int taiKhoanId)` vào `ISinhVienRepository` + `SinhVienRepository`.
2. Thêm `GetMeAsync(int taiKhoanId)` vào `ISinhVienService` + `SinhVienService`.
3. Thêm action `[HttpGet("me")]` vào `SinhVienController`, authorize role `Sinh viên`.

---

### 1.2 Backend — Bảng điểm (`DanhSachLopHP`)

**Files mới cần tạo:**

```
Application/DTOs/DanhSachLopHP/
  DanhSachLopHPDto.cs         # id, maLopHp, tenMon, hocKy, loaiDangKy,
                               # diemQt1, diemQt2, diemThi, diemTongKet,
                               # trangThaiDuyet, trangThaiDongTien, soTienPhaiDong
Application/Interfaces/Services/
  IDanhSachLopHPService.cs
Application/Services/
  DanhSachLopHPService.cs
API/Controllers/
  DanhSachLopHPController.cs
```

**Endpoints:**

```
GET /api/danh-sach-lop-hp/me
  → Lấy toàn bộ môn học + điểm của sinh viên đang đăng nhập
  → Authorize: Sinh viên
  → Response: List<DanhSachLopHPDto> (include: maLopHp, tenMon, hocKy, điểm)

GET /api/danh-sach-lop-hp?lopHpId={id}
  → Danh sách sinh viên trong 1 lớp học phần (dùng cho GV nhập điểm sau)
  → Authorize: Admin, Giáo vụ, Giáo viên
```

**Mapping:** `DanhSachLopHP` → `DanhSachLopHPDto`
- Include `LopHocPhan` → `ChiTietCTDT` → `MonHoc` để lấy `tenMon`
- Include `LopHocPhan` → `HocKy` để lấy `tenHocKy`

---

### 1.3 Frontend — Trang Hồ sơ (`/ho-so`)

**File mới:** `src/pages/HoSoPage.tsx`

Layout: 2 cột
- Trái: Avatar (dùng `Upload` của AntD hoặc chỉ hiển thị), tên, vai trò
- Phải: Descriptions table với các trường: MSSV, Lớp sinh hoạt, Email, Trạng thái, Ngày tạo

```tsx
// API call
GET /api/sinh-vien/me  →  hiển thị SinhVienDto
```

**Thêm vào sidebar** (chỉ hiện cho Sinh viên):
```ts
{ key: "/ho-so", icon: <UserOutlined />, label: "Hồ sơ cá nhân", roles: [ROLES.SINH_VIEN] }
```

---

### 1.4 Frontend — Trang Bảng điểm (`/bang-diem`)

**File mới:** `src/pages/BangDiemPage.tsx`

Layout:
- Tabs theo học kỳ (groupBy `tenHocKy`)
- Mỗi tab: Table với các cột:

| Môn học | Mã lớp HP | Loại ĐK | ĐQT1 | ĐQT2 | ĐThi | ĐTổng kết | Trạng thái |
|---|---|---|---|---|---|---|---|

- Footer mỗi tab: GPA học kỳ đó (tính từ `diemTongKet` của các môn `tinhDiemTb`)
- Badge màu cho điểm: ≥ 8 → xanh, 5–7.9 → vàng, < 5 → đỏ

```tsx
// API call
GET /api/danh-sach-lop-hp/me  →  List<DanhSachLopHPDto>
// Group by hocKy trên frontend
```

**Thêm vào sidebar** (chỉ Sinh viên):
```ts
{ key: "/bang-diem", icon: <ReadOutlined />, label: "Bảng điểm", roles: [ROLES.SINH_VIEN] }
```

---

### 1.5 Frontend — Cập nhật Dashboard cho Sinh viên

**File sửa:** `src/pages/DashboardPage.tsx`

Khi `user.vaiTro === ROLES.SINH_VIEN`, hiện block riêng thay vì stats chung:
- Card: MSSV + Lớp sinh hoạt (lấy từ `/api/sinh-vien/me`)
- Card: Số môn đã học / đang học (lấy từ `/api/danh-sach-lop-hp/me`)
- Card: GPA tích lũy (tính từ bảng điểm)
- Card: Học phí học kỳ hiện tại — trạng thái (placeholder, sẽ wire sau Phase 3)
- Quick links: "Xem bảng điểm", "Chương trình đào tạo của tôi", "Tạo yêu cầu hành chính"

---

## Phase 2 — Yêu cầu hành chính & Yêu cầu sửa điểm

### 2.1 Backend — Yêu cầu hành chính

**Files mới:**

```
Application/DTOs/YeuCauHanhChinh/
  YeuCauHanhChinhDto.cs       # id, loaiYeuCau, noiDung, fileDinhKem,
                               # trangThai, nguoiDuyetId, tenNguoiDuyet, ngayTao
  CreateYeuCauHanhChinhDto.cs # loaiYeuCau, noiDung, fileDinhKem (optional)
  DuyetYeuCauDto.cs           # trangThai: "Đã duyệt" | "Từ chối", ghiChu?
Application/Interfaces/Services/
  IYeuCauHanhChinhService.cs
Application/Services/
  YeuCauHanhChinhService.cs
Infrastructure/Repositories/
  YeuCauHanhChinhRepository.cs  # GetBySinhVienAsync(sinhVienId)
API/Controllers/
  YeuCauHanhChinhController.cs
```

**Endpoints:**

```
GET  /api/yeu-cau-hanh-chinh
     ?page&pageSize&trangThai
     → Sinh viên: chỉ thấy yêu cầu của mình (filter by JWT sinhVienId)
     → Admin/GiaoVu: thấy tất cả

POST /api/yeu-cau-hanh-chinh
     → Sinh viên tạo yêu cầu mới
     Body: { loaiYeuCau, noiDung, fileDinhKem? }

PUT  /api/yeu-cau-hanh-chinh/{id}/duyet
     → Admin / GiaoVu duyệt hoặc từ chối
     Body: { trangThai, ghiChu? }
```

**Loại yêu cầu (enum / string):** `Xác nhận sinh viên`, `Hoãn học phí`, `Bảo lưu`, `Khác`

---

### 2.2 Backend — Yêu cầu sửa điểm

> Lưu ý: `YeuCauSuaDiem` do **Giáo viên** tạo (yêu cầu mở khoá bảng điểm), **Admin** duyệt. Sinh viên không tạo, chỉ hưởng kết quả (điểm được sửa sau khi duyệt).

**Files mới:**

```
Application/DTOs/YeuCauSuaDiem/
  YeuCauSuaDiemDto.cs         # id, lopHpId, maLopHp, tenMon, giaoVienId,
                               # tenGiaoVien, lyDo, trangThai, nguoiDuyetId, createdAt
  CreateYeuCauSuaDiemDto.cs   # lopHpId, lyDo
  DuyetYeuCauSuaDiemDto.cs    # trangThai: "Đã duyệt" | "Từ chối"
Application/Interfaces/Services/
  IYeuCauSuaDiemService.cs
Application/Services/
  YeuCauSuaDiemService.cs
Infrastructure/Repositories/
  YeuCauSuaDiemRepository.cs  # GetByLopHpAsync, GetByGiaoVienAsync
API/Controllers/
  YeuCauSuaDiemController.cs
```

**Endpoints:**

```
GET  /api/yeu-cau-sua-diem
     → Admin: thấy tất cả, filter theo trangThai
     → Giáo viên: thấy yêu cầu mình đã tạo

POST /api/yeu-cau-sua-diem
     → Giáo viên tạo yêu cầu mở khoá bảng điểm
     Body: { lopHpId, lyDo }

PUT  /api/yeu-cau-sua-diem/{id}/duyet
     → Admin duyệt / từ chối
     → Nếu duyệt: set LopHocPhan.KhoaBangDiem = false
     Body: { trangThai }
```

---

### 2.3 Frontend — Trang Yêu cầu hành chính (`/yeu-cau-hanh-chinh`)

**File mới:** `src/pages/YeuCauHanhChinhPage.tsx`

**Sinh viên view:**
- Nút "Tạo yêu cầu mới" → Modal với form: Loại yêu cầu (Select), Nội dung (TextArea), File đính kèm (Upload)
- Bảng danh sách yêu cầu của mình: Loại, Nội dung (truncate), Ngày tạo, Trạng thái (Badge)
- Trạng thái badge: `Chờ duyệt` → xám, `Đã duyệt` → xanh, `Từ chối` → đỏ

**Admin / Giáo vụ view** (cùng route, khác quyền):
- Bảng tất cả yêu cầu, filter theo Trạng thái
- Nút "Duyệt" / "Từ chối" inline

**Thêm vào sidebar:**
```ts
{ key: "/yeu-cau-hanh-chinh", icon: <FileTextOutlined />, label: "Yêu cầu hành chính",
  roles: [ROLES.SINH_VIEN, ROLES.ADMIN, ROLES.GIAO_VU] }
```

---

### 2.4 Frontend — Trang Yêu cầu sửa điểm (`/yeu-cau-sua-diem`)

**File mới:** `src/pages/YeuCauSuaDiemPage.tsx`

**Giáo viên view:**
- Nút "Tạo yêu cầu" → Modal: chọn Lớp học phần (dropdown), nhập Lý do
- Bảng yêu cầu đã gửi: Lớp HP, Môn, Lý do, Trạng thái

**Admin view:**
- Bảng tất cả yêu cầu, filter theo Trạng thái
- Nút "Duyệt" → mở khoá bảng điểm của lớp HP đó

**Sinh viên**: không thấy mục này trong sidebar; ảnh hưởng được phản ánh qua điểm cập nhật trong Bảng điểm.

**Thêm vào sidebar:**
```ts
{ key: "/yeu-cau-sua-diem", icon: <EditOutlined />, label: "Yêu cầu sửa điểm",
  roles: [ROLES.ADMIN, ROLES.GIAO_VIEN] }
```

---

## Phase 3 — Học phí, Thông báo, Thời khóa biểu

### 3.1 Backend — Học phí

**Endpoints:**

```
GET /api/hoc-phi/me
    → Sinh viên xem danh sách học phí theo từng học kỳ
    Response: List<HocPhiDto>
      { id, hocKyId, tenHocKy, soTien, trangThaiDong, createdAt }

GET /api/hoc-phi?sinhVienId=&hocKyId=
    → Admin / GiaoVu xem + lọc

PUT /api/hoc-phi/{id}/trang-thai
    → Admin / GiaoVu cập nhật trạng thái đóng tiền
    Body: { trangThaiDong: "Đã đóng" | "Chưa đóng" }
```

---

### 3.2 Backend — Thông báo

**Endpoints:**

```
GET  /api/thong-bao
     → Sinh viên: thông báo gửi toàn trường (lopNhanId = null)
       + thông báo gửi lớp của mình (lopNhanId = lopId)
     → Admin / GiaoVu: tất cả

POST /api/thong-bao
     → Admin / GiaoVu tạo thông báo
     Body: { loaiThongBao, mucDo, tieuDe, noiDung, lopNhanId? }

PUT  /api/thong-bao/{id}/da-doc
     → Sinh viên đánh dấu đã đọc
     → Upsert vào ThongBaoDaDoc

GET  /api/thong-bao/unread-count
     → Sinh viên: đếm số thông báo chưa đọc (dùng cho badge sidebar)
```

---

### 3.3 Backend — Thời khóa biểu

**Endpoints:**

```
GET /api/thoi-khoa-bieu/me?hocKyId=
    → Sinh viên: lấy TKB của tất cả lớp HP mình đăng ký trong học kỳ
    Response: List<ThoiKhoaBieuDto>
      { lopHpId, maLopHp, tenMon, thu, tietBatDau, tietKetThuc, phongHoc,
        tuanHocId, tenTuan, ngayBatDauTuan }
```

---

### 3.4 Frontend — Trang Học phí (`/hoc-phi`)

**File mới:** `src/pages/HocPhiPage.tsx`

- Table: Học kỳ | Số tiền | Trạng thái | Hành động (Admin: cập nhật trạng thái)
- Sinh viên chỉ xem; Admin/GiaoVu có nút "Đã thu tiền"
- Summary card: tổng chưa đóng

---

### 3.5 Frontend — Trang Thông báo (`/thong-bao`)

**File mới:** `src/pages/ThongBaoPage.tsx`

- List thông báo: Badge mức độ (Khẩn cấp → đỏ, Quan trọng → cam, Bình thường → xám)
- Click vào → mở Modal xem nội dung → tự động gọi `PUT /da-doc`
- Sidebar badge: hiện số chưa đọc (query `unread-count`, refetch mỗi 60s)
- Admin/GiaoVu: thêm nút "Tạo thông báo"

---

### 3.6 Frontend — Trang Thời khóa biểu (`/thoi-khoa-bieu`)

**File mới:** `src/pages/ThoiKhoaBieuPage.tsx`

- Select học kỳ (dropdown)
- Calendar/grid view: 7 cột (Thứ 2 → Thứ 8), hàng = tiết (1–10)
- Mỗi cell: tên môn, phòng học, tên tuần
- Fallback: nếu backend chưa sẵn, hiện Table đơn giản (Thứ, Tiết, Môn, Phòng)

---

## Tóm tắt sidebar sau khi hoàn thành

```
--- Chung (tất cả roles) ---
/ Tổng quan

--- Sinh viên ---
/ho-so               Hồ sơ cá nhân
/bang-diem           Bảng điểm
/hoc-phi             Học phí
/thoi-khoa-bieu      Thời khóa biểu
/thong-bao           Thông báo  [badge unread]
/yeu-cau-hanh-chinh  Yêu cầu hành chính
/chuong-trinh-dt     Chương trình đào tạo

--- Giáo viên ---
/yeu-cau-sua-diem    Yêu cầu sửa điểm

--- Admin / Giáo vụ ---
/sinh-vien           Sinh viên
/giao-vien           Giáo viên
/lop-sinh-hoat       Lớp sinh hoạt
/tai-khoan           Tài khoản
/yeu-cau-hanh-chinh  Yêu cầu hành chính (duyệt)
/yeu-cau-sua-diem    Yêu cầu sửa điểm (duyệt)
/thong-bao           Thông báo (tạo + quản lý)

--- Tất cả roles ---
/nganh-hoc           Ngành học
/chuong-trinh-dt     Chương trình đào tạo
/mon-hoc             Môn học
```

---

## Checklist tổng quát

### Phase 1 — Hồ sơ & Bảng điểm
- [ ] BE: `GetByTaiKhoanIdAsync` vào `ISinhVienRepository` + `SinhVienRepository`
- [ ] BE: `GetMeAsync` vào `ISinhVienService` + `SinhVienService`
- [ ] BE: `GET /api/sinh-vien/me` endpoint trong `SinhVienController`
- [ ] BE: `DanhSachLopHPDto` + mapping (include MonHoc, HocKy)
- [ ] BE: `IDanhSachLopHPService` + `DanhSachLopHPService` với `GetBySinhVienAsync`
- [ ] BE: `DanhSachLopHPController` với `GET /me` và `GET ?lopHpId=`
- [ ] BE: Đăng ký service vào `DependencyInjection.cs`
- [ ] FE: API function `sinhVienMe()` + `danhSachLopHPMe()`
- [ ] FE: Types `DanhSachLopHPDto`
- [ ] FE: `HoSoPage.tsx` (`/ho-so`)
- [ ] FE: `BangDiemPage.tsx` (`/bang-diem`) với tabs theo học kỳ
- [ ] FE: Cập nhật `DashboardPage.tsx` cho Sinh viên
- [ ] FE: Cập nhật `AppLayout.tsx` sidebar cho Sinh viên
- [ ] FE: Thêm routes vào `App.tsx`

### Phase 2 — Yêu cầu hành chính & Sửa điểm
- [ ] BE: `YeuCauHanhChinhRepository` + interface
- [ ] BE: `YeuCauHanhChinhDto`, `CreateYeuCauHanhChinhDto`, `DuyetYeuCauDto`
- [ ] BE: `IYeuCauHanhChinhService` + `YeuCauHanhChinhService`
- [ ] BE: `YeuCauHanhChinhController` (GET, POST, PUT /duyet)
- [ ] BE: `YeuCauSuaDiemRepository` + interface
- [ ] BE: `YeuCauSuaDiemDto`, `CreateYeuCauSuaDiemDto`, `DuyetYeuCauSuaDiemDto`
- [ ] BE: `IYeuCauSuaDiemService` + `YeuCauSuaDiemService`
- [ ] BE: `YeuCauSuaDiemController` (GET, POST, PUT /duyet)
- [ ] BE: Cập nhật `UnitOfWork` + `IUnitOfWork` thêm 2 repo mới
- [ ] FE: `YeuCauHanhChinhPage.tsx` (`/yeu-cau-hanh-chinh`)
- [ ] FE: `YeuCauSuaDiemPage.tsx` (`/yeu-cau-sua-diem`)
- [ ] FE: Cập nhật sidebar, routes, types, api modules

### Phase 3 — Học phí, Thông báo, Thời khóa biểu
- [ ] BE: `HocPhiController` (GET /me, GET ?, PUT /trang-thai)
- [ ] BE: `ThongBaoController` (GET, POST, PUT /da-doc, GET /unread-count)
- [ ] BE: `ThoiKhoaBieuController` (GET /me?hocKyId=)
- [ ] FE: `HocPhiPage.tsx` (`/hoc-phi`)
- [ ] FE: `ThongBaoPage.tsx` (`/thong-bao`) với unread badge
- [ ] FE: `ThoiKhoaBieuPage.tsx` (`/thoi-khoa-bieu`)
- [ ] FE: Cập nhật sidebar, routes, types, api modules

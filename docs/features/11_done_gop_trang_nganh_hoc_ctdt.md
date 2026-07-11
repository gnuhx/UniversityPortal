# Gộp trang "Chương trình đào tạo" và "Ngành học" thành 1 trang

**Task:** #11
**Trạng thái:** done
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Hiện có 2 trang riêng biệt, nội dung liên quan chặt với nhau nhưng
tách rời khiến người dùng phải chuyển qua lại:

- **`/nganh-hoc`** (`NganhHocPage.tsx`) — danh sách Ngành học (mã
  ngành, tên ngành, ngành cha, khoa/phòng ban quản lý), CRUD dành cho
  Admin, có nút "Xem môn học" mở `NganhMonHocModal`.
- **`/chuong-trinh-dt`** (`ChuongTrinhDTPage.tsx`) — danh sách Chương
  trình đào tạo (CTĐT: mã CTĐT, ngành, khoá học), có filter "Khoá học"
  (dropdown, mặc định chọn khoá mới nhất), CRUD dành cho Admin, nút
  "Xem môn học" điều hướng sang `ChiTietCTDTPage`.

Không có khu vực nào nổi bật **ngành mà sinh viên đang học** — sinh
viên phải tự dò trong danh sách chung để tìm đúng ngành/CTĐT của
mình. Quan hệ dữ liệu hiện tại (đã kiểm chứng trong code):

```
SinhVien.LopId (nullable)
   → LopSinhHoat.ChuongTrinhDtId (bắt buộc)
        → ChuongTrinhDT.NganhId (bắt buộc)
             → NganhHoc
```

Chỉ có 1 điểm có thể null là `SinhVien.LopId` (sinh viên chưa được
phân lớp, hoặc đã tốt nghiệp/rời lớp). Từ Lop trở đi, CTĐT và Ngành
luôn tồn tại. Backend đã có endpoint `GET /api/sinh-vien/me` (dùng JWT
claim để xác định sinh viên đang đăng nhập) nhưng **chưa trả về thông
tin ngành/CTĐT** — `SinhVienDto` hiện chỉ có `LopId`, `TenLop`.

## 2. Mục tiêu

- Gộp 2 trang thành 1 trang duy nhất.
- Hiển thị **toàn bộ Ngành học** trong hệ thống (không ẩn ngành nào).
- **Khu vực A** (trên cùng, nổi bật): "Ngành học của tôi" — chỉ hiện
  với người dùng vai trò Sinh viên có ngành xác định được. Hiển thị
  tên ngành, mã ngành, khoa quản lý, CTĐT hiện tại (mã CTĐT, khoá
  học), có link xem chi tiết môn học của CTĐT đó.
- **Khu vực B**: danh sách toàn bộ ngành học khác (hoặc tất cả ngành),
  có **filter "Khoá học"** để xem CTĐT tương ứng của từng ngành theo
  khoá học đã chọn.
- Không phá vỡ quyền quản trị hiện có: Admin vẫn thêm/sửa/xoá được cả
  Ngành học và CTĐT.

## 3. Giải pháp đề xuất

### 3.1. Cấu trúc trang mới

Tạo trang mới `NganhHocPage.tsx` (thay thế nội dung cũ), đặt tại route
`/nganh-hoc`, gồm:

1. **Khu vực A — "Ngành học của tôi"** (Card nổi bật, chỉ render khi
   `vaiTro === "Sinh viên"` và API trả về ngành xác định được):
   - Gọi API `/me` đã mở rộng (xem 3.2) để lấy ngành + CTĐT hiện tại.
   - Hiển thị: Tên ngành, Mã ngành, Mã CTĐT, Khoá học. (Đã bỏ "Khoa
     quản lý" khỏi bản triển khai thật để không phải mở rộng `/me`
     thêm 1 field/join nữa — thông tin khoa đã có sẵn ở bảng Ngành học
     bên dưới nếu cần tra cứu.)
   - Nút "Xem chương trình đào tạo" → điều hướng
     `/chuong-trinh-dt/:id` (trang `ChiTietCTDTPage` giữ nguyên, không
     đổi).
   - Nếu sinh viên chưa có `LopId` (chưa phân lớp) → hiện trạng thái
     rỗng "Chưa xác định được ngành học" thay vì ẩn hẳn khu vực, để
     sinh viên biết cần liên hệ giáo vụ.

2. **Khu vực B — "Danh sách ngành học"**:
   - Tái sử dụng `CrudTable` hiện tại của `NganhHocPage` (giữ nguyên
     cột: mã ngành, tên ngành, ngành cha, khoa; CRUD gated theo
     `isAdmin` như hiện tại — không đổi hành vi này).
   - Thêm dropdown filter **"Khoá học"** phía trên bảng (tái sử dụng
     đúng logic đang có ở `ChuongTrinhDTPage.tsx` — lấy `khoaHoc`
     distinct từ `chuongTrinhDTApi.getAll()`, sort giảm dần, mặc định
     chọn khoá mới nhất, không cho clear).
   - Khi chọn 1 khoá học, nút "Xem môn học" (mở `NganhMonHocModal`) sẽ
     dùng `khoaHoc` đã chọn để resolve đúng CTĐT của ngành đó (modal
     này đã có sẵn logic dùng `khoaHoc` để build option CTĐT — chỉ cần
     truyền `khoaHoc` được chọn thay vì để modal tự chọn).

3. **Khu vực C — "Quản lý chương trình đào tạo"** (chỉ Admin thấy,
   dạng Tab hoặc Collapse riêng, ẩn hoàn toàn với vai trò khác):
   - Giữ nguyên `CrudTable` của CTĐT (mã CTĐT, ngành, khoá học,
     CRUD) — nội dung gần như y hệt `ChuongTrinhDTPage.tsx` hiện tại,
     chỉ chuyển vào làm 1 tab con trong trang gộp thay vì trang riêng.
   - Lý do tách riêng thay vì gộp chung bảng với Khu vực B: Ngành học
     và CTĐT là 2 entity khác nhau (1 ngành có thể có nhiều CTĐT theo
     nhiều khoá học) — gộp chung 1 bảng sẽ làm rối form thêm/sửa vốn
     đang rất đơn giản và tách bạch.

Dùng AntD `Tabs`: Tab mặc định "Tổng quan" (Khu vực A + B), Tab "Quản
lý CTĐT" (Khu vực C, chỉ Admin). Nếu không phải Admin, Tab quản lý
CTĐT không hiện — trang chỉ có 1 tab (có thể ẩn luôn thanh Tabs khi
chỉ có 1 tab, tuỳ lúc code).

### 3.2. Backend — mở rộng `/api/sinh-vien/me`

- Thêm vào `SinhVienDto`: `NganhId`, `MaNganh`, `TenNganh`,
  `CtdtId`, `MaCtdt`, `KhoaHoc` (nullable — null khi sinh viên chưa
  có Lop).
- Thêm repository method `GetByIdWithLopCtdtNganhAsync` trong
  `SinhVienRepository` (mở rộng `GetByIdWithLopCtdtAsync` hiện có,
  thêm `.ThenInclude(c => c.Nganh)`).
- `SinhVienService.GetMeAsync` dùng method mới, map thêm các field
  trên vào DTO trả về. Không cần fallback qua `HocBa.CtdtId` như
  `GetTotNghiepMeAsync` — nếu `LopId` null thì các field ngành/CTĐT
  trả về `null`, frontend tự hiện trạng thái rỗng (sinh viên chưa
  phân lớp thì chưa có "ngành đang học" theo đúng nghĩa, khác với tra
  cứu tốt nghiệp vốn cần nhìn cả lịch sử `HocBa`).

### 3.3. Routing & menu

- `App.tsx`: xoá route liệt kê `/chuong-trinh-dt` (trang danh sách),
  **giữ nguyên** `/chuong-trinh-dt/:id` (`ChiTietCTDTPage`, không đổi
  gì cả). Thêm redirect `/chuong-trinh-dt` → `/nganh-hoc` để không vỡ
  link cũ đã lưu (bookmark, liên kết trong `NganhMonHocModal`,
  `LopSinhHoatPage`, `ChiTietCTDTPage`,...).
- `AppLayout.tsx`: xoá mục menu "Chương trình đào tạo", đổi nhãn mục
  "Ngành học" thành **"Ngành học & CTĐT"** để phản ánh nội dung gộp
  (đề xuất — có thể giữ nguyên "Ngành học" nếu muốn nhãn ngắn gọn).

### 3.4. Điểm đã chốt (dùng đúng phương án đề xuất mặc định)

Người yêu cầu xác nhận "làm luôn" mà không chỉnh lại đề xuất, nên đã
triển khai đúng 4 điểm mặc định đã nêu ở bản nháp:

1. Nhãn menu/tiêu đề trang mới: **"Ngành học & CTĐT"**.
2. Route `/chuong-trinh-dt` (danh sách): **redirect** sang
   `/nganh-hoc`.
3. Khu vực C (Quản lý CTĐT): tách riêng thành **Tab** "Quản lý CTĐT",
   chỉ Admin thấy.
4. Chỉ vai trò **Sinh viên** thấy Khu vực A; Admin/Giáo viên/Giáo vụ
   chỉ thấy Khu vực B (và Khu vực C nếu là Admin).

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Application/DTOs/SinhVien/SinhVienDto.cs` | Thêm field `NganhId`, `MaNganh`, `TenNganh`, `CtdtId`, `MaCtdt`, `KhoaHoc` (nullable) |
| `src/UniversityPortal.Infrastructure/Repositories/SinhVienRepository.cs` | Thêm `GetByIdWithLopCtdtNganhAsync` (mở rộng `GetByIdWithLopCtdtAsync` + `.ThenInclude(c => c.Nganh)`) |
| `src/UniversityPortal.Application/Interfaces/ISinhVienRepository.cs` (hoặc file interface tương ứng) | Khai báo method mới |
| `src/UniversityPortal.Application/Services/SinhVienService.cs` | `GetMeAsync` dùng method mới, map field ngành/CTĐT vào DTO |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/pages/NganhHocPage.tsx` | Viết lại thành trang gộp: Khu vực A (Card ngành của tôi) + Khu vực B (bảng Ngành học + filter Khoá học) + Tab Khu vực C (Admin) |
| `frontend/src/pages/ChuongTrinhDTPage.tsx` | Không xoá file — chuyển nội dung thành component con được nhúng vào Tab "Quản lý CTĐT" trong `NganhHocPage.tsx`, hoặc giữ nguyên file và import lại (tuỳ cách refactor lúc code) |
| `frontend/src/components/NganhMonHocModal.tsx` | Nhận thêm prop `khoaHoc` (điều khiển từ ngoài) thay vì tự chọn mặc định, để đồng bộ với filter Khoá học ở Khu vực B |
| `frontend/src/App.tsx` | Xoá route `/chuong-trinh-dt` (danh sách), thêm `<Navigate>` redirect sang `/nganh-hoc`; giữ nguyên `/chuong-trinh-dt/:id` |
| `frontend/src/components/AppLayout.tsx` | Xoá menu item "Chương trình đào tạo"; cập nhật nhãn "Ngành học" nếu cần |
| `frontend/src/api/modules.ts` / `sinhVienMeApi` | Không đổi endpoint, chỉ cần type response mới |
| `frontend/src/types/index.ts` | Thêm field mới vào type `SinhVien` (khớp DTO backend) |
| (tuỳ chọn) `frontend/src/hooks/useKhoaHocOptions.ts` | Tách logic lấy danh sách Khoá học distinct (đang lặp lại ở `ChuongTrinhDTPage` và `LopSinhHoatPage`) thành 1 hook dùng chung, tránh copy thêm lần 3 |

### Dữ liệu / Migration

Không cần migration DB — chỉ thêm field tính toán (join) vào response
API, không đổi schema.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~1-1.5 ngày (backend nhỏ ~1-2 giờ; frontend là phần chính do gộp UI + điều chỉnh routing/menu) |
| Mức độ rủi ro | Trung bình — đổi route đang tồn tại (`/chuong-trinh-dt`), cần rà hết nơi đang link tới nó (`NganhMonHocModal`, `LopSinhHoatPage`, `DashboardPage`, `ChiTietCTDTPage`) để không bị vỡ liên kết |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert các file frontend/backend liệt kê ở mục 4; route cũ có thể khôi phục dễ dàng nếu cần lùi |

## 6. Kế hoạch triển khai

1. Rà toàn bộ nơi đang điều hướng/link tới `/chuong-trinh-dt` (danh
   sách) để liệt kê đầy đủ chỗ cần cập nhật sau khi đổi route.
2. Backend: thêm field ngành/CTĐT vào `SinhVienDto`, repository
   method mới, cập nhật `GetMeAsync`. Viết/kiểm tra thủ công bằng tài
   khoản Sinh viên thật (đã có Lop) và 1 tài khoản chưa có Lop (nếu
   có dữ liệu test) để xác nhận field trả về đúng/null đúng.
3. Frontend: tạo hook `useKhoaHocOptions` dùng chung (nếu chốt làm),
   refactor `ChuongTrinhDTPage.tsx` và `LopSinhHoatPage.tsx` dùng hook
   này.
4. Frontend: dựng Khu vực A (Card ngành của tôi) trong `NganhHocPage`,
   dùng `sinhVienMeApi.getMe()` mở rộng.
5. Frontend: dựng Khu vực B (bảng Ngành học hiện có + filter Khoá học
   mới), nối `khoaHoc` đã chọn vào `NganhMonHocModal`.
6. Frontend: dựng Khu vực C (Tab Quản lý CTĐT, Admin-only) từ nội
   dung `ChuongTrinhDTPage.tsx` hiện tại.
7. Cập nhật `App.tsx` (redirect route cũ) và `AppLayout.tsx` (menu).
8. `tsc --noEmit` + build backend.
9. Kiểm thử trên trình duyệt bằng dữ liệu thật, tối thiểu 3 vai trò:
   Sinh viên (thấy Khu vực A đúng ngành của mình), Giáo vụ/Giáo viên
   (không thấy Khu vực A, không thấy Tab quản lý), Admin (thấy đủ,
   CRUD cả Ngành học lẫn CTĐT hoạt động, filter Khoá học hoạt động
   đúng như `ChuongTrinhDTPage` cũ).
10. Rà lại toàn bộ liên kết cũ tới `/chuong-trinh-dt` (mục 1) — xác
    nhận không có liên kết chết.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Menu chỉ còn 1 mục ("Ngành học & CTĐT") thay cho 2 mục "Ngành
      học" / "Chương trình đào tạo" trước đây.
- [x] Sinh viên đăng nhập, có Lop hợp lệ → thấy Khu vực A hiển thị
      đúng ngành + CTĐT + khoá học của lớp mình. Kiểm thử với tài
      khoản `sv.an` (dữ liệu thật): hiện đúng "Hệ thống Thông tin /
      HTTT / HTTT-K23 / 2023-2027".
- [x] Sinh viên chưa có Lop → Khu vực A hiện trạng thái rỗng rõ ràng,
      không lỗi/crash trang. (Xác nhận qua code: `AutoMapper` trả
      `null` cho các field ngành/CTĐT khi `Lop == null`, UI render
      `Empty` — chưa có tài khoản test loại này trong DB thật nên
      chưa chụp ảnh minh hoạ trực tiếp.)
- [x] Khu vực B hiển thị đầy đủ tất cả Ngành học trong hệ thống (đã
      thấy đủ 5 ngành CNTT/HTTT/KHMT/KTPM/QTKD trong DB thật).
- [x] Filter "Khoá học" ở Khu vực B hoạt động, ảnh hưởng đúng tới CTĐT
      được resolve khi xem môn học của 1 ngành (đã xác nhận: chọn khoá
      "2023-2027" → mở "Xem môn học" ngành HTTT tự chọn đúng
      "HTTT-K23 — 2023-2027" thay vì CTĐT đầu tiên bất kỳ).
- [x] Admin vẫn CRUD được Ngành học (Khu vực B) và CTĐT (Khu vực C)
      như hành vi cũ, không mất tính năng nào (đã thấy nút Thêm
      mới/Sửa/Xoá ở cả 2 tab).
- [x] Route cũ `/chuong-trinh-dt` không bị vỡ (redirect sang
      `/nganh-hoc`), các link nội bộ trỏ tới nó đã được cập nhật
      (`ChiTietCTDTPage` nút "Quay lại").
- [x] `tsc --noEmit` sạch, backend build sạch (`dotnet build` 0 lỗi,
      chỉ còn cảnh báo cũ không liên quan).
- [x] Đã kiểm thử trên trình duyệt (Playwright headless, dữ liệu thật
      qua API + DB thật) cho vai trò Sinh viên và Admin — không có lỗi
      console. Vai trò Giáo viên/Giáo vụ **chưa** bấm thử trực tiếp
      trên trình duyệt, chỉ xác nhận qua code rằng điều kiện
      `isSinhVien`/`isAdmin` trong `NganhHocPage.tsx` loại các vai trò
      này khỏi Khu vực A và Tab quản lý một cách tương tự Admin/Giáo
      vụ đã hoạt động đúng ở các trang cũ.

## 8. Ghi chú

- Không cần tách `useKhoaHocOptions` dùng ở `LopSinhHoatPage.tsx` như
  dự tính ban đầu — trang đó chỉ cần danh sách CTĐT cho 1 dropdown
  chọn, không dùng đúng pattern "distinct khoá học + mặc định khoá mới
  nhất", nên không phải nơi dùng chung thứ 3. Hook mới chỉ dùng ở 2
  nơi: `ChuongTrinhDTPage.tsx` (giờ là Tab quản lý) và
  `NganhHocPage.tsx` (Khu vực B) — vẫn đáng tách vì tránh lặp logic y
  hệt giữa 2 nơi đó.
- Backend build thành công và endpoint `/api/sinh-vien/me` đã được gọi
  trực tiếp qua `curl` với tài khoản `sv.an` thật, xác nhận JOIN
  `Lop → ChuongTrinhDT → Nganh` trả đúng dữ liệu trước khi kiểm tra UI.

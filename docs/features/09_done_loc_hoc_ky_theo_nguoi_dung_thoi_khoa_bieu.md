# Thời khoá biểu (Sinh viên/Giáo viên) — dropdown Học kỳ đang liệt kê TOÀN BỘ học kỳ hệ thống, không lọc theo người dùng

**Task:** #09
**Trạng thái:** done
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Người dùng báo cáo: đăng nhập bằng tài khoản sinh viên có CTĐT
"2023-2027" (nghĩa là nhập học năm 2023), nhưng ở trang "Thời khoá
biểu" dropdown "Chọn học kỳ" vẫn liệt kê được "Học kỳ 2021-2022" — một
học kỳ **trước khi sinh viên này nhập học**.

Đã kiểm tra `ThoiKhoaBieuPage.tsx` (`MyLichHoc`, dùng chung cho cả
Sinh viên lẫn Giáo viên vì `ThoiKhoaBieuPage` chỉ tách nhánh
Admin/Giáo vụ ở component cấp trên):

```tsx
const { data: hocKys = [] } = useQuery({ queryKey: ["hoc-ky"], queryFn: () => hocKyApi.getAll() });
```

`hocKyApi.getAll()` gọi `GET /api/hoc-ky/all` — trả về **toàn bộ**
`hoc_ky` của **toàn trường**, mọi năm học, không lọc theo người dùng
đang đăng nhập. Dropdown build thẳng từ danh sách này
(`hocKys.map(...)`) nên bất kỳ sinh viên/giáo viên nào cũng thấy được
mọi học kỳ có trong hệ thống, kể cả học kỳ không liên quan gì đến họ
(trước khi họ nhập học, hoặc sau khi họ đã tốt nghiệp/rời trường).

Đã kiểm tra `HocKyController`/`HocKyService`/`IHocKyRepository`
(task #07 vừa thêm CRUD cho học kỳ): chưa có endpoint nào trả về "học
kỳ liên quan tới người dùng hiện tại" — chỉ có `GET /api/hoc-ky`
(phân trang, lọc theo `namHocId`) và `GET /api/hoc-ky/all` (toàn bộ,
không lọc).

So sánh với `ThoiKhoaBieuController`: endpoint `GET /api/thoi-khoa-bieu/me`
đã tự phân nhánh Sinh viên/Giáo viên và join đúng dữ liệu của người đó
(`GetForSinhVienMeAsync`/`GetForGiaoVienMeAsync` trong
`ThoiKhoaBieuService.cs`) — cùng pattern này chưa được áp dụng cho
dropdown Học kỳ.

Đã kiểm tra `HocPhiPage.tsx` để loại trừ: trang này gọi
`hocKyApi.getAll()` với `enabled: isAdmin` — chỉ Admin/Giáo vụ mới tải
danh sách học kỳ đầy đủ (dùng cho form tạo/lọc), sinh viên chỉ thấy
`hocPhiApi.getMe()` (dữ liệu học phí thật của chính họ, tự nhiên đã
đúng phạm vi) — **không** bị lỗi tương tự.

## 2. Mục tiêu

- Dropdown "Chọn học kỳ" ở trang Thời khoá biểu (Sinh viên/Giáo viên)
  chỉ hiển thị học kỳ **thực sự liên quan** tới người dùng đang đăng
  nhập:
  - Sinh viên: học kỳ mà họ có ít nhất 1 lớp học phần đã đăng ký
    (`danh_sach_lop_hp`).
  - Giáo viên: học kỳ mà họ có ít nhất 1 lớp học phần đang dạy
    (`lop_hoc_phan.giao_vien_id`).

## 3. Giải pháp đề xuất

Thêm endpoint `GET /api/hoc-ky/me` (theo đúng pattern
`GET /api/thoi-khoa-bieu/me` đã có), tự phân nhánh theo vai trò:

1. `IHocKyRepository` + `HocKyRepository`: thêm
   `GetForSinhVienAsync(int sinhVienId)` (join `danh_sach_lop_hp` →
   `lop_hoc_phan` → `hoc_ky`, `Distinct`, include `NamHoc`, sắp giảm
   dần theo `ngay_bat_dau`) và `GetForGiaoVienAsync(int giaoVienId)`
   (join `lop_hoc_phan` → `hoc_ky` theo `giao_vien_id`, tương tự).
2. `IHocKyService` + `HocKyService`: thêm `GetForSinhVienMeAsync(int
   taiKhoanId)` và `GetForGiaoVienMeAsync(int taiKhoanId)` — tra
   `SinhVien`/`GiaoVien` theo `taiKhoanId` (dùng lại
   `uow.SinhViens.GetByTaiKhoanIdAsync`/`uow.GiaoViens.GetByTaiKhoanIdAsync`
   đã có), rồi gọi repository method tương ứng ở bước 1.
3. `HocKyController`: thêm `GET /api/hoc-ky/me`
   (`[Authorize(Roles = "Sinh viên,Giáo viên")]`), branch theo
   `User.IsInRole("Sinh viên")` — copy đúng cấu trúc
   `ThoiKhoaBieuController.GetMe`.
4. Frontend: thêm `hocKyApi.getMe()` (gọi `/hoc-ky/me`); đổi
   `MyLichHoc` trong `ThoiKhoaBieuPage.tsx` từ `hocKyApi.getAll()`
   sang `hocKyApi.getMe()`.

**Không đổi** `AdminThoiKhoaBieu` (nhánh Admin/Giáo vụ) — Admin/Giáo vụ
cần thấy và lọc theo mọi học kỳ để quản lý toàn trường, giữ nguyên
`hocKyApi.getAll()` ở đó.

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `Application/Interfaces/Repositories/IHocKyRepository.cs` | Thêm `GetForSinhVienAsync(int sinhVienId)`, `GetForGiaoVienAsync(int giaoVienId)` |
| `Infrastructure/Repositories/HocKyRepository.cs` | Cài đặt 2 hàm trên — join qua `danh_sach_lop_hp`/`lop_hoc_phan`, `Distinct`, sắp giảm dần theo `ngay_bat_dau` |
| `Application/Interfaces/Services/IHocKyService.cs` + `Services/HocKyService.cs` | Thêm `GetForSinhVienMeAsync(int taiKhoanId)`, `GetForGiaoVienMeAsync(int taiKhoanId)` |
| `API/Controllers/HocKyController.cs` | Thêm `GET /api/hoc-ky/me` (`Sinh viên,Giáo viên`), branch theo vai trò |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/api/modules.ts` | Thêm `hocKyApi.getMe()` → `GET /hoc-ky/me` |
| `frontend/src/pages/ThoiKhoaBieuPage.tsx` | `MyLichHoc`: đổi `hocKyApi.getAll()` → `hocKyApi.getMe()` (chỉ nhánh Sinh viên/Giáo viên, không đổi `AdminThoiKhoaBieu`) |

### Dữ liệu / Migration

Không cần — không đổi schema, chỉ thêm endpoint đọc lọc theo user.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~20–30 phút (endpoint + 1 dòng đổi ở frontend) |
| Mức độ rủi ro | Thấp — chỉ thêm endpoint mới (`GET /me`), không đổi hành vi endpoint cũ (`/all`, dùng ở `AdminThoiKhoaBieu`/`HocPhiPage`) |
| Ảnh hưởng dữ liệu hiện có | Không — thuần đọc |
| Khả năng rollback | Revert 1 dòng frontend (`getMe` → `getAll`) + xoá endpoint mới |

## 6. Kế hoạch triển khai

1. Thêm `GetForSinhVienAsync`/`GetForGiaoVienAsync` vào
   `IHocKyRepository`/`HocKyRepository`.
2. Thêm `GetForSinhVienMeAsync`/`GetForGiaoVienMeAsync` vào
   `IHocKyService`/`HocKyService`.
3. Thêm `GET /api/hoc-ky/me` vào `HocKyController`.
4. `dotnet build`; test qua REST client bằng tài khoản sinh viên có
   CTĐT 2023-2027: xác nhận `GET /api/hoc-ky/me` **không** trả về học
   kỳ 2021-2022 (trừ khi sinh viên đó thực sự có lớp học phần trong
   học kỳ đó — vd. học cải thiện/trả nợ môn của khoá trước, trường hợp
   hợp lệ cần giữ).
5. Frontend: thêm `hocKyApi.getMe()`, đổi `MyLichHoc` dùng hàm này.
6. `tsc --noEmit`; test UI bằng tài khoản sinh viên thật: dropdown chỉ
   còn học kỳ họ thực sự có lớp; test thêm bằng tài khoản giáo viên.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] `GET /api/hoc-ky/me` (Sinh viên) chỉ trả về học kỳ mà sinh viên
      đó có ít nhất 1 dòng `danh_sach_lop_hp`.
- [x] `GET /api/hoc-ky/me` (Giáo viên) chỉ trả về học kỳ mà giáo viên
      đó có ít nhất 1 `lop_hoc_phan` đang dạy.
- [x] Trang Thời khoá biểu của sinh viên có CTĐT 2023-2027 (không có
      lớp học phần nào ở học kỳ 2021-2022) **không** còn hiện học kỳ
      2021-2022 trong dropdown.
- [x] Trang Thời khoá biểu của Admin/Giáo vụ không đổi hành vi (vẫn
      thấy mọi học kỳ để lọc/quản lý toàn trường).
- [x] Build backend + frontend không lỗi, `tsc --noEmit` sạch.
- [x] Kiểm thử thật trên tài khoản sinh viên `sv.phuong` (HTTT23A,
      khoá 2023 — đúng kịch bản người dùng báo cáo) và tài khoản giáo
      viên `gv.hoa`: cả hai đều chỉ còn thấy "Học kỳ 1 (2024-2025)" ở
      dropdown thay vì toàn bộ 12 học kỳ như trước khi sửa. Xem mục 8.

## 8. Ghi chú

- **Lưu ý khi kiểm thử:** cổng 8080 trên máy dev đang có 1 tiến trình
  backend chạy sẵn dưới quyền `root` (`dotnet UniversityPortal.API.dll`,
  không phải do phiên làm việc này khởi động) — không tắt/khởi động
  lại tiến trình đó để tránh ảnh hưởng service đang chạy. Đã kiểm thử
  bằng cách chạy 1 instance backend tạm thời ở cổng khác (8081, cùng
  connection string tới DB thật) chỉ để xác minh, sau đó tắt đi ngay;
  tiến trình gốc ở 8080 không bị đụng tới.
- Sinh viên học cải thiện/trả nợ môn ở một lớp thuộc học kỳ **trước**
  năm nhập học chính thức (trường hợp hợp lệ, có thật trong dữ liệu
  mẫu — xem `seed_test_data.sql`) vẫn sẽ thấy đúng học kỳ đó, vì bộ lọc
  dựa trên "có `danh_sach_lop_hp` thật" chứ không suy luận cứng từ
  `khoa_hoc` (text tự do, không đáng tin cậy — xem hạn chế đã ghi ở
  task #04). Đây là hành vi đúng, không phải bug.
- Nếu sinh viên/giáo viên chưa có bất kỳ lớp học phần nào (mới tạo tài
  khoản, chưa ghi danh), dropdown sẽ rỗng — trang hiện lịch trống thay
  vì lịch của một học kỳ không liên quan. Chấp nhận được, đúng tinh
  thần "chỉ hiện dữ liệu thật của người dùng".

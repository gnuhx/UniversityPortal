# Trang quản lý Học kỳ độc lập + thông báo rõ ràng khi xoá bị chặn

**Task:** #12
**Trạng thái:** done
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Người yêu cầu đã thử xoá 1 học kỳ và gặp lỗi SQL thô:

> The DELETE statement conflicted with the REFERENCE constraint
> "FK_chi_tiet_ctdt_hoc_ky_hoc_ky_id". The conflict occurred in
> database "db_acaeb4_datn", table "dbo.chi_tiet_ctdt", column
> 'hoc_ky_id'.

**Đã xác minh trong code**: lỗi này xảy ra khi chạy `DELETE` trực tiếp
bằng SQL client (file `.sql` người dùng đang mở trong IDE trỏ tới DB
`db_acaeb4_datn` trên `site4now.net`), **bỏ qua hoàn toàn tầng ứng
dụng**. Qua giao diện Admin, `HocKyService.DeleteAsync`
(`src/UniversityPortal.Application/Services/HocKyService.cs:75-92`) đã
có sẵn 3 bước kiểm tra trước khi xoá và ném `BadRequestException` với
thông báo tiếng Việt thân thiện nếu còn:

1. Lớp học phần liên kết (`uow.LopHocPhans.GetPagedByHocKyAsync`)
2. Chi tiết CTĐT liên kết (`uow.ChiTietCTDTs.ExistsByHocKyAsync`)
3. Khoản học phí liên kết (`uow.HocPhis.ExistsByHocKyAsync`)

`GlobalExceptionMiddleware`
(`src/UniversityPortal.API/Middleware/GlobalExceptionMiddleware.cs`)
đã ánh xạ `BadRequestException` → HTTP 400 + JSON
`{ success: false, message }`. Vậy về mặt kỹ thuật, **thao tác xoá
qua Admin UI đã an toàn** — vấn đề thực sự nằm ở 2 chỗ:

- **Thông báo còn chung chung, dừng ở điều kiện đầu gặp**: ví dụ nếu
  vừa có lớp học phần vừa có chi tiết CTĐT, Admin chỉ thấy "Không thể
  xoá học kỳ đang có lớp học phần liên kết." → xoá xong lớp học phần,
  bấm xoá lại mới biết tiếp còn chi tiết CTĐT → không biết trước toàn
  bộ phạm vi việc cần làm, và thông báo không nói **số lượng** hay
  **nên vào trang nào** để xử lý.
- **Học kỳ hiện chỉ quản lý được qua trang lồng trong Năm học**
  (`/nam-hoc/:id` → `HocKyPage.tsx`), không có 1 trang "Học kỳ" độc
  lập để xem/quản lý toàn bộ học kỳ của mọi năm học cùng lúc.

Việc chạy `DELETE` trực tiếp bằng SQL (như người yêu cầu vừa làm) luôn
bỏ qua mọi guard này — đây không phải lỗi của ứng dụng, nhưng đáng ghi
chú lại để tránh lặp lại (xem mục 8).

## 2. Mục tiêu

- Có **1 trang Admin độc lập** để quản lý Học kỳ (không bắt buộc phải
  vào từng Năm học mới xem được).
- Khi xoá 1 học kỳ thất bại vì còn dữ liệu liên kết, thông báo phải
  cho Admin biết **đầy đủ, chính xác 1 lần**: loại dữ liệu nào, bao
  nhiêu bản ghi, và **nên vào trang nào để xử lý trước** — không cần
  thử xoá nhiều lần mới biết hết.

## 3. Giải pháp đề xuất

### 3.1. KHÔNG tự động cascade xoá — chỉ chặn + thông báo rõ (đã xác nhận, đã triển khai)

Yêu cầu gốc có nhắc đến việc xoá học kỳ "sẽ xoá luôn mọi lớp và dữ liệu
liên quan để xử lý xung đột". Sau khi kiểm tra sơ đồ khoá ngoại thật,
đề xuất **KHÔNG làm cascade tự động** — người yêu cầu đã xác nhận
("ok do it now") sau khi thấy lý do cụ thể dưới đây:

- `lop_hoc_phan.chi_tiet_ctdt_id` là **Restrict**
  (`LopHocPhanConfiguration.cs:23`) — một dòng `chi_tiet_ctdt` (môn
  học ở 1 học kỳ trong chương trình đào tạo) có thể bị bất kỳ lớp học
  phần nào tham chiếu, **kể cả lớp học phần được mở ở một học kỳ khác**
  (tình huống học lại/học cải thiện: sinh viên trượt môn ở học kỳ X,
  môn đó theo chương trình gốc thuộc `chi_tiet_ctdt` của học kỳ X,
  nhưng lớp học lại thực tế được mở ở học kỳ Y sau này — vẫn trỏ về
  cùng `chi_tiet_ctdt_id`).
- Do đó, nếu cascade xoá "học kỳ X" bằng cách xoá luôn các
  `chi_tiet_ctdt` có `hoc_ky_id = X`, rồi cascade tiếp xuống
  `lop_hoc_phan` tham chiếu chúng, **có thể xoá nhầm lớp học phần (và
  toàn bộ điểm, học phí, thời khoá biểu, yêu cầu sửa điểm của lớp đó)
  đang thuộc về một học kỳ hoàn toàn khác** mà Admin không hề định
  đụng tới. Đây là kiểu mất dữ liệu khó phát hiện ngay và không thể
  hoàn tác.
- Ngược lại, phương án "chặn + thông báo rõ" an toàn tuyệt đối (không
  bao giờ xoá nhầm dữ liệu học kỳ khác), đúng với đúng bản chất
  `DeleteBehavior.Restrict` mà DB đã thiết kế sẵn cho `hoc_ky_id`
  trong cả 3 bảng `chi_tiet_ctdt`, `lop_hoc_phan`, `hoc_phi`
  (`ChiTietCTDTConfiguration.cs:23`, `LopHocPhanConfiguration.cs:24`,
  `HocPhiConfiguration.cs:22`) — và cũng khớp với yêu cầu cụ thể hơn
  ở tin nhắn sau của người yêu cầu ("cho biết cái gì và ở đâu cần
  xoá" thay vì tự xoá hộ).

**Nếu vẫn muốn có lựa chọn "xoá cưỡng bức" cho Admin hiểu rõ rủi ro**,
có thể làm thêm ở giai đoạn sau (không nằm trong phạm vi task này).

### 3.2. Backend — gộp toàn bộ điều kiện kiểm tra, trả danh sách lý do cụ thể

- Thêm field `Errors` (`IEnumerable<string>?`) vào
  `BadRequestException` (hiện tại chỉ có `message` string) — dùng lại
  đúng field `Errors` đã có sẵn trong `ApiResponseDto<T>.Fail(message,
  errors)` (`src/UniversityPortal.Application/DTOs/Common/ApiResponseDto.cs:13`)
  nhưng hiện chưa nơi nào truyền vào.
- `GlobalExceptionMiddleware.HandleExceptionAsync` truyền
  `(ex as BadRequestException)?.Errors` vào `ApiResponseDto<object>.Fail`
  thay vì bỏ trống.
- Viết lại `HocKyService.DeleteAsync`: kiểm tra **cả 3 điều kiện
  trong 1 lần** (không `return`/`throw` sớm ở điều kiện đầu), đếm số
  lượng thực tế, gom thành danh sách gợi ý, vd:
  ```
  message: "Không thể xoá học kỳ 'HK1 (2023-2024)' vì còn dữ liệu liên kết."
  errors: [
    "5 lớp học phần đang dùng học kỳ này — xử lý tại trang \"Lớp học phần\".",
    "12 môn học trong chương trình đào tạo áp dụng học kỳ này — xử lý tại trang \"Ngành học & CTĐT\" → tab \"Quản lý CTĐT\" → Xem môn học.",
    "8 khoản học phí đã lập cho học kỳ này — xử lý tại trang \"Học phí\"."
  ]
  ```
  Chỉ đưa vào danh sách các mục có số lượng > 0.
- Đổi `ChiTietCTDTRepository.ExistsByHocKyAsync` /
  `HocPhiRepository.ExistsByHocKyAsync` (hiện trả `bool`, chỉ 1 nơi
  gọi — chính là `HocKyService.DeleteAsync`) thành
  `CountByHocKyAsync` trả `int`, để lấy được số lượng thật thay vì chỉ
  biết có/không. `LopHocPhan` đã có sẵn số lượng qua
  `GetPagedByHocKyAsync(id, 1, 1).Total`, không cần thêm method mới.

### 3.3. Frontend — hiển thị danh sách lý do dạng thông báo rõ ràng

- `CrudTable.tsx` (dùng chung cho toàn bộ trang CRUD, bao gồm Học kỳ):
  sửa `deleteMutation.onError` — nếu response có `errors` (mảng, đã có
  sẵn trong type `ApiResponse<T>` ở
  `frontend/src/types/index.ts:1-6`) thì hiện `Modal.error` với tiêu
  đề là `message` và nội dung là danh sách `errors` dạng gạch đầu
  dòng; nếu không có `errors` thì giữ nguyên hành vi cũ
  (`message.error`). Thay đổi này chung cho `CrudTable`, không riêng
  Học kỳ — trang nào sau này cũng trả `errors` list khi chặn xoá sẽ tự
  động được hưởng thông báo chi tiết này.

### 3.4. Trang "Học kỳ" độc lập (không còn chỉ xem qua Năm học)

- Route mới `/hoc-ky` (menu mục "Học kỳ" cạnh "Năm học"), dùng lại gần
  như nguyên `HocKyPage.tsx` hiện tại nhưng:
  - Bỏ bắt buộc `namHocId` từ URL param; đọc từ query string
    `?namHocId=` (tuỳ chọn — có thì lọc sẵn, không có thì hiện tất cả
    học kỳ mọi năm học).
  - Thêm cột **"Năm học"** vào bảng (hiện `HocKyDto` đã có `TenNamHoc`
    theo mapping `HocKy → HocKyDto`, kiểm tra lại DTO có field này
    chưa, nếu chưa thì thêm) và thêm dropdown filter "Năm học" (lấy từ
    `namHocApi.getAll()`).
  - Form thêm/sửa vẫn cần chọn Năm học (bổ sung field `namHocId` dạng
    select vào `formFields` — hiện tại field này bị "ẩn" khỏi form vì
    được set cứng qua `extraParams`/tham số route, giờ phải cho chọn
    tường minh vì trang không còn gắn với 1 năm học cố định).
- `App.tsx`: đổi route `/nam-hoc/:id` thành redirect sang
  `/hoc-ky?namHocId=:id` (theo đúng pattern đã dùng ở task #11 khi gộp
  trang Ngành học/CTĐT — giữ link cũ không vỡ).
- `AppLayout.tsx`: thêm menu "Học kỳ" → `/hoc-ky`.
- `NamHocPage.tsx`: nút "Xem học kỳ" đổi điều hướng sang
  `/hoc-ky?namHocId=${record.id}` thay vì `/nam-hoc/${record.id}`.

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Domain/Exceptions/BadRequestException.cs` | Thêm property `Errors` (`IEnumerable<string>?`), constructor nhận thêm tham số tuỳ chọn |
| `src/UniversityPortal.API/Middleware/GlobalExceptionMiddleware.cs` | Truyền `Errors` từ `BadRequestException` vào `ApiResponseDto<object>.Fail(message, errors)` |
| `src/UniversityPortal.Application/Services/HocKyService.cs` | Viết lại `DeleteAsync`: kiểm tra cả 3 điều kiện, đếm số lượng, gộp danh sách lý do vào `BadRequestException` |
| `src/UniversityPortal.Application/Interfaces/Repositories/IChiTietCTDTRepository.cs` | Đổi `ExistsByHocKyAsync` → `CountByHocKyAsync` (trả `int`) |
| `src/UniversityPortal.Infrastructure/Repositories/ChiTietCTDTRepository.cs` | Cài đặt `CountByHocKyAsync` |
| `src/UniversityPortal.Application/Interfaces/Repositories/IHocPhiRepository.cs` | Đổi `ExistsByHocKyAsync` → `CountByHocKyAsync` (trả `int`) |
| `src/UniversityPortal.Infrastructure/Repositories/HocPhiRepository.cs` | Cài đặt `CountByHocKyAsync` |
| `src/UniversityPortal.Application/DTOs/HocKy/HocKyDto.cs` | Kiểm tra/bổ sung `TenNamHoc` nếu chưa có (cần cho cột "Năm học" ở trang mới) |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/components/CrudTable.tsx` | `deleteMutation.onError`: hiện `Modal.error` liệt kê `errors[]` khi có, fallback `message.error` như cũ khi không có |
| `frontend/src/pages/HocKyPage.tsx` | Bỏ phụ thuộc cứng vào route param `id`; đọc `namHocId` từ query string (tuỳ chọn); thêm cột + filter "Năm học"; thêm field `namHocId` (select) vào form |
| `frontend/src/App.tsx` | Route `/hoc-ky` (mới) → `HocKyPage`; `/nam-hoc/:id` → `<Navigate>` sang `/hoc-ky?namHocId=:id` |
| `frontend/src/components/AppLayout.tsx` | Thêm menu "Học kỳ" → `/hoc-ky` |
| `frontend/src/pages/NamHocPage.tsx` | Nút "Xem học kỳ" điều hướng sang `/hoc-ky?namHocId=...` |
| `frontend/src/types/index.ts` | `HocKy` type: đảm bảo có `tenNamHoc` (đã có sẵn, xác nhận lại khi code) |

### Dữ liệu / Migration

Không cần migration DB — không đổi schema, chỉ đổi cách kiểm tra và
hiển thị lỗi.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~0.5-1 ngày (backend nhỏ ~1-2 giờ; frontend chủ yếu là chuyển `HocKyPage` từ route-param sang query-param + thêm filter) |
| Mức độ rủi ro | Thấp — không đổi hành vi xoá (vẫn chặn như cũ), chỉ làm thông báo rõ hơn; đổi route `/nam-hoc/:id` cần rà lại nơi đang link tới (đã biết trước từ `NamHocPage.tsx`) |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert các file liệt kê ở mục 4 |

## 6. Kế hoạch triển khai

1. Backend: mở rộng `BadRequestException` với `Errors`, cập nhật
   `GlobalExceptionMiddleware` truyền qua.
2. Backend: đổi `ExistsByHocKyAsync` → `CountByHocKyAsync` ở
   `ChiTietCTDTRepository`/`HocPhiRepository` + interface tương ứng.
3. Backend: viết lại `HocKyService.DeleteAsync` gộp toàn bộ điều kiện,
   build danh sách lý do cụ thể kèm số lượng + gợi ý trang xử lý.
4. Backend: `dotnet build`; kiểm tra thủ công qua Swagger/curl với 1
   học kỳ có đồng thời nhiều loại dữ liệu liên kết, xác nhận
   `errors[]` trả đủ, đúng số lượng.
5. Frontend: sửa `CrudTable.tsx` để hiện `Modal.error` liệt kê
   `errors[]` khi có.
6. Frontend: chuyển `HocKyPage.tsx` sang đọc `namHocId` từ query
   string, thêm cột/filter "Năm học", thêm field năm học vào form.
7. Frontend: cập nhật `App.tsx` (route mới + redirect route cũ),
   `AppLayout.tsx` (menu), `NamHocPage.tsx` (nút điều hướng).
8. `tsc --noEmit` + `dotnet build`.
9. Kiểm thử trên trình duyệt bằng dữ liệu thật: tạo/chọn 1 học kỳ có
   lớp học phần + chi tiết CTĐT + học phí, bấm Xoá, xác nhận thông báo
   liệt kê đủ cả 3 loại kèm số lượng đúng và tên trang xử lý; xoá 1
   học kỳ trống (không có gì liên kết) để xác nhận xoá thành công bình
   thường; kiểm tra trang `/hoc-ky` hiện đủ học kỳ mọi năm học, filter
   theo năm học hoạt động đúng; kiểm tra `/nam-hoc/:id` cũ redirect
   đúng sang `/hoc-ky?namHocId=...`.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Trang "Học kỳ" độc lập tại `/hoc-ky`, có trong menu, hiện đủ học
      kỳ của mọi năm học, lọc được theo năm học. Đã kiểm thử: trang
      hiện đủ 10 học kỳ thuộc 5 năm học khác nhau (2021-2022 →
      2024-2025) trên cùng 1 bảng.
- [x] Xoá học kỳ còn dữ liệu liên kết → 1 lần xoá hiện đủ các lý do kèm
      số lượng chính xác và tên trang cần xử lý trước. Đã kiểm thử với
      dữ liệu thật: xoá "HK1 2023-2024" (id=9) hiện đúng
      `Modal.error` liệt kê "6 môn học trong chương trình đào tạo... —
      xử lý tại trang Ngành học & CTĐT..." và "4 khoản học phí... — xử
      lý tại trang Học phí." (2 loại cùng lúc, đúng số lượng thật từ
      DB) — không phải thử xoá nhiều lần mới biết hết.
- [x] Xoá học kỳ không còn dữ liệu liên kết nào → xoá thành công bình
      thường như trước. Đã kiểm thử: tạo học kỳ test
      "TEST-XOA-HK99", xoá thành công, không còn trong danh sách sau
      khi xoá (dọn sạch dữ liệu test, không để lại rác).
- [x] Route cũ `/nam-hoc/:id` redirect đúng sang `/hoc-ky?namHocId=...`,
      không vỡ link. Đã kiểm thử: `/nam-hoc/1` → redirect thành công
      sang `/hoc-ky?namHocId=1`.
- [x] `CrudTable` vẫn hoạt động đúng như cũ cho các trang khác khi
      backend không trả `errors[]` (fallback `message.error`) — xác
      nhận qua code: nhánh `else` trong `onError` mới giữ nguyên hành
      vi `message.error` cũ, không đổi cho các trang khác.
- [x] `tsc --noEmit` sạch, `dotnet build` sạch.
- [x] Đã kiểm thử trên trình duyệt với dữ liệu thật (Playwright headless
      + backend/DB thật, vai trò Admin).

## 8. Ghi chú

- **Không thao tác xoá dữ liệu trực tiếp bằng SQL** trên DB đang chạy
  ứng dụng thật (`db_acaeb4_datn`) — thao tác này bỏ qua toàn bộ guard
  nghiệp vụ (ràng buộc theo học kỳ khác, xoá mềm tài khoản sinh viên,
  v.v.) mà tầng ứng dụng đã cố tình thiết kế. Nên luôn xoá qua trang
  Admin để được cảnh báo đầy đủ trước khi mất dữ liệu.
- Chưa làm tính năng "xem chi tiết từng bản ghi đang chặn" (vd bấm vào
  "5 lớp học phần" để nhảy thẳng tới danh sách 5 lớp đó, lọc sẵn theo
  học kỳ) — nếu cần trải nghiệm sâu hơn nữa thì làm ở task riêng sau,
  phạm vi task này dừng ở việc cho biết đúng loại + số lượng + tên
  trang.
- Không phát hiện học kỳ thật nào trong DB hiện có bị chặn bởi lớp học
  phần (`lop_hoc_phan`) — dữ liệu mẫu hiện tại chỉ tạo chi tiết CTĐT và
  học phí cho các học kỳ cũ, chưa có lớp học phần nào gắn trực tiếp.
  Logic đếm lớp học phần đã được viết và build thành công, nhưng chưa
  có cơ hội quan sát nhánh này hiện trên `Modal.error` với dữ liệu thật
  — nếu cần chắc chắn 100%, có thể tạo 1 lớp học phần test và thử lại.

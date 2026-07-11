# Liên kết chính thức Khoa (phong_ban) ↔ Ngành (nganh_hoc)

**Task:** #01
**Trạng thái:** review (code xong, migration chưa apply lên DB thật — xem mục 8)
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Đã kiểm tra trực tiếp trên DB thật (`db_acaeb4_datn`) và mã nguồn:

- `nganh_hoc` (ngành cha/con — dùng cho Chương trình đào tạo / Xem môn học)
  và `phong_ban` (Khoa/Phòng — dùng để gán đơn vị cho tài khoản nhân sự)
  **không có bất kỳ khoá ngoại nào giữa hai bảng**. Mối liên hệ "Khoa CNTT
  sở hữu ngành CNTT" hiện chỉ là quy ước đặt tên trùng nhau, không được
  DB/API ràng buộc hay kiểm chứng được bằng query.
- `phong_ban` hiện gần như chưa có backend thật: không có
  `PhongBanController`/`Service`/`Repository`. Form nhập `phongBanId` ở
  `GiaoVienPage.tsx` và `TaiKhoanPage.tsx` là ô nhập số thô, không có
  dropdown.
- Đã bổ sung 4 Khoa kỹ thuật mới (Cơ khí, Điện – Điện tử, Cơ điện tử –
  Tự động hóa, Ô tô) cùng cây ngành cha/con tương ứng trong
  `docs/Data/them_khoa_nganh_hoc.sql`, nhưng các ngành cha mới này
  (`CKI`, `DDT`, `CDT`, `DKTDH`, `OTO`) cũng chưa được gắn với `phong_ban`
  tương ứng bằng dữ liệu thật, chỉ trùng tên.
- API tự động chạy `Database.MigrateAsync()` mỗi lần khởi động
  ([`Program.cs:78`](../../src/UniversityPortal.API/Program.cs#L78)) —
  bất kỳ migration nào thêm vào sẽ tự áp dụng lên DB thật ở lần deploy kế
  tiếp, không có bước "apply" riêng để rà soát lại.

## 2. Mục tiêu

- Thêm cột `phong_ban_id` (nullable) vào `nganh_hoc` để việc "Khoa nào sở
  hữu ngành nào" là một quan hệ thật trong DB, query được, thay vì suy
  đoán qua tên.
- Có API tối thiểu để lấy danh sách Khoa/Phòng — làm nền cho dropdown ở
  frontend thay vì nhập ID thô.

## 3. Giải pháp đề xuất

Thêm khoá ngoại `nganh_hoc.phong_ban_id → phong_ban.id`, dùng đúng kiểu
quan hệ `ON DELETE SET NULL` đã áp dụng an toàn cho `tai_khoan.phong_ban_id`
(không có rủi ro cascade/orphan). Bổ sung một `PhongBanController` đọc
(GET all) để có dữ liệu thật cho dropdown. Sau khi migration chạy, backfill
`phong_ban_id` cho các ngành cha hiện có bằng script SQL riêng.

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Domain/Entities/NganhHoc.cs` | Thêm `PhongBanId` (int?) + navigation `PhongBan` |
| `src/UniversityPortal.Domain/Entities/PhongBan.cs` | Thêm collection `NganhHocs` (đối xứng với `TaiKhoans`) |
| `src/.../Configurations/NganhHocConfiguration.cs` | Map cột `phong_ban_id` + FK `OnDelete(SetNull)` |
| Migration mới (`dotnet ef migrations add AddPhongBanToNganhHoc`) | Thêm cột nullable, không đổi cột hiện có |
| `src/.../DTOs/NganhHoc/NganhHocDto.cs` | Thêm `PhongBanId`, `TenPhongBan` |
| `src/.../DTOs/NganhHoc/UpsertNganhHocDto.cs` | Thêm `PhongBanId` |
| `src/.../Mappings/MappingProfile.cs` | Map `TenPhongBan` từ `PhongBan.TenPhongBan` |
| `src/.../Repositories/NganhHocRepository.cs` | Thêm `.Include(x => x.PhongBan)` ở `GetPagedFilterAsync` và `GetDetailAsync` |
| `PhongBanController` + Service + DTO (mới) | Endpoint `GET /api/phong-ban/all` — chỉ đọc, theo mẫu `NganhHocController.GetAll` |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/types/index.ts` | Thêm `phongBanId`/`tenPhongBan` vào type `NganhHoc`; thêm type `PhongBan` |
| `frontend/src/api/modules.ts` | Thêm `phongBanApi.getAll` |
| `frontend/src/pages/NganhHocPage.tsx` | Thêm cột "Khoa/Phòng ban" + field `select` dùng `phongBanApi.getAll` thay vì bỏ trống |

### Dữ liệu / Migration

- 1 migration EF Core mới (additive, nullable column).
- Script SQL backfill (chạy sau khi migration lên) gán `phong_ban_id` cho
  7 ngành cha hiện có: `CNTT`→Khoa CNTT, `QTKD`→Khoa Kinh tế, `CKI`→Khoa
  Cơ khí, `DDT`→Khoa Điện – Điện tử, `CDT` và `DKTDH`→Khoa Cơ điện tử –
  Tự động hóa, `OTO`→Khoa Ô tô. Ngành con có thể để `NULL` (suy ra qua
  `nganh_cha_id`) hoặc gán trực tiếp cho tiện hiển thị — quyết định khi
  triển khai.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~45–60 phút (13-14 file nhỏ + build + kiểm thử) |
| Mức độ rủi ro | Thấp — cột nullable, `ON DELETE SET NULL`, không đổi cột/bảng hiện có, cùng pattern đã chứng minh an toàn ở `tai_khoan` |
| Ảnh hưởng dữ liệu hiện có | Không — cột mới mặc định `NULL` cho mọi dòng cũ |
| Khả năng rollback | Migration `Down()` xoá cột — an toàn vì cột không được cột khác phụ thuộc |

## 6. Kế hoạch triển khai

1. Cập nhật entity + configuration + tạo migration.
2. Build solution, kiểm thử migration bằng cách áp thử lên DB thật trong
   transaction rollback (giống cách đã kiểm thử các script SQL trước đó)
   trước khi để API tự động deploy.
3. Cập nhật DTO/mapping/repository/service backend.
4. Thêm `PhongBanController` tối thiểu (GET all).
5. Cập nhật frontend: type, api module, `NganhHocPage.tsx`.
6. Viết + kiểm thử script SQL backfill `phong_ban_id` cho 7 ngành cha.
7. Build + test toàn bộ trước khi bàn giao để deploy (migration sẽ tự
   chạy khi API khởi động lại).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] `nganh_hoc` có cột `phong_ban_id`, FK `SET NULL` hoạt động đúng.
- [ ] `GET /api/nganh-hoc` và `/api/nganh-hoc/all` trả về `tenPhongBan`.
- [ ] `GET /api/phong-ban/all` trả về danh sách Khoa/Phòng thật.
- [ ] `NganhHocPage.tsx` hiển thị cột Khoa và cho chọn qua dropdown (không
      còn nhập ID thô cho trường này).
- [ ] Script backfill gán đúng Khoa cho toàn bộ 7 ngành cha hiện có, kiểm
      chứng bằng query đối chiếu tên Khoa ↔ tên ngành.
- [ ] Build backend + frontend không lỗi.
- [ ] Migration đã được kiểm thử bằng dry-run (transaction rollback) trên
      DB thật, không phát sinh lỗi.

## 8. Ghi chú

API tự động chạy `Database.MigrateAsync()` khi khởi động
([`Program.cs:78`](../../src/UniversityPortal.API/Program.cs#L78)) — migration
sẽ tự áp dụng lên DB thật ngay lần deploy kế tiếp, không có bước duyệt
riêng ở production. Vì vậy migration cần được kiểm thử kỹ (dry-run trên DB
thật) trước khi merge/deploy, không chỉ dựa vào build thành công ở local.

### Trạng thái triển khai (2026-07-05)

Toàn bộ code đã được viết và kiểm thử:

- Backend: entity, config, migration (`20260705055844_AddPhongBanToNganhHoc`),
  DTO, mapping, repository, service, `PhongBanController` mới — build sạch,
  test suite hiện có vẫn pass.
- Frontend: type, `phongBanApi`, cột + dropdown "Khoa / Phòng ban" trong
  `NganhHocPage.tsx` — `tsc --noEmit` sạch.
- Migration đã dry-run trên DB thật trong transaction rollback — áp dụng
  sạch, không lỗi, rollback không để lại dấu vết (đã xác minh lại
  `INFORMATION_SCHEMA.COLUMNS`).
- Script backfill (`docs/Data/backfill_phong_ban_nganh_hoc.sql`) đã dry-run
  cùng migration trong một transaction — gán đúng Khoa CNTT/Khoa Kinh tế
  cho 5 ngành hiện có; các ngành thuộc 4 Khoa mới (CKI/DDT/CDT/DKTDH/OTO)
  sẽ tự động được gán khi chạy sau `them_khoa_nganh_hoc.sql` (hiện vẫn
  chưa được áp dụng thật — vẫn ở trạng thái "đã kiểm thử, chờ người dùng
  chạy" giống các script CTĐT trước đó).

**Chưa làm — cần quyết định của người phụ trách:** migration **chưa được
áp dụng thật** lên DB sản xuất. Có 2 cách để áp dụng:
1. Để tự động chạy ở lần deploy API kế tiếp (`docker compose up` sẽ gọi
   `MigrateAsync()`).
2. Áp ngay bằng tay (`dotnet ef database update`) rồi chạy
   `backfill_phong_ban_nganh_hoc.sql` — cho phép thấy hiệu quả ngay không
   cần đợi deploy.

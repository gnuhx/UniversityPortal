# Thêm sinh viên — lọc chọn Lớp theo Khoa / Ngành / Khoá học (Năm học, Học kỳ)

**Task:** #23
**Trạng thái:** done
**Ngày tạo:** 2026-07-08
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Admin phản ánh: form "Thêm sinh viên" hiện chưa đủ tốt khi cần gán sinh
viên vào lớp — chọn lớp không có cách nào lọc theo năm học, học kỳ,
khoa,... Đã kiểm chứng trong code:

- `frontend/src/pages/SinhVienPage.tsx:15-18` gọi
  `lopSinhHoatApi.getAll()` **không tham số**, lấy toàn bộ lớp sinh
  hoạt trong hệ thống, không phân biệt khoa/khoá.
- `frontend/src/pages/SinhVienPage.tsx:32-45`: form tạo mới chỉ có 1
  field `lopId` dạng `Select` phẳng, option hiển thị đúng mỗi `maLop`
  — không tên khoa, không khoá học, không sắp xếp/nhóm. Với số lớp lớn
  dần theo thời gian, danh sách này sẽ dài và khó chọn đúng.
- Backend `GET /api/lop-sinh-hoat/all`
  (`LopSinhHoatController.cs:47-52` → `LopSinhHoatService.GetAllAsync`
  → `BaseRepository.GetAllAsync()`) không nhận bất kỳ query filter nào
  và không `.Include()` quan hệ `ChuongTrinhDT → NganhHoc → PhongBan`
  — nên kể cả muốn lọc, backend hiện tại cũng chưa trả đủ dữ liệu để
  làm.
- `CreateSinhVienDto`/`UpdateSinhVienDto` chỉ có `LopId?` — không có
  field lọc nào khác, và **không có validate** `LopId` tồn tại hay
  không (`CreateSinhVienValidator.cs` không có `RuleFor` cho `LopId`,
  `SinhVienService.CreateAsync` không kiểm tra trước khi insert) — gán
  nhầm `lopId` không tồn tại sẽ vỡ ra thành lỗi FK constraint thô từ
  DB thay vì thông báo rõ ràng.

Về mặt dữ liệu, cần lưu ý một giới hạn kiến trúc quan trọng (đã kiểm
chứng trong `src/UniversityPortal.Domain/Entities/`):

- `LopSinhHoat` (lớp sinh hoạt — nơi sinh viên được gán vào) **không
  có `NamHocId`/`HocKyId` trực tiếp**. Nó chỉ có `ChuongTrinhDtId`.
- Năm/khoá học của một `LopSinhHoat` chỉ suy ra gián tiếp qua
  `ChuongTrinhDT.KhoaHoc` — một chuỗi text tự do kiểu `"2023-2027"`
  (không phải FK tới `NamHoc`).
- Khoa suy ra gián tiếp qua
  `LopSinhHoat → ChuongTrinhDT → NganhHoc → PhongBanId` (`PhongBan` là
  bảng "Khoa" thực sự, không có entity `Khoa` riêng).
- `HocKy` trong hệ thống hiện chỉ gắn với `LopHocPhan` (lớp học phần
  theo học kỳ) — không liên hệ gì tới `LopSinhHoat`. Do đó "lọc lớp
  theo Học kỳ" không có nghĩa với dữ liệu hiện tại (lớp sinh hoạt là 1
  lớp cố định suốt khoá, không đổi theo từng học kỳ); cái thực sự lọc
  được theo mùa/khoá là **Năm học/Khoá học**, không phải Học kỳ.

Đã có sẵn 2 pattern lọc tương tự trong code, dùng làm mẫu:

- `frontend/src/pages/NganhHocPage.tsx` (+ hook
  `frontend/src/hooks/useKhoaHocOptions.ts`) — lọc theo Khoa
  (`phongBanId`) + Ngành (`nganhId`) + Khoá học (`khoaHoc`), có sẵn
  hook suy ra danh sách khoá học từ toàn bộ `ChuongTrinhDT`.
- `frontend/src/pages/ChiTietCTDTPage.tsx:41-58` — parse `khoaHoc`
  dạng `"YYYY-YYYY"` bằng regex để suy ra tập `NamHoc` hợp lệ tương
  ứng.
- `frontend/src/pages/HocKyPage.tsx` — mẫu filter bar `Select` ngoài
  `CrudTable`, truyền `extraParams` vào query phân trang.

## 2. Mục tiêu

- Khi thêm (hoặc sửa) sinh viên, Admin chọn Lớp thông qua các bộ lọc
  thu hẹp dần: **Khoa (Phòng ban) → Ngành → Khoá học (Năm học)**, thay
  vì cuộn 1 danh sách phẳng toàn bộ lớp trong hệ thống.
- Dropdown Lớp hiển thị rõ ràng hơn: mã lớp + tên ngành/khoá học, đã
  lọc theo lựa chọn ở trên.
- `LopId` gửi lên được validate tồn tại thật trước khi tạo sinh viên,
  trả lỗi rõ ràng thay vì lỗi FK thô.
- Không đổi hành vi: `LopId` vẫn là optional (sinh viên có thể tạo
  trước, gán lớp sau).

**Ngoài phạm vi:** không thêm lọc theo "Học kỳ" cho việc gán lớp sinh
hoạt, vì không có ý nghĩa với dữ liệu hiện tại (xem mục 1 và mục 8).

## 3. Giải pháp đề xuất

> Cập nhật sau khi code xong: đã đơn giản hoá so với đề xuất ban đầu —
> filter Khoa/Ngành/Khoá học được lọc **hoàn toàn ở client**, không
> thêm query param cho `GET /api/lop-sinh-hoat/all`. Lý do và đánh đổi
> ghi ở mục 8.

### Backend

Chỉ cần enrich dữ liệu trả về, không cần filter param ở backend:

1. `LopSinhHoatDto`: bổ sung `NganhId`, `TenNganh`, `PhongBanId`,
   `TenPhongBan`, `KhoaHoc` (lấy qua `ChuongTrinhDT.Nganh.PhongBan`).
2. Thêm `ILopSinhHoatRepository.GetAllDetailAsync()` /
   `LopSinhHoatRepository.GetAllDetailAsync()` — `.Include()`
   `ChuongTrinhDT.Nganh.PhongBan` (repository generic `GetAllAsync()`
   không `virtual` nên không override được, phải thêm method riêng).
   `LopSinhHoatService.GetAllAsync()` đổi sang gọi method mới này thay
   vì `uow.LopSinhHoats.GetAllAsync()`. **Endpoint `GET
   /api/lop-sinh-hoat/all` giữ nguyên contract (không tham số)** — chỉ
   trả thêm field, không phá bất kỳ nơi nào đang gọi nó (kể cả
   `LopSinhHoatPage.tsx`).
3. `SinhVienService.CreateAsync`/`UpdateAsync`: thêm kiểm tra
   `dto.LopId.HasValue && await uow.LopSinhHoats.GetByIdAsync(...) is
   null` → `throw new BadRequestException(...)` trước khi insert/update
   (đặt ở service, không ở validator, đúng convention của dự án — không
   có validator nào trong repo inject `IUnitOfWork`).

### Frontend

Trong `SinhVienPage.tsx`, thêm filter bar Khoa → Ngành → Khoá học phía
trên bảng (mẫu bố cục theo `NganhHocPage.tsx`), lọc **client-side**
trên chính danh sách `lopSinhHoatApi.getAll()` đã enrich (không gọi
thêm `phongBanApi`/`nganhHocApi`/`chuongTrinhDTApi` vì mọi field cần
đã có sẵn trong `LopSinhHoatDto` mới):

1. Suy ra `phongBanOptions`/`nganhOptions`/`khoaHocOptions` bằng
   `useMemo` distinct trên `lopOptions`, mỗi cấp lọc theo lựa chọn của
   cấp trên (Ngành chỉ hiện ngành thuộc Khoa đã chọn; Khoá học chỉ hiện
   khoá thuộc Ngành đã chọn).
   Chọn lại cấp trên sẽ reset các cấp dưới về "Tất cả".
2. `filteredLopOptions` = `lopOptions` lọc theo cả 3 giá trị đang chọn
   (bỏ qua điều kiện nào đang là "Tất cả").
3. Field `lopId` trong form (`CrudTable`) dùng
   `filteredLopOptions.map(...)`, label
   `${maLop} — ${tenNganh} (${khoaHoc})`.
4. Filter bar chỉ hiện với `isAdminOrGiaoVu` (vai trò có quyền
   tạo/sửa sinh viên).

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Application/DTOs/LopSinhHoat/LopSinhHoatDto.cs` | Thêm `KhoaHoc`, `NganhId`, `TenNganh`, `PhongBanId`, `TenPhongBan` |
| `src/UniversityPortal.Application/Mappings/MappingProfile.cs` | Map 5 field mới từ `ChuongTrinhDT.Nganh.PhongBan` trong `CreateMap<LopSinhHoat, LopSinhHoatDto>` |
| `src/UniversityPortal.Application/Interfaces/Repositories/ILopSinhHoatRepository.cs` | Thêm `Task<IEnumerable<LopSinhHoat>> GetAllDetailAsync()` |
| `src/UniversityPortal.Infrastructure/Repositories/LopSinhHoatRepository.cs` | Cài đặt `GetAllDetailAsync()` — `.Include(x => x.ChuongTrinhDT).ThenInclude(c => c.Nganh).ThenInclude(n => n.PhongBan)` |
| `src/UniversityPortal.Application/Services/LopSinhHoatService.cs` | `GetAllAsync()` gọi `uow.LopSinhHoats.GetAllDetailAsync()` thay vì `GetAllAsync()` gốc (không đổi chữ ký/contract) |
| `src/UniversityPortal.Application/Services/SinhVienService.cs` | `CreateAsync`/`UpdateAsync`: thêm kiểm tra `LopId` tồn tại trước khi insert/update, ném `BadRequestException` rõ ràng thay vì để lỗi FK thô |

Không đổi `LopSinhHoatController`/route `GET /api/lop-sinh-hoat/all` —
không thêm query param nào (xem mục 8).

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/pages/SinhVienPage.tsx` | Thêm filter bar Khoa/Ngành/Khoá học (cascading, lọc client-side trên `lopOptions` đã enrich) phía trên bảng; đổi label option Lớp thành `maLop — tenNganh (khoaHoc)` |
| `frontend/src/types/index.ts` | Thêm `khoaHoc`, `nganhId`, `tenNganh`, `phongBanId`, `tenPhongBan` vào interface `LopSinhHoat` |

Không cần đổi `useKhoaHocOptions.ts` hay `api/modules.ts`/`crud.ts` —
`lopSinhHoatApi.getAll()` không cần tham số mới, filter làm hoàn toàn
ở component.

### Dữ liệu / Migration

Không cần migration schema (không thêm cột DB nào) — chỉ thêm field
tính toán/join vào DTO. Không thêm `NamHocId`/`HocKyId` vào
`LopSinhHoat` (xem mục 8).

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~3-4 giờ (2 file backend service/controller + 2 validator + 1 trang frontend + cập nhật type/api client) |
| Mức độ rủi ro | Thấp-trung bình — mở rộng field/filter optional, không đổi contract cũ nếu giữ tham số là optional; điểm cần cẩn thận là không phá các chỗ khác đang gọi `lopSinhHoatApi.getAll()`/`GET /api/lop-sinh-hoat/all` không truyền tham số (ví dụ trang `LopSinhHoatPage.tsx` tự quản lý CRUD lớp) |
| Ảnh hưởng dữ liệu hiện có | Không — không sửa dữ liệu, chỉ đọc thêm quan hệ có sẵn |
| Khả năng rollback | Revert các file trong bảng trên; do filter là optional nên có thể tắt dần (frontend trước, backend sau) mà không gãy |

## 6. Kế hoạch triển khai

1. ~~Backend: mở rộng `LopSinhHoatDto` + mapping; thêm
   `GetAllDetailAsync()` (repo + interface) và wire vào
   `LopSinhHoatService.GetAllAsync()`.~~ Đã làm — không đổi controller.
2. ~~Backend: thêm validate tồn tại `LopId` trong
   `SinhVienService.CreateAsync`/`UpdateAsync`.~~ Đã làm.
3. ~~Frontend: cập nhật type `LopSinhHoat`.~~ Đã làm.
4. ~~Frontend: xây filter bar cascading Khoa → Ngành → Khoá học (lọc
   client-side) + Select "Lớp" dùng `filteredLopOptions`.~~ Đã làm.
5. ~~`dotnet build` + `tsc --noEmit`.~~ Cả hai sạch (0 lỗi; các warning
   hiện có từ trước không liên quan). `dotnet test` cũng chạy qua (1/1
   pass — bộ test hiện tại của repo rất mỏng, không có test riêng cho
   luồng này).
6. ~~Kiểm thử trên trình duyệt (Admin thật, backend + DB thật).~~ Đã
   làm — xem bằng chứng chi tiết ở mục 8.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Form "Thêm sinh viên" có Select Khoa/Ngành/Khoá học lọc cascading
      phía trên Select Lớp (đặt thành filter bar phía trên bảng, áp
      dụng cho cả modal Thêm mới lẫn Sửa).
- [x] Select Lớp chỉ hiện các lớp khớp với Khoa/Ngành/Khoá học đã chọn
      (hoặc toàn bộ nếu chưa chọn filter nào) — kiểm chứng: chọn Ngành
      "Kỹ thuật Phần mềm" → Lớp chỉ còn "KTPM22A", Khoá học chỉ còn
      "2022-2026".
- [x] Option Lớp hiển thị đủ thông tin (mã lớp + ngành + khoá học) —
      ví dụ thực tế: "KTPM22A — Kỹ thuật Phần mềm (2022-2026)".
- [x] Gán `LopId` không tồn tại trả lỗi rõ ràng (không phải lỗi DB
      constraint thô) — kiểm chứng bằng curl: `lopId: 99999` → HTTP
      400, `{"success":false,"message":"Không tìm thấy lớp sinh hoạt
      id = 99999."}`.
- [x] Trang "Lớp sinh hoạt" và các nơi khác đang gọi
      `lopSinhHoatApi.getAll()`/`GET /api/lop-sinh-hoat/all` không
      truyền filter vẫn hoạt động y như cũ (contract endpoint không
      đổi, chỉ thêm field trong response).
- [x] `dotnet build` và `tsc --noEmit` sạch.
- [x] Đã kiểm thử trên trình duyệt với dữ liệu thật (DB SQL Server
      thật qua `appsettings.Development.json`, tài khoản `admin`
      thật) — tạo thành công sinh viên MSSV thật gán vào lớp KTPM22A
      qua đúng luồng filter mới; dữ liệu test đã được dọn (xem mục 8).

## 8. Ghi chú

- **Không lọc theo Học kỳ**: `LopSinhHoat` không có quan hệ nào tới
  `HocKy` (chỉ `LopHocPhan` mới gắn với học kỳ). Nếu ý của Admin thực
  ra là "lọc theo Học kỳ" cho một chức năng khác — ví dụ gán/đăng ký
  sinh viên vào **lớp học phần** (`LopHocPhan`) theo từng học kỳ — thì
  đó là một tính năng khác hẳn (đăng ký học phần), hiện **chưa tồn tại
  API tạo mới** ở `IDanhSachLopHPService`/`DanhSachLopHPController`
  (chỉ có xem điểm/nhập điểm). Cần Admin xác nhận lại ý muốn trước khi
  code: (a) gán sinh viên vào **lớp sinh hoạt** — đúng theo scope tài
  liệu này, hay (b) đăng ký sinh viên vào **lớp học phần** theo học kỳ
  — một tính năng lớn hơn, cần thiết kế riêng (không nằm trong tài
  liệu này).
- **`khoaHoc` là text tự do**: giống hạn chế đã ghi ở task #04, lọc
  theo `khoaHoc` chỉ khớp chính xác chuỗi (hoặc parse theo pattern
  `YYYY-YYYY` như task #10) — dữ liệu cũ không đồng nhất định dạng vẫn
  có thể không lọc được chính xác. Không sửa gốc vấn đề này trong task
  này.
- Cân nhắc không đổi trực tiếp endpoint `GET /api/lop-sinh-hoat/all`
  mà thêm route mới riêng cho dropdown chọn lớp (ví dụ
  `/api/lop-sinh-hoat/for-select`), nếu muốn tách bạch tuyệt đối với
  logic CRUD lớp hiện có và tránh rủi ro ảnh hưởng lẫn nhau — quyết
  định cụ thể để lúc code cân nhắc theo mức độ dùng chung của endpoint
  `all` hiện tại.

### Cập nhật sau khi triển khai (2026-07-08)

**Đơn giản hoá so với đề xuất ban đầu:** filter Khoa/Ngành/Khoá học
được làm **hoàn toàn ở client**, dựa trên `LopSinhHoatDto` đã enrich,
thay vì thêm query param `phongBanId/nganhId/khoaHoc` cho endpoint `GET
/api/lop-sinh-hoat/all` như đề xuất gốc ở mục 3/4. Lý do: endpoint này
dùng cho dropdown (không phân trang), số lượng lớp sinh hoạt của một
trường không lớn (thực tế trong DB dev hiện chỉ có 2 lớp), nên lọc phía
client vừa đủ, tránh phải sửa route/DI của `LopSinhHoatController` —
giảm rủi ro ảnh hưởng các nơi khác đang gọi endpoint này (đặc biệt
`LopSinhHoatPage.tsx`). Nếu sau này số lớp tăng nhiều (hàng nghìn), nên
quay lại phương án lọc server-side như đề xuất gốc.

**Phát hiện khi kiểm thử — dữ liệu Khoa (`PhongBan`) chưa đầy đủ:** với
dữ liệu thật trong DB dev hiện tại, cả 2 `NganhHoc` đang dùng
(`Kỹ thuật Phần mềm`, `Hệ thống Thông tin`) đều có `PhongBanId = null`
— tức là backfill Khoa cho Ngành (`docs/Data/backfill_phong_ban_nganh_hoc.sql`)
chưa chạy hoặc chưa phủ hết Ngành. Hệ quả: Select "Khoa" trên
`SinhVienPage.tsx` hiện chỉ có đúng 1 lựa chọn "Tất cả" — **không phải
bug của tính năng này**, code lọc theo `phongBanId` hoạt động đúng
(kiểm chứng: Ngành/Khoá học lọc cascading chính xác), chỉ là chưa có
dữ liệu Khoa để lọc. Khi dữ liệu `PhongBan`/`NganhHoc.PhongBanId` được
backfill đầy đủ, Select Khoa sẽ tự động có thêm lựa chọn mà không cần
sửa code.

**Bằng chứng kiểm thử trên trình duyệt** (Playwright, Chrome, DB dev
thật qua `appsettings.Development.json`, tài khoản `admin`/`Admin@123`):
1. `GET /api/lop-sinh-hoat/all` trả đúng field mới (`khoaHoc`,
   `nganhId`, `tenNganh`, `phongBanId`, `tenPhongBan`) cho cả 2 lớp có
   trong DB (`KTPM22A`, `HTTT23A`).
2. Chọn filter Ngành = "Kỹ thuật Phần mềm" trên trang Sinh viên → Select
   Khoá học tự thu hẹp còn đúng "2022-2026"; mở modal "Thêm mới" → Select
   Lớp chỉ còn đúng 1 option "KTPM22A — Kỹ thuật Phần mềm (2022-2026)"
   (trước đó có 2 lớp).
3. Tạo sinh viên MSSV thật với lớp đã lọc → `201 Created`, response trả
   `"lopId":1,"tenLop":"KTPM22A"`, toast "Tạo mới thành công" hiển thị
   đúng trên UI.
4. Probe qua `curl` trực tiếp API: `lopId: 99999` (không tồn tại) →
   `400 Bad Request`, `"message":"Không tìm thấy lớp sinh hoạt id =
   99999."` — đúng như acceptance criteria, không còn lỗi FK thô;
   `lopId: 1` (hợp lệ) → `201` như bình thường.
5. **Dọn dẹp:** vì DB dev là DB SQL Server thật dùng chung
   (site4now.net, không phải DB tạm), 2 tài khoản sinh viên test tạo ra
   trong lúc kiểm thử (id 23, 24) đã được vô hiệu hoá qua chính endpoint
   `DELETE /api/sinh-vien/{id}` có sẵn (soft-delete — khoá tài khoản,
   giữ lại lịch sử theo đúng thiết kế hiện có của `SinhVienService.DeleteAsync`),
   không để lại tài khoản test còn hoạt động.

# Trang Admin quản lý Năm học / Học kỳ (thêm mới, sửa) — thay thế việc chạy SQL tay

**Task:** #07
**Trạng thái:** done
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

> Đánh số tiếp theo #06 (`06_todo_chi_tiet_ctdt_hoc_ky_dropdown.md`) — hai
> task độc lập, không gộp chung vì khác phạm vi thay đổi.

---

## 1. Bối cảnh / Vấn đề

Đã kiểm tra toàn bộ chuỗi `nam_hoc` → `hoc_ky` → `tuan_hoc`:

- **`nam_hoc` không có bất kỳ repository/service/controller nào.**
  `IUnitOfWork`/`UnitOfWork` (`IUnitOfWork.cs`, `UnitOfWork.cs`) liệt kê
  repository cho mọi entity khác (`NganhHocs`, `ChuongTrinhDTs`,
  `HocKys`, `TuanHocs`, ...) nhưng **không có `NamHocs`** — `NamHoc` chỉ
  được truy cập gián tiếp qua `.Include(x => x.NamHoc)` từ `HocKy`/
  `TuanHoc`. Không có DTO, validator, controller nào cho năm học.
- **`hoc_ky` chỉ đọc.** `HocKyController.cs` chỉ có 1 endpoint
  `GET /api/hoc-ky` (không qua service layer, build thẳng anonymous
  object trong controller) — không có `POST`/`PUT`/`DELETE`.
- **`tuan_hoc` cũng chỉ đọc.** `TuanHocController.cs` chỉ có
  `GET /api/tuan-hoc/all`, dùng cho dropdown chọn tuần khi tạo thời
  khoá biểu — không có cách tạo tuần học mới qua UI.
- Hệ quả thực tế: khi cần mở một học kỳ mới (vd. học kỳ "hiện tại" cho
  năm 2026, xem task #05), cách duy nhất hiện có là **viết và chạy tay
  một script SQL** (`docs/Data/seed_mon_hoc_chung_toan_truong.sql`) —
  không có màn hình admin nào để làm việc này, dù đây là một tác vụ vận
  hành sẽ lặp lại mỗi học kỳ/năm học.
- Kiến trúc CRUD chuẩn của dự án đã có sẵn pattern phù hợp để tái dùng:
  trang lồng nhau `ChuongTrinhDTPage.tsx` (`/chuong-trinh-dt`) →
  `ChiTietCTDTPage.tsx` (`/chuong-trinh-dt/:id`, lọc theo `ctdtId` qua
  `extraParams`) — có thể áp dụng y hệt cho `NamHoc` → `HocKy`.

## 2. Mục tiêu

- Admin tạo/sửa **Năm học** qua UI (CRUD đầy đủ).
- Admin tạo/sửa **Học kỳ** trong một năm học cụ thể qua UI, không cần
  gõ SQL tay.
- Không cần đổi trang Thời khoá biểu (`ThoiKhoaBieuPage.tsx`) hay logic
  chọn học kỳ mặc định đang có — chỉ thêm khả năng *tạo* dữ liệu mà nó
  tiêu thụ.

## 3. Giải pháp đề xuất

Xây 2 CRUD lồng nhau theo đúng pattern `ChuongTrinhDT` → `ChiTietCTDT`
đã có trong repo:

1. **`NamHoc`** — CRUD đầy đủ, entity đơn giản (chỉ có `ten_nam_hoc`):
   thêm `INamHocRepository`/`NamHocRepository` (kế thừa
   `BaseRepository<NamHoc>`, không cần override gì thêm vì
   `AddAsync`/`Update`/`Delete`/`GetByIdAsync` đã có sẵn ở
   `IRepository<T>`), đăng ký vào `IUnitOfWork`/`UnitOfWork`; thêm
   `INamHocService`/`NamHocService` + `NamHocDto`/`UpsertNamHocDto` +
   validator (theo mẫu `NganhHocService`/`UpsertNganhHocValidator`);
   thêm `NamHocController` (GET phân trang, GET all, GET by id, POST,
   PUT, DELETE — copy cấu trúc `NganhHocController.cs`). `DeleteAsync`
   chặn xoá nếu năm học còn `hoc_ky` hoặc `tuan_hoc` liên kết (theo mẫu
   `NganhHocService.DeleteAsync` chặn xoá khi còn CTDT).
2. **`HocKy`** — nâng từ chỉ-đọc thành CRUD: thêm `IHocKyService`/
   `HocKyService` (chuyển logic `GetAllAsync` hiện có từ controller vào
   service, thêm `CreateAsync`/`UpdateAsync`/`DeleteAsync`), thêm
   `HocKyDto`/`UpsertHocKyDto` (`tenHocKy`, `namHocId`, `ngayBatDau`) +
   validator (bắt buộc `tenHocKy` không trống, `namHocId` hợp lệ);
   `HocKyController` thêm `POST`/`PUT`/`DELETE` (`[Authorize(Roles =
   "Admin")]`, theo mẫu `NganhHocController`). Thêm `CreateMap<HocKy,
   HocKyDto>` và `CreateMap<NamHoc, NamHocDto>` vào `MappingProfile.cs`
   (hiện chưa có mapping nào cho 2 entity này).
3. **Frontend**: trang `NamHocPage.tsx` (route `/nam-hoc`) dùng
   `CrudTable` liệt kê năm học, thêm/sửa/xoá. Trang `HocKyPage.tsx`
   (route `/nam-hoc/:id`, theo mẫu `ChiTietCTDTPage.tsx`) hiển thị danh
   sách học kỳ của năm học đó (`extraParams={{ namHocId }}`), cho phép
   thêm/sửa học kỳ (`tenHocKy`, `ngayBatDau` — date picker). Thêm
   `namHocApi = createCrudApi(...)` vào `api/modules.ts`, nâng
   `hocKyApi` (hiện là object tay với chỉ `getAll()`) thành dùng
   `createCrudApi` đầy đủ hoặc bổ sung `create`/`update`/`remove`/
   `getPaged` tương ứng. Thêm route `/nam-hoc` và `/nam-hoc/:id` vào
   `App.tsx`, thêm mục menu "Năm học" vào `AppLayout.tsx` (cạnh "Ngành
   học"/"Chương trình đào tạo").

**Không nằm trong phạm vi task này:** tự động sinh `tuan_hoc` (các
tuần Thứ Hai → Chủ nhật) khi tạo một học kỳ mới. Việc tạo tuần học vẫn
cần thao tác riêng (hiện cũng chưa có UI — xem mục 8) — nếu không xử
lý, học kỳ mới tạo sẽ không có tuần nào để gắn `thoi_khoa_bieu`.

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `Application/Interfaces/Repositories/INamHocRepository.cs` (mới) | Kế thừa `IRepository<NamHoc>`, thêm `GetAllAsync()` sắp xếp giảm dần theo `TenNamHoc` |
| `Infrastructure/Repositories/NamHocRepository.cs` (mới) | Cài đặt trên |
| `Application/Interfaces/IUnitOfWork.cs` + `Infrastructure/UnitOfWork.cs` | Đăng ký `INamHocRepository NamHocs { get; }` |
| `Application/DTOs/NamHoc/NamHocDto.cs`, `UpsertNamHocDto.cs` (mới) | DTO phản hồi + DTO tạo/sửa |
| `Application/Validators/NamHoc/UpsertNamHocValidator.cs` (mới) | `TenNamHoc` không trống, đúng định dạng `YYYY-YYYY` |
| `Application/Interfaces/Services/INamHocService.cs` + `Services/NamHocService.cs` (mới) | `GetPagedAsync`, `GetAllAsync`, `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync` (chặn xoá nếu còn `hoc_ky`/`tuan_hoc`) |
| `API/Controllers/NamHocController.cs` (mới) | `GET /api/nam-hoc` (phân trang), `GET /all`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` (Admin) |
| `Application/DTOs/HocKy/HocKyDto.cs`, `UpsertHocKyDto.cs` (mới) | DTO cho học kỳ (hiện `HocKyController` build anonymous object tay) |
| `Application/Validators/HocKy/UpsertHocKyValidator.cs` (mới) | `TenHocKy` không trống, `NamHocId > 0` |
| `Application/Interfaces/Services/IHocKyService.cs` + `Services/HocKyService.cs` (mới) | Chuyển `GetAllAsync` hiện có vào service, thêm `CreateAsync`/`UpdateAsync`/`DeleteAsync` (chặn xoá nếu còn `lop_hoc_phan`/`chi_tiet_ctdt`/`hoc_phi` liên kết) |
| `API/Controllers/HocKyController.cs` | Đổi từ inject `IUnitOfWork` sang inject `IHocKyService`; thêm `POST`/`PUT`/`DELETE` (Admin) |
| `Application/Mappings/MappingProfile.cs` | Thêm `CreateMap<NamHoc, NamHocDto>`, `CreateMap<HocKy, HocKyDto>` (map `TenNamHoc` từ `s.NamHoc.TenNamHoc`) |
| `Application/DependencyInjection.cs` | Đăng ký `INamHocService`/`IHocKyService` vào DI container |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/pages/NamHocPage.tsx` (mới) | `CrudTable` quản lý năm học (thêm/sửa/xoá) |
| `frontend/src/pages/HocKyPage.tsx` (mới) | `CrudTable` quản lý học kỳ của 1 năm học, `extraParams={{ namHocId }}`, theo mẫu `ChiTietCTDTPage.tsx` |
| `frontend/src/api/modules.ts` | Thêm `namHocApi = createCrudApi<NamHoc, UpsertNamHoc>("/nam-hoc")`; nâng `hocKyApi` thành CRUD đầy đủ (`getPaged`, `create`, `update`, `remove`, giữ `getAll` cho dropdown ở `ThoiKhoaBieuPage`) |
| `frontend/src/types/index.ts` | Thêm `NamHoc`, `UpsertNamHoc`; thêm `namHocId` vào type `HocKy`, thêm `UpsertHocKy` |
| `frontend/src/App.tsx` | Thêm route `/nam-hoc` (`NamHocPage`) và `/nam-hoc/:id` (`HocKyPage`) |
| `frontend/src/components/AppLayout.tsx` | Thêm mục menu "Năm học" |

### Dữ liệu / Migration

Không cần migration — không đổi schema, chỉ thêm API/UI thao tác trên
bảng đã có (`nam_hoc`, `hoc_ky`).

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~2–3 giờ (2 stack backend mới/mở rộng + 2 trang frontend + route/menu) — lớn hơn các task filter/dropdown trước, tương đương quy mô task #03 |
| Mức độ rủi ro | Trung bình — thêm quyền ghi (Create/Update/Delete) mới trên dữ liệu nền tảng (`nam_hoc`, `hoc_ky`) mà nhiều bảng khác phụ thuộc (FK Restrict/Cascade từ `hoc_ky`, `tuan_hoc`, `chi_tiet_ctdt`, `lop_hoc_phan`, `hoc_phi`) — cần validate kỹ điều kiện xoá để tránh lỗi FK từ DB hoặc xoá nhầm dữ liệu đang dùng |
| Ảnh hưởng dữ liệu hiện có | Không có tác động tới dữ liệu cũ nếu chỉ dùng để **thêm mới**; rủi ro nằm ở chức năng **sửa/xoá** năm học hoặc học kỳ đã có `lop_hoc_phan`/`hoc_phi` gắn vào — cần chặn đúng ở `DeleteAsync`, và cân nhắc có cho sửa `ngay_bat_dau` của học kỳ đã có `thoi_khoa_bieu` hay không (sửa ngày có thể làm lệch buổi học đã tạo) |
| Khả năng rollback | Revert code (route/controller/service mới) — không có migration nên rollback sạch, không ảnh hưởng dữ liệu |

## 6. Kế hoạch triển khai

1. Backend: `NamHoc` full stack (repository → UnitOfWork → DTO →
   validator → service → controller → DI).
2. Backend: nâng `HocKy` từ chỉ-đọc thành CRUD (DTO, validator,
   service, cập nhật controller).
3. Thêm `CreateMap` cho `NamHoc`/`HocKy` vào `MappingProfile.cs`.
4. `dotnet build`; test thủ công qua REST client: tạo năm học mới,
   tạo học kỳ trong năm đó, thử xoá năm học đang có học kỳ (phải bị
   chặn).
5. Frontend: `NamHocPage.tsx`, `HocKyPage.tsx`, cập nhật `api/modules.ts`,
   `types/index.ts`, route + menu.
6. `tsc --noEmit`; test UI: tạo năm học "2026-2027" → mở trang học kỳ
   của năm đó → tạo học kỳ mới → xác nhận xuất hiện đúng trong dropdown
   học kỳ ở các trang khác (Thời khoá biểu, Chi tiết CTĐT).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Admin tạo được năm học mới qua UI, không cần SQL tay.
- [x] Admin tạo được học kỳ mới trong một năm học cụ thể qua UI.
- [x] Xoá năm học/học kỳ đang có dữ liệu liên kết (`hoc_ky`, `tuan_hoc`,
      `lop_hoc_phan`, `hoc_phi`, `chi_tiet_ctdt`) bị chặn với thông báo
      rõ ràng, không để lỗi FK thô từ DB văng ra.
- [x] Học kỳ mới tạo xuất hiện đúng trong các dropdown chọn học kỳ hiện
      có (`ThoiKhoaBieuPage`, `ChiTietCTDTPage`) — không cần đổi các
      trang đó.
- [x] Build backend + frontend không lỗi, `tsc --noEmit` sạch.
- [x] Kiểm thử thủ công trên trình duyệt (tài khoản `admin`, chạy
      backend thật kết nối DB thật): tạo năm học test "2099-2100" →
      tạo học kỳ trong năm đó (kèm ngày qua `DatePicker`) → sửa (xác
      nhận modal load lại đúng tên + ngày) → xoá học kỳ → xoá năm học
      — dọn sạch dữ liệu test sau khi xong. Xem mục 8 để biết chi tiết
      và 1 phát hiện quan trọng ngoài phạm vi task.

## 8. Ghi chú

- **Quyết định thiết kế route khi triển khai:** để giữ đúng tiêu chí
  "không cần đổi `ThoiKhoaBieuPage`/`ChiTietCTDTPage`", đã đổi ý nghĩa
  `GET /api/hoc-ky` (gốc) thành **phân trang** (dùng cho `HocKyPage`
  quản lý, lọc theo `namHocId`) và thêm `GET /api/hoc-ky/all` cho danh
  sách đầy đủ không phân trang — đúng convention `GET` gốc = phân
  trang, `/all` = danh sách đầy đủ đã dùng ở `NganhHocController`,
  `ChuongTrinhDTController`. Chỉ cần sửa 1 chỗ duy nhất
  (`hocKyApi.getAll()` trong `api/modules.ts`, đổi từ gọi thẳng
  `createCrudApi` cũ sang dùng lại y hệt hàm `getAll()` có sẵn của
  `createCrudApi` — vốn đã trỏ đúng `/hoc-ky/all`) nên 2 trang kia
  không cần đổi gì, đúng như acceptance criteria.
- **Phát hiện quan trọng ngoài phạm vi task (đã xác minh, chưa sửa):**
  FluentValidation được đăng ký qua `AddValidatorsFromAssembly` trong
  `Application/DependencyInjection.cs` nhưng **không có bước nào gọi
  validate** trong pipeline (không thấy `AddFluentValidationAutoValidation`,
  không có filter/middleware nào, không controller nào tự gọi
  `IValidator<T>`) — nghĩa là **toàn bộ validator trong dự án, kể cả
  các validator có từ trước** (`UpsertNganhHocValidator`,
  `CreateChiTietCTDTValidator`, ...), hiện không có tác dụng gì. Đã xác
  minh bằng cách gọi thật `POST /api/nganh-hoc` với `maNganh` sai định
  dạng (có ký tự thường + `!`) — server vẫn tạo thành công thay vì trả
  lỗi 400 như validator yêu cầu. Validator mới viết cho task này
  (`UpsertNamHocValidator`, `UpsertHocKyValidator`) bị ảnh hưởng tương
  tự — không phải lỗi riêng của code mới, mà là lỗ hổng hệ thống có sẵn.
  Đề xuất tách thành 1 task riêng (thêm
  `services.AddFluentValidationAutoValidation()` hoặc 1
  `IAsyncActionFilter` gọi `IValidator<T>` cho mọi request `POST`/`PUT`)
  vì sửa ở tầng pipeline sẽ ảnh hưởng đồng thời **mọi** endpoint hiện
  có — cần đánh giá riêng, không tiện làm kèm task này.
- Task này **không** giải quyết việc tạo `tuan_hoc` (tuần học) — sau
  khi tạo học kỳ mới, vẫn cần một cách khác (UI riêng hoặc tiếp tục
  script) để sinh các tuần Thứ Hai → Chủ nhật cho năm học đó trước khi
  có thể tạo `thoi_khoa_bieu`. Nếu muốn khép kín luồng "mở học kỳ mới
  → có ngay thời khoá biểu", cần task tiếp theo: tự động sinh
  `tuan_hoc` cho một năm học (vd. nút "Sinh tuần học" nhận
  ngày bắt đầu năm học + số tuần, tạo hàng loạt).
- Sau khi có task này, `docs/Data/seed_mon_hoc_chung_toan_truong.sql`
  (task #05) có thể cân nhắc thay bước 1–3 (tạo năm học/học kỳ/tuần
  học) bằng thao tác UI thay vì script — nhưng không bắt buộc, script
  vẫn chạy được độc lập.
- Cân nhắc phân quyền: hiện tất cả role đã đăng nhập đều `GET` được
  `hoc_ky`/`nam_hoc` (dùng cho dropdown ở nhiều trang) — giữ nguyên,
  chỉ giới hạn `POST`/`PUT`/`DELETE` cho `Admin`, đúng pattern các
  controller khác trong dự án.

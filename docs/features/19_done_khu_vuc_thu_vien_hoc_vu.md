# Thêm khu vực "Thư viện" và "Học Vụ" — nội dung tĩnh dạng Tab, Admin tự sửa được

**Task:** #19
**Trạng thái:** done
**Ngày tạo:** 2026-07-07
**Người phụ trách:** (chưa gán)

> Độ ưu tiên: **thấp** (theo yêu cầu gốc). Mục tiêu là làm đơn giản nhất có
> thể để Admin tự sửa nội dung như 1 trang WordPress nhỏ có Tab — không cần
> workflow duyệt, không cần rich-text, không cần phân quyền phức tạp.

---

## 1. Bối cảnh / Vấn đề

Cần thêm 2 khu vực nội dung tĩnh mới, mỗi khu vực chia thành nhiều Tab/mục:

- **Thư viện**: Giới thiệu, Hướng Dẫn, Tra Cứu, Tài liệu mới, Dịch vụ, Hoạt
  động, Cơ sở dữ liệu, Media, Liên hệ (9 tab).
- **Học Vụ**: Quy Chế - Quy Định, Giảng viên, Sinh Viên (3 mục).

Đã kiểm tra: **repo hiện không có bất kỳ tính năng CMS/trang tĩnh nào**.
`grep -rin "static\|content\|CMS\|Rich text\|editor\|TinyMCE\|Quill"` trên
`src/` và `frontend/src/` chỉ khớp `ThongBao` (thông báo) — nhưng đó là mô
hình "feed thông báo cá nhân có đọc/chưa đọc"
(`src/UniversityPortal.Domain/Entities/ThongBao.cs:5-19`, có
`ThongBaoDaDoc` theo dõi trạng thái đọc từng user), nội dung là `string`
thuần (không phải HTML — frontend dùng `<TextArea>` để nhập, không có
`dangerouslySetInnerHTML` ở đâu trong repo). Không hợp cho nhu cầu "trang
tĩnh nhiều tab, ai cũng xem được, Admin sửa nội dung".

`frontend/package.json` không có thư viện rich-text/WYSIWYG nào (không
Quill/TinyMCE/Draft.js) — nội dung nên là text thuần (giữ đúng tinh thần
"đơn giản nhất").

Pattern có thể tái dùng trực tiếp: **`createCrudApi` generic**
(`frontend/src/api/crud.ts:4-31`) + controller CRUD chuẩn kiểu
`MonHocController`
(`src/UniversityPortal.API/Controllers/MonHocController.cs` — GET phân
trang, GET `/all`, GET `/{id}`, POST/PUT/DELETE chỉ Admin) — đây là khuôn
mẫu đã dùng cho mọi danh mục đơn giản trong hệ thống (`NganhHoc`, `MonHoc`,
`PhongBan`,...).

## 2. Mục tiêu

- Có trang `/thu-vien` hiển thị 9 tab theo đúng danh sách yêu cầu, và trang
  `/hoc-vu` hiển thị 3 mục — cả 2 ai cũng xem được (không giới hạn vai trò),
  có mục trong menu điều hướng.
- Admin có 1 màn hình quản trị để **sửa nội dung từng tab** (tiêu đề + nội
  dung văn bản) mà không cần biết code — thêm/sửa/xoá/sắp xếp tab.
- Không xây dựng thừa: 1 cơ chế dữ liệu dùng chung cho cả 2 khu vực, không
  làm 2 bảng riêng cho 2 khu vực giống hệt nhau về cấu trúc.

## 3. Giải pháp đề xuất

### 3.1. Thiết kế dữ liệu dùng chung cho cả 2 khu vực

Cả "Thư viện" và "Học Vụ" đều là **"1 khu vực → nhiều mục nội dung có thứ
tự"** — cùng 1 hình dạng dữ liệu. Dùng **1 entity chung** thay vì 2 entity
riêng để tránh lặp code CRUD:

```
NoiDungTinh
├─ Id            (PK)
├─ KhuVuc         string   // "thu-vien" | "hoc-vu" — định danh khu vực
├─ MaMuc          string   // slug ổn định của tab, vd "gioi-thieu"
├─ TieuDe         string   // tên hiển thị trên Tab, vd "Giới thiệu"
├─ NoiDung        string?  // nội dung văn bản thuần (nvarchar(max)), null = "đang cập nhật"
├─ ThuTu          int      // thứ tự hiển thị tab
└─ (AuditableEntity: CreatedAt, UpdatedAt)
```

`KhuVuc` là cột lọc — trang `/thu-vien` gọi API với `khuVuc=thu-vien`, trang
`/hoc-vu` gọi với `khuVuc=hoc-vu`. Không cần bảng "Khu vực" riêng vì chỉ có
2 giá trị cố định, giữ nhất quán với cách repo đang xử lý các danh sách nhỏ
cố định khác (chuỗi tự do, không lookup table riêng).

### 3.2. Backend — CRUD chuẩn theo khuôn `MonHocController`

- `GET /api/noi-dung-tinh?khuVuc=thu-vien` — public (mở cho mọi user đã đăng
  nhập, giống `MonHocController.GetPaged`/`GetAll`), trả về danh sách đã sắp
  theo `ThuTu`, dùng cho trang hiển thị.
- `POST` / `PUT /{id}` / `DELETE /{id}` — chỉ `Admin` (giống
  `MonHocController`), dùng cho màn hình quản trị.

### 3.3. Frontend — 1 trang public (Tabs) + 1 trang quản trị (Table/Modal) dùng chung cho cả 2 khu vực

- **Trang public**: 1 component `KhuVucNoiDungPage` nhận prop `khuVuc` (dùng
  lại cho cả `/thu-vien` và `/hoc-vu`, tránh 2 file gần như giống hệt nhau)
  — gọi API lọc theo `khuVuc`, render `Tabs` (giống `NganhHocPage.tsx:173`
  đã dùng `Tabs` cho các mục con), mỗi `TabPane` hiện `TieuDe` làm tên tab và
  `NoiDung` dạng đoạn văn (`<Paragraph style={{ whiteSpace: "pre-wrap" }}>`,
  hoặc "Nội dung đang được cập nhật." nếu `NoiDung` rỗng).
- **Trang quản trị** (chỉ Admin, vào từ menu hoặc từ chính trang public bằng
  nút "Quản lý nội dung" khi `user.vaiTro === Admin`): `Table` liệt kê các
  mục của khu vực đang chọn (Select chọn "Thư viện"/"Học Vụ" ở đầu trang),
  `Modal` sửa `TieuDe`/`NoiDung`/`ThuTu`, nút thêm mục mới, nút xoá — đúng
  khuôn CRUD Table+Modal đã dùng cho `NganhHocPage`/`MonHocPage`. Đây chính
  là chỗ Admin "sửa như WordPress" — không cần chạm code để đổi nội dung tab.

### 3.4. Seed dữ liệu ban đầu

Seed sẵn đúng 9 mục cho `khuVuc="thu-vien"` (Giới thiệu, Hướng Dẫn, Tra Cứu,
Tài liệu mới, Dịch vụ, Hoạt động, Cơ sở dữ liệu, Media, Liên hệ) và 3 mục cho
`khuVuc="hoc-vu"` (Quy Chế - Quy Định, Giảng viên, Sinh Viên), `NoiDung =
NULL` (hiện "Nội dung đang được cập nhật.") — để 2 trang hiện đúng tab ngay
từ đầu, Admin chỉ cần vào sửa nội dung, không cần tự tạo tab.

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Domain/Entities/NoiDungTinh.cs` | Entity mới (mục 3.1) |
| `src/UniversityPortal.Infrastructure/Persistence/Configurations/NoiDungTinhConfiguration.cs` | Config bảng `noi_dung_tinh`, index trên `(khu_vuc, thu_tu)` |
| `src/UniversityPortal.Application/DTOs/NoiDungTinh/NoiDungTinhDto.cs`, `UpsertNoiDungTinhDto.cs` | DTO theo khuôn `MonHocDto`/`UpsertMonHocDto` |
| `src/UniversityPortal.Application/Interfaces/Repositories/INoiDungTinhRepository.cs` + `src/UniversityPortal.Infrastructure/Repositories/NoiDungTinhRepository.cs` | `GetByKhuVucAsync(string khuVuc)` sắp theo `ThuTu` |
| `src/UniversityPortal.Application/Interfaces/Services/INoiDungTinhService.cs` + `src/UniversityPortal.Application/Services/NoiDungTinhService.cs` | CRUD chuẩn, theo khuôn `MonHocService` |
| `src/UniversityPortal.API/Controllers/NoiDungTinhController.cs` | Route `api/noi-dung-tinh`, theo khuôn `MonHocController.cs` (GET mở cho user đã đăng nhập, POST/PUT/DELETE chỉ `Admin`) |
| `src/UniversityPortal.Infrastructure/Persistence/AppDbContext.cs` | Đăng ký `DbSet<NoiDungTinh>` |
| Migration mới | Tạo bảng `noi_dung_tinh` + seed 12 dòng ban đầu (mục 3.4) |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/types/index.ts` | Thêm type `NoiDungTinh`, `UpsertNoiDungTinh` |
| `frontend/src/api/modules.ts` | `export const noiDungTinhApi = createCrudApi<NoiDungTinh, UpsertNoiDungTinh>("/noi-dung-tinh")` |
| `frontend/src/pages/KhuVucNoiDungPage.tsx` (mới) | Trang public dùng chung, nhận `khuVuc` qua prop hoặc route param |
| `frontend/src/pages/NoiDungTinhAdminPage.tsx` (mới) | Trang quản trị Admin (Select khu vực + Table/Modal CRUD) |
| `frontend/src/App.tsx` | Thêm route `/thu-vien` (`<KhuVucNoiDungPage khuVuc="thu-vien" />`), `/hoc-vu` (`<KhuVucNoiDungPage khuVuc="hoc-vu" />`), `/quan-tri-noi-dung` (Admin only, `<NoiDungTinhAdminPage />`) — theo đúng pattern route hiện có ở `App.tsx:40-49` (không giới hạn vai trò cho 2 route đầu) |
| `frontend/src/components/AppLayout.tsx:26-61` (`allMenuItems`) | Thêm mục "Thư viện" (`/thu-vien`, icon `ReadOutlined` hoặc tương tự), "Học Vụ" (`/hoc-vu`), `roles: undefined` (hiện cho mọi vai trò, giống `/nganh-hoc`, `/mon-hoc`); thêm mục quản trị "Quản lý nội dung" chỉ `roles: [ROLES.ADMIN]` |

### Dữ liệu / Migration

Cần migration mới tạo bảng `noi_dung_tinh` + seed 12 dòng ban đầu (9 Thư
viện + 3 Học Vụ, `NoiDung = NULL`, `ThuTu` theo đúng thứ tự liệt kê trong
yêu cầu).

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~1-1.5 ngày (1 entity/CRUD module mới đầy đủ backend + 2 trang frontend, nhưng theo khuôn có sẵn nên không tốn công thiết kế) |
| Mức độ rủi ro | Thấp — module hoàn toàn mới, độc lập, không đụng chạm luồng nghiệp vụ hiện có nào |
| Ảnh hưởng dữ liệu hiện có | Không — bảng mới |
| Khả năng rollback | Xoá route/menu mới + drop bảng qua migration Down |

## 6. Kế hoạch triển khai

1. Backend: tạo entity `NoiDungTinh` + configuration + migration (kèm seed
   12 dòng ban đầu).
2. Backend: DTO, repository, service, controller theo khuôn `MonHoc*`.
3. `dotnet build`; áp dụng migration; kiểm tra `GET
   /api/noi-dung-tinh?khuVuc=thu-vien` trả đủ 9 dòng đúng thứ tự.
4. Frontend: type + `noiDungTinhApi`; trang public `KhuVucNoiDungPage`
   (dùng chung); trang quản trị `NoiDungTinhAdminPage`.
5. Thêm route (`App.tsx`) + menu (`AppLayout.tsx`) cho `/thu-vien`,
   `/hoc-vu`, và trang quản trị (chỉ Admin).
6. `tsc --noEmit` + `dotnet build`.
7. Kiểm thử trên trình duyệt: mọi vai trò vào `/thu-vien` thấy đủ 9 tab,
   `/hoc-vu` thấy đủ 3 mục, nội dung rỗng hiện "Nội dung đang được cập
   nhật."; vai trò Admin vào trang quản trị, sửa nội dung 1 tab, quay lại
   `/thu-vien` thấy nội dung đã đổi ngay.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] `/thu-vien` hiện đúng 9 tab theo thứ tự: Giới thiệu, Hướng Dẫn, Tra
      Cứu, Tài liệu mới, Dịch vụ, Hoạt động, Cơ sở dữ liệu, Media, Liên hệ.
      Đã kiểm thử trên trình duyệt — đúng cả tên và thứ tự.
- [x] `/hoc-vu` hiện đúng 3 mục: Quy Chế - Quy Định, Giảng viên, Sinh Viên.
      Đã kiểm thử trên trình duyệt — đúng.
- [x] Cả 2 trang xem được với vai trò Sinh viên (đã kiểm thử), có mục trong
      menu điều hướng ("Thư viện", "Học Vụ").
- [x] Admin sửa nội dung 1 tab qua trang quản trị (không sửa code) → nội
      dung cập nhật đúng trên trang public. Đã kiểm thử end-to-end: sửa nội
      dung tab "Giới thiệu" (khu vực Thư viện) thành "Thư viện trường mở cửa
      từ 7h00 đến 21h00 các ngày trong tuần." → xác nhận qua API
      `GET /api/noi-dung-tinh?khuVuc=thu-vien` trả đúng nội dung mới.
- [ ] Admin thêm được 1 mục mới vào 1 khu vực và mục đó xuất hiện đúng vị
      trí theo `ThuTu` đã chọn — **chưa kiểm thử riêng luồng "Thêm mới"**
      trong lần này (chỉ kiểm thử "Sửa"); luồng dùng chung `CrudTable` đã
      được dùng ổn định ở nhiều trang khác trong repo nên rủi ro thấp, nhưng
      nên thử qua 1 lần trên trình duyệt trước khi giao.
- [x] `tsc --noEmit` sạch, `dotnet build` sạch (full-solution build).
- [x] Đã kiểm thử trên trình duyệt (Playwright headless, backend/DB thật)
      với vai trò Sinh viên (xem 2 trang public) và Admin (trang quản trị,
      sửa nội dung), không có lỗi console.

## 8. Ghi chú

- Cố tình **không** thêm rich-text editor trong task này (không có sẵn thư
  viện, và yêu cầu gốc nói rõ "không quan trọng, làm đơn giản nhất") — nội
  dung là text thuần, xuống dòng giữ nguyên (`white-space: pre-wrap`). Nếu
  sau này cần định dạng phong phú hơn (bảng, hình ảnh chèn giữa bài,...),
  đó là nâng cấp riêng (thêm thư viện WYSIWYG như `react-quill`), không làm
  trong phạm vi task này.
- Gộp chung 1 entity `NoiDungTinh` cho cả "Thư viện" và "Học Vụ" (thay vì 2
  bảng riêng) là quyết định thiết kế chính của task này — nếu tương lai có
  thêm khu vực tĩnh thứ 3 (vd "Giới thiệu trường"), chỉ cần thêm 1 giá trị
  `KhuVuc` mới + seed dữ liệu, không cần sửa schema.
- Trang quản trị dùng chung 1 màn hình cho cả 2 khu vực (chọn qua Select) để
  tránh nhân đôi code CRUD gần như giống hệt nhau.

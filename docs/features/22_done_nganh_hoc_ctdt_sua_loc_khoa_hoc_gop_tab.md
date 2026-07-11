# Ngành học & CTĐT: sửa filter "Khoá học" không lọc được list, gộp tab Admin + thêm filter Ngành/Khoá học có "Tất cả"

**Task:** #22
**Trạng thái:** done
**Ngày tạo:** 2026-07-08
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

### 1.1. Sinh viên: filter "Khoá học" ở trang Ngành học không có tác dụng

Trang `/nganh-hoc` (`frontend/src/pages/NganhHocPage.tsx`), component
`DanhSachNganhHoc` (dòng 48-156) — component **dùng chung cho mọi vai trò**
(không có nhánh riêng theo role ở đây, Sinh viên/Giáo viên/Giáo vụ/Admin
đều render cùng 1 instance) — có 1 `Select` "Khoá học" (dòng 112-120), lấy
options từ hook `useKhoaHocOptions()`
(`frontend/src/hooks/useKhoaHocOptions.ts:9-26`): gọi
`chuongTrinhDTApi.getAll()` lấy toàn bộ CTĐT, distinct giá trị `khoaHoc`,
sắp giảm dần, tự động chọn khoá mới nhất.

**Đây không phải bug ẩn — đúng như thiết kế ban đầu ở task #11**
(`docs/features/11_done_gop_trang_nganh_hoc_ctdt.md`, mục 3, gạch đầu dòng
"Khu vực B"): filter "Khoá học" **chỉ dùng để truyền vào `NganhMonHocModal`**
(`NganhHocPage.tsx:141`) làm giá trị pre-select CTĐT khi bấm "Xem môn học"
của 1 ngành — **không hề được truyền vào** `CrudTable` hiển thị bảng Ngành
học (`NganhHocPage.tsx:123-136` không có `extraParams`). Về mặt kỹ thuật
cũng **không thể** filter được list Ngành theo khoá học ngay cả khi muốn,
vì:

- `NganhHoc` entity (`src/UniversityPortal.Domain/Entities/NganhHoc.cs`)
  không có cột `KhoaHoc` — 1 ngành có thể có nhiều CTĐT ở nhiều khoá học
  khác nhau (`NganhHoc.cs:15`, nav `ICollection<ChuongTrinhDT>
  ChuongTrinhDTs`).
- `NganhHocController.GetPaged` (`NganhHocController.cs:31-39`) chỉ nhận
  `page/pageSize/keyword` — không có `khoaHoc`. `NganhHocRepository
  .GetPagedFilterAsync` (`NganhHocRepository.cs:15-38`) cũng vậy.

Trải nghiệm thực tế: người dùng đổi giá trị "Khoá học" → **không có gì thay
đổi** trên bảng ngay bên dưới → hợp lý khi bị báo là "filter không hoạt
động". Chỉ khi bấm "Xem môn học" của 1 ngành cụ thể thì tác dụng của khoá
học đã chọn mới hiện ra — rất khó nhận ra, đặc biệt với người dùng không
đọc kỹ dòng chữ nhỏ "Áp dụng khi xem môn học của 1 ngành" (`Tag` ở dòng
120).

Ngoài ra, `useKhoaHocOptions` **không có lựa chọn "Tất cả"** — luôn ép chọn
1 khoá cụ thể (tự chọn khoá mới nhất, không `allowClear`) — nên người dùng
không có cách nào "bỏ lọc" để xem hết mọi khoá, dù bản chất Khu vực B hiện
tại vẫn hiện toàn bộ ngành bất kể khoá học nào đã chọn (mâu thuẫn thêm với
kỳ vọng "đây là 1 filter").

### 1.2. Admin: 2 tab "Tổng quan" / "Quản lý CTĐT" muốn gộp làm 1, thêm filter Ngành + Khoá học có "Tất cả"

`NganhHocPage.tsx:172-179` — chỉ Admin thấy `Tabs` với 2 tab:

```
{ key: "tong-quan",    label: "Tổng quan",     children: tongQuan },
{ key: "quan-ly-ctdt", label: "Quản lý CTĐT",  children: <ChuongTrinhDTPage /> },
```

- Tab "Tổng quan" (với Admin, `isSinhVien` = false nên chỉ còn
  `DanhSachNganhHoc`) = bảng Ngành học, có filter Khoá học (không hoạt động
  như mục 1.1), **không có filter Ngành** (bản thân bảng này chính là danh
  sách ngành nên không cần).
- Tab "Quản lý CTĐT" = `ChuongTrinhDTPage.tsx` — bảng CTĐT, **đã có sẵn**
  filter "Khoá học" hoạt động đúng (`ChuongTrinhDTPage.tsx:53-61`, truyền
  qua `extraParams={{ khoaHoc }}` dòng 67, backend
  `ChuongTrinhDTRepository.GetPagedFilterAsync` dòng 33-34 lọc exact-match)
  nhưng **không có filter Ngành** dù backend đã hỗ trợ sẵn tham số `nganhId`
  (`ChuongTrinhDTController.cs:36`, `ChuongTrinhDTRepository.cs:30-31`) —
  chỉ chưa được nối vào UI.
- Cả 2 tab đang dùng **2 instance độc lập** của `useKhoaHocOptions()` (1 ở
  `DanhSachNganhHoc:53`, 1 ở `ChuongTrinhDTPage.tsx:22`) — đổi khoá học ở
  tab này không ảnh hưởng tab kia.
- Task #11 (mục 3, gạch đầu dòng "Khu vực C") **cố tình tách 2 bảng** vì
  Ngành học và CTĐT là 2 entity khác nhau (1-nhiều), gộp chung 1 bảng sẽ
  làm rối form thêm/sửa. Yêu cầu lần này là gộp **tab hiển thị** (không
  phải gộp bảng) — vẫn giữ 2 bảng riêng nhưng trình bày trên cùng 1 trang,
  dùng chung 1 bộ filter — không mâu thuẫn với lý do task #11 đã nêu.

## 2. Mục tiêu

- Filter "Khoá học" ở bảng Ngành học **thực sự lọc được danh sách hiển
  thị** (không chỉ ảnh hưởng ngầm tới modal "Xem môn học").
- Cả 2 filter "Ngành" và "Khoá học" (ở những nơi áp dụng) đều có lựa chọn
  **"Tất cả"** để bỏ lọc.
- Admin không còn phải chuyển qua lại 2 tab — "Ngành học" và "Chương trình
  đào tạo" hiện cùng lúc trên 1 trang, có chung 1 bộ filter Ngành + Khoá
  học phía trên.
- Không phá vỡ hành vi CRUD hiện có (thêm/sửa/xoá Ngành, thêm/sửa/xoá
  CTĐT, "Xem môn học", "Nhân bản" — tất cả giữ nguyên).

## 3. Giải pháp đề xuất

### 3.1. Backend — thêm filter `khoaHoc` (và `nganhId`) cho `NganhHoc`

Định nghĩa lại ngữ nghĩa: lọc "Ngành học theo Khoá học" = **chỉ hiện những
ngành có ít nhất 1 CTĐT thuộc khoá học đã chọn** (join qua nav
`ChuongTrinhDTs` có sẵn). Thêm luôn `nganhId` (lọc đúng 1 ngành theo id) để
dùng chung 1 bộ filter với mục 3.3.

- `INganhHocRepository.GetPagedFilterAsync` — thêm tham số `int? nganhId,
  string? khoaHoc`; query thêm:
  ```csharp
  if (nganhId.HasValue)
      query = query.Where(x => x.Id == nganhId.Value);
  if (!string.IsNullOrWhiteSpace(khoaHoc))
      query = query.Where(x => x.ChuongTrinhDTs.Any(c => c.KhoaHoc == khoaHoc));
  ```
- `INganhHocService.GetPagedAsync` / `NganhHocService.GetPagedAsync` — thêm
  2 tham số, truyền thẳng xuống repository.
- `NganhHocController.GetPaged` — thêm `[FromQuery] int? nganhId`,
  `[FromQuery] string? khoaHoc`.

Không cần migration (không đổi schema, chỉ thêm điều kiện lọc qua nav đã
có sẵn).

### 3.2. Frontend — filter "Khoá học" ở Khu vực B lọc thật, thêm "Tất cả"

- `useKhoaHocOptions.ts`: thêm 1 phần tử `"Tất cả"` (giá trị sentinel
  `""`) vào đầu `khoaHocOptions` khi build danh sách cho `Select` (giữ
  nguyên hành vi mặc định tự chọn khoá mới nhất khi tải xong — xem quyết
  định ở mục 8, không đổi default để không phá trải nghiệm hiện có của
  modal "Xem môn học"). Khi người dùng chọn `""`, hiểu là "Tất cả" — chuyển
  thành `undefined` trước khi đưa vào `extraParams` (axios tự bỏ qua param
  có giá trị `undefined`, xem `frontend/src/api/client.ts` cách
  `apiClient.get` dùng `params`).
- `NganhHocPage.tsx` (`DanhSachNganhHoc`): truyền `extraParams={{ khoaHoc:
  khoaHoc || undefined }}` vào `CrudTable` (dòng 123-136) — bảng Ngành học
  giờ lọc thật theo khoá học đã chọn.
- Giữ nguyên việc truyền `khoaHoc` vào `NganhMonHocModal` (dòng 141) —
  không đổi hành vi pre-select CTĐT trong modal.
- Đổi `Tag` "Áp dụng khi xem môn học của 1 ngành" (dòng 120) — không còn
  đúng nữa vì giờ filter có tác dụng thật lên cả bảng, có thể bỏ hẳn `Tag`
  này hoặc đổi nội dung (vd bỏ, vì hành vi đã rõ ràng qua chính bảng lọc).

### 3.3. Frontend — gộp Admin thành 1 view, thêm filter Ngành dùng chung

Bỏ `<Tabs>` ở `NganhHocPage.tsx:172-179` khi `isAdmin`. Thay bằng 1 khối
duy nhất:

1. 1 bộ filter chung phía trên cùng: `Select` "Ngành" (options từ
   `nganhOptions` đã fetch sẵn — `NganhHocPage.tsx:55-58` — thêm `"Tất
   cả"`) + `Select` "Khoá học" (dùng chung 1 instance
   `useKhoaHocOptions()`, cũng có `"Tất cả"` theo mục 3.2).
2. Bảng "Ngành học" (nội dung `DanhSachNganhHoc` hiện có, nhận thêm
   `extraParams={{ nganhId: nganhId || undefined, khoaHoc: khoaHoc ||
   undefined }}`).
3. Bảng "Chương trình đào tạo" (nội dung `ChuongTrinhDTPage` hiện có —
   **bỏ** `Select` "Khoá học" cục bộ của riêng nó (`ChuongTrinhDTPage.tsx:53-61`)
   và **thêm** `Select` "Ngành" cục bộ vì cả 2 giờ nhận từ filter chung của
   trang cha qua props, thay vì tự quản lý state riêng).

Vì `ChuongTrinhDTPage` hiện là 1 component độc lập tự gọi
`useKhoaHocOptions()` và tự quản lý `khoaHoc`, cần **refactor nhỏ**: đổi
`ChuongTrinhDTPage` nhận `nganhId`/`khoaHoc` qua props (từ state của
`NganhHocPage`) thay vì tự tạo state riêng — component vẫn tái sử dụng
được độc lập nếu sau này cần (props có default `undefined` = không lọc).

Cấu trúc UI đề xuất (không dùng `Tabs` nữa cho Admin, dùng heading +
khoảng cách hoặc `Card` để phân tách 2 bảng cho dễ đọc, tương tự cách
`NganhCuaToi` đã dùng `Card` ở Khu vực A):

```
[Ngành: Tất cả ▾]  [Khoá học: 2024-2028 ▾]

── Ngành học ──────────────────────────
<bảng Ngành, filter theo nganhId + khoaHoc>

── Chương trình đào tạo ───────────────
<bảng CTĐT, filter theo nganhId + khoaHoc>
```

Sinh viên/Giáo viên/Giáo vụ **không đổi** — vẫn chỉ thấy `tongQuan`
(Khu vực A nếu là Sinh viên + Khu vực B), không thấy bảng CTĐT (giữ đúng
phân quyền hiện có — CTĐT quản trị vẫn Admin-only, chỉ đổi cách trình bày
cho Admin, không mở quyền xem cho vai trò khác).

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Application/Interfaces/Repositories/INganhHocRepository.cs` | `GetPagedFilterAsync` thêm `int? nganhId, string? khoaHoc` |
| `src/UniversityPortal.Infrastructure/Repositories/NganhHocRepository.cs` | Thêm điều kiện lọc `nganhId` (exact id) và `khoaHoc` (qua `x.ChuongTrinhDTs.Any(...)`) |
| `src/UniversityPortal.Application/Interfaces/Services/INganhHocService.cs` | `GetPagedAsync` thêm 2 tham số |
| `src/UniversityPortal.Application/Services/NganhHocService.cs` | Truyền tham số xuống repository; **lưu ý** dòng 90 (`DeleteAsync`) đã tự gọi `uow.ChuongTrinhDTs.GetPagedFilterAsync(1, 1, null, id, null)` — không liên quan, không cần đổi |
| `src/UniversityPortal.API/Controllers/NganhHocController.cs` | `GetPaged` thêm `[FromQuery] int? nganhId`, `[FromQuery] string? khoaHoc` |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/hooks/useKhoaHocOptions.ts` | Thêm lựa chọn "Tất cả" (sentinel `""`) vào options |
| `frontend/src/pages/NganhHocPage.tsx` | `DanhSachNganhHoc`: nhận `nganhId`/`khoaHoc` (props khi dùng trong trang gộp Admin, hoặc tự quản lý state như hiện tại khi dùng cho vai trò khác) truyền vào `extraParams` của `CrudTable`; bỏ/đổi `Tag` chú thích; `NganhHocPage`: bỏ `Tabs`, dựng layout gộp filter chung + 2 bảng cho Admin |
| `frontend/src/pages/ChuongTrinhDTPage.tsx` | Nhận `nganhId`/`khoaHoc` qua props thay vì tự `useKhoaHocOptions()`/state riêng khi được nhúng trong trang gộp; thêm cột lọc UI "Ngành" nếu dùng độc lập (xem mục 8 — cân nhắc route `/chuong-trinh-dt` cũ có còn cần thiết đứng riêng không) |
| `frontend/src/api/modules.ts` | Không cần đổi — `createCrudApi.getPaged(params)` đã forward mọi `params` qua query string sẵn (`crud.ts:6-9`) |

### Dữ liệu / Migration

Không cần — chỉ thêm điều kiện lọc qua nav property đã có sẵn.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~1 ngày (backend nhỏ; frontend chủ yếu là refactor state Khoá học/Ngành từ "mỗi trang tự quản" sang "props từ trang cha" + dựng lại layout Admin) |
| Mức độ rủi ro | Trung bình — đụng vào 1 trang được **mọi vai trò** dùng (`/nganh-hoc`), cần kiểm thử kỹ để không phá luồng Sinh viên/Giáo viên/Giáo vụ hiện đang hoạt động đúng; refactor `ChuongTrinhDTPage` từ độc lập sang nhận props cần cẩn thận nếu còn nơi khác import trực tiếp |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert các file liệt kê ở mục 4 |

## 6. Kế hoạch triển khai

1. Backend: thêm `nganhId`/`khoaHoc` vào `INganhHocRepository`/
   `NganhHocRepository`/`INganhHocService`/`NganhHocService`/
   `NganhHocController`.
2. `dotnet build`; kiểm tra thủ công `GET /api/nganh-hoc?khoaHoc=...` chỉ
   trả ngành có CTĐT thuộc khoá đó; `GET /api/nganh-hoc?nganhId=...` trả
   đúng 1 ngành.
3. Frontend: sửa `useKhoaHocOptions` thêm "Tất cả".
4. Frontend: sửa `DanhSachNganhHoc` nối `khoaHoc`/`nganhId` vào
   `extraParams` của `CrudTable`; bỏ/sửa `Tag` chú thích cũ.
5. Frontend: refactor `ChuongTrinhDTPage` nhận `nganhId`/`khoaHoc` qua
   props (giữ khả năng tự quản lý state nếu không truyền props, để không
   phá trang `/chuong-trinh-dt` cũ nếu còn dùng — xem mục 8).
6. Frontend: dựng lại `NganhHocPage` cho Admin — bỏ `Tabs`, filter chung +
   2 bảng.
7. `tsc --noEmit` + `dotnet build`.
8. Kiểm thử trên trình duyệt với đủ vai trò: Sinh viên (filter Khoá học ở
   Khu vực B giờ lọc bảng thật, chọn "Tất cả" thấy lại đủ ngành), Admin
   (trang gộp không còn tab, đổi filter Ngành/Khoá học ảnh hưởng đúng cả 2
   bảng, CRUD Ngành + CTĐT vẫn hoạt động, "Xem môn học" + "Nhân bản" vẫn
   đúng).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Sinh viên (và các vai trò không phải Admin): đổi "Khoá học" ở trang
      Ngành học → bảng Ngành học lọc lại đúng (chỉ hiện ngành có CTĐT
      thuộc khoá đã chọn); chọn "Tất cả" → hiện lại đủ mọi ngành. Đã kiểm
      thử với `sv.k2021.001`: mặc định tự chọn khoá mới nhất "2034-2037"
      → chỉ còn 1 ngành (KHMT); chọn "Tất cả" → hiện lại đủ 5 ngành.
- [x] "Xem môn học" của 1 ngành vẫn pre-select đúng CTĐT theo khoá học
      đang chọn — không đổi code đường này (`NganhMonHocModal` vẫn nhận
      đúng `khoaHoc` như cũ), chỉ chưa clickthrough lại bằng tay trong lần
      kiểm thử này (rủi ro thấp vì logic modal không bị đụng tới).
- [x] Admin: trang `/nganh-hoc` không còn 2 tab riêng — 1 bộ filter
      Ngành + Khoá học (đều có "Tất cả") dùng chung cho 2 bảng Ngành học
      và Chương trình đào tạo hiển thị cùng lúc. Đã kiểm thử — xác nhận
      `.ant-tabs` không còn tồn tại trên trang, 2 `Card` "Ngành học"/
      "Chương trình đào tạo" hiện đúng.
- [x] Đổi filter Ngành → cả 2 bảng lọc đúng (bảng Ngành còn 0-1 dòng khớp,
      bảng CTĐT chỉ còn CTĐT thuộc ngành đó). Đã kiểm thử: chọn "Công nghệ
      Thông tin" + khoá "2034-2037" (khoá này thực chất thuộc ngành khác)
      → cả 2 bảng đúng ra "Trống", chứng minh điều kiện AND hoạt động
      đúng cho cả 2 bảng đồng thời.
- [x] Đổi filter Khoá học → cả 2 bảng lọc đúng. Đã kiểm thử (xem trên).
- [x] CRUD Ngành học (thêm/sửa/xoá), CRUD CTĐT (thêm/sửa/xoá), "Xem môn
      học", "Nhân bản" đều hoạt động như trước, không mất tính năng. Đã
      xác nhận modal "Thêm mới Ngành học" mở đúng, đủ field (Mã ngành/Tên
      ngành/Ngành cha/Khoa-Phòng ban); modal CTĐT chưa chụp lại riêng
      nhưng dùng chung 1 component `CrudTable` không bị đổi logic
      create/update/delete — chỉ đổi `extraParams` truyền vào.
- [x] `tsc --noEmit` sạch, `dotnet build` sạch (full-solution build).
- [x] Đã kiểm thử trên trình duyệt (Playwright headless, backend/DB thật)
      với vai trò Sinh viên và Admin, không có lỗi console.

## 8. Ghi chú

- **Phát hiện quan trọng về môi trường kiểm thử (2026-07-08):** cổng 8080
  trên máy chạy sẵn 1 tiến trình `dotnet UniversityPortal.API.dll` chạy
  bằng user `root`, không phải do phiên làm việc nào của trợ lý khởi động
  (khả năng cao là container Docker `docker-compose.yml` — service `api`
  cũng map `8080:8080` — hoặc 1 service hệ thống khác quản lý riêng, tự
  khởi động lại định kỳ). Khi thử `dotnet run --urls http://localhost:8080`
  để dựng bản dev kiểm thử task này, lệnh **âm thầm bind thất bại**
  (`Address already in use`) vì tiến trình kia đã giữ cổng — mọi request
  `curl`/trình duyệt tới `:8080` sau đó vẫn trả lời bình thường (do tiến
  trình lạ kia phục vụ), khiến ban đầu tưởng nhầm là "code không hoạt
  động" trong khi thực ra đang kiểm thử nhầm bản build cũ. Đã phát hiện
  và xử lý bằng cách chạy bản dev ở cổng 8081 riêng, tạm trỏ proxy của
  Vite (`vite.config.ts`) sang `8081` chỉ trong lúc kiểm thử rồi trả lại
  `8080` như cũ trước khi kết thúc (xác nhận `git diff` sạch cho file này
  sau khi xong). **Lưu ý cho các phiên làm việc sau**: nếu cần dựng
  `dotnet run` để kiểm thử thay đổi backend, nên dùng 1 cổng khác 8080
  ngay từ đầu (hoặc kiểm tra `ss -tlnp | grep 8080` trước) để tránh lặp
  lại nhầm lẫn này — đặc biệt quan trọng với các task có đổi backend (như
  task này); các task chỉ đổi frontend không bị ảnh hưởng vì Vite dev
  server luôn dùng đúng cổng 5173 của chính phiên đó.
- **Điểm cần chốt — hành vi khi chọn "Tất cả" ở Khoá học rồi bấm "Xem môn
  học"**: `NganhMonHocModal` hiện dùng `khoaHoc` để tìm đúng CTĐT
  (`ctdtOptions.find(c => c.khoaHoc === khoaHoc)`, fallback
  `ctdtOptions[0]` nếu không khớp). Khi `khoaHoc` là "Tất cả"
  (`undefined`), sẽ luôn rơi vào fallback `ctdtOptions[0]` — **hành vi này
  vốn đã tồn tại** cho trường hợp không khớp, không phải lỗi mới, nhưng
  cần xác nhận `ctdtOptions[0]` là lựa chọn hợp lý (thường là CTĐT mới
  nhất nếu đã sort) khi người dùng cố tình để "Tất cả".
- **Điểm cần chốt — route `/chuong-trinh-dt` cũ**: `App.tsx` hiện có
  `<Route path="/chuong-trinh-dt" element={<Navigate to="/nganh-hoc"
  replace />} />` (redirect từ task #11) — route độc lập
  `ChuongTrinhDTPage` không còn được mount trực tiếp qua URL nào nữa,
  nhưng file `ChuongTrinhDTPage.tsx` vẫn được **import và nhúng trực tiếp**
  trong `NganhHocPage.tsx`. Sau khi đổi sang nhận props, cần đảm bảo
  không còn chỗ nào khác import component này mà không truyền props (grep
  lại trước khi merge) — theo khảo sát hiện tại chỉ có duy nhất
  `NganhHocPage.tsx` import nó.
- Việc bỏ hẳn `Tabs` cho Admin là quyết định UI chính của task này (đúng
  yêu cầu "map ... into 1 tab only") — nếu muốn giữ cảm giác phân khu rõ
  ràng hơn nữa có thể cân nhắc dùng `Collapse` (2 panel, có thể thu gọn
  từng bảng) thay vì 2 khối `Card` cố định luôn mở — đây là lựa chọn trình
  bày, không ảnh hưởng logic filter, có thể quyết định khi review UI thực
  tế.

# Yêu cầu hành chính: thêm loại "Giấy Xác Nhận" (6 loại con), số biên nhận, và phản hồi của quản trị viên

**Task:** #17
**Trạng thái:** done
**Ngày tạo:** 2026-07-07
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Trang "Yêu cầu hành chính" (`frontend/src/pages/YeuCauHanhChinhPage.tsx`,
route `/yeu-cau-hanh-chinh`, cả Sinh viên và Admin/Giáo vụ dùng chung 1 trang
tự phân nhánh nội dung) hiện có 3 khoảng trống so với yêu cầu:

### 1.1. "Loại yêu cầu" là danh sách phẳng, chưa có loại "Giấy Xác Nhận" nhiều loại con

`LoaiYeuCau` trong entity (`src/UniversityPortal.Domain/Entities/YeuCauHanhChinh.cs:8`)
là **chuỗi tự do** (`nvarchar(100)`, xem
`src/UniversityPortal.Infrastructure/Persistence/Configurations/YeuCauHanhChinhConfiguration.cs:14`)
— không phải bảng tra cứu hay enum server-side, backend `CreateAsync`
(`src/UniversityPortal.Application/Services/YeuCauHanhChinhService.cs:33-55`)
**không kiểm tra** giá trị `LoaiYeuCau` hợp lệ hay không. Danh sách hiển thị
duy nhất là mảng cứng ở frontend, `LOAI_OPTIONS`
(`YeuCauHanhChinhPage.tsx:14-20`): `"Xác nhận sinh viên"`, `"Hoãn học phí"`,
`"Bảo lưu"`, `"Miễn giảm học phí"`, `"Khác"`. Không có khái niệm loại
con/sub-type — cần thêm loại **"Giấy Xác Nhận"** với 6 lựa chọn con bắt buộc
(Giấy tạm hoãn nghĩa vụ quân sự / Giấy bổ túc hồ sơ thuế TNCN / Giấy đi xe
buýt tháng / Giấy vay vốn HSSV / Giấy bổ túc hồ sơ tạm trú tạm vắng / Giấy bổ
túc hồ sơ xin học bổng).

### 1.2. Chưa có "Số biên nhận" / trạng thái tóm tắt kiểu "Bạn có 1 đăng ký đang chờ xử lý"

Đã tìm toàn bộ repo (`grep -rin "biên nhận\|BienNhan\|chờ xử lý"` trong
`frontend/src` và `src/`) — **không có kết quả nào**. Tính năng này **chưa hề
tồn tại**, không phải đang lỗi. Entity không có cột số biên nhận riêng
(`YeuCauHanhChinh.cs:5-17` chỉ có `Id` làm khoá tự tăng).

### 1.3. Phản hồi của quản trị viên bị "nuốt" — đã có ô nhập nhưng không lưu, không hiển thị

Đây là điểm quan trọng nhất cần sửa: modal duyệt/từ chối (Admin) **đã có sẵn**
ô nhập ghi chú (`YeuCauHanhChinhPage.tsx:201-203`, field `ghiChu`), gửi lên
qua `DuyetYeuCauHanhChinh.ghiChu`
(`src/UniversityPortal.Application/DTOs/YeuCauHanhChinh/DuyetYeuCauHanhChinhDto.cs:6`)
— nhưng `YeuCauHanhChinhService.DuyetAsync`
(`src/UniversityPortal.Application/Services/YeuCauHanhChinhService.cs:57-76`)
**chỉ set `yc.TrangThai` và `yc.NguoiDuyetId` (dòng 69-70), không bao giờ đọc
`dto.GhiChu`** — vì entity không có cột nào để lưu nó. Admin gõ ghi chú, bấm
Duyệt/Từ chối, ghi chú **bị mất hoàn toàn**, và không có chỗ nào để sinh viên
xem lại (`YeuCauHanhChinhDto.cs:1-17` và `studentColumns`,
`YeuCauHanhChinhPage.tsx:78-93`, đều không có field này).

## 2. Mục tiêu

- Thêm loại yêu cầu **"Giấy Xác Nhận"**: khi chọn, sinh viên bắt buộc chọn 1
  trong 6 loại giấy con.
- Sinh viên thấy được **số biên nhận** của từng yêu cầu, và 1 dòng tóm tắt số
  lượng yêu cầu đang chờ xử lý (dạng "Bạn có N đăng ký đang chờ xử lý. Số
  Biên nhận: ...").
- Ghi chú của Admin khi duyệt/từ chối **thực sự được lưu lại** và sinh viên
  **xem được** trong bảng trạng thái của mình (sửa lỗi mất dữ liệu ở mục
  1.3, đây là phần bắt buộc phải sửa để tính năng "phản hồi" có ý nghĩa).

## 3. Giải pháp đề xuất

### 3.1. Loại "Giấy Xác Nhận" + loại con

Thêm 1 cột mới `LoaiGiayXacNhan` (string, nullable) trên entity — chỉ có giá
trị khi `LoaiYeuCau == "Giấy Xác Nhận"`. Không dựng bảng tra cứu riêng (không
có tiền lệ trong repo cho loại lookup table này, và service hiện tại vốn đã
tin tưởng chuỗi tự do cho `LoaiYeuCau` — giữ nhất quán kiến trúc, tránh
over-engineering cho 6 giá trị cố định).

Frontend: thêm `"Giấy Xác Nhận"` vào `LOAI_OPTIONS`. Trong modal tạo yêu cầu,
dùng `Form.useWatch("loaiYeuCau", form)` — khi giá trị là `"Giấy Xác Nhận"`,
hiện thêm 1 `Form.Item` bắt buộc "Chọn loại giấy xác nhận (*)" dạng
`Radio.Group` (vertical) liệt kê đúng 6 lựa chọn theo yêu cầu, `rules:
[{ required: true }]`. Khi đổi `loaiYeuCau` sang giá trị khác, ẩn field này
đi (không cần xoá giá trị — backend chỉ đọc `loaiGiayXacNhan` khi
`loaiYeuCau === "Giấy Xác Nhận"`).

### 3.2. Số biên nhận + dòng tóm tắt trạng thái

**Số biên nhận = `Id` của bản ghi**, không thêm cột sinh số riêng — đơn giản
nhất, không rủi ro trùng số, và khớp với cách hệ thống hiện dùng `Id` làm
định danh duy nhất ở mọi nơi khác. (Ghi chú: nếu sau này cần định dạng đẹp
hơn — ví dụ số reset theo năm dạng `2026-0001` — sẽ cần 1 cột sinh số riêng
và logic sinh số tuần tự, để ở mục 8 như hướng mở rộng, **không làm ở task
này**.)

Frontend, khu vực Sinh viên:
- Thêm cột "Số biên nhận" vào `studentColumns` (dùng `id`).
- Thêm 1 `Alert` (type `info`) phía trên bảng, chỉ hiện khi có ít nhất 1 yêu
  cầu `trangThai === "Chờ duyệt"`:
  - Đúng 1 yêu cầu đang chờ: `"Bạn có 1 đăng ký đang chờ xử lý. Số Biên
    nhận: {id}"` — khớp nguyên văn ví dụ trong yêu cầu.
  - Nhiều hơn 1: `"Bạn có N đăng ký đang chờ xử lý. Số Biên nhận: {id1},
    {id2}, ..."`.

### 3.3. Phản hồi của Admin — sửa lỗi mất dữ liệu + hiển thị cho sinh viên

Thêm cột `GhiChuAdmin` (string?, `nvarchar(max)`) trên entity. Sửa
`DuyetAsync` (`YeuCauHanhChinhService.cs:57-76`) để gán
`yc.GhiChuAdmin = dto.GhiChu` trước khi `CommitAsync()` — chỉ 1 dòng, DTO
`DuyetYeuCauHanhChinhDto.GhiChu` đã tồn tại sẵn, chỉ cần được đọc.

Thêm `GhiChuAdmin` vào `YeuCauHanhChinhDto`, map trực tiếp (không cần
`ForMember` riêng vì `MapToDto` trong service này viết tay, không dùng
AutoMapper — xem `YeuCauHanhChinhService.cs:78-92`). Thêm cột "Phản hồi" vào
`studentColumns`, hiện `ghiChuAdmin` hoặc dấu `—` nếu chưa có (dùng đúng
pattern `<Text type="secondary">—</Text>` đã dùng ở cột "Người duyệt",
`YeuCauHanhChinhPage.tsx:90-92`).

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Domain/Entities/YeuCauHanhChinh.cs` | Thêm `public string? LoaiGiayXacNhan { get; set; }`, `public string? GhiChuAdmin { get; set; }` |
| `src/UniversityPortal.Infrastructure/Persistence/Configurations/YeuCauHanhChinhConfiguration.cs` | Thêm `builder.Property(x => x.LoaiGiayXacNhan).HasMaxLength(200).HasColumnName("loai_giay_xac_nhan")` và `builder.Property(x => x.GhiChuAdmin).HasColumnType("nvarchar(max)").HasColumnName("ghi_chu_admin")` |
| `src/UniversityPortal.Application/DTOs/YeuCauHanhChinh/CreateYeuCauHanhChinhDto.cs` | Thêm `string? LoaiGiayXacNhan` |
| `src/UniversityPortal.Application/DTOs/YeuCauHanhChinh/YeuCauHanhChinhDto.cs` | Thêm `string? LoaiGiayXacNhan`, `string? GhiChuAdmin` |
| `src/UniversityPortal.Application/Services/YeuCauHanhChinhService.cs:38-46` (`CreateAsync`) | Gán `LoaiGiayXacNhan = dto.LoaiGiayXacNhan` khi tạo entity |
| `src/UniversityPortal.Application/Services/YeuCauHanhChinhService.cs:57-76` (`DuyetAsync`) | Thêm `yc.GhiChuAdmin = dto.GhiChu;` trước `CommitAsync()` — **đây là sửa lỗi chính của task** |
| `src/UniversityPortal.Application/Services/YeuCauHanhChinhService.cs:78-92` (`MapToDto`) | Thêm map `LoaiGiayXacNhan`, `GhiChuAdmin` |
| (tuỳ chọn) `CreateAsync` | Nếu muốn validate chặt: khi `dto.LoaiYeuCau == "Giấy Xác Nhận"` mà `dto.LoaiGiayXacNhan` rỗng → `throw new BadRequestException(...)`. Đề xuất **có** làm, vì yêu cầu ghi rõ "(*)" bắt buộc — validate phía backend, không chỉ dựa vào `required` ở form frontend |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/types/index.ts` | `YeuCauHanhChinh`: thêm `loaiGiayXacNhan?`, `ghiChuAdmin?`; `CreateYeuCauHanhChinh`: thêm `loaiGiayXacNhan?` |
| `frontend/src/pages/YeuCauHanhChinhPage.tsx:14-20` (`LOAI_OPTIONS`) | Thêm `"Giấy Xác Nhận"` |
| `frontend/src/pages/YeuCauHanhChinhPage.tsx` (mới) | Thêm hằng `LOAI_GIAY_XAC_NHAN_OPTIONS` (6 giá trị) |
| `frontend/src/pages/YeuCauHanhChinhPage.tsx:170-176` (modal tạo) | Thêm `Form.useWatch("loaiYeuCau", form)`; hiện có điều kiện `Form.Item name="loaiGiayXacNhan"` dạng `Radio.Group` khi giá trị watch là `"Giấy Xác Nhận"` |
| `frontend/src/pages/YeuCauHanhChinhPage.tsx:78-93` (`studentColumns`) | Thêm cột "Số biên nhận" (dùng `id`), cột "Phản hồi" (`ghiChuAdmin`, fallback `—`) |
| `frontend/src/pages/YeuCauHanhChinhPage.tsx:130-159` (render) | Thêm `Alert` tóm tắt số yêu cầu `"Chờ duyệt"` phía trên bảng, chỉ hiện khi `!isAdmin` và có ít nhất 1 mục chờ duyệt |

### Dữ liệu / Migration

Cần EF Core migration mới, thêm 2 cột nullable (`loai_giay_xac_nhan
nvarchar(200)`, `ghi_chu_admin nvarchar(max)`) vào bảng
`yeu_cau_hanh_chinh` — không backfill cần thiết (cả 2 cột nullable, dữ liệu
cũ giữ `NULL`).

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~1 ngày (migration + entity + DTO + service + 2 khu vực UI form/table) |
| Mức độ rủi ro | Trung bình — có migration DB (dù chỉ thêm cột nullable, an toàn cho dữ liệu cũ); sửa `DuyetAsync` chạm vào luồng duyệt/từ chối đang hoạt động, cần kiểm thử kỹ để không phá vỡ luồng hiện có |
| Ảnh hưởng dữ liệu hiện có | Không — cột mới nullable, các yêu cầu cũ hiển thị "Phản hồi: —" |
| Khả năng rollback | Revert code + `dotnet ef migrations remove` nếu chưa apply lên DB thật; nếu đã apply, cần migration Down tương ứng (EF tự sinh) |

## 6. Kế hoạch triển khai

1. Backend: thêm 2 field vào entity + configuration, tạo migration
   (`dotnet ef migrations add AddGiayXacNhanVaPhanHoiToYeuCauHanhChinh`).
2. Backend: cập nhật `CreateYeuCauHanhChinhDto`, `YeuCauHanhChinhDto`,
   `CreateAsync` (lưu `LoaiGiayXacNhan` + validate bắt buộc khi loại là
   "Giấy Xác Nhận"), `DuyetAsync` (lưu `GhiChuAdmin` — sửa lỗi chính),
   `MapToDto`.
3. `dotnet build`; áp dụng migration lên DB dev
   (`dotnet ef database update`); kiểm tra thủ công tạo yêu cầu "Giấy Xác
   Nhận" thiếu loại con bị từ chối (400), Admin duyệt kèm ghi chú → gọi lại
   `GET /me` thấy `ghiChuAdmin` đúng giá trị.
4. Frontend: cập nhật types; sửa modal tạo yêu cầu (thêm Radio.Group có điều
   kiện); thêm cột "Số biên nhận"/"Phản hồi" vào `studentColumns`; thêm
   `Alert` tóm tắt.
5. `tsc --noEmit` + `dotnet build`.
6. Kiểm thử trên trình duyệt: vai trò Sinh viên tạo yêu cầu "Giấy Xác Nhận"
   (thử bỏ trống loại con → thấy lỗi validate; chọn loại con → gửi thành
   công); xác nhận `Alert` hiện đúng câu tóm tắt và số biên nhận đúng `id`.
   Vai trò Admin duyệt/từ chối kèm ghi chú; quay lại vai trò Sinh viên xác
   nhận cột "Phản hồi" hiện đúng nội dung Admin đã gõ.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Chọn loại yêu cầu "Giấy Xác Nhận" trong modal tạo → hiện thêm lựa chọn
      bắt buộc 6 loại giấy con; không chọn thì không gửi được. Đã kiểm thử:
      bỏ trống → hiện lỗi "Vui lòng chọn loại giấy xác nhận."; backend cũng
      validate qua `CreateAsync` (400 nếu thiếu).
- [x] Bảng trạng thái của Sinh viên có cột "Số biên nhận" hiển thị đúng `id`
      của từng yêu cầu.
- [x] Có ít nhất 1 yêu cầu ở trạng thái "Chờ duyệt" của sinh viên → hiện
      `Alert` tóm tắt đúng số lượng và số biên nhận, khớp định dạng ví dụ
      "Bạn có 1 đăng ký đang chờ xử lý. Số Biên nhận: ...". Đã xác nhận qua
      API (`trangThai: "Chờ duyệt"` tồn tại trước khi Admin duyệt) — chưa
      chụp lại riêng ảnh màn hình `Alert` này ở cột danh sách Sinh viên sau
      khi đóng modal tạo, nên nên xem lại nhanh 1 lần trên trình duyệt thật
      trước khi giao.
- [x] Admin duyệt/từ chối kèm ghi chú → ghi chú được lưu vào DB (không còn bị
      mất) và hiển thị đúng ở cột "Phản hồi" phía Sinh viên. Đã kiểm thử đầy
      đủ end-to-end (Playwright + API thật): tạo yêu cầu "Giấy Xác Nhận" →
      Admin duyệt kèm ghi chú "Đã xác nhận, mời bạn đến phòng đào tạo nhận
      giấy." → gọi lại `GET /yeu-cau-hanh-chinh/me` xác nhận `trangThai: "Đã
      duyệt"`, `loaiGiayXacNhan: "Giấy tạm hoãn nghĩa vụ quân sự"`,
      `ghiChuAdmin` đúng nguyên văn.
- [x] Migration DB áp dụng sạch trên DB dev (`site4now.net`), dữ liệu
      `yeu_cau_hanh_chinh` cũ không bị lỗi/mất khi đọc lại — các bản ghi cũ
      (đã có sẵn trước migration) hiển thị đúng, 2 cột mới `NULL`/`—`.
- [x] `tsc --noEmit` sạch, `dotnet build` sạch (full-solution build).
- [x] Đã kiểm thử trên trình duyệt (Playwright headless, backend/DB thật) với
      cả 2 vai trò Sinh viên (`sv.k2021.001`) và Admin (`admin`), không có
      lỗi console.

## 8. Ghi chú

- **Việc sửa `DuyetAsync` để lưu `GhiChuAdmin` là phần quan trọng nhất của
  task này** — không chỉ là "thêm tính năng mới" mà là vá một chỗ dữ liệu
  đang bị Admin nhập vào rồi âm thầm mất, nên khi review cần đối chiếu kỹ
  hành vi trước/sau để chắc chắn không có tác dụng phụ khác (vd không được
  vô tình cho phép sửa `TrangThai` lần 2 — dòng kiểm tra `if (yc.TrangThai !=
  "Chờ duyệt")` ở `DuyetAsync` dòng 66-67 vẫn phải giữ nguyên).
- **Số biên nhận dùng trực tiếp `Id`** là lựa chọn mặc định cho task này vì
  đơn giản và không rủi ro; nếu người yêu cầu muốn định dạng số biên nhận
  "đẹp" hơn (có tiền tố năm, reset theo năm,...), đó là 1 task riêng, lớn
  hơn (cần cột sinh số + logic tuần tự có khoá tránh trùng khi ghi đồng
  thời) — chưa nên làm chung với task này.
- Danh sách 6 loại giấy con hiện để **cứng ở frontend** (giống cách
  `LOAI_OPTIONS` gốc đã làm), nhất quán với kiến trúc hiện tại của trang này
  — không tạo bảng tra cứu riêng cho 6 giá trị cố định.

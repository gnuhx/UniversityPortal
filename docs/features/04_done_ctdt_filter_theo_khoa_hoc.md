# Chương trình đào tạo — lọc theo Khoá học (dropdown, chỉ hiện 1 khoá tại 1 thời điểm)

**Task:** #04
**Trạng thái:** done
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Đã kiểm tra `ChuongTrinhDTPage.tsx`, `ChuongTrinhDTController.cs`,
`ChuongTrinhDTRepository.cs`:

- Trang "Chương trình đào tạo" hiện liệt kê **tất cả** CTĐT của **mọi
  khoá học** cùng lúc trong một bảng phân trang (`GET /api/chuong-trinh-dt`
  chỉ lọc theo `keyword` tự do và `nganhId`, không có lọc theo khoá học).
- Cột `khoaHoc` (`chuong_trinh_dt.khoa_hoc`) là chuỗi tự do, không chuẩn
  hoá — dữ liệu thật hiện có các giá trị như `"2020"`, `"2021"`, `"2022"`,
  `"2023"`, `"2022-2026"`, `"2023-2027"`, `"2021"` (không theo một định
  dạng duy nhất). Không có bảng/enum riêng cho "khoá học".
- `keyword` hiện tại đã match cả `maCtdt` lẫn `khoaHoc` (substring,
  không phân biệt hoa thường) nhưng là ô tìm kiếm tự do, không phải
  dropdown, và không giới hạn hiển thị về đúng 1 khoá.

## 2. Mục tiêu

- Thêm dropdown lọc theo Khoá học trên trang Chương trình đào tạo.
- Mặc định trang chỉ hiển thị **một khoá học tại một thời điểm** (không
  trộn lẫn nhiều khoá cùng lúc) — mặc định chọn khoá học mới nhất.

## 3. Giải pháp đề xuất

Thêm tham số lọc chính xác `khoaHoc` (exact match) song song với `keyword`
hiện có (giữ nguyên `keyword` cho tìm theo mã CTĐT). Dropdown lấy danh
sách khoá học **duy nhất** (distinct) suy ra từ
`chuongTrinhDTApi.getAll()` đã có sẵn (dùng cho dropdown ở
`LopSinhHoatPage`), sắp xếp giảm dần theo chuỗi. Do `khoaHoc` là text tự
do, không parse thành số — sắp xếp là string sort, chấp nhận hạn chế này
(ghi rõ trong ghi chú).

Frontend luôn set `khoaHoc` mặc định = khoá đầu tiên trong danh sách
(giá trị lớn nhất theo string sort) ngay khi tải xong danh sách khoá học,
và **không cho xoá lựa chọn** (không dùng `allowClear`) — đảm bảo bảng
luôn lọc đúng 1 khoá, đúng yêu cầu "không để mọi khoá hiện cùng lúc".

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `Application/Interfaces/Repositories/IChuongTrinhDTRepository.cs` | Thêm tham số `string? khoaHoc` vào `GetPagedFilterAsync` |
| `Infrastructure/Repositories/ChuongTrinhDTRepository.cs` | Lọc thêm `x.KhoaHoc == khoaHoc` khi có giá trị (exact match, độc lập với `keyword`) |
| `Application/Interfaces/Services/IChuongTrinhDTService.cs` + `Services/ChuongTrinhDTService.cs` | `GetPagedAsync` nhận thêm `khoaHoc` |
| `API/Controllers/ChuongTrinhDTController.cs` | `GET /api/chuong-trinh-dt` nhận thêm `[FromQuery] string? khoaHoc` |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/pages/ChuongTrinhDTPage.tsx` | Thêm `Select` lọc Khoá học (options suy ra distinct từ `chuongTrinhDTApi.getAll()`, sort giảm dần, không `allowClear`), mặc định chọn khoá mới nhất, truyền `khoaHoc` vào `extraParams` của `CrudTable` |

### Dữ liệu / Migration

Không cần — không đổi schema, chỉ thêm filter trên field có sẵn.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~20–30 phút (4 file backend nhỏ + 1 file frontend) |
| Mức độ rủi ro | Rất thấp — thêm filter tuỳ chọn, không đổi hành vi khi không truyền `khoaHoc` |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Bỏ tham số + revert UI, không ảnh hưởng phần khác |

## 6. Kế hoạch triển khai

1. Thêm `khoaHoc` filter vào repository/service/controller backend.
2. Build backend, kiểm thử `GET /api/chuong-trinh-dt?khoaHoc=2022` trả
   đúng kết quả.
3. Thêm dropdown Khoá học ở `ChuongTrinhDTPage.tsx`, mặc định chọn khoá
   mới nhất, không cho bỏ chọn.
4. `tsc --noEmit`, kiểm thử UI: đổi khoá học → bảng chỉ hiện đúng CTĐT
   của khoá đó; tải trang lần đầu không hiện trộn nhiều khoá.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] `GET /api/chuong-trinh-dt?khoaHoc=X` chỉ trả CTĐT có `khoaHoc`
      khớp chính xác `X`.
- [ ] Trang Chương trình đào tạo có dropdown Khoá học, không có tuỳ chọn
      "bỏ lọc" — luôn có đúng 1 khoá đang được chọn.
- [ ] Mở trang lần đầu tự động chọn khoá học mới nhất, bảng chỉ hiện
      CTĐT của khoá đó (không trộn nhiều khoá).
- [ ] Build backend + frontend không lỗi, `tsc --noEmit` sạch.

## 8. Ghi chú

`khoaHoc` là text tự do, dữ liệu thật không đồng nhất định dạng (có năm
đơn `"2022"`, có khoảng `"2022-2026"`). Dropdown chỉ distinct + sort theo
string, không cố parse thành năm thật — nếu sau này cần sort theo đúng
thứ tự thời gian, cần chuẩn hoá lại cột này thành kiểu có cấu trúc (việc
này ngoài phạm vi task này).

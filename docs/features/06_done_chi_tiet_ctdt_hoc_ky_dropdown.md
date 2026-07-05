# Chi tiết CTĐT — chọn Học kỳ bằng dropdown thay vì gõ Id thô

**Task:** #06
**Trạng thái:** done — xem [08_done_chi_tiet_ctdt_group_by_hoc_ky.md](08_done_chi_tiet_ctdt_group_by_hoc_ky.md), fix dropdown này được làm cùng lúc với việc viết lại trang theo nhóm học kỳ.
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Đã kiểm tra `ChiTietCTDTPage.tsx`, `ChiTietCTDTController.cs`,
`ChiTietCTDTService.cs`, `CreateChiTietCTDTValidator.cs`:

- Ở trang "Môn học trong CTĐT" (`/chuong-trinh-dt/:id`), khi Admin bấm
  "Thêm mới", field học kỳ hiện là:
  ```
  { name: "hocKyId", label: "Mã học kỳ (Id)", type: "number", required: true, hideOnEdit: true }
  ```
  (`ChiTietCTDTPage.tsx:49`) — một ô nhập **số nguyên thô** (khoá chính
  bảng `hoc_ky`), không phải dropdown chọn theo tên học kỳ.
- Admin gõ một con số (vd. "5") kỳ vọng nó tương ứng với "học kỳ thứ 5"
  của chương trình, nhưng con số đó thực chất là **Id toàn cục** trong
  bảng `hoc_ky` — được đánh theo thứ tự insert của mọi năm học, không
  liên quan gì đến CTĐT hay khoá học đang xem. Ví dụ thực tế người dùng
  gặp: đang thêm môn cho CTĐT "Hệ thống Thông tin - 2023-2027", gõ
  `hocKyId = 5`, nhưng Id 5 trong DB lại là "HK1 2021-2022" — một học kỳ
  của khoá học hoàn toàn khác.
- Backend (`CreateChiTietCTDTValidator.cs:20-21`) chỉ kiểm tra
  `HocKyId > 0`, không có ràng buộc nào đối chiếu học kỳ được chọn với
  năm học/khoá học của CTĐT đang thêm môn — nên giá trị sai vẫn được
  lưu thành công, không có cảnh báo, dữ liệu bị gán nhầm học kỳ một
  cách âm thầm.
- Trang đã có sẵn `hocKyApi` (`api/modules.ts:102-107`, gọi
  `GET /api/hoc-ky`, trả về `{ id, tenHocKy, ngayBatDau, tenNamHoc }`,
  sắp xếp giảm dần theo `ngayBatDau` — xem `HocKyRepository.cs`) và
  page đã dùng đúng pattern "fetch all rồi map thành options `select`"
  cho `nganhId`/`monHocId` ở `ChuongTrinhDTPage.tsx` và
  `ChiTietCTDTPage.tsx` — chỉ chưa áp dụng cho field `hocKyId`.

## 2. Mục tiêu

- Admin chọn học kỳ bằng tên thật (vd. "HK1 (2021-2022)") qua dropdown,
  không phải đoán/gõ Id thô — loại bỏ khả năng gán nhầm học kỳ do nhập
  sai số.

## 3. Giải pháp đề xuất

Đổi field `hocKyId` trong `formFields` của `ChiTietCTDTPage.tsx` từ
`type: "number"` sang `type: "select"`, lấy options từ `hocKyApi.getAll()`
(fetch qua `useQuery`, cùng pattern với `monHocOptions` đã có trong
chính file này), label hiển thị dạng `${tenHocKy} (${tenNamHoc})` để
phân biệt rõ các học kỳ trùng tên ở các năm khác nhau (vd. nhiều
"HK1" của nhiều năm học).

Không đổi backend — validator vẫn giữ nguyên `HocKyId > 0` (việc thêm
ràng buộc "học kỳ phải cùng năm/khoá với CTĐT" là thay đổi nghiệp vụ
lớn hơn, ngoài phạm vi fix UX này; xem mục 8).

## 4. Phạm vi thay đổi

### Backend

Không đổi.

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/pages/ChiTietCTDTPage.tsx` | Thêm `useQuery` gọi `hocKyApi.getAll()`; đổi field `hocKyId` từ `type: "number"` sang `type: "select"` với `options` map từ danh sách học kỳ (label `${tenHocKy} (${tenNamHoc})`, value `id`) |

### Dữ liệu / Migration

Không cần — chỉ đổi UI, không đổi schema/API.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~10 phút (1 file frontend) |
| Mức độ rủi ro | Rất thấp — chỉ đổi cách nhập liệu, không đổi API/DTO/validator |
| Ảnh hưởng dữ liệu hiện có | Không — không sửa dữ liệu cũ đã bị gán nhầm học kỳ (nếu có), chỉ ngăn lỗi mới phát sinh |
| Khả năng rollback | Revert field về `type: "number"` |

## 6. Kế hoạch triển khai

1. Thêm `useQuery(["hoc-ky-all"], () => hocKyApi.getAll())` vào
   `ChiTietCTDTPage.tsx`.
2. Đổi field `hocKyId` sang `type: "select"`, options map từ kết quả
   trên.
3. `tsc --noEmit`, kiểm thử UI: mở "Thêm mới" ở trang Môn học của một
   CTĐT bất kỳ, xác nhận dropdown Học kỳ hiện đúng tên + năm học, chọn
   xong lưu đúng `hocKyId` tương ứng (đối chiếu qua cột "Học kỳ" của
   bảng sau khi thêm).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] Form "Thêm môn học vào CTĐT" hiển thị dropdown Học kỳ (không còn
      ô nhập số Id thô).
- [ ] Dropdown hiển thị tên học kỳ kèm năm học, phân biệt được các học
      kỳ trùng tên ở năm học khác nhau.
- [ ] Sau khi chọn và lưu, cột "Học kỳ" trong bảng hiển thị đúng học kỳ
      đã chọn (không lệch sang học kỳ/năm khác).
- [ ] `tsc --noEmit` sạch.

## 8. Ghi chú

- Đây là fix UX ở tầng nhập liệu — **không** giải quyết việc dữ liệu
  `chi_tiet_ctdt` hiện có (nếu đã từng bị gán nhầm `hoc_ky_id` do thao
  tác gõ tay trước đây) — cần rà soát thủ công riêng nếu nghi ngờ có
  dòng dữ liệu sai.
- Về lâu dài, nếu muốn chặn hẳn việc gán môn học của CTĐT khoá
  2023-2027 vào một học kỳ thuộc năm 2021-2022, cần thêm ràng buộc
  nghiệp vụ ở backend (so khớp năm học của `hoc_ky` với khoảng năm suy
  ra từ `chuong_trinh_dt.khoa_hoc`) — việc này đòi hỏi chuẩn hoá cột
  `khoa_hoc` (xem ghi chú ở task #04) nên tách thành task riêng, không
  gộp vào đây.

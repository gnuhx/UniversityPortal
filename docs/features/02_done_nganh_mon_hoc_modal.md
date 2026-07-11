# Xem môn học CTĐT ngay từ trang Ngành học (popup)

**Task:** #02
**Trạng thái:** todo
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Trang Ngành học (`NganhHocPage.tsx`) hiện chỉ liệt kê danh mục ngành
(mã ngành, tên ngành, ngành cha, khoa/phòng ban) và cho CRUD ở mức bản ghi
`nganh_hoc`. Muốn xem môn học thuộc một ngành, người dùng phải tự chuyển
sang trang "Chương trình đào tạo", lọc thủ công theo ngành, rồi bấm vào
từng CTĐT — không có lối tắt từ chính trang Ngành học.

Một Ngành có thể có **nhiều Chương trình đào tạo** (nhiều khoá học, ví dụ
CTĐT khoá 2021, khoá 2022 của cùng ngành CNTT), nên "môn học của một
ngành" thực chất là môn học của một CTĐT cụ thể thuộc ngành đó.

## 2. Mục tiêu

- Từ danh sách Ngành học, bấm vào một dòng là xem được ngay danh sách môn
  học (chi tiết CTĐT) của ngành đó, **dưới dạng popup modal**, không cần
  điều hướng sang trang khác.
- Nếu ngành có nhiều CTĐT (nhiều khoá học), cho phép chọn CTĐT muốn xem
  trong modal.

## 3. Giải pháp đề xuất

Không cần thay đổi backend — hai API đã hỗ trợ đúng filter cần dùng:
- `GET /api/chuong-trinh-dt?nganhId=` — lấy các CTĐT của một ngành.
- `GET /api/chi-tiet-ctdt?ctdtId=` — lấy môn học của một CTĐT.

Thêm 1 component modal dùng lại đúng 2 API trên, và 1 nút hành động trên
mỗi dòng của bảng Ngành học để mở modal đó (chỉ đọc, không có CRUD trong
modal — sửa/xoá môn học vẫn thực hiện ở trang Chương trình đào tạo hiện
có).

## 4. Phạm vi thay đổi

### Backend

Không thay đổi.

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/components/NganhMonHocModal.tsx` (mới) | Modal nhận `nganhId`, `tenNganh`, `open`, `onClose`. Gọi `chuongTrinhDTApi.getPaged({ nganhId })` để lấy danh sách CTĐT của ngành, hiển thị `Select` cho phép chọn CTĐT (mặc định CTĐT đầu tiên). Gọi `chiTietCTDTApi.getPaged({ ctdtId })` cho CTĐT đang chọn, hiển thị bảng môn học (Mã môn, Tên môn, Học kỳ, Số tín chỉ, Tính điểm TB — cùng cột với `ChiTietCTDTPage.tsx`, chỉ đọc). Hiển thị `Empty` nếu ngành chưa có CTĐT nào. |
| `frontend/src/pages/NganhHocPage.tsx` | Thêm cột hành động "Xem môn học" trên mỗi dòng, mở `NganhMonHocModal` với `id`/`tenNganh` của dòng đó (state cục bộ lưu ngành đang xem). |

### Dữ liệu / Migration

Không có.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~15–20 phút (1 file mới + 1 file sửa, chỉ frontend) |
| Mức độ rủi ro | Rất thấp — không đổi backend, không đổi API contract, chỉ thêm UI đọc dữ liệu sẵn có |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Xoá file mới + revert cột hành động, không ảnh hưởng phần khác |

## 6. Kế hoạch triển khai

1. Tạo `NganhMonHocModal.tsx` dùng lại `chuongTrinhDTApi`/`chiTietCTDTApi` đã có.
2. Sửa `NganhHocPage.tsx` thêm nút "Xem môn học" + state điều khiển modal.
3. `tsc --noEmit` kiểm tra type, chạy dev server thử luồng: ngành có 1 CTĐT,
   ngành có nhiều CTĐT (đổi Select cập nhật đúng bảng môn học), ngành chưa
   có CTĐT (hiện Empty state).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] Bấm "Xem môn học" trên một dòng Ngành học mở modal hiển thị đúng môn
      học của ngành đó.
- [ ] Ngành có nhiều CTĐT: đổi lựa chọn CTĐT trong modal cập nhật đúng
      bảng môn học tương ứng.
- [ ] Ngành chưa có CTĐT nào: modal hiện thông báo trống, không lỗi.
- [ ] Không có CRUD (sửa/xoá) môn học trong modal — chỉ xem.
- [ ] `tsc --noEmit` sạch.

## 8. Ghi chú

Không có rủi ro deploy — thay đổi thuần frontend, không đụng API hay DB.

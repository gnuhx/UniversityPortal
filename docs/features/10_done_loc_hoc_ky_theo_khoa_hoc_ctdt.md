# Môn học trong CTĐT — nhóm theo học kỳ vẫn hiện học kỳ ngoài khoảng năm của CTĐT

**Task:** #10
**Trạng thái:** done
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Người dùng báo cáo: trang "Môn học trong CTĐT: HTTT-K23 (Hệ thống
Thông tin - 2023-2027)" vẫn hiển thị đầy đủ các nhóm học kỳ
`HK1 2020-2021`, `HK2 2020-2021`, `HK1 2021-2022`, `HK2 2021-2022`,
`HK1 2022-2023`, `HK2 2022-2023` — tất cả đều "Chưa có môn học" — dù
CTĐT này ghi rõ khoá học là **2023-2027**, nghĩa là các học kỳ trước
năm 2023 không thể liên quan gì đến chương trình này. Vấn đề xảy ra ở
cả 2 vai trò xem trang (Admin và Sinh viên) vì cùng dùng chung
`ChiTietCTDTPage.tsx`.

Đây chính là hạn chế đã ghi nhận trước ở task #08
(`08_done_chi_tiet_ctdt_group_by_hoc_ky.md`, mục 8): việc nhóm theo
học kỳ dựa trên **toàn bộ** `hoc_ky` trong hệ thống
(`hocKyApi.getAll()`), không lọc theo `khoaHoc` của CTĐT đang xem — lúc
đó chấp nhận được vì số học kỳ còn ít, nhưng giờ gây nhiễu rõ rệt.

## 2. Mục tiêu

- Trang "Môn học trong CTĐT" chỉ nhóm/hiện các học kỳ nằm trong khoảng
  năm học thực tế của CTĐT đang xem (suy ra từ `khoaHoc`, vd
  "2023-2027" → chỉ các năm học 2023-2024, 2024-2025, 2025-2026,
  2026-2027).
- Dropdown "Học kỳ" trong modal "Thêm môn học" cũng chỉ cho chọn trong
  đúng khoảng năm đó — tránh Admin gán nhầm môn vào học kỳ ngoài
  khoảng năm của CTĐT.

## 3. Giải pháp đề xuất

`khoaHoc` là text tự do, nhưng khi đúng định dạng phổ biến
`"YYYY-YYYY"` (năm nhập học - năm tốt nghiệp dự kiến, vd "2023-2027")
thì suy ra được chính xác tập `ten_nam_hoc` hợp lệ: với mọi năm `y` từ
năm bắt đầu đến năm kết thúc trừ 1, năm học `"{y}-{y+1}"` là hợp lệ.

Trong `ChiTietCTDTPage.tsx`: thêm `relevantNamHoc` (parse `khoaHoc`
bằng regex `^(\d{4})-(\d{4})$`, build `Set` các `tenNamHoc` hợp lệ) và
`relevantHocKys` (lọc `hocKys` theo `tenNamHoc` nằm trong set đó). Dùng
`relevantHocKys` thay cho `hocKys` ở cả 2 chỗ: tính `hocKyGroups`
(hiển thị) và options của `Select` Học kỳ trong modal thêm mới.

Nếu `khoaHoc` **không** đúng định dạng khoảng năm (dữ liệu cũ không
đồng nhất — xem hạn chế đã ghi ở task #04), `relevantNamHoc` là `null`
và giữ nguyên hành vi cũ (hiện toàn bộ học kỳ) — thà hiện dư còn hơn
lọc sai và ẩn mất học kỳ thật sự liên quan.

## 4. Phạm vi thay đổi

### Backend

Không đổi — lọc hoàn toàn ở frontend dựa trên dữ liệu đã có sẵn
(`ctdt.khoaHoc` + danh sách học kỳ đã tải).

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/pages/ChiTietCTDTPage.tsx` | Thêm `relevantNamHoc`/`relevantHocKys` (suy ra từ `ctdt.khoaHoc` qua regex `YYYY-YYYY`); dùng `relevantHocKys` thay `hocKys` khi tính `hocKyGroups` và khi build options `Select` Học kỳ trong modal thêm mới |

### Dữ liệu / Migration

Không cần.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~15 phút (1 file frontend) |
| Mức độ rủi ro | Thấp — chỉ lọc hiển thị/dropdown ở 1 trang, có fallback an toàn khi không parse được `khoaHoc` |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert file `ChiTietCTDTPage.tsx` |

## 6. Kế hoạch triển khai

1. Thêm `relevantNamHoc`/`relevantHocKys` vào `ChiTietCTDTPage.tsx`,
   áp dụng cho `hocKyGroups` và dropdown Học kỳ trong modal.
2. `tsc --noEmit`.
3. Kiểm thử trên trình duyệt (tài khoản Admin thật, backend/DB thật):
   mở CTĐT "HTTT-K23 (2023-2027)", xác nhận các học kỳ 2020-2021 →
   2022-2023 không còn hiện; chỉ còn 2023-2024, 2024-2025 (2 năm đã có
   dữ liệu thật trong DB, 2025-2026/2026-2027 chưa tồn tại nên không
   có nhóm nào cho chúng — đúng vì `relevantHocKys` lọc trên danh sách
   `hoc_ky` **đã có sẵn**, không tự tạo nhóm cho năm chưa tồn tại).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] CTĐT có `khoaHoc` dạng "YYYY-YYYY" chỉ hiện nhóm học kỳ trong
      đúng khoảng năm đó.
- [x] Dropdown Học kỳ ở modal "Thêm môn học" cũng chỉ cho chọn học kỳ
      trong khoảng năm đó.
- [x] CTĐT có `khoaHoc` không đúng định dạng khoảng năm vẫn hiện đầy
      đủ học kỳ như hành vi cũ (không bị lọc sai/ẩn nhầm).
- [x] `tsc --noEmit` sạch.
- [x] Đã kiểm thử trên trình duyệt bằng dữ liệu thật (CTĐT HTTT-K23,
      2023-2027): xác nhận các học kỳ 2020-2021 → 2022-2023 biến mất
      khỏi danh sách nhóm, chỉ còn 2023-2024 và 2024-2025.

## 8. Ghi chú

- Vì `relevantHocKys` chỉ **lọc** trên danh sách học kỳ đã tồn tại
  trong DB (không tự sinh học kỳ giả định cho những năm chưa tạo), nên
  nếu CTĐT "2023-2027" mà năm học 2025-2026/2026-2027 chưa được tạo
  (task #07 — trang "Năm học/Học kỳ" mới cho phép tạo), trang này sẽ
  không hiện nhóm nào cho 2 năm đó — không phải bug, chỉ đơn giản là
  chưa có học kỳ nào tồn tại để nhóm.
- Không sửa hạn chế gốc về `khoa_hoc` là text tự do (đã ghi ở task
  #04) — giải pháp này chỉ tận dụng định dạng phổ biến "YYYY-YYYY" khi
  có, không chuẩn hoá lại cột dữ liệu.

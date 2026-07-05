# Dữ liệu mẫu: Môn học chung toàn trường trên Thời khoá biểu, rơi đúng ngày hiện tại

**Task:** #05
**Trạng thái:** todo
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Trang "Thời khoá biểu" (task #03) đã hoạt động nhưng DB thật hiện chưa
có dòng `thoi_khoa_bieu` nào (đã xoá sạch dữ liệu test sau khi kiểm thử
task #03). Đồng thời, dữ liệu học kỳ/tuần học thật hiện có
(`docs/Data/seed_data.sql`, `seed_test_data.sql`) chỉ phủ các năm học
2021-2022 → 2024-2025 — không có học kỳ/tuần học nào chứa ngày hiện tại
thực tế của hệ thống. Vì vậy nếu chỉ thêm dữ liệu vào học kỳ cũ, khi mở
trang Thời khoá biểu sẽ không thấy gì ở "hôm nay" trừ khi người dùng tự
điều hướng lịch về quá khứ.

Yêu cầu: có một môn học được gán cho **toàn bộ sinh viên của trường**
(không riêng ngành/CTĐT nào — kiểu môn bắt buộc chung như Giáo dục Quốc
phòng, Giáo dục thể chất), với vị trí mặc định khi mở trang là **đúng
ngày hiện tại, trong học kỳ hiện tại**.

## 2. Mục tiêu

- Có 1 script SQL, chạy được nhiều lần an toàn (idempotent), tự tính
  toán "học kỳ hiện tại" / "tuần hiện tại" dựa theo `GETDATE()` tại thời
  điểm chạy (không hard-code ngày tháng cụ thể) — để "hiện tại" luôn
  đúng bất kể khi nào script được thực thi.
- Ghi danh **mọi** sinh viên trong bảng `sinh_vien` vào lớp học phần của
  môn này, không phân biệt ngành/CTĐT.
- Tạo đúng 1 buổi học (`thoi_khoa_bieu`) rơi vào ngày chạy script, để
  trang Thời khoá biểu mở lên (mặc định chọn học kỳ mới nhất, lịch nhảy
  tới tháng bắt đầu học kỳ đó — theo code đã có ở `ThoiKhoaBieuPage.tsx`)
  sẽ hiển thị ngay buổi học ở đúng ngày hôm nay.

## 3. Giải pháp đề xuất

Viết 1 script T-SQL duy nhất
(`docs/Data/seed_mon_hoc_chung_toan_truong.sql`), mỗi bước dùng
`IF NOT EXISTS` để idempotent, theo thứ tự phụ thuộc khoá ngoại:

1. **Năm học** "hiện tại" — suy ra nhãn kiểu `"2025-2026"` từ
   `GETDATE()` (quy ước năm học VN bắt đầu tháng 9: nếu tháng hiện tại
   ≥ 9 thì năm học là `năm nay-năm sau`, ngược lại `năm trước-năm nay`).
2. **Học kỳ** "hiện tại" — `ngay_bat_dau` = thứ Hai của tuần hiện tại,
   tính bằng công thức `DATEADD(day, DATEDIFF(day, 0, GETDATE())/7*7, 0)`
   (mốc ngày 0 = 1900-01-01 luôn là thứ Hai trong SQL Server, nên công
   thức này không phụ thuộc `@@DATEFIRST`/locale server).
3. **Tuần học** "hiện tại" — 1 tuần (thứ Hai → Chủ nhật) chứa ngày chạy
   script.
4. **Môn học** mới: mã `GDQPAN` — "Giáo dục Quốc phòng và An ninh"
   (không tính vào điểm trung bình — đúng thực tế môn này ở đa số
   trường VN).
5. **Chi tiết CTĐT**: bắt buộc phải gắn vào 1 `ctdt_id` cụ thể vì cột
   này `NOT NULL` trong schema — chọn tạm CTĐT có `Id` nhỏ nhất
   (`SELECT TOP 1 ... ORDER BY Id`) chỉ để thoả khoá ngoại kỹ thuật.
   Việc này **không** giới hạn phạm vi ghi danh — sinh viên được ghi
   danh trực tiếp theo `sinh_vien_id` ở bước 7, không lọc qua CTĐT.
6. **Lớp học phần** duy nhất `GDQPAN-TOANTRUONG` cho môn này trong học
   kỳ hiện tại, gán cho giáo viên có `Id` nhỏ nhất (có thể đổi sau bằng
   tay nếu cần đúng người phụ trách).
7. **Ghi danh** (`danh_sach_lop_hp`): `INSERT ... SELECT` toàn bộ
   `sinh_vien`, có `NOT EXISTS` để không ghi trùng nếu chạy lại.
8. **Buổi học** (`thoi_khoa_bieu`): `thu` tính từ khoảng cách ngày giữa
   ngày chạy script và thứ Hai đầu tuần (`+2` theo quy ước `Thu = 2..8`
   đã dùng ở task #03), tiết 1–3, phòng "Hội trường A - Sân vận động"
   (hợp lý cho lớp học chung toàn trường).

Script kết thúc bằng 1 câu `SELECT` đối chiếu (tên học kỳ/tuần, ngày học
thực tế tính ra, số sinh viên đã ghi danh) để xác minh nhanh sau khi
chạy — theo đúng thói quen kiểm chứng đã dùng ở các task trước.

## 4. Phạm vi thay đổi

### Backend / Frontend

Không đổi — dùng đúng API/luồng đã có từ task #03
(`ThoiKhoaBieuController`, `ThoiKhoaBieuPage.tsx`).

### Dữ liệu / Migration

| File | Thay đổi |
|---|---|
| `docs/Data/seed_mon_hoc_chung_toan_truong.sql` (mới) | Script idempotent tạo năm học/học kỳ/tuần học "hiện tại" + môn học chung + lớp học phần + ghi danh toàn trường + 1 buổi học đúng ngày chạy script |

Không có migration EF Core — chỉ là dữ liệu (DML), không đổi schema.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~15 phút viết + kiểm thử dry-run trong transaction |
| Mức độ rủi ro | Thấp-trung bình — có `INSERT` thật vào DB (bao gồm ghi danh **toàn bộ** sinh viên), không phải thao tác chỉ-đọc; cần dry-run trước khi áp dụng thật |
| Ảnh hưởng dữ liệu hiện có | Thêm mới thuần tuý (không `UPDATE`/`DELETE` bản ghi có sẵn); idempotent nên chạy nhiều lần không nhân đôi dữ liệu |
| Khả năng rollback | Xoá theo thứ tự ngược: `thoi_khoa_bieu` → `danh_sach_lop_hp` → `lop_hoc_phan` → `chi_tiet_ctdt` → `mon_hoc` (mã `GDQPAN`) → `tuan_hoc`/`hoc_ky`/`nam_hoc` (mã bắt đầu `CUR_`/"hiện tại") — tất cả nhận diện được qua mã/tên đã đặt |

## 6. Kế hoạch triển khai

1. Dry-run script trong transaction rollback trên DB thật (giống cách
   đã làm ở task #01) — kiểm tra câu `SELECT` đối chiếu cuối script cho
   ra ngày học đúng = ngày hôm đó, số sinh viên ghi danh = tổng số
   sinh viên trong `sinh_vien`.
2. Sau khi xác nhận kết quả dry-run đúng, chạy thật (cần người phụ
   trách bấm nút chạy — không tự động chạy khi merge, vì đây là DML
   trực tiếp trên DB thật, không qua migration tự động của API).
3. Đăng nhập thử 1 tài khoản sinh viên bất kỳ, mở `/thoi-khoa-bieu`,
   xác nhận thấy buổi "Giáo dục Quốc phòng và An ninh" ở đúng ngày hôm
   nay trong học kỳ hiện tại (mặc định được chọn sẵn).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] Script chạy lại nhiều lần không tạo dữ liệu trùng (idempotent).
- [ ] Sau khi chạy, học kỳ "hiện tại" xuất hiện đầu danh sách
      `GET /api/hoc-ky` (do sắp theo `ngay_bat_dau` giảm dần).
- [ ] `danh_sach_lop_hp` có đúng 1 dòng cho **mỗi** sinh viên trong
      `sinh_vien`, trỏ tới lớp `GDQPAN-TOANTRUONG`.
- [ ] Mở `/thoi-khoa-bieu` bằng tài khoản sinh viên bất kỳ → mặc định
      hiện học kỳ hiện tại, lịch nhảy đúng tháng, thấy buổi học đúng
      ngày hôm nay.
- [ ] Mở bằng tài khoản giáo viên được gán dạy lớp này → cũng thấy đúng
      buổi học đó.

## 8. Ghi chú

- Chưa chạy trên DB thật — cần dry-run trong transaction trước (xem
  mục 6), theo đúng quy trình thận trọng đã áp dụng ở các task trước
  liên quan tới `db_acaeb4_datn`.
- Vì "học kỳ hiện tại" được tạo mới hoàn toàn (không trùng với các học
  kỳ 2021-2025 đã seed sẵn), môn `GDQPAN` này sẽ **không** xuất hiện
  trong bảng điểm/kết quả học tập của các học kỳ cũ — đây là môn của
  học kỳ mới, tách biệt, đúng như yêu cầu "hiện tại".
- Nếu chạy script này nhiều lần ở các ngày khác nhau trong cùng một
  tuần, bước 8 vẫn idempotent theo `(lop_hp_id, tuan_hoc_id, thu)` —
  nhưng nếu chạy ở tuần khác, sẽ tạo thêm `tuan_hoc` + buổi học mới cho
  tuần đó (đúng ý đồ "luôn có buổi học ở ngày hiện tại").

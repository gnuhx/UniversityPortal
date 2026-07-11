# Trang Học phí (Sinh viên): thêm khối "THÔNG TIN BẢO HIỂM Y TẾ"

**Task:** #18
**Trạng thái:** done
**Ngày tạo:** 2026-07-07
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Trang Học phí (`frontend/src/pages/HocPhiPage.tsx`) hiện tại, phần Sinh viên,
gồm: header + 3 `Statistic` (Tổng/Đã đóng/Chưa đóng, dòng 279-319) → error/
empty state → `StudentFeeByYear` (Collapse theo Năm học, dòng 343-347, thêm ở
task #13). Trang **không gọi bất kỳ API hồ sơ sinh viên nào**
(`sinhVienApi`/`sinhVienMeApi`) — chỉ gọi `hocPhiApi.getMe()`
(dòng 172) — nên hiện chưa có chỗ nào hiển thị thông tin cá nhân (MSSV, họ
tên, lớp).

Yêu cầu thêm 1 khối "THÔNG TIN BẢO HIỂM Y TẾ" gồm 5 dòng: Mã HSSV, Họ & Tên,
Ngày sinh, Lớp, Tình trạng đóng BHYT.

Đã kiểm tra dữ liệu sẵn có:

- **Mã HSSV, Họ & Tên, Lớp**: có sẵn qua `GET /sinh-vien/me` →
  `SinhVienDto` (`src/UniversityPortal.Application/DTOs/SinhVien/SinhVienDto.cs:9-27`)
  — các field `Mssv` (dòng 11), `HoTen` (dòng 14), `TenLop` (dòng 13, `null`
  nếu sinh viên chưa được phân lớp sinh hoạt). API này **đã có sẵn và đang
  được dùng ở trang khác** (`HoSoPage.tsx`), chỉ cần gọi thêm ở
  `HocPhiPage.tsx`.
- **Ngày sinh**: **không tồn tại trong toàn bộ hệ thống**. Đã `grep -rn
  "NgaySinh"` trên cả `src/` và `frontend/src/` — 0 kết quả. Cả entity
  `SinhVien` (`src/UniversityPortal.Domain/Entities/SinhVien.cs`, chỉ có
  `TaiKhoanId`, `Mssv`, `LopId`) lẫn `TaiKhoan`
  (`src/UniversityPortal.Domain/Entities/TaiKhoan.cs`, có `HoTen`, `Email`,...)
  đều không có cột ngày sinh.
- **Tình trạng đóng BHYT**: **không tồn tại trong toàn bộ hệ thống**. Đã
  `grep -rin "BHYT\|BaoHiem\|insurance"` trên `src/` và `frontend/src/` — 0
  kết quả. Không có tích hợp BHYT thật nào — đúng như ví dụ người yêu cầu
  đưa ra ("Không có thông tin về việc chưa đóng BHYT."), đây là 1 dòng thông
  báo tĩnh, không phải dữ liệu tính toán.

## 2. Mục tiêu

- Trang Học phí (Sinh viên) hiện thêm 1 `Card` "THÔNG TIN BẢO HIỂM Y TẾ" với
  đủ 5 dòng thông tin theo đúng yêu cầu.
- Dữ liệu nào có thật thì lấy thật (Mã HSSV, Họ & Tên, Lớp); dữ liệu không
  tồn tại trong hệ thống (Ngày sinh, tình trạng BHYT) thì hiển thị rõ ràng là
  "chưa có dữ liệu" thay vì bịa số liệu.

## 3. Giải pháp đề xuất

Vì đây là màn hình chỉ hiển thị thông tin cá nhân + 1 câu trạng thái tĩnh,
chia làm 2 việc tách biệt, không cần thêm bảng/entity mới:

**A. Lấy dữ liệu thật đã có sẵn** — gọi `sinhVienMeApi.getMe()`
(`frontend/src/api/modules.ts:42-46`, đã tồn tại, đang dùng ở
`HoSoPage.tsx`) trong `HocPhiPage.tsx`, hiển thị `mssv`, `hoTen`, `tenLop`
(fallback "Chưa phân lớp" nếu `null`, đúng ý nghĩa field).

**B. Ngày sinh + Tình trạng BHYT — không có dữ liệu thật, cần quyết định
phạm vi:**

- **Ngày sinh**: hiển thị "Chưa cập nhật" (không thêm cột DB mới trong task
  này). Việc thêm ngày sinh thật là một thay đổi lớn hơn phạm vi khối thông
  tin này (cần: thêm cột, quyết định ai nhập/sửa được — sinh viên tự cập
  nhật hồ sơ hay Admin nhập khi tạo tài khoản, có migration, có thể cần màn
  hình sửa hồ sơ) — đề xuất tách thành task riêng nếu người yêu cầu thực sự
  cần theo dõi ngày sinh, không làm chung với task hiển thị BHYT này.
- **Tình trạng đóng BHYT**: hiển thị chuỗi tĩnh, đúng nguyên văn ví dụ:
  `"Không có thông tin về việc chưa đóng BHYT."` — không có trường dữ liệu
  thật đứng sau, vì hệ thống chưa tích hợp BHYT. Nếu sau này có nhu cầu theo
  dõi tình trạng đóng BHYT thật (đã đóng/chưa đóng/miễn), đó là một tính
  năng riêng có phạm vi tương đương "Học phí" hiện tại (cần entity, trạng
  thái, quy trình xác nhận) — không tự suy diễn quy trình đó trong task này.

## 4. Phạm vi thay đổi

### Backend

Không có thay đổi backend — toàn bộ dữ liệu thật cần dùng (`mssv`, `hoTen`,
`tenLop`) đã có sẵn qua API `GET /sinh-vien/me` hiện hành.

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/pages/HocPhiPage.tsx` | Thêm `useQuery` gọi `sinhVienMeApi.getMe()` (chỉ khi `!isAdmin`); thêm `Card title="Thông tin bảo hiểm y tế"` (đặt sau header, trước khối `Statistic` hoặc ngay trước `StudentFeeByYear` — vị trí cụ thể để UI review quyết định) hiển thị 5 dòng: Mã HSSV = `mssv`, Họ & Tên = `hoTen`, Ngày sinh = `"Chưa cập nhật"` (chuỗi tĩnh), Lớp = `tenLop ?? "Chưa phân lớp"`, Tình trạng đóng BHYT = `"Không có thông tin về việc chưa đóng BHYT."` (chuỗi tĩnh) |

### Dữ liệu / Migration

Không cần migration cho phạm vi task này (không thêm Ngày sinh/BHYT thật vào
DB — xem quyết định ở mục 3B).

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~1-2 giờ (chỉ frontend, 1 API call có sẵn + 1 Card tĩnh) |
| Mức độ rủi ro | Rất thấp — không đổi API, không đổi luồng nghiệp vụ nào |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert `HocPhiPage.tsx` |

## 6. Kế hoạch triển khai

1. Frontend: thêm `useQuery` gọi `sinhVienMeApi.getMe()` trong
   `HocPhiPage.tsx` (chỉ chạy khi `!isAdmin`, giống cách `myData` hiện tại
   chỉ fetch khi không phải Admin).
2. Thêm `Card` "Thông tin bảo hiểm y tế" hiển thị 5 dòng theo mục 3.
3. `tsc --noEmit`.
4. Kiểm thử trên trình duyệt với tài khoản Sinh viên — xác nhận Mã HSSV/Họ
   Tên/Lớp đúng với dữ liệu thật của tài khoản đó; Ngày sinh và tình trạng
   BHYT hiện đúng chuỗi tĩnh.
5. Xác nhận khối này **không hiện với vai trò Admin/Giáo vụ** (trang Học phí
   của Admin xem học phí toàn trường, không có "sinh viên hiện tại" để gắn
   thông tin cá nhân).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Vai trò Sinh viên vào trang Học phí thấy khối "THÔNG TIN BẢO HIỂM Y
      TẾ" với đúng Mã HSSV, Họ & Tên, Lớp của tài khoản đang đăng nhập. Đã
      kiểm thử với `sv.k2021.001`: Mã HSSV "2021001", Họ & Tên "Nguyễn Văn
      An" đúng.
- [x] Ngày sinh hiện "Chưa cập nhật", Tình trạng đóng BHYT hiện đúng nguyên
      văn "Không có thông tin về việc chưa đóng BHYT.". Lớp hiện "Chưa phân
      lớp" (tài khoản test chưa được gán lớp sinh hoạt) — đúng nhánh fallback
      đã thiết kế.
- [ ] Vai trò Admin/Giáo vụ vào trang Học phí **không** thấy khối này — code
      đã bọc điều kiện `!isAdmin`, nhưng **chưa chụp lại màn hình Admin để
      xác nhận trực quan** trong lần kiểm thử này.
- [x] `tsc --noEmit` sạch.
- [x] Đã kiểm thử trên trình duyệt (Playwright headless, backend/DB thật) với
      vai trò Sinh viên.

## 8. Ghi chú

- Đây là task **hiển thị thuần**, cố tình không bịa dữ liệu Ngày sinh/BHYT
  không tồn tại — nếu người yêu cầu muốn các trường này phản ánh dữ liệu thật
  (đặc biệt Ngày sinh, vì đây là thông tin định danh sinh viên hợp lý cần
  lưu), cần 1 task riêng để: (1) thêm cột `NgaySinh` vào `SinhVien` hoặc
  `TaiKhoan` kèm migration, (2) quyết định nơi nhập liệu (form tạo/sửa Sinh
  viên phía Admin, ở `SinhVienPage.tsx`), (3) cập nhật `SinhVienDto` và màn
  hình liên quan. Khuyến nghị hỏi lại người yêu cầu trước khi mở rộng phạm
  vi này.
- Tương tự, nếu về sau cần tình trạng BHYT thật (không phải câu tĩnh), đó là
  một tính năng nghiệp vụ độc lập, không nên gộp vào task hiển thị này.

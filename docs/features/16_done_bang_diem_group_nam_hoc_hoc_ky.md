# Bảng điểm (Kết quả học tập): nhóm theo Năm học rồi tới Học kỳ

**Task:** #16
**Trạng thái:** todo
**Ngày tạo:** 2026-07-07
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Trang "Bảng điểm" hiện tại (`frontend/src/pages/BangDiemPage.tsx`, route
`/bang-diem`, chỉ vai trò `SINH_VIEN`) lấy dữ liệu từ `GET
/api/danh-sach-lop-hp/me` rồi nhóm **hoàn toàn ở client** thành các `Tabs`
kiểu thẻ (`type="card"`), mỗi tab là 1 học kỳ (`BangDiemPage.tsx:84-94,137`).
Hai vấn đề:

1. **Không có cấp Năm học** — tất cả học kỳ của mọi năm nằm chung một hàng
   tab phẳng. Sinh viên học nhiều năm sẽ có rất nhiều tab, khó thấy ranh giới
   "năm nào". Đây đúng là mô hình trang Học phí *trước khi* sửa ở task #13
   (`docs/features/13_done_cai_tien_hoc_phi.md`).
2. **Khoá nhóm sai kiểu** — nhóm theo `item.tenHocKy` (chuỗi tên, xem
   `BangDiemPage.tsx:86`), không phải `hocKyId`. `DanhSachLopHPDto` đã có sẵn
   `HocKyId` (`src/UniversityPortal.Application/DTOs/DanhSachLopHP/DanhSachLopHPDto.cs:10`)
   nhưng không được dùng làm khoá — nếu 2 học kỳ khác nhau trùng tên hiển thị
   (dữ liệu seed từng có rủi ro này, xem ghi chú task #08), điểm của chúng sẽ
   bị gộp nhầm vào 1 tab.

`DanhSachLopHPDto` hiện **chưa có** `NamHocId`/`TenNamHoc`
(`DanhSachLopHPDto.cs:1-24` — chỉ có `HocKyId`, `TenHocKy`), và
`DanhSachLopHPRepository.GetBySinhVienAsync`
(`src/UniversityPortal.Infrastructure/Repositories/DanhSachLopHPRepository.cs:10-21`)
chỉ `.Include(lhp => lhp.HocKy)` chứ chưa `.ThenInclude(hk => hk.NamHoc)` — y
hệt lỗ hổng mà task #13 đã vá cho `HocPhiDto`/`HocPhiRepository`.

## 2. Mục tiêu

- Trang Bảng điểm nhóm rõ theo **Năm học → Học kỳ**, không còn là 1 hàng tab
  phẳng chứa mọi học kỳ mọi năm.
- Sửa luôn lỗi khoá nhóm theo tên (string) thay vì `hocKyId` (số).

## 3. Giải pháp đề xuất

Áp dụng đúng pattern đã dùng ở task #13 cho `HocPhiPage.tsx`
(`StudentFeeByYear`, `HocPhiPage.tsx:41-85`): `Collapse` theo Năm học (mặc
định chỉ mở năm học mới nhất), bên trong giữ nguyên UI hiện có của mỗi học kỳ.

Vì UI hiện tại của mỗi học kỳ là 1 `Tabs.items` con (bảng điểm + card GPA/số
môn, `BangDiemPage.tsx:96-120`), cách ít xáo trộn nhất: bọc ngoài bằng
`Collapse` theo `namHocId` (copy cấu trúc `StudentFeeByYear`), mỗi panel chứa
đúng `<Tabs items={...} type="card" />` hiện tại nhưng chỉ với các học kỳ
thuộc năm đó — giữ nguyên toàn bộ logic `columns`, `tinhGpa`, `diemTag` không
đổi.

Nhóm bên trong mỗi năm vẫn cần đổi khoá từ `tenHocKy` sang `hocKyId` (dùng
`tenHocKy` chỉ để làm label hiển thị).

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Application/DTOs/DanhSachLopHP/DanhSachLopHPDto.cs` | Thêm `NamHocId` (int), `TenNamHoc` (string) |
| `src/UniversityPortal.Application/Mappings/MappingProfile.cs:92-101` | Thêm 2 dòng `.ForMember(d => d.NamHocId, ...)` / `.ForMember(d => d.TenNamHoc, ...)` map qua `s.LopHocPhan.HocKy.NamHoc` |
| `src/UniversityPortal.Infrastructure/Repositories/DanhSachLopHPRepository.cs:10-21` (`GetBySinhVienAsync`) | Đổi `.ThenInclude(lhp => lhp.HocKy)` (dòng 16-17) thành `.ThenInclude(lhp => lhp.HocKy).ThenInclude(hk => hk.NamHoc)` để `NamHoc` không null khi map |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/types/index.ts` | `DanhSachLopHP`: thêm `namHocId`, `tenNamHoc` |
| `frontend/src/pages/BangDiemPage.tsx` | Đổi nhóm cấp trong (dòng 84-94) từ khoá `tenHocKy` sang `hocKyId`; thêm nhóm cấp ngoài theo `namHocId` bọc bằng `Collapse` (copy pattern `StudentFeeByYear` ở `HocPhiPage.tsx:41-64`), mặc định mở năm học mới nhất; GPA tích lũy toàn khoá (`gpaAll`, dòng 123, 133) giữ nguyên tính trên toàn bộ `data`, không đổi |

### Dữ liệu / Migration

Không cần migration — chỉ thêm field tính toán (join) vào response API,
giống hệt task #13.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~0.25-0.5 ngày (backend 2 dòng mapping + 1 include; frontend là tái sử dụng pattern Collapse đã có sẵn từ task #13) |
| Mức độ rủi ro | Thấp — chỉ thêm field đọc, không đổi endpoint hay logic tính điểm/GPA |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert các file liệt kê ở mục 4 |

## 6. Kế hoạch triển khai

1. Backend: thêm `NamHocId`/`TenNamHoc` vào `DanhSachLopHPDto`, cập nhật
   mapping và `.Include` trong `GetBySinhVienAsync`.
2. `dotnet build`; kiểm tra thủ công `GET /api/danh-sach-lop-hp/me` trả đủ
   `namHocId`/`tenNamHoc`.
3. Frontend: cập nhật type `DanhSachLopHP`; sửa `BangDiemPage.tsx` — nhóm học
   kỳ theo `hocKyId` (sửa bug khoá nhóm), bọc `Collapse` theo `namHocId` bên
   ngoài `Tabs` hiện có.
4. `tsc --noEmit` + `dotnet build`.
5. Kiểm thử trên trình duyệt với tài khoản sinh viên có nhiều năm học/học kỳ
   (vd `sv.k2021.001`, đã dùng ở task #13) — xác nhận panel năm học mới nhất
   mở sẵn, các năm cũ thu gọn, mỗi panel chỉ chứa đúng học kỳ của năm đó, GPA
   từng kỳ và GPA tích lũy vẫn đúng như trước khi đổi.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] Trang Bảng điểm hiện `Collapse` theo Năm học, mặc định chỉ mở năm học
      mới nhất.
- [ ] Trong mỗi panel Năm học, các học kỳ hiện đúng dạng `Tabs` như cũ (bảng
      điểm + GPA/số môn từng kỳ), nhóm theo `hocKyId` (không còn theo tên).
- [ ] GPA tích lũy ở góc trên bên phải không đổi giá trị so với trước khi sửa
      (kiểm thử với cùng 1 tài khoản, so sánh trước/sau).
- [ ] `tsc --noEmit` sạch, `dotnet build` sạch.
- [ ] Đã kiểm thử trên trình duyệt với dữ liệu thật, vai trò Sinh viên.

## 8. Ghi chú

- Đây là task "sinh đôi" của #13 (Học phí) — cùng một lỗ hổng thiếu
  `NamHocId`/`TenNamHoc` trên DTO, cùng một cách vá. Có thể cân nhắc (ở một
  task riêng, không phải task này) trích chung 1 hook/component
  `useGroupByNamHoc` hoặc 1 component `NamHocCollapse` dùng lại cho cả Học
  phí lẫn Bảng điểm nếu sau này còn thêm trang thứ 3 cần kiểu nhóm này —
  hiện tại 2 chỗ chưa đủ để bắt buộc trừu tượng hoá.
- Việc đổi khoá nhóm từ `tenHocKy` sang `hocKyId` là một **sửa lỗi tiềm ẩn**
  đi kèm, không chỉ là thêm tính năng — cần lưu ý khi review vì hành vi hiển
  thị có thể đổi nếu dữ liệu seed thực tế có học kỳ trùng tên.

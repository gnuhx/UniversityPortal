# Cải tiến trang Học phí: chia rõ theo học kỳ (sinh viên) + bộ lọc/tìm kiếm khi tạo đơn lẻ (Admin)

**Task:** #13
**Trạng thái:** done
**Ngày tạo:** 2026-07-06
**Người phụ trách:** (chưa gán)

---

## 0. Trả lời trước: Học phí được tính như thế nào? Đã "map" chưa?

Đã có sẵn và hoạt động đúng (không phải làm mới), luồng như sau:

1. Admin vào trang Học phí, bấm **"Tạo hàng loạt theo học kỳ"**, chọn 1 Học kỳ
   + nhập **giá tiền/1 tín chỉ** (số này Admin gõ tay mỗi lần chạy, hệ thống
   **không lưu** một bảng "đơn giá tín chỉ" cố định nào — xem `HocPhiService.GenerateAsync`,
   `src/UniversityPortal.Application/Services/HocPhiService.cs:70-109`).
2. Hệ thống lấy toàn bộ `danh_sach_lop_hp` (đăng ký học phần) của mọi sinh viên
   trong học kỳ đó (`DanhSachLopHPRepository.GetByHocKyWithDetailsAsync`,
   `src/UniversityPortal.Infrastructure/Repositories/DanhSachLopHPRepository.cs:32-38`),
   gộp nhóm theo sinh viên, cộng dồn **số tín chỉ** của từng lớp học phần
   (`LopHocPhan.ChiTietCTDT.SoTinChi` — tín chỉ môn học đó trong chương trình
   đào tạo).
3. `Số tiền = Tổng tín chỉ đã đăng ký trong kỳ × Giá tiền/tín chỉ`. Sinh viên
   đã có học phí cho kỳ đó rồi thì bị bỏ qua (không tạo trùng, không cập nhật
   lại nếu giá thay đổi).
4. **"Tạo đơn lẻ"** là lối thoát thủ công cho từng trường hợp riêng (sinh viên
   nhập học trễ, cần sửa/thêm ngoài luồng tự động,...) — Admin tự gõ
   `sinhVienId` (ID nội bộ, không hiện tên), chọn Học kỳ, tự nhập Số tiền
   (không tự tính theo tín chỉ).

**Một điểm đáng lưu ý (chưa phải bug cần sửa ngay, chỉ ghi nhận)**:
`GetByHocKyWithDetailsAsync` lấy **toàn bộ** đăng ký trong học kỳ, không lọc
theo `TrangThaiDuyet` — nghĩa là đăng ký đang "Chờ duyệt" hoặc đã "Từ chối"
(nếu tồn tại) vẫn được tính vào học phí giống như đã "Đã duyệt". Nếu đây là
hành vi không mong muốn, cần một task riêng để xác nhận và sửa.

## 1. Bối cảnh / Vấn đề

### 1.1. Sinh viên: trang Học phí "chưa chia theo từng học kỳ như mong muốn"

Task #trước đã đổi bảng phẳng thành 1 Card/học kỳ xếp dạng lưới (grid), sắp
theo thứ tự học kỳ mới nhất trước. Người yêu cầu xem lại vẫn thấy **chưa đúng
như mong muốn** — nhiều khả năng vì 2 lý do cộng lại:

- **Đang xem bản build cũ ở Docker (`http://localhost:3100`)**: đã xác minh
  container `universityportal-frontend` đang chạy từ ~22 giờ trước (trước khi
  có mọi thay đổi trong phiên làm việc này), nên `:3100` **chưa hề phản ánh**
  bản redesign chia-theo-học-kỳ đã làm — cần `docker compose build && docker
  compose up -d` (hoặc lệnh deploy tương ứng) để thấy bản mới. Đây gần như
  chắc chắn là nguyên nhân chính khiến trang "nhìn như cũ".
- **Cách chia hiện tại có thể chưa đúng ý**: lưới thẻ (mỗi thẻ 1 học kỳ) tuy
  đã tách từng học kỳ, nhưng vẫn là 1 khối liên tục, không phân theo **năm
  học** — nếu sinh viên học nhiều năm, muốn thấy rõ ranh giới "năm nào" trước
  khi nhìn "học kỳ nào" thì lưới phẳng chưa đủ rõ.

Đề xuất mục 3.1 dưới đây để chia rõ hơn theo cả 2 cấp: Năm học → Học kỳ.

### 1.2. Admin: modal "Tạo học phí đơn lẻ" khó dùng

Hiện tại (`frontend/src/pages/HocPhiPage.tsx`, modal "Tạo học phí đơn lẻ"):

- Trường "Sinh viên" là ô nhập **ID số** thô (`InputNumber`, field
  `sinhVienId`) — Admin phải biết trước ID nội bộ trong DB, không thấy tên,
  MSSV hay email để xác nhận đúng người.
- Trường "Học kỳ" là 1 dropdown liệt kê **toàn bộ** học kỳ mọi năm học cùng
  lúc, không lọc được theo Năm học trước — càng nhiều năm học, danh sách càng
  dài và khó tìm.

## 2. Mục tiêu

- Trang Học phí của sinh viên chia rõ ràng theo **Năm học**, trong mỗi năm học
  hiện các **Học kỳ** của năm đó — không còn là 1 lưới phẳng chung.
- Modal "Tạo học phí đơn lẻ" (Admin):
  - Chọn **Năm học** trước để lọc dropdown **Học kỳ** (chỉ hiện học kỳ thuộc
    năm đã chọn).
  - Trường sinh viên đổi từ nhập ID thô sang **tìm kiếm gợi ý** theo tên, MSSV
    hoặc email — chọn xong tự động lấy đúng `sinhVienId`.

## 3. Giải pháp đề xuất

### 3.1. Sinh viên — chia theo Năm học rồi tới Học kỳ

`HocPhiDto` hiện đã có `TenHocKy` nhưng **chưa có tên Năm học** riêng (chỉ có
tên học kỳ dạng chuỗi tự do như "HK1 2023-2024", không tách được năm học một
cách đáng tin cậy bằng string parsing — xem hạn chế tương tự đã ghi ở task #04
về `khoaHoc` tự do). Cách chắc chắn: thêm `TenNamHoc`/`NamHocId` vào
`HocPhiDto` (map qua `HocPhi.HocKy.NamHoc`), tương tự cách `SinhVienDto` đã
được mở rộng thêm thông tin ngành ở task #11.

Giao diện: dùng AntD `Collapse` — mỗi panel là 1 Năm học (vd "2024-2025"),
panel mặc định mở là năm học **mới nhất** (hoặc tất cả mở, tuỳ chốt ở mục
3.3), bên trong là lưới Card theo học kỳ (giữ nguyên `SemesterFeeCard` đã có,
chỉ đổi cách nhóm bên ngoài). Nhóm theo `namHocId`, sắp năm học giảm dần theo
học kỳ mới nhất trong năm đó.

### 3.2. Admin — modal "Tạo học phí đơn lẻ"

- Thêm 2 field mới **chỉ để lọc UI** (không gửi lên API, `CreateHocPhi` DTO
  giữ nguyên `{ sinhVienId, hocKyId, soTien }`):
  - `namHocId` (Select, tuỳ chọn) → lọc option của Select "Học kỳ" chỉ còn học
    kỳ thuộc năm đã chọn. Chọn lại Năm học thì reset lựa chọn Học kỳ đang chọn
    nếu không còn thuộc năm mới.
- Đổi field `sinhVienId` từ `InputNumber` sang AntD `Select` chế độ tìm kiếm từ
  xa (`showSearch` + `onSearch` debounce + `filterOption={false}`), gọi
  `sinhVienApi.getPaged({ keyword, page: 1, pageSize: 20 })` (API đã có sẵn,
  hỗ trợ tìm theo họ tên hoặc MSSV — xem
  `SinhVienRepository.GetPagedFilterAsync`,
  `src/UniversityPortal.Infrastructure/Repositories/SinhVienRepository.cs:42-70`).
  Option hiển thị dạng `"Nguyễn Văn An — 2021001"`, giá trị thực là
  `sinhVienId`.
- Người yêu cầu muốn tìm được cả theo **email** — keyword hiện tại chỉ khớp
  họ tên/MSSV (`x.TaiKhoan.HoTen...Contains` hoặc `x.Mssv...Contains`), **chưa**
  khớp email. Cần thêm điều kiện `|| x.TaiKhoan.Email.ToLower().Contains(tuKhoa)`
  vào cùng chỗ đó (1 dòng, không đổi signature).

### 3.3. Điểm đã chốt (dùng đúng phương án đề xuất mặc định)

1. Trang Học phí sinh viên: **Collapse theo Năm học**, chỉ mở sẵn năm học mới
   nhất, các năm cũ thu gọn.
2. Áp dụng bộ lọc "Năm học → Học kỳ" cho **cả 2 modal** ("Tạo hàng loạt theo
   học kỳ" và "Tạo học phí đơn lẻ") để nhất quán.
3. Không hiển thị trạng thái "đã có học phí kỳ này chưa" trong gợi ý tìm sinh
   viên — giữ nguyên cơ chế báo lỗi trùng sẵn có của `CreateAsync`.

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Application/DTOs/HocPhi/HocPhiDto.cs` | Thêm `NamHocId`, `TenNamHoc` |
| `src/UniversityPortal.Application/Mappings/MappingProfile.cs` (hoặc nơi map `HocPhi → HocPhiDto` thủ công trong `HocPhiService`) | Map thêm 2 field trên qua `hp.HocKy.NamHoc` |
| `src/UniversityPortal.Infrastructure/Repositories/HocPhiRepository.cs` | Các query trả `HocPhi` cho sinh viên (`GetBySinhVienAsync`, `GetAllWithDetailsAsync`,...) cần `.Include(x => x.HocKy).ThenInclude(hk => hk.NamHoc)` để `NamHoc` không null khi map |
| `src/UniversityPortal.Infrastructure/Repositories/SinhVienRepository.cs` | `GetPagedFilterAsync`: thêm điều kiện khớp `Email` vào keyword search |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/types/index.ts` | `HocPhi`: thêm `namHocId`, `tenNamHoc` |
| `frontend/src/pages/HocPhiPage.tsx` | Sinh viên: nhóm `items` theo `namHocId`, render `Collapse` (1 panel/năm học) bọc ngoài lưới `SemesterFeeCard` hiện có. Admin: modal "Tạo đơn lẻ" (và "Tạo hàng loạt" nếu chốt mục 3.3.2) thêm Select "Năm học" lọc Select "Học kỳ"; đổi field sinh viên sang `Select` `showSearch` gọi `sinhVienApi.getPaged` |

### Dữ liệu / Migration

Không cần migration DB — chỉ thêm field tính toán (join) vào response API.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~0.5 ngày (backend nhỏ; frontend chủ yếu là Collapse + Select tìm kiếm từ xa) |
| Mức độ rủi ro | Thấp — không đổi API tạo/sửa học phí, chỉ thêm field đọc và cải thiện UI nhập liệu |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert các file liệt kê ở mục 4 |

## 6. Kế hoạch triển khai

1. Backend: thêm `NamHocId`/`TenNamHoc` vào `HocPhiDto`, cập nhật mapping và
   `.Include` cần thiết; thêm điều kiện khớp email vào
   `SinhVienRepository.GetPagedFilterAsync`.
2. `dotnet build`; kiểm tra thủ công `/api/hoc-phi/me` trả đủ `tenNamHoc`.
3. Frontend: cập nhật type `HocPhi`, dựng `Collapse` theo Năm học ở trang Học
   phí sinh viên (giữ nguyên `SemesterFeeCard`/lưới bên trong mỗi panel).
4. Frontend: modal "Tạo học phí đơn lẻ" — thêm Select Năm học lọc Select Học
   kỳ; đổi ô Sinh viên sang `Select showSearch` gọi API tìm theo tên/MSSV/email.
   Áp dụng tương tự cho modal "Tạo hàng loạt" nếu chốt ở mục 3.3.2.
5. `tsc --noEmit` + `dotnet build`.
6. Kiểm thử trên trình duyệt với dữ liệu thật (tài khoản `sv.k2021.001` —
   nhiều năm học/học kỳ để thấy rõ Collapse; Admin tạo học phí đơn lẻ bằng
   cách gõ tên/email tìm sinh viên).
7. **Nhắc người yêu cầu**: sau khi duyệt và merge, cần
   `docker compose build && docker compose up -d` (hoặc quy trình deploy hiện
   dùng) để `http://localhost:3100` phản ánh bản mới — cả bản redesign lần
   trước lẫn thay đổi task này.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Trang Học phí sinh viên hiện `Collapse` theo Năm học, mỗi panel chứa
      đúng các học kỳ của năm đó. Đã kiểm thử với tài khoản `sv.k2021.001`
      (4 năm học, 7 học kỳ): panel "2024-2025" mở sẵn đúng như chốt ở mục
      3.3.1; mở thêm "2023-2024" hiện đúng 2 học kỳ của năm đó.
- [x] Modal "Tạo học phí đơn lẻ": chọn Năm học lọc đúng Select Học kỳ. Đã
      kiểm thử: chọn "2024-2025" → Select Học kỳ chỉ còn 4 lựa chọn, đều
      thuộc đúng năm đó.
- [x] Modal "Tạo học phí đơn lẻ": gõ tên/MSSV/email sinh viên hiện gợi ý đúng,
      chọn xong gửi đúng `sinhVienId`. Đã kiểm thử: gõ "Nguyễn Văn An" →
      gợi ý "Nguyễn Văn An — 2021001", chọn xong field hiện đúng nhãn đó.
- [x] Modal "Tạo hàng loạt theo học kỳ" cũng có filter Năm học → Học kỳ
      tương tự (áp dụng theo quyết định mục 3.3.2).
- [x] `tsc --noEmit` sạch, `dotnet build` sạch.
- [x] Đã kiểm thử trên trình duyệt với dữ liệu thật (Playwright headless +
      backend/DB thật), vai trò Sinh viên và Admin, không có lỗi console.

## 8. Ghi chú

- Bản redesign "chia theo học kỳ" ở task trước **đã tồn tại trong source
  code** nhưng chưa lên `:3100` (container Docker chưa rebuild) khi task này
  bắt đầu — đã xác nhận qua `docker ps -a`. Trong lúc hoàn thành task này,
  2 container (`universityportal-api-1`, `universityportal-frontend-1`) đã tự
  chuyển sang "Exited (0)" — không phải do thao tác nào trong phiên làm việc
  này (không có lệnh `docker stop`/`docker compose down` nào được chạy ở đây),
  nên nhiều khả năng người yêu cầu đã tự dừng/chuẩn bị rebuild ở nơi khác.
  Cần `docker compose build && docker compose up -d` (hoặc quy trình deploy
  hiện dùng) để `:3100` phản ánh bản mới nhất — bao gồm cả bản redesign lần
  trước lẫn thay đổi task này.
- Ghi nhận riêng (không thuộc phạm vi task này): `GenerateAsync` không lọc
  theo `TrangThaiDuyet` khi tính học phí theo tín chỉ — nếu cần chỉ tính đăng
  ký "Đã duyệt", cần task riêng để xác nhận đây có phải hành vi mong muốn hay
  không trước khi sửa.
- AntD nâng lên v6 trong repo này: cú pháp tìm kiếm từ xa của `Select` đã đổi
  từ prop rời (`onSearch`, `filterOption`) sang gộp trong object
  `showSearch={{ onSearch, filterOption }}` — đã áp dụng đúng cú pháp mới ở
  modal "Tạo học phí đơn lẻ".

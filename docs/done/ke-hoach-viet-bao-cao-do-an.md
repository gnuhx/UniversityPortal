# Kế hoạch viết Báo cáo Đồ án tốt nghiệp — University Portal

> Mục tiêu: chuẩn bị báo cáo + demo cho giáo viên hướng dẫn, dựa trên source code thực tế hiện có (ASP.NET Core 10 Clean Architecture + **SQL Server** + React 19/Vite/Ant Design + Docker, đã seed dữ liệu mẫu). Chi tiết kiến trúc & trạng thái đầy đủ: xem `docs/done/project-plan.md`.

---

## 1. Cấu trúc chương báo cáo (đề xuất)

### Chương 1 — Tổng quan đề tài
- Lý do chọn đề tài: nhu cầu quản lý thông tin sinh viên, giáo viên, chương trình đào tạo, điểm số, học phí... tại một khoa/trường.
- Mục tiêu đề tài: xây dựng hệ thống **Cổng thông tin Đại học (University Portal)** hỗ trợ 4 vai trò: Admin, Giáo vụ, Giáo viên, Sinh viên.
- Đối tượng & phạm vi: các module đã triển khai (xem mục 2).
- Phương pháp & công nghệ: ASP.NET Core (C#), MySQL, Entity Framework Core (Code First), Docker Compose, JWT Authentication, kiến trúc Clean Architecture.

### Chương 2 — Cơ sở lý thuyết / Công nghệ sử dụng
- Kiến trúc Clean Architecture (Domain – Application – Infrastructure – API), Repository Pattern, Unit of Work, Service Layer.
- ASP.NET Core Web API (.NET 10), Entity Framework Core (Code First + Migrations, auto-migrate lúc khởi động), **SQL Server**.
- Xác thực & phân quyền: JWT access token + refresh token (xoay vòng), Role-based Authorization (Admin/Giáo vụ/Giáo viên/Sinh viên).
- AutoMapper, FluentValidation.
- Docker & Docker Compose để đóng gói; build image đẩy lên **GHCR** rồi pull về server chạy (xem `docs/done/deploy-docker-registrry.md`, `docs/done/deploy-ec2.md`).
- ReactJS 19 + Vite + TypeScript cho frontend, dùng **Ant Design** làm UI kit (form, table, modal, notification có sẵn), TanStack Query cho data fetching, Zustand cho state, Axios cho gọi API.

### Chương 3 — Phân tích & Thiết kế hệ thống
- **Phân tích yêu cầu**: theo từng vai trò (Admin, Giáo vụ, Giáo viên, Sinh viên) — lấy từ các Controller hiện có.
- **Sơ đồ Use Case** cho từng vai trò.
- **Thiết kế cơ sở dữ liệu**:
  - Sơ đồ ERD (đã có sẵn tại `docs/erd_schema.html` — render ra ảnh để chèn báo cáo).
  - Danh sách các bảng/entity chính: `TaiKhoan`, `VaiTro`, `PhongBan`, `SinhVien`, `GiaoVien`, `LopSinhHoat`, `NganhHoc`, `ChuongTrinhDT`, `ChiTietCTDT`, `MonHoc`, `NamHoc`, `HocKy`, `TuanHoc`, `LopHocPhan`, `DanhSachLopHP`, `ThoiKhoaBieu`, `HocPhi`, `ThongBao`, `YeuCauHanhChinh`, `YeuCauSuaDiem`, `NoiDungTinh` (đã có Controller/Service/API/trang UI đầy đủ).
  - Các entity **chỉ có schema** (Entity + EF Configuration), chưa có Service/Controller/UI riêng: `DanhSachThiLai`, `HocBa`, `DiemRenLuyen`, `KetQuaAnhVanDauVao`, `ThongBaoDaDoc`, `BinhLuanThongBao`, `BienBanSHCN`, `ChiTietCongViec`, `ChiTietVangSHCN`, `DatPhongThucHanh`, `KhaoSatYKien`, `DienDanGiaoVien` — nên nêu rõ trong Chương 7 (hạn chế/hướng phát triển) thay vì trình bày như đã hoàn thiện.
  - Giải thích các Enum: `VaiTroEnum`, `LoaiDangKyEnum`, `TrangThaiDuyetEnum`, `TrangThaiDongTienEnum`, `LoaiThongBaoEnum`, `MucDoThongBaoEnum`.
- **Thiết kế kiến trúc hệ thống**: vẽ sơ đồ luồng request đi qua API → Application (Service) → Infrastructure (Repository/UnitOfWork) → Domain.
- **Thiết kế API**: tổng hợp endpoint theo Controller (xem mục 3 bên dưới), có thể export từ `docs/UniversityPortal.postman_collection.json`.

### Chương 4 — Triển khai hệ thống
- Mô tả cấu trúc solution (4 project: Domain, Application, Infrastructure, API + Tests).
- Trình bày chi tiết 2–3 chức năng tiêu biểu (mỗi chức năng đi từ Entity → Repository → Service → Controller → DTO), ví dụ:
  - Đăng nhập & phân quyền JWT (`AuthController`, `AuthService`).
  - Quản lý sinh viên (`SinhVienController`, `SinhVienService`).
  - Quản lý giáo viên / lớp sinh hoạt (`GiaoVienController`, `LopSinhHoatController`).
  - Quản lý chương trình đào tạo (`ChuongTrinhDTController`, `ChiTietCTDTController`).
- Trình bày Middleware xử lý lỗi toàn cục (`GlobalExceptionMiddleware`).
- Trình bày cấu hình triển khai bằng Docker (`docker-compose.yml`) và lên AWS EC2 (đã có sẵn `docs/deploy-ec2.md` — dùng làm tư liệu).

### Chương 5 — Kiểm thử (Testing)
- Giới thiệu unit test (`tests/UniversityPortal.Tests`) — liệt kê các service/repository đã viết test.
- Kiểm thử API bằng Postman collection có sẵn (`docs/UniversityPortal.postman_collection.json`).
- Bảng kết quả test (pass/fail, coverage nếu có).

### Chương 6 — Kết quả & Demo
- Ảnh chụp màn hình Swagger UI cho từng nhóm API.
- (Nếu có frontend) Ảnh chụp UI từng vai trò: đăng nhập, dashboard, quản lý sinh viên, điểm, học phí, thông báo...
- Bảng tài khoản demo (lấy từ README): admin, giaovu01, gv.tuan, sv.an...
- Kịch bản demo trực tiếp (xem mục 4).

### Chương 7 — Kết luận & Hướng phát triển
- Kết quả đạt được so với mục tiêu ban đầu.
- Hạn chế (module chưa hoàn thiện, frontend chưa có/đang làm...).
- Hướng phát triển: thêm module (học phí online, thông báo realtime, app mobile...), tối ưu hiệu năng, CI/CD.

---

## 2. Bảng tổng hợp module theo Controller hiện có (21 controllers)

| Controller | Chức năng chính | Vai trò sử dụng |
|---|---|---|
| `AuthController` | Đăng nhập, refresh token, đăng xuất | Tất cả |
| `TaiKhoanController` | Quản lý tài khoản người dùng | Admin |
| `SinhVienController` | Quản lý thông tin sinh viên | Admin, Giáo vụ, Sinh viên |
| `GiaoVienController` | Quản lý thông tin giáo viên | Admin, Giáo vụ, Giáo viên |
| `PhongBanController` | Quản lý phòng ban/khoa | Admin |
| `LopSinhHoatController` | Quản lý lớp sinh hoạt (chủ nhiệm, sinh viên trong lớp) | Admin, Giáo viên |
| `NganhHocController` | Quản lý ngành học | Admin, Giáo vụ |
| `ChuongTrinhDTController` | Quản lý chương trình đào tạo | Admin, Giáo vụ |
| `ChiTietCTDTController` | Chi tiết chương trình đào tạo (môn học theo ngành/khóa/học kỳ) | Admin, Giáo vụ |
| `MonHocController` | Quản lý môn học | Admin, Giáo vụ |
| `NamHocController` | Quản lý năm học | Admin, Giáo vụ |
| `HocKyController` | Quản lý học kỳ | Admin, Giáo vụ |
| `TuanHocController` | Quản lý tuần học | Admin, Giáo vụ |
| `LopHocPhanController` | Mở/đóng lớp học phần, khoá/mở bảng điểm | Admin, Giáo vụ, Giáo viên |
| `DanhSachLopHPController` | Đăng ký học phần, duyệt, **nhập điểm** (qt1/qt2/thi → tổng kết) | Sinh viên, Giáo viên, Giáo vụ |
| `ThoiKhoaBieuController` | Thời khoá biểu | Tất cả |
| `HocPhiController` | Học phí | Admin, Giáo vụ, Sinh viên |
| `ThongBaoController` | Thông báo | Tất cả |
| `YeuCauHanhChinhController` | Yêu cầu hành chính (giấy xác nhận, biên nhận, phản hồi) | Sinh viên, Giáo vụ |
| `YeuCauSuaDiemController` | Yêu cầu mở lại bảng điểm đã khoá | Giáo viên, Admin |
| `NoiDungTinhController` | Nội dung tĩnh — khu vực thư viện/học vụ | Tất cả (đọc), Admin (quản trị) |

> Việt hóa lại bảng này trong báo cáo với mô tả chi tiết từng endpoint (HTTP method, route, mô tả, role yêu cầu) — lấy trực tiếp từ source code mỗi Controller.

---

## 3. Tài nguyên có sẵn nên tái sử dụng

- `docs/erd_schema.html` → mở bằng browser, chụp/export ảnh ERD cho Chương 3.
- `docs/schema.txt` → liệt kê schema chi tiết, dùng để viết phần mô tả CSDL.
- `docs/Data/seed_data.sql` (+ các file seed bổ sung trong `docs/Data/`) → mô tả dữ liệu mẫu dùng cho demo.
- `docs/UniversityPortal.postman_collection.json` → import vào Postman, dùng để test & chụp ảnh kết quả API cho Chương 5/6.
- `docs/done/deploy-docker-registrry.md`, `docs/done/deploy-ec2.md` → tư liệu cho phần triển khai (Chương 4): build → push GHCR → pull image trên EC2.
- `docs/done/project-plan.md` → tư liệu cho phần kiến trúc + trạng thái as-built (Chương 2, 3, 7).
- `docs/features/01`…`22` → nhật ký từng tính năng đã làm (feature-by-feature), hữu ích cho Chương 4 khi chọn ví dụ tiêu biểu; `14_todo_xoa_ctdt_gop_mon_hoc.md` còn ở trạng thái TODO.
- `README.md` → bảng tài khoản demo cho Chương 6.

---

## 4. Kịch bản Demo cho giáo viên (gợi ý 10–15 phút)

1. **Khởi động hệ thống**: `docker compose up --build` → mở Swagger UI (`http://localhost:8080/swagger`) và frontend (`http://localhost:3100`).
2. **Demo đăng nhập & phân quyền**: login bằng `admin`, `giaovu01`, `gv.tuan`, `sv.an` (xem README) → cho thấy token JWT chứa role khác nhau, giao diện đổi theo role.
3. **Demo CRUD cơ bản**: tạo/sửa/xóa 1 Sinh viên hoặc Môn học qua UI (Ant Design table/form) hoặc Swagger, show validation lỗi (FluentValidation) khi nhập sai.
4. **Demo nghiệp vụ đặc trưng**: xem chương trình đào tạo của một ngành (`ChiTietCTDTController`), nhập điểm học phần rồi khoá bảng điểm (`DanhSachLopHPController` + `LopHocPhanController`), tạo yêu cầu hành chính và duyệt cấp giấy xác nhận (`YeuCauHanhChinhController`).
5. **Demo xử lý lỗi tập trung**: gọi 1 request gây lỗi (id không tồn tại) → show response lỗi chuẩn từ `GlobalExceptionMiddleware`.
6. **Demo frontend đầy đủ theo 4 vai trò**: đăng nhập Admin/Giáo vụ/Giáo viên/Sinh viên, thao tác 1–2 chức năng chính trên UI (thời khoá biểu, bảng điểm, học phí, thông báo).

> Đã bỏ bước "demo unit test" — hiện `tests/UniversityPortal.Tests` chỉ có 1 test rỗng, cần viết test thật trước khi đưa vào kịch bản demo (xem mục 5).

---

## 5. Việc cần làm trước khi viết báo cáo

- [ ] Liệt kê đầy đủ endpoint của từng Controller (method, route, request/response DTO, role) thành 1 bảng phụ lục — 21 controller hiện có, xem `docs/done/project-plan.md`.
- [ ] Export ERD từ `docs/erd_schema.html` thành ảnh PNG độ phân giải cao.
- [ ] Viết thêm unit test thật cho `tests/UniversityPortal.Tests` trước khi chạy `dotnet test` lấy số liệu cho Chương 5 — hiện chỉ có 1 test case rỗng (`UnitTest1.cs`), chưa đủ để báo cáo.
- [ ] Chuẩn bị bộ ảnh chụp Swagger/Postman + UI (Ant Design) cho từng nhóm chức năng.
- [ ] Viết phụ lục hướng dẫn cài đặt (dựa trên README.md hiện có — lưu ý CSDL là SQL Server, không phải MySQL).
- [ ] Quyết định cách trình bày các entity chỉ có schema chưa có API/UI (`HocBa`, `DiemRenLuyen`, `DanhSachThiLai`, `BienBanSHCN`, `DatPhongThucHanh`, `KhaoSatYKien`, `DienDanGiaoVien`, ...) — đưa vào Chương 7 "hướng phát triển" thay vì Chương 4 "đã triển khai".

---

## 6. Gợi ý khung thời gian (nếu cần lập tiến độ)

| Giai đoạn | Nội dung | Thời lượng gợi ý |
|---|---|---|
| 1 | Viết Chương 1, 2 (tổng quan, công nghệ) | 2–3 ngày |
| 2 | Viết Chương 3 (phân tích, thiết kế, ERD, API design) | 3–4 ngày |
| 3 | Viết Chương 4 (triển khai chi tiết các module) | 4–5 ngày |
| 4 | Viết Chương 5 (testing) + chạy test, thu thập số liệu | 1–2 ngày |
| 5 | Viết Chương 6 (kết quả, demo, screenshot) | 1–2 ngày |
| 6 | Viết Chương 7 (kết luận) + rà soát toàn bộ | 1–2 ngày |
| 7 | Chuẩn bị slide + thử demo trước buổi báo cáo | 1–2 ngày |

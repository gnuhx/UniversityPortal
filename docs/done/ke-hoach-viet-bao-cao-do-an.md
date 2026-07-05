# Kế hoạch viết Báo cáo Đồ án tốt nghiệp — University Portal

> Mục tiêu: chuẩn bị báo cáo + demo cho giáo viên hướng dẫn, dựa trên source code thực tế hiện có (ASP.NET Core Clean Architecture + MySQL + Docker, đã seed dữ liệu mẫu).

---

## 1. Cấu trúc chương báo cáo (đề xuất)

### Chương 1 — Tổng quan đề tài
- Lý do chọn đề tài: nhu cầu quản lý thông tin sinh viên, giáo viên, chương trình đào tạo, điểm số, học phí... tại một khoa/trường.
- Mục tiêu đề tài: xây dựng hệ thống **Cổng thông tin Đại học (University Portal)** hỗ trợ 4 vai trò: Admin, Giáo vụ, Giáo viên, Sinh viên.
- Đối tượng & phạm vi: các module đã triển khai (xem mục 2).
- Phương pháp & công nghệ: ASP.NET Core (C#), MySQL, Entity Framework Core (Code First), Docker Compose, JWT Authentication, kiến trúc Clean Architecture.

### Chương 2 — Cơ sở lý thuyết / Công nghệ sử dụng
- Kiến trúc Clean Architecture (Domain – Application – Infrastructure – API), Repository Pattern, Unit of Work, Service Layer.
- ASP.NET Core Web API, Entity Framework Core (Code First + Migrations).
- Xác thực & phân quyền: JWT, Role-based Authorization.
- AutoMapper, FluentValidation.
- Docker & Docker Compose để đóng gói và triển khai.
- (Nếu có) ReactJS cho frontend.

### Chương 3 — Phân tích & Thiết kế hệ thống
- **Phân tích yêu cầu**: theo từng vai trò (Admin, Giáo vụ, Giáo viên, Sinh viên) — lấy từ các Controller hiện có.
- **Sơ đồ Use Case** cho từng vai trò.
- **Thiết kế cơ sở dữ liệu**:
  - Sơ đồ ERD (đã có sẵn tại `docs/erd_schema.html` — render ra ảnh để chèn báo cáo).
  - Danh sách các bảng/entity chính: `TaiKhoan`, `VaiTro`, `SinhVien`, `GiaoVien`, `LopSinhHoat`, `NganhHoc`, `ChuongTrinhDT`, `ChiTietCTDT`, `MonHoc`, `LopHocPhan`, `DanhSachLopHP`, `DanhSachThiLai`, `HocBa`, `DiemRenLuyen`, `ThoiKhoaBieu`, `HocPhi`, `ThongBao`, `BienBanSHCN`, `YeuCauHanhChinh`, `YeuCauSuaDiem`, `DatPhongThucHanh`, `KhaoSatYKien`, `DienDanGiaoVien`...
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

## 2. Bảng tổng hợp module theo Controller hiện có

| Controller | Chức năng chính | Vai trò sử dụng |
|---|---|---|
| `AuthController` | Đăng nhập, refresh token | Tất cả |
| `TaiKhoanController` | Quản lý tài khoản người dùng | Admin |
| `SinhVienController` | Quản lý thông tin sinh viên | Admin, Giáo vụ, Sinh viên |
| `GiaoVienController` | Quản lý thông tin giáo viên | Admin, Giáo vụ, Giáo viên |
| `LopSinhHoatController` | Quản lý lớp sinh hoạt (chủ nhiệm, sinh viên trong lớp) | Admin, Giáo viên |
| `NganhHocController` | Quản lý ngành học | Admin, Giáo vụ |
| `ChuongTrinhDTController` | Quản lý chương trình đào tạo | Admin, Giáo vụ |
| `ChiTietCTDTController` | Chi tiết chương trình đào tạo (môn học theo ngành/khóa) | Admin, Giáo vụ |
| `MonHocController` | Quản lý môn học | Admin, Giáo vụ |

> Việt hóa lại bảng này trong báo cáo với mô tả chi tiết từng endpoint (HTTP method, route, mô tả, role yêu cầu) — lấy trực tiếp từ source code mỗi Controller.

---

## 3. Tài nguyên có sẵn nên tái sử dụng

- `docs/erd_schema.html` → mở bằng browser, chụp/export ảnh ERD cho Chương 3.
- `docs/schema.txt` → liệt kê schema chi tiết, dùng để viết phần mô tả CSDL.
- `docs/seed_data.sql` → mô tả dữ liệu mẫu dùng cho demo.
- `docs/UniversityPortal.postman_collection.json` → import vào Postman, dùng để test & chụp ảnh kết quả API cho Chương 5/6.
- `docs/deploy-ec2.md` → tư liệu cho phần triển khai (Chương 4).
- `docs/project-plan.md` → tư liệu cho phần kiến trúc (Chương 2, 3).
- `README.md` → bảng tài khoản demo cho Chương 6.

---

## 4. Kịch bản Demo cho giáo viên (gợi ý 10–15 phút)

1. **Khởi động hệ thống**: `docker compose up --build` → mở Swagger UI (`http://localhost:8080/swagger`).
2. **Demo đăng nhập & phân quyền**: login bằng `admin`, `gv.tuan`, `sv.an` → cho thấy token JWT chứa role khác nhau.
3. **Demo CRUD cơ bản**: tạo/sửa/xóa 1 Sinh viên hoặc Môn học qua Swagger, show validation lỗi (FluentValidation) khi nhập sai.
4. **Demo nghiệp vụ đặc trưng**: ví dụ xem chương trình đào tạo của một ngành (`ChiTietCTDTController`), xem danh sách sinh viên trong lớp sinh hoạt (`LopSinhHoatController`).
5. **Demo xử lý lỗi tập trung**: gọi 1 request gây lỗi (id không tồn tại) → show response lỗi chuẩn từ `GlobalExceptionMiddleware`.
6. **Demo unit test**: chạy `dotnet test` → show kết quả pass.
7. (Nếu có) **Demo frontend**: đăng nhập từng vai trò, thao tác 1–2 chức năng chính trên UI.

---

## 5. Việc cần làm trước khi viết báo cáo

- [ ] Kiểm tra lại frontend: project-plan có đề cập `frontend/university-portal-ui` nhưng hiện chưa thấy trong repo — xác nhận có hay không để quyết định đưa vào báo cáo.
- [ ] Liệt kê đầy đủ endpoint của từng Controller (method, route, request/response DTO, role) thành 1 bảng phụ lục.
- [ ] Export ERD từ `docs/erd_schema.html` thành ảnh PNG độ phân giải cao.
- [ ] Chạy `dotnet test` để lấy số liệu test (số lượng, pass/fail) cho Chương 5.
- [ ] Chuẩn bị bộ ảnh chụp Swagger/Postman cho từng nhóm chức năng.
- [ ] Viết phụ lục hướng dẫn cài đặt (dựa trên README.md hiện có).

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

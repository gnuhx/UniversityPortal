# Kế hoạch xây dựng Frontend Lite (PoC) — University Portal

> Mục tiêu: xây dựng phiên bản frontend "lite" bằng React, đóng gói Docker, chạy cùng `docker compose` với API hiện có, dùng làm bản demo (PoC) để thuyết trình kêu gọi đầu tư/tài trợ.

---

## 1. Đánh giá khả thi

Khả thi cao. Backend hiện đã có sẵn:

- 8 Controller REST API với JWT Authentication & phân quyền theo role (Admin, Giáo vụ, Giáo viên, Sinh viên).
- Swagger UI để tham chiếu API trong quá trình phát triển frontend.
- Docker Compose sẵn cho API + MySQL.

→ Frontend lite chỉ cần tập trung vào UI/UX gọi API hiện có, không cần thay đổi backend. Mục tiêu là "trông như sản phẩm thật, demo được luồng của từng vai trò", không cần đầy đủ nghiệp vụ chi tiết.

---

## 2. Phạm vi theo từng module (Controller)

| Module | Màn hình Lite |
|---|---|
| Auth | Trang đăng nhập, lưu JWT, redirect/guard theo role |
| TaiKhoan (Admin) | Danh sách tài khoản + tạo/khóa tài khoản |
| SinhVien | Danh sách, tìm kiếm, tạo/sửa, xem chi tiết |
| GiaoVien | Danh sách, tạo/sửa, xem chi tiết |
| LopSinhHoat | Danh sách lớp, xem danh sách sinh viên trong lớp |
| NganhHoc | Danh sách + tạo/sửa |
| ChuongTrinhDT | Danh sách chương trình đào tạo + xem chi tiết |
| ChiTietCTDT | Xem môn học theo chương trình/khóa (lồng trong ChuongTrinhDT) |
| MonHoc | Danh sách + tạo/sửa |

### Dashboard theo vai trò

- **Admin**: toàn quyền truy cập tất cả module trên.
- **Giáo vụ**: SinhVien, GiaoVien, NganhHoc, ChuongTrinhDT, ChiTietCTDT, MonHoc.
- **Giáo viên**: danh sách lớp sinh hoạt phụ trách, xem thông tin sinh viên cơ bản.
- **Sinh viên**: xem hồ sơ cá nhân + chương trình đào tạo của ngành mình (read-only).

---

## 3. Công nghệ sử dụng

- **Vite + React 18 + TypeScript** — build nhanh, image nhỏ.
- **UI Library: Ant Design** — có sẵn Table/Form/Modal, làm CRUD nhanh, giao diện chuyên nghiệp cho demo.
- **TanStack Query + Axios** — gọi API, cache, refresh token.
- **React Router v6** — định tuyến + bảo vệ route theo role.
- Component dùng chung `<CrudTable entity="SinhVien" .../>` để tái sử dụng cho các màn CRUD lite, giảm trùng lặp code.

---

## 4. Tích hợp Docker

- `frontend/Dockerfile`: multi-stage — build bằng `node:20`, serve file static bằng `nginx:alpine`.
- Thêm service `frontend` vào `docker-compose.yml` (port `3000` → `80`).
- Nginx proxy `/api/*` sang service `api` để tránh CORS, chạy toàn bộ hệ thống chỉ bằng `docker compose up`.

---

## 5. Tiến độ dự kiến (1 dev)

| Giai đoạn | Công việc | Thời gian |
|---|---|---|
| 1 | Scaffold app, auth, routing, layout, Docker | 1–2 ngày |
| 2 | CRUD chung + SinhVien/GiaoVien/MonHoc/NganhHoc | 2–3 ngày |
| 3 | ChuongTrinhDT/ChiTietCTDT (lồng nhau), LopSinhHoat, TaiKhoan | 2–3 ngày |
| 4 | Dashboard theo role, hoàn thiện UI cho demo | 1–2 ngày |
| **Tổng** | | **~6–10 ngày** |

---

## 6. Kết quả mong đợi

Một bản demo có thể click qua được, phân quyền theo từng vai trò, chạy trên dữ liệu thật từ database hiện có — đủ để trình bày cho nhà đầu tư một sản phẩm hoạt động được, không cần xây dựng đầy đủ mọi nghiệp vụ.

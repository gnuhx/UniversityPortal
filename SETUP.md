# Hướng dẫn cài đặt và chạy dự án

## Yêu cầu

- [Docker](https://www.docker.com/products/docker-desktop) và Docker Compose đã được cài đặt

---

## Bước 1 — Tạo file cấu hình

Sao chép file mẫu và điền thông tin kết nối của bạn:

```bash
cp src/UniversityPortal.API/appsettings.Development.example.json \
   src/UniversityPortal.API/appsettings.Development.json
```

Mở file `src/UniversityPortal.API/appsettings.Development.json` và chỉnh sửa phần `ConnectionStrings`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=HOST;Port=3306;Database=TEN_DATABASE;Uid=USERNAME;Pwd=PASSWORD;SslMode=None;"
  },
  "JWT": {
    "Secret": "THAY_BANG_1_CHUOI_BI_MAT_DAI_IT_NHAT_32_KY_TU"
  }
}
```

> **Lưu ý:** File này chứa thông tin nhạy cảm, **không được commit lên Git**.

---

## Bước 2 — (Chỉ cần nếu MySQL dùng SSL)

Nếu database của bạn yêu cầu SSL (ví dụ: Aiven), bạn cần thêm 2 bước sau:

**2a.** Đặt file chứng chỉ SSL (`ca.pem`) vào thư mục `docs/`:

```
docs/ca.pem
```

**2b.** Tạo file `docker-compose.override.yml` ở thư mục gốc:

```yaml
services:
  api:
    volumes:
      - ./docs/ca.pem:/app/certs/ca.pem:ro
```

Và cập nhật connection string trong `appsettings.Development.json` để thêm `SslCa`:

```
SslMode=VerifyCA;SslCa=/app/certs/ca.pem;
```

---

## Bước 3 — Chạy ứng dụng

```bash
docker compose up --build
```

API sẽ chạy tại: **http://localhost:8080**

Swagger UI: **http://localhost:8080/swagger**

---

## Liên hệ admin

Nếu bạn muốn kết nối vào database chung của nhóm, hãy liên hệ admin để nhận:

- Thông tin kết nối MySQL (host, port, username, password)
- File `ca.pem` nếu database dùng SSL

---

## Dữ liệu mẫu (tự động seed khi khởi động)

Khi ứng dụng chạy lần đầu, hệ thống sẽ tự động tạo dữ liệu mẫu vào database. Bạn có thể dùng ngay để test mà không cần nhập tay.

### Tài khoản đăng nhập

| Vai trò | Tên đăng nhập | Mật khẩu | Ghi chú |
|---|---|---|---|
| Admin | `admin` | `Admin@123` | Quản trị toàn hệ thống |
| Giáo vụ | `giaovu01` | `Giaovu@123` | Nguyễn Thị Lan — Phòng Đào tạo |
| Giáo viên | `gv.tuan` | `Giaovien@123` | TS. Trần Văn Tuấn — Khoa CNTT |
| Giáo viên | `gv.hoa` | `Giaovien@123` | ThS. Lê Thị Hoa — Khoa CNTT |
| Giáo viên | `gv.minh` | `Giaovien@123` | TS. Phạm Văn Minh — Khoa CNTT |
| Sinh viên | `sv.an` | `Sinhvien@123` | Nguyễn Văn An — KTPM22A |
| Sinh viên | `sv.binh` | `Sinhvien@123` | Trần Thị Bình — KTPM22A |
| Sinh viên | `sv.cuong` | `Sinhvien@123` | Lê Văn Cường — KTPM22A |
| Sinh viên | `sv.dung` | `Sinhvien@123` | Phạm Thị Dung — HTTT23A |
| Sinh viên | `sv.em` | `Sinhvien@123` | Hoàng Văn Em — HTTT23A |
| Sinh viên | `sv.phuong` | `Sinhvien@123` | Vũ Thị Phương — HTTT23A |
| Sinh viên | `sv.quan` | `Sinhvien@123` | Đặng Văn Quân — KTPM22A |

### Dữ liệu hệ thống

**Lớp sinh hoạt:**
- `KTPM22A` — Kỹ thuật Phần mềm K22, GVCN: TS. Trần Văn Tuấn
- `HTTT23A` — Hệ thống Thông tin K23, GVCN: ThS. Lê Thị Hoa

**Môn học:** Nhập môn Lập trình, Cấu trúc Dữ liệu & Giải thuật, Lập trình Web, Cơ sở Dữ liệu, Công nghệ Phần mềm, Toán cao cấp, Tiếng Anh cơ bản

**Năm học / Học kỳ:** 2024-2025 gồm HK1 (từ 02/09/2024) và HK2 (từ 03/02/2025), có đủ 20 tuần học

### Dữ liệu điểm số để test

| Tình huống | Sinh viên | Môn | Điểm tổng kết |
|---|---|---|---|
| Điểm tốt | `sv.an`, `sv.cuong` | INT101-01 | 8.0 — 8.9 |
| Điểm trung bình | `sv.binh` | INT101-01 | 6.1 |
| Rớt môn (< 5đ) | `sv.quan` | INT101-01 | 4.1 |
| Học cải thiện | `sv.cuong` | INT302-01 | 8.5 |
| Chưa có điểm thi | `sv.dung`, `sv.em` | INT101-02 | — |
| Chờ duyệt đăng ký | `sv.phuong` | INT101-02 | — |

### Thông báo có sẵn

- **Học vụ / Quan trọng** — Lịch thi học kỳ 1 năm học 2024-2025
- **Học phí / Khẩn cấp** — Nhắc đóng học phí hạn chót 30/11/2024
- **Đoàn Hội / Bình thường** — Chương trình tình nguyện mùa hè xanh 2025
- **Học vụ / Bình thường** — Kết quả xét học bổng học kỳ 1

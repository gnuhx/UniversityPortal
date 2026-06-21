# University Portal

Hệ thống quản lý đại học gồm backend .NET 10 và frontend React + Vite.

---

## Yêu cầu

| Công cụ | Phiên bản |
|---|---|
| [Docker](https://www.docker.com/products/docker-desktop) + Docker Compose | Bất kỳ |
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0 (nếu chạy local không dùng Docker) |
| [Node.js](https://nodejs.org) | 20+ (nếu chạy frontend local) |
| SQL Server | Đã có sẵn (dùng connection string của nhóm) |

---

## Cách 1 — Chạy bằng Docker Compose (khuyến nghị)

### Bước 1: Cấu hình connection string

Mở file `docker-compose.yml`, tìm dòng `ConnectionStrings__Default` và thay bằng thông tin SQL Server của bạn:

```yaml
ConnectionStrings__Default: "Data Source=HOST;Initial Catalog=DATABASE;User Id=USERNAME;Password=PASSWORD;Encrypt=True;TrustServerCertificate=True;"
```

### Bước 2: Chạy

```bash
docker compose up --build
```

| Service | URL |
|---|---|
| Frontend | http://localhost:3100 |
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |

### Dừng ứng dụng

```bash
docker compose down
```

---

## Cách 2 — Chạy local không dùng Docker

### Backend (.NET)

**Bước 1:** Tạo file cấu hình:

```bash
cp src/UniversityPortal.API/appsettings.json \
   src/UniversityPortal.API/appsettings.Development.json
```

**Bước 2:** Mở `src/UniversityPortal.API/appsettings.Development.json` và điền thông tin kết nối:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=HOST;Initial Catalog=DATABASE;User Id=USERNAME;Password=PASSWORD;Encrypt=True;TrustServerCertificate=True;"
  },
  "JWT": {
    "Secret": "THAY_BANG_CHUOI_BI_MAT_IT_NHAT_32_KY_TU"
  }
}
```

> **Lưu ý:** Không commit file này lên Git.

**Bước 3:** Chạy API:

```bash
cd src/UniversityPortal.API
dotnet run
```

API chạy tại: **http://localhost:8080** — Swagger: **http://localhost:8080/swagger**

---

### Frontend (React + Vite)

```bash
cd frontend
npm install
npm run dev
```

Frontend chạy tại: **http://localhost:5173**

> Mặc định frontend gọi API qua `/api`. Nếu chạy local riêng lẻ, tạo file `frontend/.env.local`:
> ```
> VITE_API_BASE_URL=http://localhost:8080
> ```

---

## Cách 3 — Chạy từ GHCR (không cần source code)

Dùng khi deploy lên server, không cần build từ source.

**Bước 1:** Login GHCR:

```bash
echo YOUR_PAT | docker login ghcr.io -u gnuhx --password-stdin
```

**Bước 2:** Tạo `docker-compose.yml`:

```yaml
services:
  frontend:
    image: ghcr.io/gnuhx/university-portal-frontend:latest
    restart: on-failure
    depends_on:
      - api
    ports:
      - "3100:80"

  api:
    image: ghcr.io/gnuhx/university-portal-api:latest
    restart: on-failure
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__Default: "Data Source=HOST;Initial Catalog=DATABASE;User Id=USERNAME;Password=PASSWORD;Encrypt=True;TrustServerCertificate=True;"
      JWT__Secret: "THAY_BANG_CHUOI_BI_MAT_IT_NHAT_32_KY_TU"
      JWT__Issuer: UniversityPortal
      JWT__Audience: UniversityPortal
      JWT__ExpiryMinutes: "60"
      JWT__RefreshTokenExpiryDays: "7"
    ports:
      - "8080:8080"
    volumes:
      - uploads_data:/app/uploads

volumes:
  uploads_data:
```

**Bước 3:** Chạy:

```bash
docker compose up -d
```

---

## Tài khoản đăng nhập mẫu

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Admin | `admin` | `Admin@123` |
| Giáo vụ | `giaovu01` | `Giaovu@123` |
| Giáo viên | `gv.tuan` | `Giaovien@123` |
| Sinh viên | `sv.an` | `Sinhvien@123` |

> Xem đầy đủ danh sách tài khoản và dữ liệu mẫu trong [docs/seed-data](docs/seed_data.sql).

---

## Liên hệ

Để nhận thông tin kết nối SQL Server của nhóm, liên hệ admin.

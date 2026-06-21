# Deploy với Docker Registry (GHCR) lên EC2

## Tổng quan flow

```
Local Machine  →  Build Images  →  Push to GHCR  →  EC2 Pull & Run
```

---

## Bước 1: Login vào GHCR (GitHub Container Registry)

```bash
docker login ghcr.io -u YOUR_GITHUB_USERNAME
# Nhập PAT khi được hỏi Password
```

### Tạo PAT (Personal Access Token)
- Vào: `https://github.com/settings/tokens/new`
- Chọn scopes: `write:packages`, `read:packages`
- Copy token sau khi tạo (chỉ hiện 1 lần)

### Lỗi gặp phải
```
Error response from daemon: Get "https://ghcr.io/v2/": denied: denied
```
**Nguyên nhân:** Dùng email thay vì GitHub username, hoặc dùng password thay vì PAT.

**Fix:** Dùng đúng GitHub username và PAT (không phải email, không phải password GitHub).

---

## Bước 2: Build Images với tag GHCR

```bash
# Từ thư mục root của project
docker build -t ghcr.io/gnuhx/university-portal-api:latest .
docker build -t ghcr.io/gnuhx/university-portal-frontend:latest ./frontend
```

### Lưu ý
- Format tag phải là `ghcr.io/USERNAME/IMAGE_NAME:TAG`
- Build backend từ root (`Dockerfile`)
- Build frontend từ thư mục `./frontend` (`frontend/Dockerfile`)

---

## Bước 3: Push Images lên GHCR

```bash
docker push ghcr.io/gnuhx/university-portal-api:latest
docker push ghcr.io/gnuhx/university-portal-frontend:latest
```

### Lỗi gặp phải
```
The push refers to repository [docker.io/your-registry/university-portal-api]
denied: requested access to the resource is denied
```
**Nguyên nhân:** Copy nguyên lệnh ví dụ với `your-registry` mà không thay thế bằng địa chỉ thật.

**Fix:** Thay `your-registry` bằng `ghcr.io/gnuhx`.

### Kiểm tra images sau khi push
Vào: `https://github.com/gnuhx?tab=packages`

---

## Bước 4: Chuẩn bị EC2

### Login GHCR trên EC2

```bash
echo YOUR_PAT | docker login ghcr.io -u gnuhx --password-stdin
```

> Trên EC2 chỉ cần scope `read:packages` vì chỉ pull, không push.

### Lỗi gặp phải
```
Error response from daemon: Get "https://ghcr.io/v2/": denied: denied
```
**Nguyên nhân:** Gõ chữ `YOUR_PAT` thay vì giá trị token thật.

**Fix:** Thay `YOUR_PAT` bằng token thật, ví dụ `ghp_xxxxxxxxxxxx`.

---

## Bước 5: Tạo docker-compose.yml trên EC2

Không dùng source code trên EC2. Chỉ cần file `docker-compose.yml` với `image:` trỏ về GHCR.

```bash
mkdir ~/UniversityPortal
nano ~/UniversityPortal/docker-compose.yml
```

Nội dung file:

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
      ConnectionStrings__Default: "Data Source=SQL1002.site4now.net;Initial Catalog=db_acaeb4_datn;User Id=db_acaeb4_datn_admin;Password=Admin@123;Encrypt=True;TrustServerCertificate=True;"
      JWT__Issuer: UniversityPortal
      JWT__Audience: UniversityPortal
      JWT__ExpiryMinutes: "60"
      JWT__RefreshTokenExpiryDays: "7"
      JWT__Secret: "CHANGE_THIS_TO_A_LONG_RANDOM_SECRET_AT_LEAST_32_CHARS"
    ports:
      - "8080:8080"
    volumes:
      - uploads_data:/app/uploads

volumes:
  uploads_data:
```

### Sự khác biệt quan trọng: `build:` vs `image:`

| `build:` | `image:` |
|---|---|
| Build từ source code có sẵn trên máy | Pull image từ registry |
| Cần Dockerfile và source code | Chỉ cần docker-compose.yml |
| Chậm hơn | Nhanh hơn |

---

## Bước 6: Pull Images và Chạy trên EC2

### Pull images thủ công (tuỳ chọn)

Bạn có thể pull trước để kiểm tra login thành công:

```bash
docker pull ghcr.io/gnuhx/university-portal-api:latest
docker pull ghcr.io/gnuhx/university-portal-frontend:latest
```

Kết quả thành công sẽ trông như sau:

```
latest: Pulling from gnuhx/university-portal-api
...
Status: Downloaded newer image for ghcr.io/gnuhx/university-portal-api:latest
```

### Chạy bằng Docker Compose

```bash
cd ~/UniversityPortal
docker compose up -d
```

`docker compose up -d` sẽ tự động pull images từ GHCR nếu chưa có trên máy, sau đó chạy containers ở chế độ nền.

### Kiểm tra trạng thái

```bash
docker compose ps          # xem containers đang chạy
docker compose logs -f     # xem logs realtime (Ctrl+C để thoát)
docker compose logs api    # xem logs riêng backend
docker compose logs frontend  # xem logs riêng frontend
```

### Truy cập ứng dụng

| Service | URL |
|---|---|
| Frontend | http://EC2_PUBLIC_IP:3100 |
| API | http://EC2_PUBLIC_IP:8080 |
| Swagger | http://EC2_PUBLIC_IP:8080/swagger |

> Nhớ mở port `3100` và `8080` trong Security Group của EC2 trên AWS Console.

### Cập nhật image mới (khi có version mới)

```bash
docker compose pull        # pull images mới nhất từ GHCR
docker compose up -d       # restart containers với image mới
```

### Dừng ứng dụng

```bash
docker compose down        # dừng và xoá containers (giữ volumes)
docker compose down -v     # dừng và xoá cả volumes (mất dữ liệu uploads)
```

---

## Tóm tắt các lỗi thường gặp

| Lỗi | Nguyên nhân | Fix |
|---|---|---|
| `denied` khi login | Dùng email thay vì GitHub username | Dùng username GitHub |
| `denied` khi login | Dùng password thay vì PAT | Tạo PAT và dùng token |
| Push về `docker.io/your-registry` | Copy nguyên placeholder | Thay bằng `ghcr.io/gnuhx` |
| Image build từ local thay vì pull | `docker-compose.yml` dùng `build:` | Đổi sang `image: ghcr.io/gnuhx/...` |

---

## GHCR vs Docker Hub

| | GHCR | Docker Hub |
|---|---|---|
| URL | `ghcr.io` | `docker.io` |
| Quản lý tại | github.com → Packages tab | hub.docker.com |
| Auth | GitHub PAT | Docker account |
| Free private images | Có (với GitHub account) | Giới hạn |

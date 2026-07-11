# Deploy UniversityPortal lên EC2 t3.micro

## Tổng quan kiến trúc

```
Internet → EC2 t3.micro (Docker: ASP.NET Core API :8080)
                ↕
         Aiven Cloud MySQL (đã có sẵn, remote)
```

Stack: **.NET 10**, **MySQL (Aiven remote)**, **Docker**, **SSL cert** cho DB connection.

---

## Bước 1 — Chuẩn bị AWS

### 1.1 Tạo EC2 instance

| Tùy chọn | Giá trị |
|---|---|
| AMI | **Ubuntu 24.04 LTS** (free tier eligible) |
| Instance type | **t3.micro** |
| Key pair | Tạo mới `.pem`, lưu lại |
| Storage | 20 GB gp3 (mặc định 8GB quá nhỏ vì Docker images) |

### 1.2 Cấu hình Security Group

Thêm các Inbound Rules sau:

| Port | Protocol | Source | Lý do |
|---|---|---|---|
| 22 | TCP | My IP | SSH |
| 8080 | TCP | 0.0.0.0/0 | API port |
| 80 | TCP | 0.0.0.0/0 | HTTP (nếu sau này thêm Nginx) |
| 443 | TCP | 0.0.0.0/0 | HTTPS (nếu sau này thêm Nginx) |

> **Lưu ý t3.micro:** RAM chỉ có 1GB. Build Docker image nặng (~500MB) **trên máy local rồi push lên Docker Hub**, không build trực tiếp trên EC2 (sẽ bị OOM kill).

**Checklist bước 1:**
- [ ] EC2 instance tạo xong, trạng thái `running`
- [ ] Key pair `.pem` đã tải về máy local
- [ ] Security Group đã mở port 22, 8080
- [ ] Storage đặt 20GB

---

## Bước 2 — Build & Push Docker Image (trên máy local)

### 2.1 Đăng nhập Docker Hub

```bash
docker login
```

### 2.2 Build image

```bash
cd /home/gnuh/UniversityPortal
docker build -t <docker-hub-username>/university-portal:latest .
```

> Image dùng `mcr.microsoft.com/dotnet/aspnet:10.0` (~200MB). Build lần đầu mất ~3-5 phút.

### 2.3 Push lên Docker Hub

```bash
docker push <docker-hub-username>/university-portal:latest
```

**Checklist bước 2:**
- [ ] `docker build` thành công (không có lỗi)
- [ ] Image xuất hiện trên Docker Hub
- [ ] `docker push` thành công

---

## Bước 3 — Chuẩn bị file cấu hình production

### 3.1 Tạo `appsettings.Production.json` (trên máy local)

Tạo file này **không commit lên git** (đã có trong `.gitignore`):

```json
{
  "ConnectionStrings": {
    "Default": "Server=mysql-270f2e77-universityportal888.g.aivencloud.com;Port=28641;Database=defaultdb;Uid=avnadmin;Pwd=<MẬT_KHẨU>;SslMode=VerifyCA;SslCa=/app/certs/ca.pem;"
  },
  "JWT": {
    "Secret": "<CHUỖI_RANDOM_TỐI_THIỂU_32_KÝ_TỰ>",
    "Issuer": "UniversityPortal",
    "Audience": "UniversityPortal",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> Tạo JWT Secret ngẫu nhiên: `openssl rand -base64 48`

### 3.2 Tạo `docker-compose.prod.yml` (trên máy local)

```yaml
services:
  api:
    image: <docker-hub-username>/university-portal:latest
    restart: unless-stopped
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ASPNETCORE_URLS: http://+:8080
      JWT__Issuer: UniversityPortal
      JWT__Audience: UniversityPortal
      JWT__ExpiryMinutes: "60"
      JWT__RefreshTokenExpiryDays: "7"
      JWT__Secret: "<CHUỖI_RANDOM>"
      ConnectionStrings__Default: "Server=mysql-270f2e77-universityportal888.g.aivencloud.com;Port=28641;Database=defaultdb;Uid=avnadmin;Pwd=<MẬT_KHẨU>;SslMode=VerifyCA;SslCa=/app/certs/ca.pem;"
    ports:
      - "8080:8080"
    volumes:
      - ./ca.pem:/app/certs/ca.pem:ro
      - uploads_data:/app/uploads

volumes:
  uploads_data:
```

> Dùng environment variables thay vì mount file `appsettings.Production.json` để tránh lộ secret trên server.

**Checklist bước 3:**
- [ ] JWT Secret đã generate (≥32 chars)
- [ ] `docker-compose.prod.yml` đã tạo với đúng connection string
- [ ] Không commit `appsettings.Production.json` hay `docker-compose.prod.yml` lên git

---

## Bước 4 — Setup EC2

### 4.1 SSH vào EC2

```bash
chmod 400 your-key.pem
ssh -i your-key.pem ubuntu@<EC2_PUBLIC_IP>
```

### 4.2 Cài Docker

```bash
sudo apt update && sudo apt upgrade -y
sudo apt install -y docker.io docker-compose-v2
sudo usermod -aG docker ubuntu
newgrp docker
```

> `newgrp docker` áp dụng quyền ngay mà không cần logout.

### 4.3 Kiểm tra Docker hoạt động

```bash
docker --version
docker compose version
```

**Checklist bước 4:**
- [ ] SSH vào EC2 thành công
- [ ] `docker --version` trả về version (≥24.x)
- [ ] `docker compose version` trả về version

---

## Bước 5 — Deploy lên EC2

### 5.1 Tạo thư mục deploy trên EC2

```bash
mkdir ~/app && cd ~/app
```

### 5.2 Copy file từ máy local lên EC2

**Chạy trên máy local** (không phải trong SSH):

```bash
# Copy SSL cert và docker-compose
scp -i your-key.pem \
  /home/gnuh/UniversityPortal/docs/ca.pem \
  ubuntu@<EC2_PUBLIC_IP>:~/app/

scp -i your-key.pem \
  /home/gnuh/UniversityPortal/docker-compose.prod.yml \
  ubuntu@<EC2_PUBLIC_IP>:~/app/
```

> File `ca.pem` là SSL cert để connect tới Aiven MySQL — **bắt buộc phải có**.

### 5.3 Pull image và chạy

**Trên EC2:**

```bash
cd ~/app
docker compose -f docker-compose.prod.yml pull
docker compose -f docker-compose.prod.yml up -d
```

### 5.4 Kiểm tra logs

```bash
docker compose -f docker-compose.prod.yml logs -f
```

API khởi động thành công khi thấy:

```
Now listening on: http://[::]:8080
Application started.
```

**Checklist bước 5:**
- [ ] `ca.pem` đã copy lên `~/app/`
- [ ] `docker-compose.prod.yml` đã copy lên `~/app/`
- [ ] `docker compose up -d` chạy thành công (exit 0)
- [ ] Logs không có lỗi kết nối DB
- [ ] `curl http://localhost:8080/swagger/index.html` trả về HTML

---

## Bước 6 — Kiểm tra từ bên ngoài

```bash
# Thay EC2_PUBLIC_IP bằng IP thật
curl http://<EC2_PUBLIC_IP>:8080/swagger/index.html
```

Hoặc mở browser: `http://<EC2_PUBLIC_IP>:8080/swagger`

**Checklist bước 6:**
- [ ] Swagger UI load được từ máy local
- [ ] Test 1 endpoint (ví dụ `/api/auth/login`) trả về đúng
- [ ] File upload test (nếu có endpoint)

---

## Bước 7 — (Tùy chọn) Tự động restart khi EC2 reboot

Docker `restart: unless-stopped` trong compose đã xử lý việc này. Tuy nhiên cần đảm bảo Docker service tự start:

```bash
sudo systemctl enable docker
```

**Checklist bước 7:**
- [ ] `sudo systemctl is-enabled docker` trả về `enabled`
- [ ] Reboot EC2 và kiểm tra container vẫn chạy sau 2 phút

---

## Tóm tắt checklist toàn bộ

```
[CHUẨN BỊ]
[ ] EC2 t3.micro tạo xong, Security Group mở port 22 + 8080
[ ] Key pair .pem đã lưu
[ ] Storage 20GB

[LOCAL BUILD]
[ ] docker build thành công
[ ] docker push lên Docker Hub xong
[ ] JWT Secret đã generate
[ ] docker-compose.prod.yml đã tạo đúng

[EC2 SETUP]
[ ] Docker đã cài (v24+)
[ ] ca.pem đã copy lên ~/app/
[ ] docker-compose.prod.yml đã copy lên ~/app/

[DEPLOY]
[ ] docker compose pull + up -d thành công
[ ] Logs sạch (không lỗi DB connection)
[ ] curl localhost:8080 trả về response

[VERIFY]
[ ] Swagger UI truy cập được từ browser bên ngoài
[ ] Ít nhất 1 API call test thành công
```

---

## Các rủi ro cần lưu ý với t3.micro

| Vấn đề | Nguyên nhân | Giải pháp |
|---|---|---|
| OOM khi pull image | RAM 1GB, .NET runtime ~300MB | Pull lúc server idle, không có traffic |
| Chậm khi cold start | t3.micro CPU burst | Dùng `T3 Unlimited` nếu cần hoặc restart lúc off-peak |
| Aiven MySQL timeout | Security Group Aiven chưa allow IP EC2 | Vào Aiven dashboard → IP Allow List → thêm Elastic IP của EC2 |
| `ca.pem` sai path | Volume mount sai | Kiểm tra `SslCa=/app/certs/ca.pem` khớp với volume trong compose |

> **Quan trọng về Aiven:** Nếu Aiven MySQL có IP Allow List bật, phải thêm **Elastic IP** của EC2 vào whitelist. Nên gán Elastic IP cho EC2 ngay từ đầu để IP không đổi khi reboot.

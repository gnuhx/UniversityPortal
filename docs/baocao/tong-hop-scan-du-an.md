# Tong hop scan du an University Portal

## 1. Tong quan du an

Du an **University Portal** la he thong cong thong tin dai hoc gom backend ASP.NET Core, frontend React/Vite, co so du lieu MySQL va Docker Compose de dong goi, trien khai.

Ket qua scan toan bo source code:

| Khu vuc | So luong file | Ghi chu |
|---|---:|---|
| Toan bo du an | 238 | Gom backend, frontend, docs, tests, Docker/config |
| `src` | 187 | Backend .NET |
| `frontend` | 36 | React + TypeScript |
| `docs` | 8 | ERD, schema, seed data, Postman, deploy, ke hoach bao cao |
| `tests` | 2 | Project xUnit hien co |

## 2. Cau truc kien truc

Backend duoc to chuc theo huong **Clean Architecture**:

- `UniversityPortal.Domain`: chua entity, enum, exception.
- `UniversityPortal.Application`: chua DTO, service, validator, AutoMapper, interface repository/service.
- `UniversityPortal.Infrastructure`: chua EF Core `AppDbContext`, repository, Unit of Work, JWT service, file service.
- `UniversityPortal.API`: chua controller, middleware, Swagger, JWT authentication, CORS, health check.
- `tests/UniversityPortal.Tests`: project test xUnit.

Quy tac phu thuoc:

```text
API -> Application -> Domain
API -> Infrastructure -> Application + Domain
```

## 3. Cong nghe su dung

Backend:

- ASP.NET Core Web API, target framework `.NET 10`.
- Entity Framework Core + Pomelo MySQL Provider.
- MySQL 8.0.
- JWT Authentication va role-based authorization.
- BCrypt de bam mat khau.
- AutoMapper de map Entity sang DTO.
- FluentValidation de validate request DTO.
- Swagger/OpenAPI de test API.
- Docker de build va chay backend.

Frontend:

- React 19, Vite, TypeScript.
- Ant Design.
- Axios.
- TanStack Query.
- Zustand.
- React Router.
- Nginx de serve ban build production.

## 4. Backend da trien khai

He thong co 9 controller API chinh:

| Controller | Route | Chuc nang |
|---|---|---|
| `AuthController` | `/api/auth` | Dang nhap, refresh token, logout |
| `TaiKhoanController` | `/api/tai-khoan` | Quan ly tai khoan, doi mat khau, upload avatar |
| `SinhVienController` | `/api/sinh-vien` | CRUD sinh vien |
| `GiaoVienController` | `/api/giao-vien` | CRUD giao vien |
| `LopSinhHoatController` | `/api/lop-sinh-hoat` | Quan ly lop sinh hoat |
| `NganhHocController` | `/api/nganh-hoc` | Quan ly nganh hoc |
| `MonHocController` | `/api/mon-hoc` | Quan ly mon hoc |
| `ChuongTrinhDTController` | `/api/chuong-trinh-dt` | Quan ly chuong trinh dao tao |
| `ChiTietCTDTController` | `/api/chi-tiet-ctdt` | Quan ly chi tiet chuong trinh dao tao |

Nhung diem noi bat cua backend:

- Co luong dang nhap JWT va refresh token.
- Co phan quyen theo vai tro nhu `Admin`, `Giao vu`.
- Co `GlobalExceptionMiddleware` de chuan hoa loi.
- Co `ApiResponseDto<T>` va `PagedResultDto<T>` de chuan hoa response.
- Co Repository Pattern va Unit of Work.
- Co EF Core migration va cau hinh entity bang `IEntityTypeConfiguration`.
- Co phuc vu file tinh trong thu muc `/uploads`.
- Co endpoint health check `/health`.

## 5. Co so du lieu va entity nghiep vu

Domain co khoang 30 entity nghiep vu. Cac entity quan trong:

- Nhom tai khoan va phan quyen: `TaiKhoan`, `VaiTro`, `PhongBan`.
- Nhom nguoi dung hoc vu: `SinhVien`, `GiaoVien`.
- Nhom dao tao: `NganhHoc`, `ChuongTrinhDT`, `ChiTietCTDT`, `MonHoc`.
- Nhom lop va hoc phan: `LopSinhHoat`, `LopHocPhan`, `DanhSachLopHP`.
- Nhom diem, hoc phi, hoc ba: `DanhSachThiLai`, `HocBa`, `DiemRenLuyen`, `HocPhi`.
- Nhom thoi gian va thoi khoa bieu: `NamHoc`, `HocKy`, `TuanHoc`, `ThoiKhoaBieu`.
- Nhom thong bao va sinh hoat chu nhiem: `ThongBao`, `ThongBaoDaDoc`, `BinhLuanThongBao`, `BienBanSHCN`, `ChiTietCongViec`, `ChiTietVangSHCN`.
- Nhom hanh chinh va khac: `YeuCauHanhChinh`, `YeuCauSuaDiem`, `DatPhongThucHanh`, `KhaoSatYKien`, `DienDanGiaoVien`, `KetQuaAnhVanDauVao`.

Tai lieu CSDL co san:

- `docs/schema.txt`: mo ta schema chi tiet.
- `docs/erd_schema.html`: so do ERD de chup/xuat anh dua vao bao cao.
- `docs/seed_data.sql`: du lieu mau dung de demo.

## 6. Frontend da trien khai

Frontend la ung dung React that, khong chi la ke hoach. Cac man hinh da co:

- Login.
- Dashboard.
- Quan ly sinh vien.
- Quan ly giao vien.
- Quan ly lop sinh hoat.
- Quan ly tai khoan.
- Quan ly nganh hoc.
- Quan ly mon hoc.
- Quan ly chuong trinh dao tao.
- Quan ly chi tiet chuong trinh dao tao.

Thanh phan quan trong:

- `frontend/src/App.tsx`: khai bao route.
- `frontend/src/store/authStore.ts`: luu access token, refresh token va thong tin user bang Zustand.
- `frontend/src/api/client.ts`: Axios client, tu dong gan Bearer token vao request.
- `frontend/src/components/CrudTable.tsx`: component bang CRUD dung chung.

## 7. Docker va trien khai

File `docker-compose.yml` cau hinh 3 service:

| Service | Cong | Vai tro |
|---|---:|---|
| `frontend` | 3100 | Serve React build bang Nginx |
| `api` | 8080 | ASP.NET Core Web API |
| `mysql` | 3306 | MySQL database |

Backend Dockerfile build project .NET va publish API. Frontend Dockerfile build bang Node, sau do copy `dist` sang Nginx.

## 8. Kiem thu

Project test da duoc tao tai `tests/UniversityPortal.Tests`, su dung xUnit. Tuy nhien hien tai chi co file `UnitTest1.cs` voi test mau rong, chua co test nghiep vu that.

Khi viet bao cao nen ghi trung thuc:

- Da cau hinh project test xUnit.
- Can bo sung unit test cho `AuthService`, cac service CRUD va repository.
- Co the dung `docs/UniversityPortal.postman_collection.json` de kiem thu API thu cong bang Postman.

## 9. Tai lieu nen dung de viet bao cao

- `README.md`: huong dan cai dat, tai khoan demo, du lieu mau.
- `docs/project-plan.md`: tong quan kien truc va ke hoach module.
- `docs/ke-hoach-viet-bao-cao-do-an.md`: khung bao cao do an.
- `docs/schema.txt`: mo ta schema.
- `docs/erd_schema.html`: ERD.
- `docs/seed_data.sql`: du lieu mau.
- `docs/UniversityPortal.postman_collection.json`: Postman collection.
- `docs/deploy-ec2.md`: huong dan deploy EC2.

## 10. Goi y khung bao cao do an

1. **Chuong 1 - Tong quan de tai**: ly do chon de tai, muc tieu, pham vi, doi tuong su dung.
2. **Chuong 2 - Co so ly thuyet va cong nghe**: Clean Architecture, ASP.NET Core, EF Core, MySQL, JWT, React, Docker.
3. **Chuong 3 - Phan tich va thiet ke he thong**: actor, use case, ERD, mo hinh du lieu, kien truc tong the.
4. **Chuong 4 - Trien khai he thong**: backend, frontend, API, authentication, CRUD, Docker.
5. **Chuong 5 - Kiem thu**: Swagger, Postman, test project hien co, ke hoach bo sung test.
6. **Chuong 6 - Ket qua va demo**: tai khoan demo, luong demo, anh Swagger/frontend.
7. **Chuong 7 - Ket luan va huong phat trien**: ket qua dat duoc, han che, huong mo rong.

## 11. Nhan xet tong ket

Repo hien tai du chat lieu de viet bao cao do an theo huong **he thong quan ly cong thong tin dai hoc full-stack**. Phan manh nhat cua du an la kien truc backend ro rang, mo hinh du lieu day du, CRUD loi, frontend quan tri va Docker demo.

Can luu y khi trinh bay: nhieu module nhu hoc phi, thong bao, thoi khoa bieu, thi lai, sinh hoat chu nhiem da co trong Domain va schema, nhung API/service hien tai moi tap trung vao cac module loi nhu auth, tai khoan, sinh vien, giao vien, lop sinh hoat, nganh hoc, mon hoc va chuong trinh dao tao.

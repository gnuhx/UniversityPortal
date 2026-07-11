# Thời khoá biểu — xem lịch học theo học kỳ + quản lý buổi học

**Task:** #03
**Trạng thái:** todo
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Đã kiểm chứng trực tiếp trên mã nguồn: entity `ThoiKhoaBieu`
(`src/UniversityPortal.Domain/Entities/ThoiKhoaBieu.cs`), EF config
(`ThoiKhoaBieuConfiguration.cs`) và bảng `thoi_khoa_bieu` trong
`AppDbContext` **đã tồn tại từ migration `InitialCreate`**, nhưng:

- Không có `IThoiKhoaBieuRepository`/`ThoiKhoaBieuRepository`.
- Không có `IThoiKhoaBieuService`/`ThoiKhoaBieuService`.
- Không có `ThoiKhoaBieuController` — grep toàn bộ `src/` không ra kết quả
  nào ngoài entity/config/DbSet.
- Frontend không có trang, route, hay API module nào cho thời khoá biểu.
- Không có endpoint liệt kê `LopHocPhan` nào ngoài `/me` (chỉ giáo viên
  xem lớp mình dạy) — Admin/Giáo vụ hiện không có cách nào duyệt danh sách
  lớp học phần để gắn buổi học vào.
- Bảng `tuan_hoc` đã có dữ liệu mẫu (20 tuần cho HK1 2024-2025 trong
  `docs/Data/seed_data.sql`) nhưng chưa có API đọc.

Nói cách khác, tính năng thời khoá biểu chưa được triển khai ở bất kỳ
tầng nào ngoài schema DB.

## 2. Mục tiêu

- Sinh viên và Giáo viên xem được lịch học/lịch dạy của mình **dưới dạng
  lịch (calendar)**, chọn được theo từng học kỳ.
- Admin/Giáo vụ có màn quản lý để thêm/sửa/xoá từng buổi học (chọn lớp học
  phần, tuần học, thứ, tiết, phòng học), có kiểm tra trùng phòng/giờ.

## 3. Giải pháp đề xuất

Xây đầy đủ 1 vertical slice mới cho `ThoiKhoaBieu` theo đúng pattern hiện
có của dự án (Repository → Service → Controller → AutoMapper → DTO, ví dụ
tham chiếu `ChiTietCTDTRepository`/`ChiTietCTDTService`), cộng thêm:

- Một endpoint tối thiểu chỉ đọc cho `TuanHoc` (theo đúng pattern
  `PhongBanController` đã dùng ở task #01) để làm dropdown chọn tuần.
- Một endpoint liệt kê `LopHocPhan` có phân trang/lọc theo học kỳ cho
  Admin/Giáo vụ (hiện chưa có), để họ duyệt và chọn lớp cần gắn buổi học.

**Quy ước ngày trong tuần (mới, cần ghi rõ trong code):** `Thu` = 2..8,
trong đó 2 = Thứ Hai … 7 = Thứ Bảy, 8 = Chủ nhật (đúng quy ước hệ thống
đào tạo VN thường dùng). Ngày học thực tế = `TuanHoc.NgayBatDau` (luôn là
thứ Hai, theo dữ liệu mẫu) `.AddDays(Thu - 2)`.

Frontend hiển thị 1 trang `/thoi-khoa-bieu` dùng chung cho mọi vai trò,
nhánh theo role (giống cách `HocPhiPage.tsx`/`ThongBaoPage.tsx` đang làm):
Sinh viên/Giáo viên thấy lịch (antd `Calendar`), Admin/Giáo vụ thấy màn
quản lý (bảng lớp học phần, mở rộng dòng để CRUD buổi học của lớp đó).

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `Application/DTOs/TuanHoc/TuanHocDto.cs` (mới) | Id, MaTuan, SoThuTuTuan, NgayBatDau, NgayKetThuc, TenNamHoc |
| `Application/Interfaces/Repositories/ITuanHocRepository.cs` (mới) | Kế thừa `IRepository<TuanHoc>`, khai báo lại `GetAllAsync` để nạp kèm `NamHoc` |
| `Infrastructure/Repositories/TuanHocRepository.cs` (mới) | `GetAllAsync` có `Include(NamHoc)`, sắp theo năm học rồi số thứ tự tuần |
| `Application/Interfaces/Services/ITuanHocService.cs` + `Services/TuanHocService.cs` (mới) | `GetAllAsync` — chỉ đọc, theo mẫu `PhongBanService` |
| `API/Controllers/TuanHocController.cs` (mới) | `GET /api/tuan-hoc/all` `[Authorize]` — theo mẫu `PhongBanController` |
| `Application/DTOs/ThoiKhoaBieu/ThoiKhoaBieuDto.cs`, `CreateThoiKhoaBieuDto.cs`, `UpdateThoiKhoaBieuDto.cs` (mới) | DTO đầy đủ (kèm `NgayHoc` tính sẵn) + DTO tạo/sửa |
| `Application/Interfaces/Repositories/IThoiKhoaBieuRepository.cs` (mới) | `GetPagedFilterAsync(page,pageSize,lopHpId?)`, `GetDetailAsync(id)`, `GetForSinhVienAsync(sinhVienId,hocKyId?)`, `GetForGiaoVienAsync(giaoVienId,hocKyId?)`, `ExistsConflictAsync(...)` kiểm tra trùng phòng/giờ |
| `Infrastructure/Repositories/ThoiKhoaBieuRepository.cs` (mới) | Cài đặt các query trên với `Include` tới `LopHocPhan.ChiTietCTDT.MonHoc`, `LopHocPhan.HocKy`, `LopHocPhan.GiaoVien.TaiKhoan`, `TuanHoc` |
| `Application/Interfaces/Services/IThoiKhoaBieuService.cs` + `Services/ThoiKhoaBieuService.cs` (mới) | CRUD (validate lớp HP/tuần tồn tại + chặn trùng phòng/giờ) + `GetForSinhVienMeAsync`/`GetForGiaoVienMeAsync` |
| `API/Controllers/ThoiKhoaBieuController.cs` (mới) | `GET /me?hocKyId=` (Sinh viên/Giáo viên, tự phân nhánh theo role), `GET`/`GET {id}`/`POST`/`PUT {id}`/`DELETE {id}` (Admin, Giáo vụ) |
| `Application/Interfaces/IUnitOfWork.cs` + `Infrastructure/UnitOfWork.cs` | Thêm `ThoiKhoaBieus`, `TuanHocs` (lazy init) |
| `Application/Mappings/MappingProfile.cs` | Thêm map `ThoiKhoaBieu → ThoiKhoaBieuDto` (tính `NgayHoc`) và `TuanHoc → TuanHocDto` |
| `Application/DependencyInjection.cs` | Đăng ký `IThoiKhoaBieuService`, `ITuanHocService` |
| `Application/Interfaces/Repositories/ILopHocPhanRepository.cs` + `Infrastructure/Repositories/LopHocPhanRepository.cs` | Thêm `GetPagedFilterAsync(page,pageSize,hocKyId?,keyword?)` có `Include` đầy đủ (giữ nguyên các hàm cũ, không dùng tới) |
| `Application/Interfaces/Services/ILopHocPhanService.cs` + `Services/LopHocPhanService.cs` | Thêm `GetPagedAsync(page,pageSize,hocKyId?,keyword?)` |
| `API/Controllers/LopHocPhanController.cs` | Thêm `GET /api/lop-hoc-phan?hocKyId=&keyword=&page=&pageSize=` `[Authorize(Roles="Admin,Giáo vụ")]` |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/types/index.ts` | Thêm `ThoiKhoaBieu`, `CreateThoiKhoaBieu`, `UpdateThoiKhoaBieu`, `TuanHoc` |
| `frontend/src/api/modules.ts` | Thêm `thoiKhoaBieuApi` (CRUD + `getMe(hocKyId?)`), `tuanHocApi.getAll()`, `lopHocPhanAdminApi.getPaged(params)` |
| `frontend/src/pages/ThoiKhoaBieuPage.tsx` (mới) | Nhánh theo role: Sinh viên/Giáo viên → `Select` học kỳ + antd `Calendar` (`cellRender`) hiển thị buổi học đúng ngày; Admin/Giáo vụ → `Select` học kỳ + bảng lớp học phần, mở rộng dòng (`expandable.expandedRowRender`) để CRUD buổi học của lớp đó (dùng lại `CrudTable`) |
| `frontend/src/App.tsx` | Thêm route `/thoi-khoa-bieu` trong cả 3 khối `ProtectedRoute` (Sinh viên, Giáo viên, Admin/Giáo vụ) — giống cách `/thong-bao`, `/hoc-phi` đang lặp lại |
| `frontend/src/components/AppLayout.tsx` | Thêm mục menu "Thời khoá biểu" (icon `CalendarOutlined`), hiển thị cho cả 4 vai trò |

### Dữ liệu / Migration

Không cần migration — bảng `thoi_khoa_bieu` đã tồn tại. Cần seed thử vài
dòng `thoi_khoa_bieu` (qua UI quản lý mới hoặc script SQL) để có dữ liệu
kiểm thử lịch.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~2.5–3.5 giờ (13 file backend mới/sửa + 4 file frontend, xuyên suốt 1 vertical slice mới) |
| Mức độ rủi ro | Thấp — không đổi entity/schema hiện có, chỉ thêm code đọc/ghi mới lên bảng đã tồn tại sẵn |
| Ảnh hưởng dữ liệu hiện có | Không — bảng `thoi_khoa_bieu` hiện đang rỗng |
| Khả năng rollback | Xoá các file/endpoint mới; không đổi entity nên không cần migration Down |

## 6. Kế hoạch triển khai

1. Thêm `TuanHoc` read-only slice (DTO, repo, service, controller).
2. Thêm `ThoiKhoaBieu` slice đầy đủ (DTO, repo, service, controller) + nối
   `IUnitOfWork`/`UnitOfWork`/`MappingProfile`/`DependencyInjection`.
3. Thêm endpoint liệt kê `LopHocPhan` cho Admin/Giáo vụ.
4. `dotnet build` + `dotnet test`, sửa lỗi nếu có.
5. Frontend: types, `api/modules.ts`.
6. Frontend: `ThoiKhoaBieuPage.tsx` (2 chế độ hiển thị theo role) + route +
   menu.
7. `tsc --noEmit`, chạy thử end-to-end: seed vài buổi học qua màn Admin,
   xem lại đúng ngày trên lịch của Sinh viên/Giáo viên; thử tạo buổi học
   trùng phòng/giờ để kiểm tra chặn trùng.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] `GET /api/tuan-hoc/all` trả về danh sách tuần kèm tên năm học.
- [ ] Admin/Giáo vụ xem được danh sách lớp học phần theo học kỳ và
      thêm/sửa/xoá buổi học (thời khoá biểu) cho từng lớp.
- [ ] Tạo buổi học trùng phòng + trùng tuần/thứ + tiết chồng lấn bị chặn
      với thông báo lỗi rõ ràng.
- [ ] Sinh viên xem `/thoi-khoa-bieu` thấy lịch đúng các buổi học đã đăng
      ký (qua `danh_sach_lop_hp`), lọc đúng theo học kỳ chọn.
- [ ] Giáo viên xem `/thoi-khoa-bieu` thấy lịch đúng các buổi mình dạy.
- [ ] Ngày hiển thị trên lịch khớp `TuanHoc.NgayBatDau + (Thu - 2)` ngày.
- [ ] Build backend + frontend không lỗi, `tsc --noEmit` sạch.

## 8. Ghi chú

- Vì bảng `thoi_khoa_bieu` rỗng ở DB thật, sau khi triển khai cần nhập
  dữ liệu mẫu (qua màn Admin mới) để có gì đó hiển thị lên lịch khi demo.
- Hai hàm `GetPagedByHocKyAsync`/`GetPagedByGiaoVienAsync` hiện có trong
  `ILopHocPhanRepository` không được nơi nào gọi tới (dead code có sẵn từ
  trước) — không đụng tới, ngoài phạm vi task này.

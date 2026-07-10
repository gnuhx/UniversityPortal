# Lớp sinh hoạt — trang chi tiết & lịch sử biên bản sinh hoạt (SHCN) cho sinh viên

**Task:** #24
**Trạng thái:** done
**Ngày tạo:** 2026-07-09
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Admin phản ánh: phía sinh viên, tính năng "Lớp sinh hoạt" (chi tiết và
"log"/lịch sử) hiện chưa tốt. Đã kiểm chứng trong code:

- Sinh viên **không có trang chi tiết lớp sinh hoạt nào cả**.
  `frontend/src/pages/LopSinhHoatPage.tsx` là trang CRUD duy nhất cho
  entity này, và route `/lop-sinh-hoat`
  (`frontend/src/App.tsx:71-76`) chỉ cho phép
  `ROLES.ADMIN, ROLES.GIAO_VU` — Sinh viên (và cả Giáo viên) không
  truy cập được.
- Trên trang hồ sơ của sinh viên
  (`frontend/src/pages/HoSoPage.tsx:51-53`), lớp sinh hoạt chỉ hiện
  như **1 dòng text** (`tenLop`) — không phải link, không xem được
  GVCN, thư ký, danh sách bạn cùng lớp, dù `SinhVienDto`
  (`src/UniversityPortal.Application/DTOs/SinhVien/SinhVienDto.cs`) đã
  có sẵn `lopId` để liên kết.
- `LopSinhHoatDto` hiện chỉ có `SoSinhVien` (đếm số lượng) — **chưa hề
  có API nào trả về danh sách bạn cùng lớp** (roster), kể cả khi có
  trang chi tiết thì cũng chưa có dữ liệu để hiển thị.
- Endpoint `GET /api/lop-sinh-hoat/{id}`
  (`LopSinhHoatController.cs:57-62`) chỉ có `[Authorize]` trần, không
  có kiểm tra quyền sở hữu — về lý thuyết 1 sinh viên có thể gọi để
  xem chi tiết **bất kỳ lớp nào**, không riêng lớp của mình. Cần lưu ý
  khi thiết kế endpoint mới cho sinh viên (nên dùng dạng `/me`, không
  nhận `id` từ client).
- **"Log" (lịch sử sinh hoạt lớp) chưa được xây dựng ở bất kỳ đâu**:
  `BienBanSHCN` (biên bản sinh hoạt chủ nhiệm — thời gian, địa điểm,
  nội dung, phản hồi GVCN) cùng 2 bảng con `ChiTietCongViec` (công
  việc trong buổi sinh hoạt) và `ChiTietVangSHCN` (điểm danh vắng từng
  sinh viên, có phép/không phép, lý do) đã có entity + EF config +
  migration (`20260704111719_AddNewFeatureTables`) nhưng **hoàn toàn
  chưa có Repository/Service/Controller/DTO/trang frontend nào** — xác
  nhận qua grep toàn repo, chỉ xuất hiện trong `AppDbContext.cs` và
  file cấu hình EF. Đây là dữ liệu chưa từng được ai (kể cả
  Admin/GVCN) tạo hay xem qua API.
- GVCN (Giáo viên chủ nhiệm) hiện **không có công cụ quản lý lớp sinh
  hoạt mình phụ trách**: `GiaoVienController` không có endpoint
  `/me`; `LopSinhHoatController.GetPaged`
  (`LopSinhHoatController.cs:32-42`) hỗ trợ lọc `gvcnId` nhưng chỉ
  `[Authorize(Roles = "Admin,Giáo vụ")]` — Giáo viên không gọi được.
  Route `/lop-hoc-phan` là trang duy nhất Giáo viên có, không liên
  quan lớp sinh hoạt.
- `DienDanGiaoVien` (diễn đàn giáo viên gắn với 1 lớp sinh hoạt) cũng
  ở tình trạng y hệt `BienBanSHCN` — chỉ có entity, chưa có gì khác.
  Nội dung do giáo viên viết, chưa rõ có nên hiển thị cho sinh viên
  hay không → xem mục 8, **không nằm trong phạm vi tài liệu này**.

Kết luận quan trọng: đây là tính năng **gần như xây từ đầu** (green-field)
cho cả 2 phía — không có sẵn "backend đã có, chỉ thiếu UI sinh viên"
như một số task trước; write-side (GVCN tạo biên bản) và read-side
(sinh viên xem) đều chưa tồn tại.

## 2. Mục tiêu

- Sinh viên xem được **trang chi tiết lớp sinh hoạt của mình**: mã
  lớp, GVCN, thư ký, ngành/CTĐT/khoá học, và **danh sách bạn cùng lớp**
  (MSSV + họ tên).
- Sinh viên xem được **lịch sử các buổi sinh hoạt chủ nhiệm** (biên
  bản SHCN) của lớp mình: thời gian, địa điểm, nội dung, danh sách
  công việc, phản hồi của GVCN — và **tình trạng vắng/có phép của
  chính mình** trong từng buổi (không xem được lý do vắng của bạn
  khác — xem mục 8 về quyết định riêng tư này).
- GVCN tạo/xem được biên bản SHCN cho **đúng lớp mình chủ nhiệm** —
  gồm nội dung buổi họp, danh sách công việc, và điểm danh vắng cho
  từng sinh viên trong lớp (write-side tối thiểu để có dữ liệu cho
  sinh viên xem — không có cái này thì không có gì để hiển thị).

**Ngoài phạm vi:**
- `DienDanGiaoVien` (diễn đàn giáo viên) — chưa rõ yêu cầu hiển thị
  cho sinh viên, để task riêng sau khi xác nhận với Admin (mục 8).
- Sửa/xoá biên bản đã tạo, thống kê tỷ lệ vắng, export biên bản ra
  file — không nằm trong phạm vi bản đầu tiên này.

## 3. Giải pháp đề xuất

> Cập nhật sau khi code xong: kiến trúc tổng thể giữ đúng đề xuất gốc.
> 2 khác biệt so với đề xuất ban đầu, xem lý do ở mục 8: (1) không tạo
> `CreateBienBanSHCNValidator` FluentValidation — dự án này đăng ký
> validator qua DI nhưng **không nơi nào thực sự gọi** chúng (không có
> `AddFluentValidationAutoValidation`, không controller nào inject
> `IValidator<T>`), nên các kiểm tra được đặt trực tiếp trong
> `BienBanSHCNService` (check-then-throw), đúng convention thực tế của
> dự án; (2) endpoint GVCN gộp thành `GET/POST /api/bien-ban-shcn/me-gvcn`
> thay vì tách `GetPagedByLopAsync` riêng — dùng chung 1 helper kiểm
> tra sở hữu `GetLopOwnedByGvcnAsync` để tránh lặp code.

### Backend

**A. Chi tiết lớp + roster cho sinh viên** (dùng pattern `/me` sẵn có,
ví dụ `sinhVienMeApi`, `danhSachLopHPApi.getMe` trong
`frontend/src/api/modules.ts`):

1. Thêm `GET /api/lop-sinh-hoat/me` trong `LopSinhHoatController`
   (`[Authorize(Roles = "Sinh viên")]`) — lấy `LopId` từ hồ sơ sinh
   viên hiện tại (theo `taiKhoanId` claim, cùng cách
   `SinhVienService.GetMeAsync` đang làm), trả 404/thông báo rõ nếu
   sinh viên chưa được phân lớp (giữ nguyên UX hiện có ở
   `NganhHocPage.tsx` "Chưa xác định được ngành học..." cho trường hợp
   tương tự).
2. DTO mới `LopSinhHoatChiTietDto` (kế thừa field của
   `LopSinhHoatDto` hiện có + thêm `DanhSachSinhVien: List<{ Id, Mssv,
   HoTen }>`) — hoặc mở rộng trực tiếp `LopSinhHoatDto` nếu không muốn
   thêm DTO mới (roster không nhạy cảm, có thể dùng chung cho cả
   Admin/GVCN/SV).
3. `ILopSinhHoatRepository`/`LopSinhHoatRepository`: thêm
   `GetDetailWithRosterAsync(id)` — `.Include(SinhViens).ThenInclude(TaiKhoan)`
   để lấy `HoTen` (roster chỉ cần Mssv + HoTen, không cần Email/thông
   tin khác).

**B. GVCN xem "lớp mình chủ nhiệm"** (còn thiếu hoàn toàn — cần cho cả
GVCN write-side lẫn để SV biết ai là GVCN):

4. Thêm `GET /api/lop-sinh-hoat/me-gvcn` (`[Authorize(Roles = "Giáo
   viên")]`) — trả danh sách lớp mà giáo viên hiện tại là `GvcnId`
   (dùng lại `GetPagedFilterAsync`/logic lọc `gvcnId` đã có, chỉ khác
   ở nguồn `gvcnId` lấy từ claim thay vì query param, và role cho
   phép).

**C. Vertical `BienBanSHCN` — hoàn toàn mới:**

5. `IBienBanSHCNRepository`/`BienBanSHCNRepository`:
   - `GetPagedByLopAsync(lopId, page, pageSize)` — `.Include(ChiTietCongViecs)`,
     `.Include(ChiTietVangSHCNs)`, `.Include(TuanHoc)`, `.Include(ThuKy.TaiKhoan)`,
     sắp xếp theo `ThoiGian` giảm dần.
   - `GetDetailAsync(id)` — include đầy đủ như trên.
   - `AddAsync` xử lý tạo `BienBanSHCN` kèm children
     (`ChiTietCongViecs`, `ChiTietVangSHCNs`) trong 1 transaction (theo
     pattern `CommitAsync` của `IUnitOfWork`).
6. DTOs (`src/UniversityPortal.Application/DTOs/BienBanSHCN/`):
   - `BienBanSHCNDto` (cho GVCN/Admin — đầy đủ `ChiTietVangSHCNs` của
     cả lớp: từng SV + `CoPhep` + `LyDo`).
   - `BienBanSHCNSinhVienDto` (cho SV — **không có** danh sách vắng
     của người khác, chỉ có field riêng `TinhTrangCuaToi: "CoMat" |
     "VangCoPhep" | "VangKhongPhep"` + `LyDoVangCuaToi`).
   - `CreateBienBanSHCNDto` — `TuanHocId`, `ThoiGian`, `DiaDiem`,
     `ThuKyId`, `NoiDung`, `List<CongViecItem { TenCongViec }>`,
     `List<DiemDanhItem { SinhVienId, CoPhep, LyDo }>` (SV không có
     trong danh sách vắng = coi như có mặt).
7. `BienBanSHCNService`:
   - GVCN/Admin: `GetPagedByLopAsync`, `GetDetailAsync`, `CreateAsync`
     — **kiểm tra quyền sở hữu**: nếu role Giáo viên, `lop.GvcnId`
     phải bằng giáo viên hiện tại, nếu không ném
     `ForbiddenException`/`BadRequestException` phù hợp với exception
     pattern hiện có (`Domain/Exceptions/`).
   - SV: `GetMeAsync(taiKhoanId)` — lấy `LopId` của SV hiện tại, gọi
     `GetPagedByLopAsync`, map sang `BienBanSHCNSinhVienDto` (ẩn vắng
     người khác, chỉ tính riêng bản ghi `ChiTietVangSHCN` của chính
     SV này).
8. `BienBanSHCNController` (`api/bien-ban-shcn`):
   - `GET /api/bien-ban-shcn?lopId=&page=&pageSize=`
     `[Authorize(Roles = "Admin,Giáo vụ,Giáo viên")]`.
   - `GET /api/bien-ban-shcn/{id}` — tương tự, kèm check sở hữu ở
     service.
   - `POST /api/bien-ban-shcn` `[Authorize(Roles = "Admin,Giáo viên")]`.
   - `GET /api/bien-ban-shcn/me` `[Authorize(Roles = "Sinh viên")]`.
9. `IUnitOfWork`: thêm `IBienBanSHCNRepository BienBanSHCNs { get; }`.
10. `MappingProfile`: thêm `CreateMap<BienBanSHCN, BienBanSHCNDto>` và
    `CreateMap<BienBanSHCN, BienBanSHCNSinhVienDto>` (dùng
    `ForMember`/custom resolver để tính `TinhTrangCuaToi` — resolver
    cần biết `sinhVienId` hiện tại, nên có thể cần map thủ công trong
    service thay vì thuần AutoMapper cho riêng field này).

### Frontend

11. Trang lớp sinh hoạt tự phân nhánh theo vai trò (giống
    `ThongBaoPage.tsx`/`ThoiKhoaBieuPage.tsx`) thay vì 3 trang riêng —
    mở route `/lop-sinh-hoat` cho cả 3 vai trò
    (`App.tsx:71`: đổi `allowedRoles` từ `[ADMIN, GIAO_VU]` thành
    `[ADMIN, GIAO_VU, GIAO_VIEN, SINH_VIEN]`):
    - **Admin/Giáo vụ**: giữ nguyên `CrudTable` hiện có.
    - **Giáo viên**: danh sách lớp mình chủ nhiệm
      (`lopSinhHoatApi.getMeGvcn()`) → chọn 1 lớp → xem danh sách biên
      bản đã tạo + nút "Tạo biên bản sinh hoạt" mở modal (chọn Tuần
      học, Thời gian, Địa điểm, Thư ký, Nội dung, danh sách Công việc
      thêm/xoá động, bảng điểm danh cả lớp: mỗi SV có Switch "Vắng" →
      hiện thêm Checkbox "Có phép" + Input "Lý do").
    - **Sinh viên**: `Descriptions` chi tiết lớp (mã lớp, GVCN, thư
      ký, ngành/CTĐT/khoá học) + `Table` roster bạn cùng lớp + `List`/
      `Timeline` các biên bản sinh hoạt (thời gian, địa điểm, nội dung
      rút gọn, badge tình trạng của mình), click vào 1 mục mở Modal
      xem chi tiết đầy đủ (nội dung, danh sách công việc, phản hồi
      GVCN).
12. `frontend/src/api/modules.ts`: thêm `bienBanShcnApi` (hand-written,
    theo mẫu `danhSachLopHPApi`) với `getPagedByLop`, `getById`,
    `create`, `getMe`; mở rộng `lopSinhHoatApi` thêm `getMe()` và
    `getMeGvcn()`.
13. `frontend/src/types/index.ts`: thêm `BienBanSHCN`,
    `BienBanSHCNSinhVien`, `CreateBienBanSHCN`, mở rộng `LopSinhHoat`
    với field roster (`danhSachSinhVien?: { id, mssv, hoTen }[]`).

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.API/Controllers/LopSinhHoatController.cs` | Thêm `GET /me` (Sinh viên), `GET /me-gvcn` (Giáo viên) |
| `src/UniversityPortal.Application/Services/LopSinhHoatService.cs` | Thêm `GetMeAsync(taiKhoanId)`, `GetMeGvcnAsync(taiKhoanId)` |
| `src/UniversityPortal.Application/DTOs/LopSinhHoat/LopSinhHoatDto.cs` | Thêm field roster (`DanhSachSinhVien`) hoặc DTO `LopSinhHoatChiTietDto` riêng |
| `src/UniversityPortal.Infrastructure/Repositories/LopSinhHoatRepository.cs` | Thêm `GetDetailWithRosterAsync`, `GetByGvcnAsync` |
| `src/UniversityPortal.Application/Interfaces/Repositories/IBienBanSHCNRepository.cs` | **Mới** — `GetPagedByLopAsync`, `GetDetailAsync`, `AddAsync` (kèm children) |
| `src/UniversityPortal.Infrastructure/Repositories/BienBanSHCNRepository.cs` | **Mới** |
| `src/UniversityPortal.Application/Interfaces/Services/IBienBanSHCNService.cs` | **Mới** |
| `src/UniversityPortal.Application/Services/BienBanSHCNService.cs` | **Mới** — kiểm tra sở hữu GVCN, tính `TinhTrangCuaToi` cho SV |
| `src/UniversityPortal.Application/DTOs/BienBanSHCN/*.cs` | **Mới** — `BienBanSHCNDto`, `BienBanSHCNSinhVienDto`, `CreateBienBanSHCNDto` |
| `src/UniversityPortal.API/Controllers/BienBanSHCNController.cs` | **Mới** |
| `src/UniversityPortal.Application/Mappings/MappingProfile.cs` | Thêm mapping cho `BienBanSHCN` |
| `src/UniversityPortal.Application/Interfaces/IUnitOfWork.cs` | Thêm `IBienBanSHCNRepository BienBanSHCNs` |
| `src/UniversityPortal.Application/DependencyInjection.cs` | Đăng ký `IBienBanSHCNService` |

Không thêm FluentValidation validator — xem lý do ở đầu mục 3.

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/pages/LopSinhHoatPage.tsx` | Tự phân nhánh theo vai trò (Admin/GiaoVu giữ CrudTable; thêm `LopSinhHoatGvcn` và `LopSinhHoatCuaToi`; thêm `TaoBienBanModal`, `BienBanGvcnTable`, `ChiTietBienBanModal` dùng chung) |
| `frontend/src/App.tsx` | Route `/lop-sinh-hoat` mở thêm cho `GIAO_VIEN`, `SINH_VIEN` |
| `frontend/src/components/AppLayout.tsx` | Menu "Lớp sinh hoạt" chuyển từ mục Admin/Giáo vụ sang mục "Tất cả roles" (thêm `GIAO_VIEN`, `SINH_VIEN`) |
| `frontend/src/api/modules.ts` | Thêm `bienBanShcnApi`; mở rộng `lopSinhHoatApi` với `getMe()`, `getMeGvcn()` |
| `frontend/src/types/index.ts` | Thêm type `BienBanSHCN`, `BienBanSHCNSinhVien`, `CreateBienBanSHCN`, `LopSinhHoatThanhVien`; mở rộng `LopSinhHoat` với `danhSachSinhVien` |
| `frontend/src/pages/HoSoPage.tsx` | Đổi dòng "Lớp sinh hoạt" từ text thành link sang `/lop-sinh-hoat` |

### Dữ liệu / Migration

Không cần migration mới — bảng `bien_ban_shcn`, `chi_tiet_cong_viec`,
`chi_tiet_vang_shcn` đã có sẵn từ migration
`20260704111719_AddNewFeatureTables`, chỉ chưa có tầng ứng dụng phía
trên. Cần seed thử ít nhất 1 vài bản ghi `bien_ban_shcn` (qua chính
API mới, không cần seed SQL) để có dữ liệu kiểm thử — lớp hiện có
trong DB dev chưa có `TuanHoc` nào trong đúng khoảng khoá học, cần
kiểm tra tồn tại `TuanHoc` phù hợp trước khi test tạo biên bản.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~2-3 ngày — vertical hoàn toàn mới (repo/service/controller/DTO/validator + 3 chế độ hiển thị frontend theo vai trò), lớn hơn nhiều so với task #23 |
| Mức độ rủi ro | Trung bình — rủi ro chính là kiểm soát quyền sở hữu (GVCN chỉ được tạo/xem biên bản của đúng lớp mình chủ nhiệm; sinh viên chỉ xem lớp/vắng của chính mình, không rò rỉ lý do vắng của bạn khác) |
| Ảnh hưởng dữ liệu hiện có | Không — chỉ thêm mới, không sửa bảng/dữ liệu hiện có |
| Khả năng rollback | Revert các file mới (an toàn vì đây là các file/route hoàn toàn mới); phần sửa `App.tsx`/`LopSinhHoatPage.tsx`/`HoSoPage.tsx` cũng dễ revert vì chỉ mở thêm route và đổi text→link |

## 6. Kế hoạch triển khai

1. ~~Backend — phần A + B: `GET /api/lop-sinh-hoat/me` (chi tiết lớp +
   roster) và `GET /api/lop-sinh-hoat/me-gvcn` (lớp GVCN phụ trách).~~
   Đã làm.
2. ~~Backend — phần C: dựng vertical `BienBanSHCN` đầy đủ (repository →
   service → DTO → controller → `IUnitOfWork`/DI registration →
   `MappingProfile`).~~ Đã làm — không tạo validator riêng, xem mục 8.
3. ~~Frontend: `bienBanShcnApi`, mở rộng `lopSinhHoatApi`, cập nhật
   `types/index.ts`.~~ Đã làm.
4. ~~Frontend: viết lại `LopSinhHoatPage.tsx` tự phân nhánh 3 vai trò;
   mở route ở `App.tsx`; sửa `HoSoPage.tsx` thành link; cập nhật menu
   ở `AppLayout.tsx`.~~ Đã làm.
5. ~~`dotnet build` + `dotnet test` + `tsc --noEmit`.~~ Cả 3 sạch.
6. ~~Kiểm thử trên trình duyệt (dữ liệu thật).~~ Đã làm — xem bằng
   chứng chi tiết ở mục 8, phát hiện và sửa 2 bug include-chain có
   thật trong lúc kiểm thử (không phải giả định).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Sinh viên vào `/lop-sinh-hoat` xem được: mã lớp, GVCN, thư ký,
      ngành/CTĐT/khoá học, danh sách bạn cùng lớp (MSSV + họ tên) —
      kiểm chứng với tài khoản `sv.dung` (lớp HTTT23A, 7 bạn cùng lớp).
- [x] Sinh viên xem được danh sách biên bản SHCN của lớp mình (thời
      gian, địa điểm, nội dung, công việc, phản hồi GVCN) và tình
      trạng vắng/có mặt của chính mình trong từng buổi — kiểm chứng cả
      2 trường hợp: `sv.dung` (bị đánh vắng) thấy tag "Vắng không phép"
      kèm lý do; `sv.em` (không bị đánh vắng, cùng lớp, cùng biên bản)
      thấy tag "Có mặt".
- [x] Sinh viên **không** xem được lý do vắng của bạn cùng lớp khác —
      xác nhận modal chi tiết phía sinh viên không có mục "Danh sách
      vắng" (kiểm tra cả bằng mắt lẫn bằng script: `getByText("Danh
      sách vắng").isVisible()` → `false`).
- [x] Sinh viên **không** xem được chi tiết lớp sinh hoạt khác lớp
      mình — endpoint `/me` tự suy ra lớp từ tài khoản đăng nhập,
      không nhận `id`/`lopId` từ client nên không có cách nào chọn lớp
      khác.
- [x] GVCN xem được (các) lớp mình chủ nhiệm và tạo được biên bản SHCN
      mới kèm công việc + điểm danh vắng cho cả lớp — kiểm chứng với
      `gv.hoa` (GVCN lớp HTTT23A): tạo biên bản thành công (`201`),
      hiện đúng trong bảng "Biên bản đã tạo", xem chi tiết thấy đầy đủ
      "Danh sách vắng" (khác với view sinh viên).
- [x] GVCN không tạo được biên bản cho lớp không phải mình chủ nhiệm
      (trả lỗi rõ ràng, không phải 500) — kiểm chứng bằng curl:
      `gv.hoa` (GVCN lớp 2) gọi `POST /api/bien-ban-shcn` với
      `lopId: 1` (lớp của GVCN khác) → `403 Forbidden`,
      `"Bạn không phải là GVCN của lớp này."`; tương tự cho
      `GET /api/bien-ban-shcn/me-gvcn?lopId=1`.
- [x] Trang `/lop-sinh-hoat` cho Admin/Giáo vụ hoạt động y như cũ —
      kiểm chứng: bảng vẫn hiện đủ 2 lớp (KTPM22A, HTTT23A) với đúng
      GVCN/Thư ký/CTĐT/Số sinh viên sau khi thêm phân nhánh vai trò.
- [x] `dotnet build`, `dotnet test` và `tsc --noEmit` sạch.
- [x] Đã kiểm thử trên trình duyệt với dữ liệu thật (DB SQL Server dev
      thật, không mock) theo đúng kịch bản mục 6, dùng Playwright điều
      khiển Chrome thật cho cả 2 phía GVCN và Sinh viên, cộng thêm
      probe quyền sở hữu qua `curl` trực tiếp API.

## 8. Ghi chú

- **Quyết định thiết kế cần Admin xác nhận — ẩn lý do vắng của bạn
  khác với sinh viên**: đề xuất mặc định là sinh viên chỉ thấy tình
  trạng của chính mình (không thấy ai vắng/lý do gì trong lớp) vì lý
  do vắng có thể nhạy cảm (ốm, việc gia đình...). Nếu Admin muốn sinh
  viên thấy được **danh sách vắng của cả lớp** (ví dụ để biết ai vắng
  hôm đó mà không cần hỏi), cần đổi `BienBanSHCNSinhVienDto` thành trả
  đầy đủ `ChiTietVangSHCNs` như bản GVCN — ảnh hưởng thiết kế DTO ở
  mục 3 phần C.
- **`DienDanGiaoVien` ngoài phạm vi**: entity này (diễn đàn giáo viên
  gắn với lớp sinh hoạt, nội dung do `GiaoVienId` viết) có cùng tình
  trạng "chỉ có model, chưa có gì khác" như `BienBanSHCN`. Không rõ
  đây có phải là điều Admin muốn nói tới khi nhắc "log" hay không —
  nếu có, cần làm rõ: đây là kênh giáo viên trao đổi với nhau (không
  phải với sinh viên) hay là kênh giáo viên thông báo cho sinh viên
  trong lớp? Tài liệu này **chỉ xử lý `BienBanSHCN`** (biên bản sinh
  hoạt chủ nhiệm) vì tên gọi và cấu trúc dữ liệu (điểm danh, công
  việc, phản hồi GVCN) khớp rõ nhất với khái niệm "log" của 1 lớp sinh
  hoạt.
- **GVCN hiện chưa có bất kỳ trang nào** ngoài `/lop-hoc-phan` (dành
  cho giảng dạy lớp học phần, không phải lớp sinh hoạt) — việc mở
  route `/lop-sinh-hoat` cho vai trò Giáo viên là **lối vào đầu tiên**
  của GVCN tới dữ liệu lớp chủ nhiệm trong toàn bộ hệ thống, không chỉ
  riêng cho tính năng biên bản này — cần lưu ý khi thiết kế layout vì
  đây có thể là nền tảng cho các tính năng GVCN khác sau này (ví dụ
  `DienDanGiaoVien` nếu được làm tiếp).
- **`TuanHocId` bắt buộc trên `BienBanSHCN`**: mỗi biên bản gắn với 1
  `TuanHoc` cụ thể (tuần học, có `NamHocId`). Cần kiểm tra dữ liệu
  dev/test đã có đủ `TuanHoc` cho năm học hiện tại chưa trước khi demo
  — nếu chưa, phải tạo qua trang "Năm học/Học kỳ" (task #07) trước.
- **Không đổi endpoint `GET /api/lop-sinh-hoat/{id}` hiện có** (dùng
  cho Admin) — endpoint mới cho sinh viên là `/me` riêng biệt, tự suy
  ra lớp từ tài khoản đăng nhập, không nhận `id` từ client, nên tránh
  hẳn được lỗ hổng ownership đã nêu ở mục 1 mà không cần sửa logic
  endpoint cũ.

### Cập nhật sau khi triển khai (2026-07-09 → 2026-07-10)

**2 bug có thật phát hiện khi kiểm thử trên trình duyệt (đã sửa trong
cùng task này, không phải hồi quy do task #23 vì tồn tại từ trước đó):**

1. `LopSinhHoatRepository.GetDetailAsync` (dùng bởi cả `GET
   /api/lop-sinh-hoat/{id}` phía Admin lẫn `/me`/`/me-gvcn` mới thêm)
   chỉ `.Include(x => x.ChuongTrinhDT)` — thiếu `.ThenInclude(c =>
   c.Nganh).ThenInclude(n => n.PhongBan)` — khiến `TenNganh`/
   `TenPhongBan` luôn trả về rỗng dù `NganhId` có giá trị đúng. Đã sửa
   bằng cách thêm `ThenInclude` tương tự như `GetAllDetailAsync` (task
   #23) vào cả `GetDetailAsync` và `GetByGvcnIdAsync`.
2. `LopSinhHoatRepository.GetPagedFilterAsync` (dùng bởi trang Admin
   CRUD) chỉ `.Include(x => x.SinhViens)` — thiếu `.ThenInclude(sv =>
   sv.TaiKhoan)` — khiến field `DanhSachSinhVien[].HoTen` mới thêm
   luôn rỗng ở màn hình Admin dù `Mssv` vẫn đúng (không ảnh hưởng UI
   Admin hiện tại vì `LopSinhHoatPage.tsx` không render roster ở view
   Admin, nhưng dữ liệu trả về qua API vẫn sai/không nhất quán so với
   `/me`). Đã thêm `ThenInclude` tương tự.

Cả 2 đều được phát hiện bằng cách so sánh dữ liệu thật qua `curl` (API
trả `tenNganh: ""` dù DB có `nganh_id` hợp lệ) trước khi tin vào giao
diện — không phải suy đoán.

**Không tạo `CreateBienBanSHCNValidator`** như phác thảo ban đầu ở mục
3/4: kiểm tra thực tế cho thấy dự án đăng ký toàn bộ FluentValidation
validator qua `AddValidatorsFromAssembly` nhưng **không có middleware
hay controller nào thực sự gọi chúng** (`grep -rn "IValidator<"` toàn
bộ `src/` không ra kết quả nào ngoài registration) — các validator
hiện có trong repo là scaffold chưa từng được wiring, việc validate
thật sự luôn nằm ở tầng Service (check-then-throw). Thêm 1 validator
nữa sẽ chỉ là dead code giống các validator khác — nên các kiểm tra
(`NoiDung`/`DiaDiem` không rỗng, `TuanHocId`/`ThuKyId`/`SinhVienId`
phải thuộc đúng lớp) được đặt thẳng trong `BienBanSHCNService.CreateAsync`.

**Bằng chứng kiểm thử trên trình duyệt** (Playwright + Chrome thật, DB
SQL Server dev thật qua `appsettings.Development.json`, không mock):
1. GVCN (`gv.hoa`, chủ nhiệm lớp HTTT23A) đăng nhập → vào
   `/lop-sinh-hoat` → thấy đúng lớp mình chủ nhiệm → mở modal "Tạo
   biên bản sinh hoạt", chọn Tuần học/Thời gian/Địa điểm/Thư ký, nhập
   nội dung, thêm 1 công việc, đánh dấu 1 sinh viên (Phạm Thị Dung)
   vắng không phép kèm lý do → submit → `201 Created`, toast "Tạo biên
   bản sinh hoạt thành công", bảng "Biên bản đã tạo" cập nhật ngay,
   xem chi tiết thấy đầy đủ bảng "Danh sách vắng" với đúng lý do.
2. Sinh viên `sv.dung` (chính là người bị đánh vắng) đăng nhập → vào
   `/lop-sinh-hoat` → thấy đúng thông tin lớp (kể cả "Ngành: Hệ thống
   Thông tin" sau khi sửa bug #1), roster 7 bạn cùng lớp, và biên bản
   vừa tạo với tag đỏ "Vắng không phép"; mở chi tiết thấy dòng "Tình
   trạng của bạn: Vắng không phép — Lý do: Bị ốm, có giấy xin phép" —
   nhưng **không** có bảng "Danh sách vắng" (xác nhận cả bằng mắt và
   bằng `getByText("Danh sách vắng").isVisible() === false`).
3. Sinh viên `sv.em` (cùng lớp, cùng biên bản, nhưng không bị đánh
   vắng) đăng nhập → thấy đúng cùng biên bản đó nhưng với tag xanh "Có
   mặt" — xác nhận field `TinhTrangCuaToi` tính đúng theo từng sinh
   viên xem, không phải giá trị tĩnh.
4. Probe quyền sở hữu qua `curl`: `gv.hoa` (GVCN lớp 2) gọi
   `GET /api/bien-ban-shcn/me-gvcn?lopId=1` và
   `POST /api/bien-ban-shcn` với `lopId: 1` (lớp của GVCN khác, TS.
   Phạm Văn Minh) → cả 2 đều `403 Forbidden`,
   `"Bạn không phải là GVCN của lớp này."` — đúng thiết kế, không lộ
   dữ liệu lớp khác.
5. Trang Admin `/lop-sinh-hoat` (CrudTable cũ) kiểm tra lại sau khi
   thêm phân nhánh vai trò — vẫn hiện đúng 2 lớp với đầy đủ GVCN/Thư
   ký/CTĐT/Số sinh viên, không bị ảnh hưởng.
6. Trong lúc kiểm thử có gặp 2 lần lỗi môi trường không liên quan đến
   code: (a) 1 lần `SqlException: Execution Timeout Expired` khi gọi
   `GET /api/lop-sinh-hoat/{id}` — chỉ là timeout tạm thời của DB dev
   từ xa (site4now.net), thử lại ngay sau đó thành công trong 0.4s;
   (b) vài lần khởi động lại backend bị quên biến môi trường
   `ASPNETCORE_ENVIRONMENT=Development` khiến ứng dụng đọc nhầm
   connection string rỗng từ `appsettings.json` — lỗi thao tác dev-loop
   của phiên làm việc, không phải bug của tính năng.

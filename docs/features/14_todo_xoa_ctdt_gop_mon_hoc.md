# Xoá Chương trình đào tạo: tự xoá liên kết Môn học, vẫn chặn khi có lớp sinh hoạt/lớp học phần thật

**Task:** #14
**Trạng thái:** todo
**Ngày tạo:** 2026-07-06
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Hiện tại `ChuongTrinhDTService.DeleteAsync`
(`src/UniversityPortal.Application/Services/ChuongTrinhDTService.cs:82-96`)
**luôn chặn xoá** một Chương trình đào tạo (CTĐT) nếu còn **bất kỳ** dòng
`chi_tiet_ctdt` (môn học đã gán vào CTĐT đó, theo học kỳ) — dù CTĐT đó
**chưa từng được dùng thật** (chưa có lớp sinh hoạt, chưa mở lớp học phần
nào). Điều này khiến Admin không xoá được một CTĐT vừa tạo nhầm/cần làm lại,
chỉ vì đã lỡ thêm vài môn học vào chương trình.

Điều đáng chú ý: ở tầng CSDL, khoá ngoại `chi_tiet_ctdt.ctdt_id →
chuong_trinh_dt.id` **đã được cấu hình `Cascade`**
(`ChiTietCTDTConfiguration.cs:21`:
`.HasForeignKey(x => x.CtdtId).OnDelete(DeleteBehavior.Cascade)`) — nghĩa là
CSDL vốn đã được thiết kế để tự xoá "liên kết Môn học" khi xoá CTĐT. Guard ở
tầng service hiện đang **chặn cứng**, vô hiệu hoá tính năng cascade này thay
vì tận dụng nó.

Ngoài ra, code hiện có 1 dòng thừa/chưa hoàn chỉnh:
```csharp
var coLop = await uow.LopSinhHoats.GetPagedFilterAsync(1, 1, null, null);
```
Biến `coLop` được gán nhưng **không hề dùng** ở đâu — nhìn cách gọi (không lọc
theo CTDT) và tên biến, đây có vẻ là ý định kiểm tra "còn Lớp sinh hoạt nào
dùng CTĐT này không" nhưng chưa viết xong (thiếu tham số lọc theo CTDT, thiếu
`if` kiểm tra kết quả). Hiện tại xoá CTĐT **không hề kiểm tra** ràng buộc với
`lop_sinh_hoat` (`lop_sinh_hoat.chuong_trinh_dt_id` cũng là **Restrict** —
xem `LopSinhHoatConfiguration.cs`) hay với `lop_hoc_phan` (mở lớp học phần
cho môn học trong CTĐT này, cũng Restrict qua `chi_tiet_ctdt_id`) — nếu 1
trong 2 loại này tồn tại, xoá CTĐT vẫn sẽ vỡ ra lỗi SQL thô ở tầng DB (đúng
kiểu lỗi mà người yêu cầu từng gặp ở task #12 cho Học kỳ), vì cascade tới
`chi_tiet_ctdt` sẽ bị chặn lại bởi các Restrict phía sau nó.

## 2. Mục tiêu

- Xoá 1 CTĐT: nếu CTĐT đó **chưa được dùng thật** (chưa có lớp sinh hoạt nào,
  chưa có lớp học phần nào mở cho môn học của nó) → xoá thành công, **tự động
  xoá kèm toàn bộ liên kết Môn học** (`chi_tiet_ctdt`) của CTĐT đó — đúng như
  yêu cầu, tận dụng cascade đã có sẵn ở CSDL.
- Nếu CTĐT **đã được dùng thật** (có lớp sinh hoạt và/hoặc lớp học phần) →
  vẫn chặn xoá, nhưng báo rõ ràng loại nào đang chặn + số lượng + nơi xử lý
  (theo đúng mẫu thông báo đã làm ở task #12 cho Học kỳ), thay vì lỗi SQL thô
  hoặc thông báo chung chung "còn chi tiết môn học liên kết" như hiện tại.

## 3. Giải pháp đề xuất

Viết lại `ChuongTrinhDTService.DeleteAsync`, bỏ hẳn guard chặn theo
`chi_tiet_ctdt` (để cascade CSDL tự lo phần này), thay bằng 2 kiểm tra đúng
với dữ liệu sử dụng thật:

1. Đếm `lop_sinh_hoat` có `chuong_trinh_dt_id = id` — cần thêm method mới
   (repository hiện không có, dòng `coLop` cũ gọi sai method và không lọc).
2. Đếm `lop_hoc_phan` mà `chi_tiet_ctdt.ctdt_id = id` (join qua navigation
   `LopHocPhan.ChiTietCTDT.CtdtId`) — cần thêm method mới, tương tự cách đã
   làm `CountByHocKyAsync` ở task #12.

Nếu tổng 2 loại > 0 → `BadRequestException` kèm `Errors` liệt kê từng loại +
số lượng + nơi xử lý (tái dùng đúng cơ chế `BadRequestException(message,
errors)` đã thêm ở task #12 — không cần sửa gì thêm ở
`GlobalExceptionMiddleware`/`CrudTable.tsx`, cả 2 đã hỗ trợ sẵn).

Nếu không có gì chặn → xoá CTĐT bình thường; CSDL tự cascade xoá toàn bộ
`chi_tiet_ctdt` liên quan.

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Application/Interfaces/Repositories/ILopSinhHoatRepository.cs` | Thêm `Task<int> CountByCtdtAsync(int ctdtId)` |
| `src/UniversityPortal.Infrastructure/Repositories/LopSinhHoatRepository.cs` | Cài đặt `CountByCtdtAsync` (`DbSet.CountAsync(x => x.ChuongTrinhDtId == ctdtId)`) |
| `src/UniversityPortal.Application/Interfaces/Repositories/ILopHocPhanRepository.cs` | Thêm `Task<int> CountByCtdtAsync(int ctdtId)` |
| `src/UniversityPortal.Infrastructure/Repositories/LopHocPhanRepository.cs` | Cài đặt `CountByCtdtAsync` (`DbSet.CountAsync(x => x.ChiTietCTDT.CtdtId == ctdtId)`) |
| `src/UniversityPortal.Application/Services/ChuongTrinhDTService.cs` | Viết lại `DeleteAsync`: bỏ guard chặn theo `chi_tiet_ctdt`; thêm 2 kiểm tra trên, gộp lỗi kiểu itemized giống `HocKyService.DeleteAsync` (task #12); bỏ dòng `coLop` thừa |

### Frontend

Không cần đổi gì — `CrudTable.tsx` (Khu vực C "Quản lý CTĐT" trong trang
Ngành học & CTĐT) đã tự hiện `Modal.error` liệt kê `errors[]` từ task #12.

### Dữ liệu / Migration

Không cần — không đổi schema, chỉ đổi logic kiểm tra trước khi xoá.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~1-2 giờ |
| Mức độ rủi ro | Trung bình — nới lỏng 1 điều kiện chặn xoá (chi_tiet_ctdt) nên cần kiểm thử kỹ để chắc chắn 2 điều kiện thay thế (lớp sinh hoạt, lớp học phần) bao phủ đủ, không để lọt trường hợp xoá nhầm dữ liệu CTĐT đang dùng thật |
| Ảnh hưởng dữ liệu hiện có | Có — CTĐT chưa có lớp sinh hoạt/lớp học phần sẽ xoá được kèm toàn bộ chi_tiet_ctdt (trước đây bị chặn hoàn toàn); hành vi mới, cần Admin hiểu rõ đây là xoá thật, không hoàn tác được |
| Khả năng rollback | Revert `ChuongTrinhDTService.cs` + 2 cặp repository/interface liệt kê ở mục 4 |

## 6. Kế hoạch triển khai

1. Thêm `CountByCtdtAsync` ở `LopSinhHoatRepository` + interface.
2. Thêm `CountByCtdtAsync` ở `LopHocPhanRepository` + interface.
3. Viết lại `ChuongTrinhDTService.DeleteAsync` theo mục 3, xoá dòng `coLop` cũ.
4. `dotnet build`.
5. Kiểm thử trên trình duyệt với dữ liệu thật:
   - Tạo 1 CTĐT mới + thêm vài môn học (chi_tiet_ctdt) nhưng chưa gán lớp
     sinh hoạt/lớp học phần nào → xoá → phải thành công, biến mất khỏi danh
     sách, các môn học của nó cũng biến mất khỏi "Xem môn học".
   - Thử xoá 1 CTĐT thật đang có lớp sinh hoạt và/hoặc lớp học phần (vd
     HTTT-K23) → phải bị chặn, hiện đúng `Modal.error` liệt kê loại + số
     lượng + nơi xử lý.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] Xoá CTĐT chưa có lớp sinh hoạt/lớp học phần nào → xoá thành công, toàn
      bộ chi_tiet_ctdt (môn học) của nó biến mất theo.
- [ ] Xoá CTĐT đang có lớp sinh hoạt và/hoặc lớp học phần → bị chặn, thông
      báo liệt kê đúng loại + số lượng + nơi xử lý, không lỗi SQL thô.
- [ ] `dotnet build` sạch, `tsc --noEmit` sạch (không đổi frontend nhưng vẫn
      chạy lại để chắc chắn không ảnh hưởng).
- [ ] Đã kiểm thử trên trình duyệt với dữ liệu thật cho cả 2 trường hợp trên.

## 8. Ghi chú

- Đây là lần thứ 2 áp dụng mẫu "gộp kiểm tra + báo lỗi itemized" (lần đầu ở
  task #12 cho Học kỳ) — cùng cơ chế `BadRequestException(message, errors)`,
  không cần sửa lại middleware hay `CrudTable.tsx`.
- Khác với Học kỳ (nơi quyết định KHÔNG cascade), ở đây **chi_tiet_ctdt được
  phép cascade** vì đó chính là dữ liệu "cấu hình chương trình" (môn nào học
  ở kỳ nào) thuộc sở hữu của CTĐT — xoá CTĐT thì cấu hình đó xoá theo là hợp
  lý, miễn là chưa có ai *thực sự học* theo cấu hình đó (chưa có lớp học phần
  mở ra từ nó).

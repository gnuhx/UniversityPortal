# Nhân bản chương trình đào tạo của 1 Ngành học sang Khoá học mới

**Task:** #15
**Trạng thái:** done
**Ngày tạo:** 2026-07-07
**Người phụ trách:** (chưa gán)

---

## 0. Đã chốt với người yêu cầu

- "Ngành học" không có field Khoá học (chỉ Chương trình đào tạo/CTĐT mới có,
  1 ngành có thể có nhiều CTĐT theo nhiều khoá học khác nhau) — nút "Nhân
  bản" thực chất là **nhân bản chương trình đào tạo (môn học) của ngành sang
  1 CTĐT mới ở khoá học mới**, không phải nhân bản bản ghi Ngành học.
- Nếu ngành đã có nhiều CTĐT, **tự động lấy CTĐT có khoá học mới nhất** làm
  nguồn để sao chép — không cần popup cho admin chọn nguồn.

## 1. Bối cảnh / Vấn đề

Khi thêm 1 khoá học mới cho 1 ngành (vd ngành đã có CTĐT "KTPM-K22"
2022-2026, giờ cần dựng CTĐT cho khoá 2026-2030), Admin hiện phải nhập tay
lại **toàn bộ danh sách môn học** (`chi_tiet_ctdt`) — với chương trình
~30-40 môn trải nhiều học kỳ, đây là việc lặp lại tốn thời gian dù nội dung
gần như giống hệt CTĐT cũ (chỉ lệch khoá học/năm).

**Vấn đề kỹ thuật cốt lõi cần giải quyết đúng**: mỗi dòng `chi_tiet_ctdt`
gắn với 1 `hoc_ky_id` **cụ thể, tuyệt đối** (vd "HK1 2022-2023" — thuộc năm
học 2022-2023 thật), **không phải** một vị trí tương đối kiểu "học kỳ 1 của
năm 1 trong chương trình". Nếu sao chép y nguyên `hocKyId` từ CTĐT nguồn
(2022-2026) sang CTĐT đích (2026-2030), toàn bộ môn học của khoá mới sẽ bị
gán nhầm vào các học kỳ **đã qua từ 4 năm trước** — dữ liệu sai hoàn toàn,
không chỉ là "chưa đẹp".

Cách xử lý đúng: với mỗi dòng `chi_tiet_ctdt` nguồn, phải suy ra **vị trí
tương đối** của nó trong khoá học nguồn (năm thứ mấy, học kỳ thứ mấy trong
năm đó), rồi tìm học kỳ **tương ứng cùng vị trí** trong khoá học đích, và
dùng `hoc_ky_id` của học kỳ đó cho dòng mới. Nếu khoá học đích **chưa có**
học kỳ ở vị trí tương ứng (vd khoá 2026-2030 mới tạo, chưa ai tạo học kỳ cho
năm 2029-2030) — **không thể** tự tạo học kỳ hộ (học kỳ có ngày bắt đầu cụ
thể, không nên đoán), nên môn đó phải **bỏ qua** và báo rõ cho Admin biết
môn nào bị bỏ qua + lý do, để Admin tạo học kỳ trước rồi thêm tay hoặc chạy
lại.

Suy ra vị trí tương đối bằng cách nào (vì `TenHocKy` là text tự do, không tin
cậy để parse — xem hạn chế đã ghi ở nhiều task trước): dùng
`NamHoc.TenNamHoc` (định dạng chuẩn "YYYY-YYYY", có ràng buộc unique) để suy
**năm thứ mấy** trong khoá (offset = năm bắt đầu của học kỳ nguồn − năm bắt
đầu của khoá nguồn), rồi trong năm học đó, sắp các học kỳ theo `NgayBatDau`
tăng dần để suy **thứ tự học kỳ trong năm** (kỳ 1, kỳ 2, kỳ 3,...). Áp dụng
cùng offset năm + cùng thứ tự trong năm lên khoá đích để tìm đúng học kỳ
tương ứng.

## 2. Mục tiêu

- Ở bảng "Ngành học" (Khu vực B, trang "Ngành học & CTĐT"), thêm nút **"Nhân
  bản"** cạnh nút "Xem môn học" hiện có (chỉ Admin thấy).
- Bấm vào mở popup: nhập **Khoá học mới** + **Mã CTĐT mới** (gợi ý sẵn, có
  thể sửa — vì mã CTĐT phải duy nhất, không thể tự đoán chắc chắn 100%).
- Xác nhận → tạo 1 CTĐT mới cho ngành đó ở khoá học mới, sao chép toàn bộ
  môn học từ CTĐT khoá học mới nhất hiện có của ngành, ánh xạ đúng học kỳ
  tương ứng theo khoá học mới (không sao chép nhầm học kỳ cũ).
- Báo cáo rõ: đã sao chép bao nhiêu môn, bỏ qua bao nhiêu môn (và vì sao —
  thường là do khoá học mới chưa có học kỳ tương ứng).

## 3. Giải pháp đề xuất

### 3.1. Backend — endpoint nhân bản mới (xử lý ở server để đảm bảo tính đúng/toàn vẹn)

Thêm `POST /api/chuong-trinh-dt/clone` (Admin), body
`{ nganhId, khoaHocMoi, maCtdtMoi }`:

1. Tìm CTĐT nguồn = CTĐT có `nganhId` tương ứng, khoá học **mới nhất** (sắp
   giảm dần theo chuỗi, cùng cách `useKhoaHocOptions`/`ChuongTrinhDTPage`
   đang làm) — nếu ngành chưa có CTĐT nào → `BadRequestException` ("Ngành
   này chưa có chương trình đào tạo nào để nhân bản.").
2. Parse năm bắt đầu của khoá nguồn và khoá đích từ chuỗi "YYYY-YYYY" (nếu
   1 trong 2 không đúng định dạng này → không suy được offset năm, trả lỗi
   rõ ràng yêu cầu Admin nhập đúng định dạng, không đoán bừa).
3. Tạo CTĐT mới `{ maCtdt: maCtdtMoi, nganhId, khoaHoc: khoaHocMoi }` (validate
   trùng mã như `CreateAsync` hiện có).
4. Với mỗi `chi_tiet_ctdt` của CTĐT nguồn: suy năm thứ mấy + thứ tự học kỳ
   trong năm (mục 1), tìm học kỳ tương ứng ở khoá đích. Nếu tìm thấy → tạo
   dòng `chi_tiet_ctdt` mới (cùng `monHocId`, `soTinChi`, `tinhDiemTb`, học
   kỳ mới tìm được). Nếu không tìm thấy → thêm vào danh sách bỏ qua (kèm mã
   môn + lý do "chưa có học kỳ năm học X trong hệ thống").
5. Trả về `{ ctdtMoiId, maCtdtMoi, soMonDaSaoChep, monBoQua: string[] }`.

### 3.2. Frontend — nút + popup ở bảng Ngành học

- `frontend/src/pages/NganhHocPage.tsx` (`DanhSachNganhHoc`): thêm cột hành
  động "Nhân bản" (Admin only) mở `NhanBanCtdtModal` mới, truyền `nganhId` +
  `tenNganh`.
- Modal mới `frontend/src/components/NhanBanCtdtModal.tsx`:
  - Field "Khoá học mới" (text, required, gợi ý placeholder "vd: 2026-2030").
  - Field "Mã CTĐT mới" (text, required) — tự gợi ý giá trị ban đầu bằng
    cách lấy CTĐT mới nhất của ngành, thay phần năm bắt đầu trong `maCtdt`
    cũ (nếu match được pattern) bằng năm bắt đầu của khoá học mới; Admin có
    thể sửa lại tự do.
  - Submit → gọi endpoint clone; hiện kết quả: "Đã tạo CTĐT '...' và sao
    chép N môn học." + nếu có môn bị bỏ qua, liệt kê rõ (dùng lại kiểu
    `Modal.error`/thông báo itemized đã dùng ở các task #12, #14).

### 3.3. Điểm cần chốt trước khi code

1. Nếu khoá học đích **không đúng định dạng "YYYY-YYYY"** (tự do như đã ghi
   nhận ở task #04) → không suy được offset năm để ánh xạ học kỳ. Đề xuất:
   chặn hẳn (báo lỗi yêu cầu nhập đúng định dạng năm-năm) thay vì cho nhân
   bản "một phần" không đoán trước được kết quả.
2. Có cần cho phép nhân bản khi CTĐT nguồn **và** đích có khoá học lệch số
   năm khác nhau không (vd nguồn 4 năm 2022-2026, đích chỉ 3 năm dạng
   "2026-2029")? Đề xuất: vẫn cho phép — chỉ ánh xạ được năm nào trùng offset
   còn tồn tại ở đích, năm dư ra (không có offset tương ứng) tự động rơi vào
   danh sách "bỏ qua", không cần chặn trước.

## 4. Phạm vi thay đổi

### Backend

| File | Thay đổi |
|---|---|
| `src/UniversityPortal.Application/DTOs/ChuongTrinhDT/CloneChuongTrinhDTDto.cs` (mới) | `NganhId`, `KhoaHocMoi`, `MaCtdtMoi` |
| `src/UniversityPortal.Application/DTOs/ChuongTrinhDT/CloneChuongTrinhDTResultDto.cs` (mới) | `CtdtMoiId`, `MaCtdtMoi`, `SoMonDaSaoChep`, `MonBoQua: List<string>` |
| `src/UniversityPortal.Application/Services/ChuongTrinhDTService.cs` | Thêm `CloneAsync` theo thuật toán ở mục 3.1 |
| `src/UniversityPortal.Application/Interfaces/Services/IChuongTrinhDTService.cs` | Khai báo `CloneAsync` |
| `src/UniversityPortal.API/Controllers/ChuongTrinhDTController.cs` | Thêm `POST /api/chuong-trinh-dt/clone` (Admin) |
| `src/UniversityPortal.Application/Interfaces/Repositories/IHocKyRepository.cs` / `HocKyRepository.cs` | Có thể cần thêm method tra học kỳ theo `namHocId` + thứ tự (`OrderBy(NgayBatDau)`) nếu chưa có sẵn cách lấy gọn |

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/components/NhanBanCtdtModal.tsx` (mới) | Popup nhập Khoá học mới + Mã CTĐT mới, gọi API clone, hiện kết quả/danh sách bỏ qua |
| `frontend/src/pages/NganhHocPage.tsx` | Thêm cột "Nhân bản" (Admin only) trong `DanhSachNganhHoc`, mở modal trên |
| `frontend/src/api/modules.ts` | Thêm `chuongTrinhDTApi.clone(dto)` |
| `frontend/src/types/index.ts` | Thêm `CloneChuongTrinhDT`, `CloneChuongTrinhDTResult` |

### Dữ liệu / Migration

Không cần migration — chỉ tạo thêm bản ghi qua nghiệp vụ có sẵn (CTĐT +
chi_tiet_ctdt), không đổi schema.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~1 ngày (thuật toán ánh xạ học kỳ là phần phức tạp nhất; còn lại là CRUD quen thuộc) |
| Mức độ rủi ro | Trung bình — chỉ tạo mới dữ liệu (không xoá/sửa gì có sẵn) nên an toàn nếu lỗi, nhưng thuật toán ánh xạ sai sẽ tạo ra chi_tiet_ctdt sai học kỳ mà Admin có thể không nhận ra ngay |
| Ảnh hưởng dữ liệu hiện có | Không — chỉ thêm dữ liệu mới |
| Khả năng rollback | Xoá CTĐT mới tạo (cascade xoá luôn chi_tiet_ctdt theo đúng hành vi đã có ở task #14) nếu kết quả nhân bản không đúng ý |

## 6. Kế hoạch triển khai

1. Backend: thêm DTO clone, `IChuongTrinhDTService.CloneAsync` + cài đặt
   thuật toán ánh xạ học kỳ theo offset năm + thứ tự trong năm.
2. Backend: thêm endpoint `POST /api/chuong-trinh-dt/clone`.
3. `dotnet build`; kiểm thử thủ công qua curl với 1 ngành có CTĐT thật, thử
   cả trường hợp khoá đích đã có đủ học kỳ và trường hợp thiếu học kỳ (xác
   nhận `monBoQua` liệt kê đúng).
4. Frontend: `chuongTrinhDTApi.clone`, `NhanBanCtdtModal.tsx`, nút "Nhân
   bản" trong `NganhHocPage.tsx`.
5. `tsc --noEmit` + `dotnet build`.
6. Kiểm thử trên trình duyệt với dữ liệu thật: nhân bản 1 ngành có CTĐT đầy
   đủ môn học sang khoá học mới đã có sẵn học kỳ tương ứng (xác nhận sao
   chép đúng số môn, đúng học kỳ tương ứng theo năm/thứ tự); thử thêm 1 ca
   khoá đích thiếu học kỳ để xác nhận thông báo "bỏ qua" hiển thị đúng.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Bảng Ngành học có nút "Nhân bản" (Admin only).
- [x] Popup nhập Khoá học mới + Mã CTĐT mới (gợi ý sẵn, sửa được). Phát hiện
      và sửa 1 race condition thật khi kiểm thử: gợi ý mã chỉ tính qua
      `onChange` của ô Khoá học, nên nếu CTĐT nguồn (query riêng) chưa tải
      xong lúc admin gõ thì gợi ý bị bỏ lỡ, ô Mã CTĐT mới trống, chặn submit
      bởi validation "required" (không tạo sai dữ liệu, nhưng gợi ý không
      hiện). Đã sửa bằng `Form.useWatch` + `useEffect` phản ứng theo cả
      `nguon` lẫn giá trị đang gõ, không phụ thuộc thứ tự tải xong.
- [x] Nhân bản thành công: CTĐT mới xuất hiện, môn học được gán đúng học kỳ
      tương ứng theo khoá học mới. Đã kiểm thử với dữ liệu thật: nhân bản
      "KTPM-K22" (2022-2026, 3 môn ở năm thứ 3 của chương trình) sang khoá
      "2020-2024" → cả 3 môn được gán đúng vào "Học kỳ 1 (2022-2023)" (đúng
      năm thứ 3 của khoá mới), khớp 100% dự đoán thủ công trước khi chạy.
- [x] Trường hợp khoá đích thiếu học kỳ tương ứng: môn đó bị bỏ qua, thông
      báo liệt kê rõ ràng. Đã kiểm thử: nhân bản sang khoá "2030-2034"
      (năm tương ứng "2032-2033" chưa tồn tại) → đúng 0/3 môn được sao chép,
      cả 3 vào danh sách bỏ qua.
- [ ] Ngành chưa có CTĐT nào: bấm "Nhân bản" báo lỗi rõ ràng, không crash —
      code đã viết (Alert cảnh báo khi `nguon` rỗng) nhưng chưa trực tiếp
      bấm thử trên 1 ngành thật sự chưa có CTĐT nào trong phiên kiểm thử này.
- [x] `dotnet build` sạch, `tsc --noEmit` sạch.
- [x] Đã kiểm thử trên trình duyệt với dữ liệu thật (Playwright headless,
      vai trò Admin) — 3 lượt nhân bản thật (2020-2024, 2030-2034,
      2050-2054), dọn sạch toàn bộ dữ liệu test sau khi xong (xoá chi_tiet_ctdt
      trước rồi xoá CTĐT — xem ghi chú mục 8), không còn dữ liệu thừa.

## 8. Ghi chú

- Nếu sau này cần nhân bản **có chọn CTĐT nguồn** (thay vì luôn lấy mới
  nhất) hoặc nhân bản **kèm tạo học kỳ còn thiếu**, đó là mở rộng cho task
  riêng — phạm vi task này dừng ở đúng 2 điểm đã chốt với người yêu cầu.
- Task #14 (nới lỏng chặn xoá CTĐT khi còn chi_tiet_ctdt) **chưa được triển
  khai** tại thời điểm làm task này — trong lúc kiểm thử, muốn xoá CTĐT vừa
  nhân bản (dữ liệu test) phải xoá từng dòng `chi_tiet_ctdt` của nó trước
  (endpoint xoá chi tiết CTDT vốn không có guard) rồi mới xoá được CTĐT (guard
  cũ vẫn chặn nếu còn chi tiết liên kết). Sau khi task #14 được làm, xoá CTĐT
  test sẽ tự cascade xoá chi_tiet_ctdt, không cần bước xoá tay này nữa.
- Trong lúc kiểm thử phát hiện dữ liệu học kỳ mẫu có vài cặp bản ghi trùng
  gần như y hệt (cùng năm học, cùng `NgayBatDau`, khác tên — vd "Học kỳ 1
  (2024-2025)" và "HK1 2024-2025" cùng ngày 2024-09-02). Đã thêm
  `.ThenBy(hk => hk.Id)` khi sắp xếp học kỳ trong năm để đảm bảo thứ tự ổn
  định bất kể trùng ngày — không sửa dữ liệu mẫu, chỉ đảm bảo thuật toán
  nhân bản không bị ảnh hưởng bởi tình trạng dữ liệu này.

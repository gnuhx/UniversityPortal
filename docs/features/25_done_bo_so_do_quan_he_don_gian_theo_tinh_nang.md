# Bộ sơ đồ quan hệ đơn giản theo từng tính năng (Sinh viên / Giáo viên / Admin)

**Task:** #25
**Trạng thái:** done
**Ngày tạo:** 2026-07-10
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Đã có 2 dạng tài liệu ERD:

- `docs/erd_schema.html` — ERD đầy đủ, 32 bảng, dùng để tra cứu chi tiết
  toàn bộ schema (đã export `docs/exports/erd-full.png`).
- `docs/erd_simple.html` — sơ đồ rút gọn đầu tiên, minh hoạ 3 vai trò
  người dùng (Admin/Giáo viên/Sinh viên) map vào `tai_khoan`/`giao_vien`/
  `sinh_vien` và các bảng lõi liên quan (7 bảng, đã export
  `docs/exports/erd-simple.png`).

Admin muốn nhân rộng phong cách "đơn giản, chỉ giữ bảng & quan hệ quan
trọng nhất" của `erd_simple.html` cho **từng tính năng riêng lẻ** trong
hệ thống (không chỉ 1 sơ đồ tổng cho toàn bộ user, mà mỗi tính năng có
1 sơ đồ nhỏ của riêng nó), để dễ tra cứu "tính năng X đụng tới bảng
nào" mà không phải nhìn sơ đồ 32-bảng rối mắt.

## 2. Mục tiêu

- Với mỗi tính năng đã được xây dựng thật (có Controller + trang
  frontend) trong 3 khu vực Sinh viên / Giáo viên / Admin, có 1 sơ đồ
  HTML tĩnh + 1 ảnh PNG xuất ra, cùng phong cách với `erd_simple.html`
  (nền trắng, độ phân giải cao, chỉ giữ cột PK/FK quan trọng, nhãn quan
  hệ ngắn gọn bằng tiếng Việt).
- Mỗi ảnh export có **tên tiếng Việt mô tả đúng nội dung** (không đặt
  tên chung chung như `erd-2.png`).
- Có 1 script export dùng chung (không viết lại logic Playwright cho
  từng tính năng), vì logic đo kích thước + chụp ảnh là hoàn toàn giống
  nhau giữa các sơ đồ, chỉ khác dữ liệu bảng/quan hệ.

**Ngoài phạm vi:** không vẽ sơ đồ cho các bảng **chưa có tính năng thật
đi kèm** — xem danh sách ở mục 8. Vẽ sơ đồ cho bảng không có
Controller/trang nào sẽ chỉ là "sơ đồ cho tương lai", không phản ánh
tính năng đang chạy thật, dễ gây hiểu lầm.

## 3. Giải pháp đề xuất

### 3.1. Kiểm kê tính năng thật (đã có Controller + trang) theo 3 khu vực

Đối chiếu trực tiếp với `frontend/src/components/AppLayout.tsx` (danh
sách menu thật, không suy đoán) và Controllers tương ứng, gộp các
trang dùng chung 1 nhóm bảng thành 1 sơ đồ (vd trang "Lớp sinh hoạt"
xuất hiện ở cả 3 vai trò nhưng chỉ cần **1** sơ đồ, có ghi chú vai trò
nào làm gì):

| # | Tính năng | Vai trò dùng | Bảng chính |
|---|---|---|---|
| 01 | Xác thực & Vai trò người dùng | Cả 3 (nền tảng) | `vai_tro`, `tai_khoan`, `giao_vien`, `sinh_vien` |
| 02 | Hồ sơ sinh viên & Điều kiện tốt nghiệp | Sinh viên (xem mình), Admin/Giáo vụ (quản lý) | `sinh_vien`, `lop_sinh_hoat`, `chuong_trinh_dt`, `chi_tiet_ctdt`, `hoc_ba`, `danh_sach_lop_hp` |
| 03 | Ngành học, Khoa & Chương trình đào tạo | Xem: cả 3 · Quản lý: Admin | `nganh_hoc`, `phong_ban`, `chuong_trinh_dt`, `mon_hoc`, `chi_tiet_ctdt` |
| 04 | Lớp sinh hoạt & Biên bản sinh hoạt chủ nhiệm | Sinh viên (xem), Giáo viên (GVCN tạo biên bản), Admin/Giáo vụ (CRUD lớp) | `lop_sinh_hoat`, `bien_ban_shcn`, `chi_tiet_cong_viec`, `chi_tiet_vang_shcn` |
| 05 | Lớp học phần, Đăng ký & Bảng điểm | Sinh viên (xem điểm), Giáo viên (nhập điểm), Admin/Giáo vụ (duyệt danh sách) | `lop_hoc_phan`, `danh_sach_lop_hp`, `chi_tiet_ctdt` |
| 06 | Thời khoá biểu | Cả 3 (tự phân nhánh theo vai trò) | `thoi_khoa_bieu`, `lop_hoc_phan`, `tuan_hoc` |
| 07 | Học phí | Sinh viên (xem/đóng), Admin/Giáo vụ (quản lý, generate) | `hoc_phi`, `sinh_vien`, `hoc_ky` |
| 08 | Thông báo & Bình luận | Sinh viên (xem), Admin/Giáo vụ (soạn/gửi) | `thong_bao`, `thong_bao_da_doc`, `binh_luan_thong_bao`, `lop_sinh_hoat` |
| 09 | Yêu cầu hành chính | Sinh viên (gửi), Admin/Giáo vụ (duyệt) | `yeu_cau_hanh_chinh`, `sinh_vien`, `tai_khoan` |
| 10 | Yêu cầu sửa điểm | Giáo viên (gửi), Admin (duyệt) | `yeu_cau_sua_diem`, `lop_hoc_phan`, `giao_vien` |
| 11 | Năm học, Học kỳ & Tuần học | Admin/Giáo vụ (quản lý), Giáo viên (xem) | `nam_hoc`, `hoc_ky`, `tuan_hoc` |
| 12 | Nội dung tĩnh (Thư viện / Học Vụ) | Xem: cả 3 · Quản lý: Admin | `noi_dung_tinh` |

Mục #01 **đã làm xong** ở lượt trước (`erd_simple.html` /
`erd-simple.png`) — trong task này sẽ đổi tên cho khớp quy ước mới
(mục 3.3), nội dung sơ đồ giữ nguyên.

### 3.2. Công cụ export dùng chung

> Cập nhật sau khi làm: script đặt tại `frontend/scripts/export-erd.mjs`
> thay vì `docs/erd/export-erd.mjs` như phác thảo ban đầu — lý do ở
> mục 8 (module resolution của `playwright`).

Tách phần đo kích thước + chụp ảnh (đã viết 2 lần giống hệt nhau cho
`erd-full.png` và `erd-simple.png`) thành **1 script chung**, nhận
đường dẫn file HTML nguồn + file PNG đích qua tham số dòng lệnh, để
không phải viết lại cho từng tính năng:

```
cd frontend
node scripts/export-erd.mjs --in ../docs/erd/04_lop_sinh_hoat_bien_ban.html \
                             --out ../docs/exports/so-do-lop-sinh-hoat-va-bien-ban-sinh-hoat.png
```

Logic bên trong (giữ nguyên kỹ thuật đã kiểm chứng ở 2 lần trước):
đo bounding box thật của toàn bộ `.table-card` ở pass 1 (viewport dư
lớn), sau đó pass 2 mở context mới với viewport khớp đúng kích thước
nội dung + `deviceScaleFactor` đủ để chiều rộng ảnh cuối ≥ 2400px,
chụp không cắt xén.

### 3.3. Quy ước đặt tên file

- **HTML nguồn** (nơi định nghĩa bảng/quan hệ, tĩnh, style giống
  `erd_simple.html`): `docs/erd/<NN>_<slug_khong_dau>.html`
  — đánh số theo đúng thứ tự bảng ở mục 3.1, vd
  `docs/erd/04_lop_sinh_hoat_bien_ban.html`.
- **Ảnh export** (tên tiếng Việt mô tả, theo đúng yêu cầu): `docs/exports/so-do-<mo-ta-khong-dau>.png`
  — dùng bản không dấu cho tên file (an toàn hệ điều hành/URL), nhưng
  tiêu đề *bên trong* ảnh (thẻ `<h1>`) vẫn viết tiếng Việt có dấu đầy
  đủ, giống `erd_simple.html` đã làm.

| # | File HTML | File ảnh export |
|---|---|---|
| 01 | `docs/erd/01_nguoi_dung_vai_tro.html` (đổi tên từ `erd_simple.html`) | `so-do-nguoi-dung-va-vai-tro.png` (đổi tên từ `erd-simple.png`) |
| 02 | `docs/erd/02_ho_so_sinh_vien_tot_nghiep.html` | `so-do-ho-so-sinh-vien-va-dieu-kien-tot-nghiep.png` |
| 03 | `docs/erd/03_nganh_hoc_ctdt.html` | `so-do-nganh-hoc-va-chuong-trinh-dao-tao.png` |
| 04 | `docs/erd/04_lop_sinh_hoat_bien_ban.html` | `so-do-lop-sinh-hoat-va-bien-ban-sinh-hoat.png` |
| 05 | `docs/erd/05_lop_hoc_phan_bang_diem.html` | `so-do-lop-hoc-phan-va-bang-diem.png` |
| 06 | `docs/erd/06_thoi_khoa_bieu.html` | `so-do-thoi-khoa-bieu.png` |
| 07 | `docs/erd/07_hoc_phi.html` | `so-do-hoc-phi.png` |
| 08 | `docs/erd/08_thong_bao.html` | `so-do-thong-bao-va-binh-luan.png` |
| 09 | `docs/erd/09_yeu_cau_hanh_chinh.html` | `so-do-yeu-cau-hanh-chinh.png` |
| 10 | `docs/erd/10_yeu_cau_sua_diem.html` | `so-do-yeu-cau-sua-diem.png` |
| 11 | `docs/erd/11_nam_hoc_hoc_ky_tuan_hoc.html` | `so-do-nam-hoc-hoc-ky-va-tuan-hoc.png` |
| 12 | `docs/erd/12_noi_dung_tinh.html` | `so-do-noi-dung-tinh-thu-vien-hoc-vu.png` |

`docs/erd_schema.html` (ERD đầy đủ 32 bảng) và
`docs/exports/erd-full.png` giữ nguyên vị trí cũ, không thuộc bộ này —
đây vẫn là bản tra cứu tổng, khác mục đích với bộ sơ đồ theo tính năng.

### 3.4. Nội dung mỗi sơ đồ (khuôn mẫu lặp lại cho cả 11 tính năng còn lại)

Mỗi file HTML là bản sao thu gọn của `erd_simple.html` (cùng CSS/JS,
chỉ thay dữ liệu `TABLES`/`POSITIONS`/`RELATIONS`/tiêu đề/ghi chú),
gồm:

- Chỉ liệt kê cột PK/FK + 1-2 cột quan trọng nhất mỗi bảng (không liệt
  kê hết cột như ERD đầy đủ).
- Nhãn quan hệ ngắn gọn tiếng Việt trên mỗi đường nối (vd "GVCN",
  "giảng dạy", "đăng ký" như đã làm ở sơ đồ #01).
- 1 ô ghi chú nhỏ (giống ô "Admin không có bảng hồ sơ riêng" ở sơ đồ
  #01) nêu đúng 1 điểm quan trọng/dễ hiểu nhầm nhất của tính năng đó —
  không phải tính năng nào cũng cần, chỉ thêm khi có điểm đáng lưu ý
  thật sự (vd tính năng #04 sẽ ghi chú "Sinh viên không thấy lý do
  vắng của bạn khác" — đúng theo thiết kế đã làm ở task #24).
- Nếu quan hệ bắt chéo hình chữ X (như #01 từng gặp), áp dụng luôn kỹ
  thuật lệch `t` dọc theo đường nối đã dùng ở sơ đồ #01 để nhãn không
  đè nhau — kiểm tra bằng mắt sau khi export, chỉnh lại nếu cần trước
  khi coi là xong.

## 4. Phạm vi thay đổi

### Backend

Không đổi — đây là tài liệu tĩnh, không đụng code ứng dụng.

### Frontend

Không đổi.

### Tài liệu / Công cụ mới

| File | Nội dung |
|---|---|
| `frontend/scripts/export-erd.mjs` | Script Playwright dùng chung (đo kích thước + chụp ảnh), nhận `--in`/`--out` |
| `docs/erd/01_nguoi_dung_vai_tro.html` | Đổi tên từ `docs/erd_simple.html` (qua `git mv`) |
| `docs/erd/02..12_*.html` | 11 file mới, theo khuôn mẫu mục 3.4 |
| `docs/exports/so-do-*.png` | 12 ảnh export (1 đổi tên + 11 mới) |

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~2-3 giờ — chủ yếu lặp lại khuôn mẫu đã kiểm chứng (10 phút/sơ đồ sau khi có script dùng chung), cộng thời gian kiểm tra nhãn đè nhau ở vài sơ đồ nhiều quan hệ chéo (#04, #05) |
| Mức độ rủi ro | Thấp — chỉ là tài liệu tĩnh (HTML + PNG), không ảnh hưởng ứng dụng đang chạy |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Xoá thư mục `docs/erd/` và các file `so-do-*.png` mới; `erd_schema.html`/`erd-full.png` không bị đụng tới |

## 6. Kế hoạch triển khai

1. ~~Tạo script export dùng chung, tham số hoá qua CLI.~~ Đã làm —
   `frontend/scripts/export-erd.mjs` (không phải `docs/erd/`, xem
   mục 8).
2. ~~Di chuyển + đổi tên `docs/erd_simple.html` → `docs/erd/01_nguoi_dung_vai_tro.html`,
   export lại, xoá `docs/exports/erd-simple.png` cũ.~~ Đã làm bằng
   `git mv` (giữ lịch sử file).
3. ~~Lần lượt tạo 11 file `docs/erd/02..12_*.html`.~~ Đã làm — tất cả
   dựa trên field/quan hệ đối chiếu thật với entity C# và với
   `docs/exports/erd-full.png` (không suy đoán tên cột).
4. ~~Export từng file, xem lại ảnh, chỉnh nhãn đè nhau nếu có.~~ Đã
   làm — phát hiện và sửa 3 lỗi chồng lấn thật (không phải giả định),
   xem mục 8.
5. ~~Báo cáo lại danh sách 12 ảnh đã xuất.~~ Xem bảng mục 3.3 — toàn bộ
   đường dẫn giữ nguyên như kế hoạch.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Có script export dùng chung, chạy được cho mọi sơ đồ qua tham số
      `--in`/`--out` (`frontend/scripts/export-erd.mjs`).
- [x] Đủ 12 file HTML trong `docs/erd/`, đánh số + đặt tên đúng quy
      ước mục 3.3.
- [x] Đủ 12 ảnh PNG trong `docs/exports/`, tên tiếng Việt mô tả đúng
      nội dung, độ phân giải ≥ 2400px chiều rộng (thực tế 2400-3510px),
      nền trắng.
- [x] Mỗi ảnh: số bảng + số đường nối hiển thị khớp đúng dữ liệu khai
      báo trong `TABLES`/`RELATIONS` của file HTML tương ứng — script
      tự in ra `Expected` vs `Rendered` mỗi lần export, cả 12 file đều
      khớp 1-1 (kể cả #12 với 0 quan hệ).
- [x] Không còn nhãn quan hệ nào đè lên nhau trong bất kỳ ảnh nào —
      đã xem từng ảnh trong số 12, phát hiện 3 lỗi chồng lấn thật ở
      #02/#03/#12 (ghi chú đè card) và 1 lỗi nhãn đè nhau ở #08, đã sửa
      và export lại, xem xác nhận lại bằng ảnh sau khi sửa.
- [x] `docs/erd_schema.html`/`docs/exports/erd-full.png` (ERD đầy đủ)
      không bị sửa/xoá — chỉ được đọc lại để đối chiếu tên cột chính
      xác cho các sơ đồ mới.

## 8. Ghi chú

- **Các bảng chưa có tính năng thật, không nằm trong bộ sơ đồ này**
  (đã kiểm chứng: không có Controller lẫn trang frontend nào, chỉ tồn
  tại dưới dạng bảng DB trong migration):
  `dat_phong_thuc_hanh`, `khao_sat_y_kien`, `dien_dan_giao_vien`,
  `danh_sach_thi_lai`, `diem_ren_luyen`, `ket_qua_anh_van_dau_vao`.
  `dien_dan_giao_vien` cũng chính là bảng đã bị loại khỏi phạm vi ở
  task #24 vì lý do tương tự. Nếu sau này các bảng này được xây thành
  tính năng thật, thêm sơ đồ tương ứng vào bộ này (đánh số tiếp `13_...`).
- **`hoc_ba`** không có trang riêng nhưng được dùng ngầm bởi API
  "Điều kiện tốt nghiệp" trên trang Hồ sơ cá nhân sinh viên — nên gộp
  vào sơ đồ #02 (Hồ sơ sinh viên) thay vì tách riêng.
- Quy ước đặt tên `so-do-*.png` (tiếng Việt không dấu) là suy đoán hợp
  lý theo đúng yêu cầu "tên mô tả tiếng Việt" — nếu Admin muốn giữ dấu
  trong tên file (vd `sơ-đồ-lớp-sinh-hoạt.png`) hoặc đổi tiền tố khác
  `so-do-`, có thể điều chỉnh trước khi chạy, đổi tên hàng loạt sau
  cũng không tốn công.
- Task này chỉ lập kế hoạch — chưa tạo file nào theo mục 3, chờ Admin
  duyệt danh sách 12 tính năng + quy ước tên ở mục 3.1/3.3 trước khi
  triển khai mục 6.

### Cập nhật sau khi triển khai (2026-07-10)

**Vị trí script khác kế hoạch:** đặt tại `frontend/scripts/export-erd.mjs`
thay vì `docs/erd/export-erd.mjs` như phác thảo mục 3.2. Lý do: gói
`playwright` chỉ được cài trong `frontend/node_modules` (không phải
dependency ở root `package.json`), và cơ chế phân giải module ESM của
Node tìm `node_modules` theo **thư mục chứa file script**, đi ngược lên
các thư mục cha — không đi "ngang" sang `frontend/node_modules` nếu
script nằm ở `docs/erd/`. Nếu đặt script ở `docs/erd/`, `import {
chromium } from 'playwright'` sẽ báo lỗi không tìm thấy module. Đặt
trong `frontend/scripts/` giải quyết gọn mà không cần thêm
`playwright` như dependency ở root hay chỉnh `NODE_PATH`.

**3 lỗi chồng lấn thật phát hiện khi kiểm tra bằng mắt** (đúng theo
tinh thần "kiểm tra bằng mắt từng ảnh" ở acceptance criteria — không
chỉ tin vào số bảng/số quan hệ khớp là đủ):

1. Sơ đồ #02 (Hồ sơ sinh viên): ô ghi chú đặt ở `top: 560px` chồng lên
   2 card `hoc_ba`/`danh_sach_lop_hp` (card thật cao tới y≈767px, đo
   trực tiếp bằng `getBoundingClientRect()` chứ không đoán) — chữ trong
   ghi chú bị 2 card đè lên, không đọc được. Sửa: dời ghi chú xuống
   `top: 830px` + tăng độ rộng lên 900px.
2. Sơ đồ #03 (Ngành học/CTĐT): ghi chú `top: 400px` chồng lên card
   `mon_hoc` (kết thúc y≈430px). Sửa: dời xuống `top: 460px`.
3. Sơ đồ #12 (Nội dung tĩnh): ghi chú `top: 260px` chồng lên chính card
   `noi_dung_tinh` (kết thúc y≈305px, đo trực tiếp). Sửa: dời xuống
   `top: 340px`.
4. Sơ đồ #08 (Thông báo): 2 nhãn quan hệ "bình luận bởi" (t=0.75) và
   "gửi tới lớp" (t=0.5 mặc định) tính ra toạ độ giữa gần trùng nhau
   (cách nhau ~23px ngang, 8px dọc) do 2 đường nối bắt chéo hình chữ X
   ở cùng khu vực giữa sơ đồ. Sửa: đổi "gửi tới lớp" sang `t=0.35` để
   kéo nhãn về gần phía `thong_bao` hơn, tách biệt khỏi nhãn kia.

Cả 4 lỗi đều được tìm ra bằng cách nhìn trực tiếp từng ảnh PNG sau khi
export (không phải suy luận trước) — một số dùng thêm script đo
`getBoundingClientRect()` để biết chính xác toạ độ cần dời tới, thay vì
đoán rồi export lại nhiều lần.

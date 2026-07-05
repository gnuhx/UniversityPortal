# Môn học trong CTĐT — nhóm theo Học kỳ để thấy ngay học kỳ còn thiếu môn

**Task:** #08
**Trạng thái:** done
**Ngày tạo:** 2026-07-05
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Trang "Môn học trong CTĐT" (`ChiTietCTDTPage.tsx`, route
`/chuong-trinh-dt/:id`) trước đây dùng `CrudTable` hiển thị danh sách
môn học của một CTĐT dưới dạng **bảng phẳng, phân trang** — các môn
thuộc nhiều học kỳ khác nhau xen kẽ nhau theo thứ tự tạo, không có
cách nào nhìn nhanh xem **học kỳ nào của chương trình đang chưa có môn
học nào cả** (vd. quên chưa khai báo môn cho HK3).

Field chọn học kỳ khi thêm môn cũng đang là lỗ hổng UX riêng đã ghi
nhận ở task #06 (`06_todo_chi_tiet_ctdt_hoc_ky_dropdown.md`): ô nhập
**số Id thô** (`type: "number"`) thay vì dropdown theo tên — dễ gõ
nhầm học kỳ hoàn toàn khác năm.

## 2. Mục tiêu

- Danh sách môn học của một CTĐT được **nhóm theo học kỳ**, sắp xếp
  tăng dần theo thời gian (học kỳ sớm nhất trước — đúng lộ trình học).
- Học kỳ chưa có môn học nào hiện rõ ràng (tag cảnh báo), để phát hiện
  ngay chỗ còn thiếu khi rà soát chương trình đào tạo.
- Nhân tiện xử lý luôn phần dropdown chọn học kỳ khi thêm môn (thay ô
  nhập Id thô) — vì modal thêm/sửa được viết lại trong task này.

## 3. Giải pháp đề xuất

Viết lại `ChiTietCTDTPage.tsx` không dùng `CrudTable` chung nữa (vì
`CrudTable` chỉ hỗ trợ bảng phẳng có phân trang, không hỗ trợ nhóm) —
tự quản lý fetch/modal/mutation ngay trong trang này:

- Lấy **toàn bộ** chi tiết CTDT của `ctdtId` một lần (`pageSize: 500`,
  đủ lớn so với số môn thực tế của một chương trình — không cần đổi
  API backend).
- Lấy toàn bộ học kỳ hiện có (`hocKyApi.getAll()`, đã dùng ở nơi khác
  trong app), sắp tăng dần theo `ngayBatDau` (ngược với thứ tự giảm
  dần mặc định của API — ở đây cần đọc theo lộ trình học, không phải
  "học kỳ mới nhất trước").
- Render bằng `Collapse`: mỗi panel là một học kỳ, tiêu đề gồm tên học
  kỳ + năm học, kèm `Tag` xanh "`N` môn" nếu có môn, hoặc `Tag` đỏ
  "Chưa có môn học" nếu rỗng. Panel có môn thì mở sẵn
  (`defaultActiveKey`), panel rỗng đóng lại (đỡ chiếm chỗ nhưng vẫn
  hiện tiêu đề + tag đỏ để nhận ra ngay).
- Modal "Thêm môn học" giữ đúng ràng buộc cũ (chỉ chọn Môn học + Học kỳ
  lúc tạo, không đổi được sau khi tạo — khớp
  `UpdateChiTietCTDTDto` ở backend chỉ nhận `soTinChi`/`tinhDiemTb`),
  nhưng đổi Học kỳ từ ô nhập số sang `Select` hiển thị
  `"${tenHocKy} (${tenNamHoc})"` — giải quyết luôn task #06.

## 4. Phạm vi thay đổi

### Backend

Không đổi — dùng nguyên API `GET/POST/PUT/DELETE /api/chi-tiet-ctdt`
và `GET /api/hoc-ky` đã có.

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/pages/ChiTietCTDTPage.tsx` | Viết lại hoàn toàn: bỏ `CrudTable`, tự quản lý `useQuery`/`useMutation`/modal; hiển thị `Collapse` nhóm theo học kỳ (tăng dần theo `ngayBatDau`, tag số môn/tag "Chưa có môn học"); đổi field Học kỳ trong modal thêm mới từ số Id thô sang `Select` theo tên học kỳ + năm học |

### Dữ liệu / Migration

Không cần.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~30–40 phút (viết lại 1 file frontend, không đổi backend) |
| Mức độ rủi ro | Thấp — chỉ đổi cách hiển thị/nhập liệu ở 1 trang, không đổi API/DTO/validator; hành vi tạo/sửa/xoá giữ nguyên logic cũ |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert file `ChiTietCTDTPage.tsx` về bản dùng `CrudTable` cũ |

## 6. Kế hoạch triển khai

1. Viết lại `ChiTietCTDTPage.tsx` theo giải pháp ở mục 3.
2. `tsc --noEmit` (đã chạy, sạch).
3. Khởi động dev server, kiểm thử bằng tài khoản Admin trên
   `/chuong-trinh-dt/:id`: xác nhận nhóm theo học kỳ đúng thứ tự, học
   kỳ rỗng hiện tag đỏ, dropdown Học kỳ trong modal hiện tên thay vì
   Id, thêm/sửa/xoá môn học vẫn hoạt động và cập nhật đúng nhóm.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [x] Mở `/chuong-trinh-dt/:id`, môn học hiển thị theo từng nhóm học
      kỳ, sắp xếp tăng dần theo thời gian.
- [x] Học kỳ không có môn học nào hiện tag đỏ "Chưa có môn học".
- [x] Học kỳ có môn học hiện tag xanh số lượng và mở sẵn.
- [x] Modal "Thêm môn học" chọn Học kỳ qua dropdown tên (không còn ô
      nhập Id số).
- [x] `tsc --noEmit` sạch.
- [x] Đã kiểm thử thủ công trên trình duyệt (dev server, tài khoản
      `admin`) — xác nhận đúng thứ tự nhóm, tag đỏ/xanh, dropdown Học
      kỳ hiện tên đúng, không có console error. Xem mục 8 về 1 bug đã
      phát hiện và sửa trong lúc kiểm thử.

## 8. Ghi chú

- **Bug phát hiện khi kiểm thử trình duyệt (đã sửa):** ban đầu dùng
  `Collapse` với `defaultActiveKey` tính từ `hocKyGroups` — nhưng
  `defaultActiveKey` chỉ được đọc **một lần lúc mount**, trong khi
  `hocKyGroups` phụ thuộc 2 API call bất đồng bộ (`hocKyApi.getAll()`
  và `chiTietCTDTApi.getPaged()`) chưa chắc đã có dữ liệu tại thời
  điểm đó — kết quả là toàn bộ panel bị đóng kể cả khi có môn học.
  Sửa bằng cách chuyển sang `activeKey` có kiểm soát
  (`useState` + `useEffect`), chỉ tính danh sách mở mặc định **sau khi
  cả 2 query đã tải xong** (`!hocKysLoading && !isLoading`). Nếu chỉ
  chờ 1 trong 2 query, panel có thể bị "chốt" nhầm thành rỗng do query
  còn lại chưa kịp trả dữ liệu ở lần tính đầu tiên.
- Trong lúc kiểm thử cũng thấy dữ liệu `hoc_ky` thật có một số cặp
  trùng ý nghĩa nhưng khác tên (vd. "Học kỳ 1 (2024-2025)" và
  "HK1 2024-2025" là 2 dòng riêng biệt) — có vẻ do 2 đợt seed dữ liệu
  khác nhau. Không thuộc phạm vi task này, chỉ ghi nhận lại.
- Task này giải quyết luôn nội dung của task #06
  (`06_todo_chi_tiet_ctdt_hoc_ky_dropdown.md`) — nên đánh dấu #06 là
  `done` và ghi chú trỏ sang đây thay vì làm lại riêng.
- Nhóm hiển thị dựa trên **toàn bộ** học kỳ hiện có trong hệ thống
  (không lọc theo năm/khoá học của CTĐT đang xem) — chấp nhận được vì
  hiện tại tổng số học kỳ trong DB còn ít (2021-2022 → 2024-2025); nếu
  sau này số học kỳ tăng nhiều (nhiều năm học tích luỹ), danh sách
  nhóm sẽ dài ra và có thể cần lọc theo giai đoạn phù hợp với
  `khoa_hoc` của CTĐT — liên quan đến hạn chế `khoa_hoc` là text tự do
  đã ghi ở task #04.
- Lấy dữ liệu bằng `pageSize: 500` thay vì phân trang thật — chấp nhận
  được vì số môn học của một chương trình đào tạo thực tế rất nhỏ (vài
  chục môn); không phù hợp nếu sau này có CTDT với số môn học lớn hơn
  nhiều.

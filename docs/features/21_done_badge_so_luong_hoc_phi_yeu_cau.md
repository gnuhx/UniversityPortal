# Badge số lượng (chấm đỏ) trên menu: Học phí chưa đóng, Yêu cầu hành chính & Yêu cầu sửa điểm đang chờ

**Task:** #21
**Trạng thái:** todo
**Ngày tạo:** 2026-07-08
**Người phụ trách:** (chưa gán)

---

## 0. Điểm cần xác nhận lại trước khi làm

Yêu cầu gốc nói "as student and admin, show pending/new item of Yêu cầu
hành chính, Yêu cầu sửa điểm". Đã kiểm tra `frontend/src/components/AppLayout.tsx:29-67`
và thấy **2 tính năng này không cùng tập vai trò**:

| Mục menu | Vai trò thấy được (hiện tại) |
|---|---|
| "Yêu cầu hành chính" (`/yeu-cau-hanh-chinh`) | Sinh viên (dòng 37) **hoặc** Admin/Giáo vụ (dòng 50) |
| "Yêu cầu sửa điểm" (`/yeu-cau-sua-diem`) | Giáo viên (dòng 41) **hoặc** Admin (dòng 51) — **Sinh viên không có mục này** |

Xác nhận thêm ở backend: `YeuCauSuaDiemController.cs:16-17` — endpoint
`GET /me` gắn `[Authorize(Roles = "Giáo viên")]`, không phải Sinh viên (đây
là luồng "Giáo viên xin mở khoá bảng điểm", Admin duyệt — khác bản chất với
"Yêu cầu hành chính" của Sinh viên).

→ Đề xuất hiểu lại yêu cầu theo đúng vai trò thực tế của từng mục thay vì
áp "student + admin" cho cả hai:
- **Yêu cầu hành chính**: badge cho Sinh viên (đơn của chính mình) và cho
  Admin/Giáo vụ (toàn bộ đơn chờ duyệt).
- **Yêu cầu sửa điểm**: badge cho Giáo viên (đơn của chính mình) và Admin
  (toàn bộ đơn chờ duyệt) — **không áp dụng cho Sinh viên** vì Sinh viên
  không có quyền/menu truy cập tính năng này.

Nếu ý người yêu cầu khác (ví dụ thực ra muốn thêm Sinh viên vào luồng Yêu
cầu sửa điểm), cần nói rõ vì đó là thay đổi phân quyền lớn hơn nhiều, ngoài
phạm vi 1 badge UI — task này **mặc định làm theo bảng vai trò hiện có**.

## 1. Bối cảnh / Vấn đề

- **Học phí (Sinh viên)**: trang `/hoc-phi` đã hiển thị số tiền "Chưa đóng"
  dưới dạng `Statistic` khi vào trang (`HocPhiPage.tsx:229-230` dùng
  `TRANG_THAI_OPTIONS`/`trangThaiDong === "Chưa đóng"`), nhưng sinh viên chỉ
  biết có hoá đơn chưa đóng **sau khi đã bấm vào** menu "Học phí". Không có
  dấu hiệu nào ở chính mục menu.
- **Yêu cầu hành chính / Yêu cầu sửa điểm**: tương tự — trạng thái "Chờ
  duyệt" chỉ thấy sau khi vào trang. `YeuCauHanhChinhPage.tsx:113` đã có sẵn
  1 biến đếm client-side `soDangChoXuLy` (lọc `trangThai === "Chờ duyệt"`)
  dùng cho `Alert` trong trang (task #17) — đây là tiền lệ tính đếm phía
  client có thể tham khảo, nhưng chưa có ở cấp menu.
- `AppLayout.tsx` **hiện không fetch dữ liệu gì** (không `useQuery`, không
  import `Badge` — đã xác nhận qua tìm kiếm toàn file) — đây sẽ là lần đầu
  tiên sidebar có dữ liệu động thay vì tĩnh.

## 2. Mục tiêu

- Sinh viên: mục "Học phí" hiện số đếm đỏ = số hoá đơn `trangThaiDong ===
  "Chưa đóng"`. Mục "Yêu cầu hành chính" hiện số đếm đỏ = số đơn của mình
  đang `"Chờ duyệt"`.
- Admin/Giáo vụ: mục "Yêu cầu hành chính" hiện số đếm đỏ = tổng số đơn
  **toàn trường** đang `"Chờ duyệt"`.
- Giáo viên: mục "Yêu cầu sửa điểm" hiện số đếm đỏ = số đơn của mình đang
  `"Chờ duyệt"`.
- Admin: mục "Yêu cầu sửa điểm" hiện số đếm đỏ = tổng số đơn đang `"Chờ
  duyệt"`.
- Số đếm = 0 thì không hiện badge (không có chấm đỏ trống).

## 3. Giải pháp đề xuất

### 3.1. Nguồn dữ liệu — tái dùng API sẵn có, không cần endpoint mới

Không có endpoint "đếm"/"summary" nào trong backend hiện tại (đã kiểm tra
`Count|Summary|Badge` trên toàn bộ Controllers/Services — không có). Nhưng
**không cần thêm** vì:

- Với danh sách "của chính mình" (Sinh viên xem học phí/đơn của mình, Giáo
  viên xem đơn sửa điểm của mình): gọi thẳng `hocPhiApi.getMe()`
  (`api/modules.ts:149-152`), `yeuCauHanhChinhApi.getMe()` (`:174-177`),
  `yeuCauSuaDiemApi.getMe()` (`:195-198`) — các API này vốn trả **toàn bộ**
  danh sách của người dùng đó (không phân trang), số lượng nhỏ (vài kỳ học/
  vài đơn), lọc đếm phía client là đủ rẻ.
- Với danh sách "toàn trường" (Admin/Giáo vụ đếm tổng số đơn chờ duyệt):
  `yeuCauHanhChinhApi.getAll(page, pageSize, trangThai)` và
  `yeuCauSuaDiemApi.getAll(...)` (`api/modules.ts:178-183`, `:199-204`) trả
  `PagedResultDto` có sẵn field `total` (tổng số bản ghi khớp filter, tính
  độc lập với `pageSize` — xem `PagedResult<T>` ở `types/index.ts:8-13`).
  Gọi với `page=1, pageSize=1, trangThai="Chờ duyệt"` thì chỉ cần đọc
  `.total` — **không tải danh sách đầy đủ**, tận dụng đúng cơ chế phân
  trang/lọc server-side đã có sẵn (`YeuCauHanhChinhController.cs` `GetAll`,
  `YeuCauSuaDiemController.cs:26-35` `GetAll`, cả hai đều nhận `trangThai`
  qua query string).

### 3.2. Nơi tính toán — thêm `useQuery` trực tiếp trong `AppLayout`

`AppLayout` là nơi duy nhất cần biết số đếm để vẽ badge trên menu, nên đặt
các `useQuery` ngay trong component này, gated theo `user.vaiTro`
(`enabled: ...`), theo đúng cách các trang khác đã làm (ví dụ
`HocPhiPage.tsx:170-174` gate theo `!isAdmin`). Tối đa 4 query, mỗi query
chỉ bật (`enabled`) đúng 1 vai trò tương ứng nên tại một thời điểm 1 người
dùng chỉ có tối đa 2 query thực sự chạy (Sinh viên: 2 query; Admin: 2 query;
Giáo vụ: 1 query; Giáo viên: 1 query).

**Tái dùng đúng `queryKey`** của các trang đích để React Query dùng chung
cache/dedupe request khi cả `AppLayout` lẫn trang đó cùng mount (ví dụ Sinh
viên đang đứng ở `/hoc-phi`, cache `["hoc-phi-me"]` dùng chung cho cả badge
lẫn nội dung trang, không gọi API 2 lần):

| Badge | `queryKey` tái dùng | Nguồn |
|---|---|---|
| Học phí chưa đóng (Sinh viên) | `["hoc-phi-me"]` | `HocPhiPage.tsx:171` |
| Yêu cầu hành chính chờ duyệt (Sinh viên) | `["yeu-cau-hanh-chinh-me"]` | `YeuCauHanhChinhPage.tsx:52` |
| Yêu cầu sửa điểm chờ duyệt (Giáo viên) | `["yeu-cau-sua-diem-me"]` | `YeuCauSuaDiemPage.tsx:35` |
| Yêu cầu hành chính chờ duyệt — tổng (Admin/Giáo vụ) | `["yeu-cau-hanh-chinh-pending-count"]` (mới, vì params `page=1,pageSize=1` khác với `["yeu-cau-hanh-chinh-all", filterTrangThai]` trang dùng — không dedupe được, chấp nhận query riêng) | — |
| Yêu cầu sửa điểm chờ duyệt — tổng (Admin) | `["yeu-cau-sua-diem-pending-count"]` (mới, lý do tương tự) | — |

Vì `AppLayout` bọc toàn bộ ứng dụng (`App.tsx:40`, mount 1 lần cho cả
session), các badge sẽ tự làm mới khi React Query refetch theo cơ chế mặc
định (mount lại, quay lại tab — `refetchOnWindowFocus`). Để badge "chờ
duyệt" cảm giác gần thời gian thực hơn (vd Admin vừa duyệt xong, Sinh viên
ở tab khác thấy badge tự giảm) mà không cần WebSocket (repo chưa có hạ tầng
realtime), đề xuất thêm `refetchInterval: 60_000` (60 giây) cho các query
badge — **điểm cần chốt**: 60s là mặc định đề xuất, có thể đổi.

### 3.3. Vẽ badge trên menu

Sửa đoạn dựng `items` cho `<Menu>` (`AppLayout.tsx:99-105`): thay vì map
thẳng `icon` gốc, bọc `<Badge size="small" count={badgeCounts[key] ?? 0}>`
quanh icon khi `badgeCounts[key] > 0`. `badgeCounts` là 1 object tính từ
kết quả các query ở mục 3.2, khoá theo đúng `key` menu (`/hoc-phi`,
`/yeu-cau-hanh-chinh`, `/yeu-cau-sua-diem`).

## 4. Phạm vi thay đổi

### Backend

Không có thay đổi backend — toàn bộ dựa trên API/param đã có sẵn.

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/components/AppLayout.tsx` | Import `Badge` từ antd, `useQuery` từ `@tanstack/react-query`, các api module cần dùng (`hocPhiApi`, `yeuCauHanhChinhApi`, `yeuCauSuaDiemApi`); thêm 4 `useQuery` gated theo vai trò (mục 3.2); tính `badgeCounts` (object `Record<string, number>`); sửa đoạn `items={visibleItems.map(...)}` (dòng 103) để bọc `Badge` quanh icon khi có số đếm > 0 |

### Dữ liệu / Migration

Không cần.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~0.5 ngày (chủ yếu là 4 query có điều kiện + 1 đoạn logic vẽ badge, không có phần khó) |
| Mức độ rủi ro | Thấp — chỉ thêm dữ liệu hiển thị, không đổi luồng nghiệp vụ nào; rủi ro chính là tăng nhẹ số lượng request định kỳ (4 query tối đa, polling 60s) |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert `AppLayout.tsx` |

## 6. Kế hoạch triển khai

1. Xác nhận lại với người yêu cầu điểm ở mục 0 (phạm vi vai trò của badge
   "Yêu cầu sửa điểm") trước khi code, tránh làm sai rồi phải sửa lại.
2. Thêm các `useQuery` gated theo vai trò trong `AppLayout.tsx`, dùng đúng
   `queryKey` tái sử dụng ở bảng mục 3.2.
3. Tính `badgeCounts`, sửa render `Menu items` để bọc `Badge`.
4. `tsc --noEmit`.
5. Kiểm thử trên trình duyệt với từng vai trò:
   - Sinh viên có hoá đơn "Chưa đóng" → badge đỏ đúng số trên "Học phí".
   - Sinh viên có đơn "Chờ duyệt" → badge đỏ đúng số trên "Yêu cầu hành
     chính".
   - Admin/Giáo vụ → badge đỏ trên "Yêu cầu hành chính" khớp tổng số đơn
     toàn trường đang chờ duyệt (đối chiếu với bảng lọc "Chờ duyệt" trong
     trang).
   - Giáo viên có đơn sửa điểm "Chờ duyệt" → badge đỏ trên "Yêu cầu sửa
     điểm".
   - Admin → badge đỏ trên "Yêu cầu sửa điểm" khớp tổng số đơn toàn trường.
   - Duyệt hết đơn chờ (hoặc đóng hết học phí) → badge biến mất (không hiện
     số 0).

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] Sinh viên có ≥1 hoá đơn "Chưa đóng" → menu "Học phí" hiện badge đỏ
      đúng số lượng.
- [ ] Sinh viên có ≥1 đơn hành chính "Chờ duyệt" → menu "Yêu cầu hành
      chính" hiện badge đỏ đúng số lượng.
- [ ] Admin và Giáo vụ: menu "Yêu cầu hành chính" hiện badge đỏ = tổng số
      đơn toàn trường đang "Chờ duyệt" (khớp con số trong trang khi lọc
      "Chờ duyệt").
- [ ] Giáo viên có ≥1 đơn sửa điểm "Chờ duyệt" → menu "Yêu cầu sửa điểm"
      hiện badge đỏ đúng số lượng.
- [ ] Admin: menu "Yêu cầu sửa điểm" hiện badge đỏ = tổng số đơn toàn
      trường đang "Chờ duyệt".
- [ ] Không còn mục nào đang chờ xử lý (0 hoá đơn chưa đóng / 0 đơn chờ
      duyệt) → không hiện badge (không có số 0 hiển thị).
- [ ] `tsc --noEmit` sạch.
- [ ] Đã kiểm thử trên trình duyệt với đủ các vai trò liên quan.

## 8. Ghi chú

- Xem mục 0 — task này **mặc định** áp badge "Yêu cầu sửa điểm" cho Giáo
  viên + Admin (không phải Sinh viên), vì đó là đúng nhóm vai trò hiện có
  quyền truy cập tính năng. Cần người yêu cầu xác nhận trước khi triển khai
  nếu hiểu khác.
- `refetchInterval: 60_000` cho các query badge là điểm cần chốt (mục 3.2)
  — có thể bỏ hẳn polling (chỉ refetch khi đổi trang/focus lại tab, ít
  request hơn nhưng badge "cũ" hơn cho tới khi người dùng tương tác lại),
  hoặc rút ngắn hơn nếu cần cảm giác thời gian thực hơn. Repo chưa có hạ
  tầng realtime (không WebSocket/SignalR) nên polling là lựa chọn khả thi
  duy nhất hiện tại.
- Vì `AppLayout` mount 1 lần cho toàn phiên đăng nhập, các query badge sẽ
  tồn tại suốt phiên — nên đã chọn cách tái dùng `queryKey` của trang đích
  ở những chỗ khớp được để giảm số request trùng lặp khi người dùng đang
  đứng ngay trong trang tương ứng.

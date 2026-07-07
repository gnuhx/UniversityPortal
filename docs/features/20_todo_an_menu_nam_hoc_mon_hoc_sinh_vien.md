# Ẩn menu "Năm học" và "Môn học" đối với vai trò Sinh viên

**Task:** #20
**Trạng thái:** todo
**Ngày tạo:** 2026-07-08
**Người phụ trách:** (chưa gán)

---

## 1. Bối cảnh / Vấn đề

Trong `allMenuItems` (`frontend/src/components/AppLayout.tsx:29-67`), 2 mục
"Năm học" (dòng 61) và "Môn học" (dòng 63) đều khai báo `roles: undefined` —
theo đúng cách `visibleItems` lọc menu (`AppLayout.tsx:84-86`:
`!item.roles || (user && item.roles.includes(user.vaiTro))`), `roles:
undefined` nghĩa là **hiện cho mọi vai trò**, bao gồm cả Sinh viên.

Quan trọng: nếu chỉ ẩn 2 mục này khỏi menu mà không đổi gì ở route, Sinh
viên vẫn **vào thẳng được bằng URL** `/nam-hoc` hoặc `/mon-hoc` — vì ở
`frontend/src/App.tsx:43` (`/nam-hoc`) và `:49` (`/mon-hoc`), 2 route này
nằm trực tiếp trong `<Route element={<ProtectedRoute />}>` (dòng 39) **không
có** `allowedRoles` — theo `ProtectedRoute.tsx:8-19`, không truyền
`allowedRoles` nghĩa là mọi vai trò đã đăng nhập đều qua được, chỉ chặn
người chưa đăng nhập. Ẩn-menu-mà-không-chặn-route là lỗ hổng UX thường gặp
trong repo này trước đây tương tự (không phải bug hiện có, chỉ là điểm cần
làm đúng ngay từ đầu ở task này).

## 2. Mục tiêu

- Vai trò Sinh viên không còn thấy mục "Năm học" và "Môn học" trong menu.
- Vai trò Sinh viên không truy cập được 2 trang này kể cả gõ thẳng URL.
- Các vai trò khác (Admin, Giáo vụ, Giáo viên) không bị ảnh hưởng — vẫn thấy
  và dùng được như cũ (hiện tại cả 3 vai trò này đều thấy được do
  `roles: undefined`).

## 3. Giải pháp đề xuất

Đổi `roles: undefined` thành danh sách tường minh loại trừ Sinh viên:
`roles: [ROLES.ADMIN, ROLES.GIAO_VU, ROLES.GIAO_VIEN]` cho cả 2 mục — quyết
định "ai được xem" chuyển từ ngầm định (mọi người) sang tường minh (mọi
người **trừ** Sinh viên), khớp với cách các mục khác trong file đã khai báo
tường minh theo mảng vai trò.

Song song, bọc 2 route `/nam-hoc` và `/mon-hoc` trong 1
`<ProtectedRoute allowedRoles={[ROLES.ADMIN, ROLES.GIAO_VU,
ROLES.GIAO_VIEN]}>` mới (đúng pattern các khối route khác đã có ở
`App.tsx:56, 62, 67, 75, 88, 93`), để chặn cả truy cập trực tiếp bằng URL,
không chỉ ẩn menu.

**Lưu ý cần xác nhận lại:** route `/nam-hoc/:id` (dòng 45,
`RedirectToHocKy`, chuyển hướng sang `/hoc-ky?namHocId=...`) hiện cũng mở
cho mọi vai trò, nhưng đích đến `/hoc-ky` đã là **Admin-only**
(`App.tsx:75-77`) — nên dù `/nam-hoc/:id` không bị chặn, Sinh viên đi qua nó
sẽ tự động bị `ProtectedRoute` của `/hoc-ky` chặn lại ở bước kế tiếp. Không
cần sửa route này, nhưng nêu ra để không bị hiểu nhầm là bỏ sót.

## 4. Phạm vi thay đổi

### Backend

Không có thay đổi backend — đây là hạn chế điều hướng UI, các API
`/api/nam-hoc` và `/api/mon-hoc` phía sau có thể đã tự có `[Authorize]`
riêng hay không thì task này không đổi (ngoài phạm vi; nếu muốn chặn cả API
cho Sinh viên cần xác nhận thêm, xem mục 8).

### Frontend

| File | Thay đổi |
|---|---|
| `frontend/src/components/AppLayout.tsx:61` | `roles: undefined` → `roles: [ROLES.ADMIN, ROLES.GIAO_VU, ROLES.GIAO_VIEN]` (mục "Năm học") |
| `frontend/src/components/AppLayout.tsx:63` | `roles: undefined` → `roles: [ROLES.ADMIN, ROLES.GIAO_VU, ROLES.GIAO_VIEN]` (mục "Môn học") |
| `frontend/src/App.tsx:43,49` | Bọc 2 route `/nam-hoc`, `/mon-hoc` trong `<Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN, ROLES.GIAO_VU, ROLES.GIAO_VIEN]} />}>` mới, tách khỏi khối route mở tự do hiện tại (dòng 41-53) |

### Dữ liệu / Migration

Không cần.

## 5. Đánh giá rủi ro & effort

| Hạng mục | Đánh giá |
|---|---|
| Effort ước tính | ~15-30 phút |
| Mức độ rủi ro | Rất thấp — chỉ đổi điều kiện hiển thị/điều hướng, không đổi logic nghiệp vụ |
| Ảnh hưởng dữ liệu hiện có | Không |
| Khả năng rollback | Revert 2 file trên |

## 6. Kế hoạch triển khai

1. Sửa `AppLayout.tsx` — đổi `roles` của 2 mục "Năm học"/"Môn học".
2. Sửa `App.tsx` — tách `/nam-hoc` và `/mon-hoc` ra khỏi khối route mở tự
   do, bọc trong `ProtectedRoute allowedRoles` mới. Giữ nguyên route
   `/nam-hoc/:id` (redirect) không đổi.
3. `tsc --noEmit`.
4. Kiểm thử trên trình duyệt: đăng nhập Sinh viên → xác nhận menu không còn
   "Năm học"/"Môn học"; gõ thẳng URL `/nam-hoc` và `/mon-hoc` → xác nhận bị
   đá về `/` (hành vi `ProtectedRoute` khi vai trò không khớp — xem
   `ProtectedRoute.tsx` để xác nhận đích đến chính xác khi bị chặn). Đăng
   nhập Admin/Giáo vụ/Giáo viên → xác nhận vẫn thấy và dùng được 2 trang này
   như cũ.

## 7. Tiêu chí hoàn thành (Acceptance Criteria)

- [ ] Vai trò Sinh viên: menu không còn "Năm học", "Môn học".
- [ ] Vai trò Sinh viên: gõ thẳng `/nam-hoc` hoặc `/mon-hoc` trên URL bị
      chặn, không thấy nội dung trang.
- [ ] Vai trò Admin, Giáo vụ, Giáo viên: menu và trang `/nam-hoc`,
      `/mon-hoc` không đổi so với trước.
- [ ] `tsc --noEmit` sạch.
- [ ] Đã kiểm thử trên trình duyệt với cả 4 vai trò.

## 8. Ghi chú

- Task này **chỉ chặn ở tầng UI/route frontend**. Nếu `GET /api/nam-hoc` và
  `GET /api/mon-hoc` phía backend hiện không có `[Authorize(Roles=...)]`
  riêng (rất có thể đang mở cho mọi vai trò đã đăng nhập, theo đúng tinh
  thần "danh mục dùng chung" như `MonHocController` đã thấy ở các task
  trước), thì Sinh viên vẫn gọi được API trực tiếp (qua DevTools/Postman)
  dù không vào được trang. Nếu người yêu cầu cần chặn triệt để ở tầng API,
  cần xác nhận thêm — đó sẽ là thay đổi backend, rủi ro cao hơn (có thể ảnh
  hưởng trang khác đang gọi chung API này, ví dụ dropdown chọn năm học/môn
  học ở trang khác mà Sinh viên vẫn cần đọc dữ liệu).

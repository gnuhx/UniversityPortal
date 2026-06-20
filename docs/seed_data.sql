-- ============================================================
-- UNIVERSITY PORTAL — DỮ LIỆU MẪU (SQL Server / T-SQL)
-- Chạy từng khối theo thứ tự từ trên xuống dưới
-- Mật khẩu đã được hash bằng BCrypt cost=11:
--   Admin@123     → tài khoản admin
--   Giaovu@123    → tài khoản giáo vụ
--   Giaovien@123  → tất cả giáo viên
--   Sinhvien@123  → tất cả sinh viên
-- ============================================================

-- ============================================================
-- BƯỚC 1: VAI TRÒ
-- ============================================================
SET IDENTITY_INSERT vai_tro ON;
INSERT INTO vai_tro (id, ten_vai_tro, created_at, updated_at) VALUES
  (1, N'Admin',      GETDATE(), GETDATE()),
  (2, N'Giáo viên',  GETDATE(), GETDATE()),
  (3, N'Sinh viên',  GETDATE(), GETDATE()),
  (4, N'Giáo vụ',   GETDATE(), GETDATE());
SET IDENTITY_INSERT vai_tro OFF;

-- ============================================================
-- BƯỚC 2: PHÒNG BAN
-- ============================================================
SET IDENTITY_INSERT phong_ban ON;
INSERT INTO phong_ban (id, ten_phong_ban, created_at, updated_at) VALUES
  (1, N'Phòng Đào tạo',              GETDATE(), GETDATE()),
  (2, N'Phòng Công tác Sinh viên',   GETDATE(), GETDATE()),
  (3, N'Khoa Công nghệ Thông tin',   GETDATE(), GETDATE()),
  (4, N'Khoa Kinh tế',               GETDATE(), GETDATE());
SET IDENTITY_INSERT phong_ban OFF;

-- ============================================================
-- BƯỚC 3: NGÀNH HỌC
-- ============================================================
SET IDENTITY_INSERT nganh_hoc ON;
INSERT INTO nganh_hoc (id, ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at) VALUES
  (1, N'CNTT', N'Công nghệ Thông tin',  NULL, GETDATE(), GETDATE()),
  (2, N'KTPM', N'Kỹ thuật Phần mềm',   1,    GETDATE(), GETDATE()),
  (3, N'HTTT', N'Hệ thống Thông tin',  1,    GETDATE(), GETDATE()),
  (4, N'QTKD', N'Quản trị Kinh doanh', NULL, GETDATE(), GETDATE());
SET IDENTITY_INSERT nganh_hoc OFF;

-- ============================================================
-- BƯỚC 4: NĂM HỌC
-- ============================================================
SET IDENTITY_INSERT nam_hoc ON;
INSERT INTO nam_hoc (id, ten_nam_hoc, created_at, updated_at) VALUES
  (1, N'2024-2025', GETDATE(), GETDATE());
SET IDENTITY_INSERT nam_hoc OFF;

-- ============================================================
-- BƯỚC 5: HỌC KỲ
-- ============================================================
SET IDENTITY_INSERT hoc_ky ON;
INSERT INTO hoc_ky (id, ten_hoc_ky, nam_hoc_id, ngay_bat_dau, created_at, updated_at) VALUES
  (1, N'Học kỳ 1 (2024-2025)', 1, '2024-09-02', GETDATE(), GETDATE()),
  (2, N'Học kỳ 2 (2024-2025)', 1, '2025-02-03', GETDATE(), GETDATE());
SET IDENTITY_INSERT hoc_ky OFF;

-- ============================================================
-- BƯỚC 6: TUẦN HỌC (20 tuần — HK1 2024-2025)
-- ============================================================
SET IDENTITY_INSERT tuan_hoc ON;
INSERT INTO tuan_hoc (id, nam_hoc_id, ma_tuan, so_thu_tu_tuan, ngay_bat_dau, ngay_ket_thuc, created_at, updated_at) VALUES
  ( 1, 1, N'T01_2425',  1, '2024-09-02', '2024-09-08', GETDATE(), GETDATE()),
  ( 2, 1, N'T02_2425',  2, '2024-09-09', '2024-09-15', GETDATE(), GETDATE()),
  ( 3, 1, N'T03_2425',  3, '2024-09-16', '2024-09-22', GETDATE(), GETDATE()),
  ( 4, 1, N'T04_2425',  4, '2024-09-23', '2024-09-29', GETDATE(), GETDATE()),
  ( 5, 1, N'T05_2425',  5, '2024-09-30', '2024-10-06', GETDATE(), GETDATE()),
  ( 6, 1, N'T06_2425',  6, '2024-10-07', '2024-10-13', GETDATE(), GETDATE()),
  ( 7, 1, N'T07_2425',  7, '2024-10-14', '2024-10-20', GETDATE(), GETDATE()),
  ( 8, 1, N'T08_2425',  8, '2024-10-21', '2024-10-27', GETDATE(), GETDATE()),
  ( 9, 1, N'T09_2425',  9, '2024-10-28', '2024-11-03', GETDATE(), GETDATE()),
  (10, 1, N'T10_2425', 10, '2024-11-04', '2024-11-10', GETDATE(), GETDATE()),
  (11, 1, N'T11_2425', 11, '2024-11-11', '2024-11-17', GETDATE(), GETDATE()),
  (12, 1, N'T12_2425', 12, '2024-11-18', '2024-11-24', GETDATE(), GETDATE()),
  (13, 1, N'T13_2425', 13, '2024-11-25', '2024-12-01', GETDATE(), GETDATE()),
  (14, 1, N'T14_2425', 14, '2024-12-02', '2024-12-08', GETDATE(), GETDATE()),
  (15, 1, N'T15_2425', 15, '2024-12-09', '2024-12-15', GETDATE(), GETDATE()),
  (16, 1, N'T16_2425', 16, '2024-12-16', '2024-12-22', GETDATE(), GETDATE()),
  (17, 1, N'T17_2425', 17, '2024-12-23', '2024-12-29', GETDATE(), GETDATE()),
  (18, 1, N'T18_2425', 18, '2024-12-30', '2025-01-05', GETDATE(), GETDATE()),
  (19, 1, N'T19_2425', 19, '2025-01-06', '2025-01-12', GETDATE(), GETDATE()),
  (20, 1, N'T20_2425', 20, '2025-01-13', '2025-01-19', GETDATE(), GETDATE());
SET IDENTITY_INSERT tuan_hoc OFF;

-- ============================================================
-- BƯỚC 7: MÔN HỌC
-- ============================================================
SET IDENTITY_INSERT mon_hoc ON;
INSERT INTO mon_hoc (id, ma_mon, ten_mon, created_at, updated_at) VALUES
  (1, N'INT101',  N'Nhập môn Lập trình',               GETDATE(), GETDATE()),
  (2, N'INT201',  N'Cấu trúc Dữ liệu & Giải thuật',   GETDATE(), GETDATE()),
  (3, N'INT301',  N'Lập trình Web',                    GETDATE(), GETDATE()),
  (4, N'INT302',  N'Cơ sở Dữ liệu',                   GETDATE(), GETDATE()),
  (5, N'INT401',  N'Công nghệ Phần mềm',               GETDATE(), GETDATE()),
  (6, N'MATH101', N'Toán cao cấp',                     GETDATE(), GETDATE()),
  (7, N'ENG101',  N'Tiếng Anh cơ bản',                GETDATE(), GETDATE());
SET IDENTITY_INSERT mon_hoc OFF;

-- ============================================================
-- BƯỚC 8: TÀI KHOẢN
-- ============================================================
SET IDENTITY_INSERT tai_khoan ON;
INSERT INTO tai_khoan (id, ten_dang_nhap, mat_khau, vai_tro_id, phong_ban_id, ho_ten, email, trang_thai, created_at, updated_at) VALUES
  -- Admin          mật khẩu: Admin@123
  (1,  N'admin',     N'$2a$11$OhB2x9FDm98.0RQVfXk7EeFWw0679YEpy6XM5jxmrv.70PmpR7Uwu', 1, 1, N'Quản trị viên',     N'admin@uni.edu.vn',           1, GETDATE(), GETDATE()),
  -- Giáo vụ       mật khẩu: Giaovu@123
  (2,  N'giaovu01',  N'$2a$11$BaxOzzvEtGst/jTR7eGAuuycIU2y0L6cSIT9Cz.8SMAEI42IfljN2', 4, 1, N'Nguyễn Thị Lan',    N'lan.nt@uni.edu.vn',           1, GETDATE(), GETDATE()),
  -- Giáo viên     mật khẩu: Giaovien@123
  (3,  N'gv.tuan',   N'$2a$11$s9ZqMA6/G2WZQczpSwNqieIMX2.GPADH4HeOXlufpVLPnp5m8TkZu', 2, 3, N'TS. Trần Văn Tuấn', N'tuan.tv@uni.edu.vn',          1, GETDATE(), GETDATE()),
  (4,  N'gv.hoa',    N'$2a$11$s9ZqMA6/G2WZQczpSwNqieIMX2.GPADH4HeOXlufpVLPnp5m8TkZu', 2, 3, N'ThS. Lê Thị Hoa',   N'hoa.lt@uni.edu.vn',           1, GETDATE(), GETDATE()),
  (5,  N'gv.minh',   N'$2a$11$s9ZqMA6/G2WZQczpSwNqieIMX2.GPADH4HeOXlufpVLPnp5m8TkZu', 2, 3, N'TS. Phạm Văn Minh', N'minh.pv@uni.edu.vn',          1, GETDATE(), GETDATE()),
  -- Sinh viên     mật khẩu: Sinhvien@123
  (6,  N'sv.an',     N'$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, N'Nguyễn Văn An',     N'an.nv22@sv.uni.edu.vn',       1, GETDATE(), GETDATE()),
  (7,  N'sv.binh',   N'$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, N'Trần Thị Bình',     N'binh.tt22@sv.uni.edu.vn',     1, GETDATE(), GETDATE()),
  (8,  N'sv.cuong',  N'$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, N'Lê Văn Cường',      N'cuong.lv22@sv.uni.edu.vn',    1, GETDATE(), GETDATE()),
  (9,  N'sv.dung',   N'$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, N'Phạm Thị Dung',     N'dung.pt22@sv.uni.edu.vn',     1, GETDATE(), GETDATE()),
  (10, N'sv.em',     N'$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, N'Hoàng Văn Em',      N'em.hv22@sv.uni.edu.vn',       1, GETDATE(), GETDATE()),
  (11, N'sv.phuong', N'$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, N'Vũ Thị Phương',     N'phuong.vt23@sv.uni.edu.vn',   1, GETDATE(), GETDATE()),
  (12, N'sv.quan',   N'$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, N'Đặng Văn Quân',     N'quan.dv23@sv.uni.edu.vn',     1, GETDATE(), GETDATE());
SET IDENTITY_INSERT tai_khoan OFF;

-- ============================================================
-- BƯỚC 9: GIÁO VIÊN
-- ============================================================
SET IDENTITY_INSERT giao_vien ON;
INSERT INTO giao_vien (id, tai_khoan_id, ma_gv, created_at, updated_at) VALUES
  (1, 3, N'GV001', GETDATE(), GETDATE()),
  (2, 4, N'GV002', GETDATE(), GETDATE()),
  (3, 5, N'GV003', GETDATE(), GETDATE());
SET IDENTITY_INSERT giao_vien OFF;

-- ============================================================
-- BƯỚC 10: CHƯƠNG TRÌNH ĐÀO TẠO
-- ============================================================
SET IDENTITY_INSERT chuong_trinh_dt ON;
INSERT INTO chuong_trinh_dt (id, ma_ctdt, nganh_id, khoa_hoc, created_at, updated_at) VALUES
  (1, N'KTPM-K22', 2, N'2022-2026', GETDATE(), GETDATE()),
  (2, N'HTTT-K23', 3, N'2023-2027', GETDATE(), GETDATE());
SET IDENTITY_INSERT chuong_trinh_dt OFF;

-- ============================================================
-- BƯỚC 11: LỚP SINH HOẠT
-- thu_ky_id để NULL trước — sẽ UPDATE sau khi có sinh viên
-- (tránh circular FK: lop_sinh_hoat ↔ sinh_vien)
-- ============================================================
SET IDENTITY_INSERT lop_sinh_hoat ON;
INSERT INTO lop_sinh_hoat (id, ma_lop, gvcn_id, thu_ky_id, chuong_trinh_dt_id, created_at, updated_at) VALUES
  (1, N'KTPM22A', 1, NULL, 1, GETDATE(), GETDATE()),
  (2, N'HTTT23A', 2, NULL, 2, GETDATE(), GETDATE());
SET IDENTITY_INSERT lop_sinh_hoat OFF;

-- ============================================================
-- BƯỚC 12: SINH VIÊN
-- ============================================================
SET IDENTITY_INSERT sinh_vien ON;
INSERT INTO sinh_vien (id, tai_khoan_id, mssv, lop_id, created_at, updated_at) VALUES
  (1,  6,  N'SV2200001', 1, GETDATE(), GETDATE()),
  (2,  7,  N'SV2200002', 1, GETDATE(), GETDATE()),
  (3,  8,  N'SV2200003', 1, GETDATE(), GETDATE()),
  (4,  9,  N'SV2200004', 2, GETDATE(), GETDATE()),
  (5,  10, N'SV2200005', 2, GETDATE(), GETDATE()),
  (6,  11, N'SV2300001', 2, GETDATE(), GETDATE()),
  (7,  12, N'SV2300002', 1, GETDATE(), GETDATE());
SET IDENTITY_INSERT sinh_vien OFF;

-- ============================================================
-- BƯỚC 13: GÁN THƯ KÝ LỚP
-- sv.an (id=1) làm thư ký lớp KTPM22A
-- sv.dung (id=4) làm thư ký lớp HTTT23A
-- ============================================================
UPDATE lop_sinh_hoat SET thu_ky_id = 1 WHERE id = 1;
UPDATE lop_sinh_hoat SET thu_ky_id = 4 WHERE id = 2;

-- ============================================================
-- BƯỚC 14: CHI TIẾT CHƯƠNG TRÌNH ĐÀO TẠO
-- ============================================================
SET IDENTITY_INSERT chi_tiet_ctdt ON;
INSERT INTO chi_tiet_ctdt (id, ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at) VALUES
  -- CTDT KTPM-K22
  (1, 1, 1, 1, 3, 1, GETDATE(), GETDATE()),  -- INT101  HK1
  (2, 1, 2, 2, 3, 1, GETDATE(), GETDATE()),  -- INT201  HK2
  (3, 1, 3, 2, 3, 1, GETDATE(), GETDATE()),  -- INT301  HK2
  (4, 1, 4, 1, 3, 1, GETDATE(), GETDATE()),  -- INT302  HK1
  (5, 1, 6, 1, 4, 0, GETDATE(), GETDATE()),  -- MATH101 HK1 (không tính ĐTB)
  -- CTDT HTTT-K23
  (6, 2, 1, 1, 3, 1, GETDATE(), GETDATE()),  -- INT101  HK1
  (7, 2, 4, 1, 3, 1, GETDATE(), GETDATE()),  -- INT302  HK1
  (8, 2, 7, 1, 3, 0, GETDATE(), GETDATE());  -- ENG101  HK1 (không tính ĐTB)
SET IDENTITY_INSERT chi_tiet_ctdt OFF;

-- ============================================================
-- BƯỚC 15: LỚP HỌC PHẦN
-- ============================================================
SET IDENTITY_INSERT lop_hoc_phan ON;
INSERT INTO lop_hoc_phan (id, chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at) VALUES
  (1, 1, 1, 1, N'INT101-01',  0, 0, GETDATE(), GETDATE()),  -- đang học
  (2, 4, 1, 2, N'INT302-01',  1, 1, GETDATE(), GETDATE()),  -- đã kết thúc
  (3, 5, 1, 3, N'MATH101-01', 1, 1, GETDATE(), GETDATE()),  -- đã kết thúc
  (4, 6, 1, 1, N'INT101-02',  0, 0, GETDATE(), GETDATE()),  -- đang học
  (5, 7, 1, 2, N'INT302-02',  1, 1, GETDATE(), GETDATE());  -- đã kết thúc
SET IDENTITY_INSERT lop_hoc_phan OFF;

-- ============================================================
-- BƯỚC 16: DANH SÁCH LỚP HỌC PHẦN + ĐIỂM
-- ============================================================
SET IDENTITY_INSERT danh_sach_lop_hp ON;
INSERT INTO danh_sach_lop_hp
  (id, sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet,
   diem_qt1, diem_qt2, diem_thi, diem_tong_ket, created_at, updated_at)
VALUES
  -- INT101-01 — lớp KTPM22A (đang học, chưa có điểm thi)
  (1,  1, 1, N'Học chính',     N'Đã duyệt',  8.5, 7.5, NULL, NULL, GETDATE(), GETDATE()),
  (2,  2, 1, N'Học chính',     N'Đã duyệt',  6.0, 7.0, NULL, NULL, GETDATE(), GETDATE()),
  (3,  3, 1, N'Học chính',     N'Đã duyệt',  9.0, 8.5, NULL, NULL, GETDATE(), GETDATE()),
  (4,  7, 1, N'Học chính',     N'Đã duyệt',  4.0, 5.0, NULL, NULL, GETDATE(), GETDATE()),

  -- INT302-01 — lớp KTPM22A (đã khoá bảng điểm)
  (5,  1, 2, N'Học chính',     N'Đã duyệt',  7.0, 8.0, 7.5, 7.5, GETDATE(), GETDATE()),
  (6,  2, 2, N'Học chính',     N'Đã duyệt',  5.5, 6.0, 4.0, 4.9, GETDATE(), GETDATE()),  -- rớt môn
  (7,  3, 2, N'Học cải thiện', N'Đã duyệt',  8.0, 9.0, 8.5, 8.5, GETDATE(), GETDATE()),

  -- INT101-02 — lớp HTTT23A (đang học)
  (8,  4, 4, N'Học chính',     N'Đã duyệt',  7.5, 8.0, NULL, NULL, GETDATE(), GETDATE()),
  (9,  5, 4, N'Học chính',     N'Đã duyệt',  6.5, 7.0, NULL, NULL, GETDATE(), GETDATE()),
  (10, 6, 4, N'Học chính',     N'Chờ duyệt', NULL, NULL, NULL, NULL, GETDATE(), GETDATE()),  -- chờ duyệt

  -- INT302-02 — lớp HTTT23A (đã khoá bảng điểm)
  (11, 4, 5, N'Học chính',     N'Đã duyệt',  8.0, 7.5, 8.0, 7.9, GETDATE(), GETDATE()),
  (12, 5, 5, N'Học chính',     N'Đã duyệt',  4.5, 5.0, 3.0, 3.9, GETDATE(), GETDATE());  -- rớt môn
SET IDENTITY_INSERT danh_sach_lop_hp OFF;

-- ============================================================
-- BƯỚC 17: THÔNG BÁO
-- ============================================================
SET IDENTITY_INSERT thong_bao ON;
INSERT INTO thong_bao (id, loai_thong_bao, muc_do, tieu_de, noi_dung, nguoi_tao_id, lop_nhan_id, ngay_tao, created_at, updated_at) VALUES
  (1, N'Học vụ',   N'Quan trọng',  N'Lịch thi học kỳ 1 năm học 2024-2025',          N'Phòng Đào tạo thông báo lịch thi cuối kỳ học kỳ 1 năm học 2024-2025. Sinh viên xem chi tiết lịch thi tại cổng thông tin.',                    2, 1,    '2025-05-26 08:00:00', GETDATE(), GETDATE()),
  (2, N'Học phí',  N'Khẩn cấp',   N'Nhắc đóng học phí học kỳ 1 - hạn chót 30/11/2024', N'Sinh viên chưa đóng học phí học kỳ 1 vui lòng hoàn thành trước ngày 30/11/2024. Quá hạn sẽ bị khoá tài khoản đăng ký môn.',               2, NULL, '2025-05-31 08:00:00', GETDATE(), GETDATE()),
  (3, N'Đoàn Hội', N'Bình thường', N'Chương trình tình nguyện mùa hè xanh 2025',     N'Đoàn trường tổ chức chương trình tình nguyện hè 2025. Sinh viên có nhu cầu tham gia đăng ký tại văn phòng Đoàn trường.',                       1, 2,    '2025-06-03 08:00:00', GETDATE(), GETDATE()),
  (4, N'Học vụ',   N'Bình thường', N'Kết quả xét học bổng học kỳ 1 năm học 2024-2025', N'Phòng Công tác Sinh viên công bố danh sách sinh viên được xét học bổng học kỳ 1. Sinh viên kiểm tra thông tin trên cổng thông tin.',          2, 1,    '2025-06-04 08:00:00', GETDATE(), GETDATE());
SET IDENTITY_INSERT thong_bao OFF;

-- ============================================================
-- BƯỚC 18: ĐIỂM RÈN LUYỆN
-- ============================================================
SET IDENTITY_INSERT diem_ren_luyen ON;
INSERT INTO diem_ren_luyen (id, sinh_vien_id, hoc_ky_id, diem_tong, xep_loai, created_at, updated_at) VALUES
  (1, 1, 1, 85, N'Tốt',        GETDATE(), GETDATE()),
  (2, 2, 1, 72, N'Khá',        GETDATE(), GETDATE()),
  (3, 3, 1, 90, N'Xuất sắc',   GETDATE(), GETDATE()),
  (4, 4, 1, 68, N'Khá',        GETDATE(), GETDATE()),
  (5, 5, 1, 55, N'Trung bình', GETDATE(), GETDATE()),
  (6, 6, 1, 78, N'Khá',        GETDATE(), GETDATE()),
  (7, 7, 1, 40, N'Yếu',        GETDATE(), GETDATE());
SET IDENTITY_INSERT diem_ren_luyen OFF;

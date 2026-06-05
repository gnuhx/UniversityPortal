-- ============================================================
-- UNIVERSITY PORTAL — DỮ LIỆU MẪU
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
INSERT INTO vai_tro (id, ten_vai_tro, created_at, updated_at) VALUES
  (1, 'Admin',      NOW(), NOW()),
  (2, 'Giáo viên',  NOW(), NOW()),
  (3, 'Sinh viên',  NOW(), NOW()),
  (4, 'Giáo vụ',   NOW(), NOW());

-- ============================================================
-- BƯỚC 2: PHÒNG BAN
-- ============================================================
INSERT INTO phong_ban (id, ten_phong_ban, created_at, updated_at) VALUES
  (1, 'Phòng Đào tạo',              NOW(), NOW()),
  (2, 'Phòng Công tác Sinh viên',   NOW(), NOW()),
  (3, 'Khoa Công nghệ Thông tin',   NOW(), NOW()),
  (4, 'Khoa Kinh tế',               NOW(), NOW());

-- ============================================================
-- BƯỚC 3: NGÀNH HỌC
-- ============================================================
INSERT INTO nganh_hoc (id, ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at) VALUES
  (1, 'CNTT', 'Công nghệ Thông tin',  NULL, NOW(), NOW()),
  (2, 'KTPM', 'Kỹ thuật Phần mềm',   1,    NOW(), NOW()),
  (3, 'HTTT', 'Hệ thống Thông tin',  1,    NOW(), NOW()),
  (4, 'QTKD', 'Quản trị Kinh doanh', NULL, NOW(), NOW());

-- ============================================================
-- BƯỚC 4: NĂM HỌC
-- ============================================================
INSERT INTO nam_hoc (id, ten_nam_hoc, created_at, updated_at) VALUES
  (1, '2024-2025', NOW(), NOW());

-- ============================================================
-- BƯỚC 5: HỌC KỲ
-- ============================================================
INSERT INTO hoc_ky (id, ten_hoc_ky, nam_hoc_id, ngay_bat_dau, created_at, updated_at) VALUES
  (1, 'Học kỳ 1 (2024-2025)', 1, '2024-09-02', NOW(), NOW()),
  (2, 'Học kỳ 2 (2024-2025)', 1, '2025-02-03', NOW(), NOW());

-- ============================================================
-- BƯỚC 6: TUẦN HỌC (20 tuần — HK1 2024-2025)
-- ============================================================
INSERT INTO tuan_hoc (id, nam_hoc_id, ma_tuan, so_thu_tu_tuan, ngay_bat_dau, ngay_ket_thuc, created_at, updated_at) VALUES
  ( 1, 1, 'T01_2425',  1, '2024-09-02', '2024-09-08', NOW(), NOW()),
  ( 2, 1, 'T02_2425',  2, '2024-09-09', '2024-09-15', NOW(), NOW()),
  ( 3, 1, 'T03_2425',  3, '2024-09-16', '2024-09-22', NOW(), NOW()),
  ( 4, 1, 'T04_2425',  4, '2024-09-23', '2024-09-29', NOW(), NOW()),
  ( 5, 1, 'T05_2425',  5, '2024-09-30', '2024-10-06', NOW(), NOW()),
  ( 6, 1, 'T06_2425',  6, '2024-10-07', '2024-10-13', NOW(), NOW()),
  ( 7, 1, 'T07_2425',  7, '2024-10-14', '2024-10-20', NOW(), NOW()),
  ( 8, 1, 'T08_2425',  8, '2024-10-21', '2024-10-27', NOW(), NOW()),
  ( 9, 1, 'T09_2425',  9, '2024-10-28', '2024-11-03', NOW(), NOW()),
  (10, 1, 'T10_2425', 10, '2024-11-04', '2024-11-10', NOW(), NOW()),
  (11, 1, 'T11_2425', 11, '2024-11-11', '2024-11-17', NOW(), NOW()),
  (12, 1, 'T12_2425', 12, '2024-11-18', '2024-11-24', NOW(), NOW()),
  (13, 1, 'T13_2425', 13, '2024-11-25', '2024-12-01', NOW(), NOW()),
  (14, 1, 'T14_2425', 14, '2024-12-02', '2024-12-08', NOW(), NOW()),
  (15, 1, 'T15_2425', 15, '2024-12-09', '2024-12-15', NOW(), NOW()),
  (16, 1, 'T16_2425', 16, '2024-12-16', '2024-12-22', NOW(), NOW()),
  (17, 1, 'T17_2425', 17, '2024-12-23', '2024-12-29', NOW(), NOW()),
  (18, 1, 'T18_2425', 18, '2024-12-30', '2025-01-05', NOW(), NOW()),
  (19, 1, 'T19_2425', 19, '2025-01-06', '2025-01-12', NOW(), NOW()),
  (20, 1, 'T20_2425', 20, '2025-01-13', '2025-01-19', NOW(), NOW());

-- ============================================================
-- BƯỚC 7: MÔN HỌC
-- ============================================================
INSERT INTO mon_hoc (id, ma_mon, ten_mon, created_at, updated_at) VALUES
  (1, 'INT101',  'Nhập môn Lập trình',               NOW(), NOW()),
  (2, 'INT201',  'Cấu trúc Dữ liệu & Giải thuật',   NOW(), NOW()),
  (3, 'INT301',  'Lập trình Web',                    NOW(), NOW()),
  (4, 'INT302',  'Cơ sở Dữ liệu',                   NOW(), NOW()),
  (5, 'INT401',  'Công nghệ Phần mềm',               NOW(), NOW()),
  (6, 'MATH101', 'Toán cao cấp',                     NOW(), NOW()),
  (7, 'ENG101',  'Tiếng Anh cơ bản',                NOW(), NOW());

-- ============================================================
-- BƯỚC 8: TÀI KHOẢN
-- ============================================================
INSERT INTO tai_khoan (id, ten_dang_nhap, mat_khau, vai_tro_id, phong_ban_id, ho_ten, email, trang_thai, created_at, updated_at) VALUES
  -- Admin          mật khẩu: Admin@123
  (1,  'admin',     '$2a$11$OhB2x9FDm98.0RQVfXk7EeFWw0679YEpy6XM5jxmrv.70PmpR7Uwu', 1, 1, 'Quản trị viên',     'admin@uni.edu.vn',           1, NOW(), NOW()),
  -- Giáo vụ       mật khẩu: Giaovu@123
  (2,  'giaovu01',  '$2a$11$BaxOzzvEtGst/jTR7eGAuuycIU2y0L6cSIT9Cz.8SMAEI42IfljN2', 4, 1, 'Nguyễn Thị Lan',    'lan.nt@uni.edu.vn',           1, NOW(), NOW()),
  -- Giáo viên     mật khẩu: Giaovien@123
  (3,  'gv.tuan',   '$2a$11$s9ZqMA6/G2WZQczpSwNqieIMX2.GPADH4HeOXlufpVLPnp5m8TkZu', 2, 3, 'TS. Trần Văn Tuấn', 'tuan.tv@uni.edu.vn',          1, NOW(), NOW()),
  (4,  'gv.hoa',    '$2a$11$s9ZqMA6/G2WZQczpSwNqieIMX2.GPADH4HeOXlufpVLPnp5m8TkZu', 2, 3, 'ThS. Lê Thị Hoa',   'hoa.lt@uni.edu.vn',           1, NOW(), NOW()),
  (5,  'gv.minh',   '$2a$11$s9ZqMA6/G2WZQczpSwNqieIMX2.GPADH4HeOXlufpVLPnp5m8TkZu', 2, 3, 'TS. Phạm Văn Minh', 'minh.pv@uni.edu.vn',          1, NOW(), NOW()),
  -- Sinh viên     mật khẩu: Sinhvien@123
  (6,  'sv.an',     '$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, 'Nguyễn Văn An',     'an.nv22@sv.uni.edu.vn',       1, NOW(), NOW()),
  (7,  'sv.binh',   '$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, 'Trần Thị Bình',     'binh.tt22@sv.uni.edu.vn',     1, NOW(), NOW()),
  (8,  'sv.cuong',  '$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, 'Lê Văn Cường',      'cuong.lv22@sv.uni.edu.vn',    1, NOW(), NOW()),
  (9,  'sv.dung',   '$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, 'Phạm Thị Dung',     'dung.pt22@sv.uni.edu.vn',     1, NOW(), NOW()),
  (10, 'sv.em',     '$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, 'Hoàng Văn Em',      'em.hv22@sv.uni.edu.vn',       1, NOW(), NOW()),
  (11, 'sv.phuong', '$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, 'Vũ Thị Phương',     'phuong.vt23@sv.uni.edu.vn',   1, NOW(), NOW()),
  (12, 'sv.quan',   '$2a$11$fo8YotSWdhOOYX1QSJR2NO4kSURDlWaCuhKzn.1x9WXOjOBpWVuu.', 3, 3, 'Đặng Văn Quân',     'quan.dv23@sv.uni.edu.vn',     1, NOW(), NOW());

-- ============================================================
-- BƯỚC 9: GIÁO VIÊN
-- ============================================================
INSERT INTO giao_vien (id, tai_khoan_id, ma_gv, created_at, updated_at) VALUES
  (1, 3, 'GV001', NOW(), NOW()),
  (2, 4, 'GV002', NOW(), NOW()),
  (3, 5, 'GV003', NOW(), NOW());

-- ============================================================
-- BƯỚC 10: CHƯƠNG TRÌNH ĐÀO TẠO
-- ============================================================
INSERT INTO chuong_trinh_dt (id, ma_ctdt, nganh_id, khoa_hoc, created_at, updated_at) VALUES
  (1, 'KTPM-K22', 2, '2022-2026', NOW(), NOW()),
  (2, 'HTTT-K23', 3, '2023-2027', NOW(), NOW());

-- ============================================================
-- BƯỚC 11: LỚP SINH HOẠT
-- thu_ky_id để NULL trước — sẽ UPDATE sau khi có sinh viên
-- (tránh circular FK: lop_sinh_hoat ↔ sinh_vien)
-- ============================================================
INSERT INTO lop_sinh_hoat (id, ma_lop, gvcn_id, thu_ky_id, chuong_trinh_dt_id, created_at, updated_at) VALUES
  (1, 'KTPM22A', 1, NULL, 1, NOW(), NOW()),
  (2, 'HTTT23A', 2, NULL, 2, NOW(), NOW());

-- ============================================================
-- BƯỚC 12: SINH VIÊN
-- ============================================================
INSERT INTO sinh_vien (id, tai_khoan_id, mssv, lop_id, created_at, updated_at) VALUES
  (1,  6,  'SV2200001', 1, NOW(), NOW()),
  (2,  7,  'SV2200002', 1, NOW(), NOW()),
  (3,  8,  'SV2200003', 1, NOW(), NOW()),
  (4,  9,  'SV2200004', 2, NOW(), NOW()),
  (5,  10, 'SV2200005', 2, NOW(), NOW()),
  (6,  11, 'SV2300001', 2, NOW(), NOW()),
  (7,  12, 'SV2300002', 1, NOW(), NOW());

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
INSERT INTO chi_tiet_ctdt (id, ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at) VALUES
  -- CTDT KTPM-K22
  (1, 1, 1, 1, 3, 1, NOW(), NOW()),  -- INT101  HK1
  (2, 1, 2, 2, 3, 1, NOW(), NOW()),  -- INT201  HK2
  (3, 1, 3, 2, 3, 1, NOW(), NOW()),  -- INT301  HK2
  (4, 1, 4, 1, 3, 1, NOW(), NOW()),  -- INT302  HK1
  (5, 1, 6, 1, 4, 0, NOW(), NOW()),  -- MATH101 HK1 (không tính ĐTB)
  -- CTDT HTTT-K23
  (6, 2, 1, 1, 3, 1, NOW(), NOW()),  -- INT101  HK1
  (7, 2, 4, 1, 3, 1, NOW(), NOW()),  -- INT302  HK1
  (8, 2, 7, 1, 3, 0, NOW(), NOW());  -- ENG101  HK1 (không tính ĐTB)

-- ============================================================
-- BƯỚC 15: LỚP HỌC PHẦN
-- ============================================================
INSERT INTO lop_hoc_phan (id, chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at) VALUES
  (1, 1, 1, 1, 'INT101-01',  0, 0, NOW(), NOW()),  -- đang học
  (2, 4, 1, 2, 'INT302-01',  1, 1, NOW(), NOW()),  -- đã kết thúc
  (3, 5, 1, 3, 'MATH101-01', 1, 1, NOW(), NOW()),  -- đã kết thúc
  (4, 6, 1, 1, 'INT101-02',  0, 0, NOW(), NOW()),  -- đang học
  (5, 7, 1, 2, 'INT302-02',  1, 1, NOW(), NOW());  -- đã kết thúc

-- ============================================================
-- BƯỚC 16: DANH SÁCH LỚP HỌC PHẦN + ĐIỂM
-- ============================================================
INSERT INTO danh_sach_lop_hp
  (id, sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet,
   diem_qt1, diem_qt2, diem_thi, diem_tong_ket, created_at, updated_at)
VALUES
  -- INT101-01 — lớp KTPM22A (đang học, chưa có điểm thi)
  (1,  1, 1, 'Học chính',     'Đã duyệt',  8.5, 7.5, NULL, NULL, NOW(), NOW()),
  (2,  2, 1, 'Học chính',     'Đã duyệt',  6.0, 7.0, NULL, NULL, NOW(), NOW()),
  (3,  3, 1, 'Học chính',     'Đã duyệt',  9.0, 8.5, NULL, NULL, NOW(), NOW()),
  (4,  7, 1, 'Học chính',     'Đã duyệt',  4.0, 5.0, NULL, NULL, NOW(), NOW()),

  -- INT302-01 — lớp KTPM22A (đã khoá bảng điểm)
  (5,  1, 2, 'Học chính',     'Đã duyệt',  7.0, 8.0, 7.5, 7.5, NOW(), NOW()),
  (6,  2, 2, 'Học chính',     'Đã duyệt',  5.5, 6.0, 4.0, 4.9, NOW(), NOW()),  -- rớt môn
  (7,  3, 2, 'Học cải thiện', 'Đã duyệt',  8.0, 9.0, 8.5, 8.5, NOW(), NOW()),

  -- INT101-02 — lớp HTTT23A (đang học)
  (8,  4, 4, 'Học chính',     'Đã duyệt',  7.5, 8.0, NULL, NULL, NOW(), NOW()),
  (9,  5, 4, 'Học chính',     'Đã duyệt',  6.5, 7.0, NULL, NULL, NOW(), NOW()),
  (10, 6, 4, 'Học chính',     'Chờ duyệt', NULL,NULL, NULL, NULL, NOW(), NOW()),  -- chờ duyệt

  -- INT302-02 — lớp HTTT23A (đã khoá bảng điểm)
  (11, 4, 5, 'Học chính',     'Đã duyệt',  8.0, 7.5, 8.0, 7.9, NOW(), NOW()),
  (12, 5, 5, 'Học chính',     'Đã duyệt',  4.5, 5.0, 3.0, 3.9, NOW(), NOW());  -- rớt môn

-- ============================================================
-- BƯỚC 17: THÔNG BÁO
-- ============================================================
INSERT INTO thong_bao (id, loai_thong_bao, muc_do, tieu_de, noi_dung, nguoi_tao_id, lop_nhan_id, ngay_tao, created_at, updated_at) VALUES
  (1, 'Học vụ',   'Quan trọng',  'Lịch thi học kỳ 1 năm học 2024-2025',   'Phòng Đào tạo thông báo lịch thi cuối kỳ học kỳ 1 năm học 2024-2025. Sinh viên xem chi tiết lịch thi tại cổng thông tin.',                                                                              2, 1,    '2025-05-26 08:00:00', NOW(), NOW()),
  (2, 'Học phí',  'Khẩn cấp',   'Nhắc đóng học phí học kỳ 1 - hạn chót 30/11/2024',  'Sinh viên chưa đóng học phí học kỳ 1 vui lòng hoàn thành trước ngày 30/11/2024. Quá hạn sẽ bị khoá tài khoản đăng ký môn.',                                             2, NULL, '2025-05-31 08:00:00', NOW(), NOW()),
  (3, 'Đoàn Hội', 'Bình thường', 'Chương trình tình nguyện mùa hè xanh 2025',          'Đoàn trường tổ chức chương trình tình nguyện hè 2025. Sinh viên có nhu cầu tham gia đăng ký tại văn phòng Đoàn trường.',                                                   1, 2,    '2025-06-03 08:00:00', NOW(), NOW()),
  (4, 'Học vụ',   'Bình thường', 'Kết quả xét học bổng học kỳ 1 năm học 2024-2025',   'Phòng Công tác Sinh viên công bố danh sách sinh viên được xét học bổng học kỳ 1. Sinh viên kiểm tra thông tin trên cổng thông tin.',                                       2, 1,    '2025-06-04 08:00:00', NOW(), NOW());

-- ============================================================
-- BƯỚC 18: ĐIỂM RÈN LUYỆN
-- ============================================================
INSERT INTO diem_ren_luyen (id, sinh_vien_id, hoc_ky_id, diem_tong, xep_loai, created_at, updated_at) VALUES
  (1, 1, 1, 85, 'Tốt',        NOW(), NOW()),
  (2, 2, 1, 72, 'Khá',        NOW(), NOW()),
  (3, 3, 1, 90, 'Xuất sắc',   NOW(), NOW()),
  (4, 4, 1, 68, 'Khá',        NOW(), NOW()),
  (5, 5, 1, 55, 'Trung bình', NOW(), NOW()),
  (6, 6, 1, 78, 'Khá',        NOW(), NOW()),
  (7, 7, 1, 40, 'Yếu',        NOW(), NOW());

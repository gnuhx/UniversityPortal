-- ============================================================
-- KHOA CÔNG NGHỆ THÔNG TIN — CHUẨN HOÁ CHƯƠNG TRÌNH ĐÀO TẠO
-- Bổ sung môn học + chi tiết CTĐT cho các ngành KTPM, HTTT, KHMT
-- sao cho mỗi CTĐT đạt khối lượng thực tế ~120-140 tín chỉ / 8 học kỳ.
--
-- An toàn khi chạy nhiều lần: mọi INSERT đều có điều kiện
-- NOT EXISTS / IF NOT EXISTS, không xoá hay sửa dữ liệu đã có
-- (kể cả 5 dòng chi_tiet_ctdt cũ của KTPM-K22 đang bị lop_hoc_phan
-- tham chiếu — không thể xoá vì FK RESTRICT).
--
-- Chạy từng khối theo thứ tự từ trên xuống dưới.
-- ============================================================

-- ============================================================
-- BƯỚC 1: NĂM HỌC 2025-2026, 2026-2027
-- (cần cho các khoá 2022, 2023 học đến năm cuối)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM nam_hoc WHERE ten_nam_hoc = N'2025-2026')
  INSERT INTO nam_hoc (ten_nam_hoc, created_at, updated_at) VALUES (N'2025-2026', GETDATE(), GETDATE());

IF NOT EXISTS (SELECT 1 FROM nam_hoc WHERE ten_nam_hoc = N'2026-2027')
  INSERT INTO nam_hoc (ten_nam_hoc, created_at, updated_at) VALUES (N'2026-2027', GETDATE(), GETDATE());

-- ============================================================
-- BƯỚC 2: HỌC KỲ cho 2 năm học trên
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2025-2026')
  INSERT INTO hoc_ky (ten_hoc_ky, nam_hoc_id, ngay_bat_dau, created_at, updated_at)
  SELECT N'HK1 2025-2026', id, '2025-09-01', GETDATE(), GETDATE() FROM nam_hoc WHERE ten_nam_hoc = N'2025-2026';

IF NOT EXISTS (SELECT 1 FROM hoc_ky WHERE ten_hoc_ky = N'HK2 2025-2026')
  INSERT INTO hoc_ky (ten_hoc_ky, nam_hoc_id, ngay_bat_dau, created_at, updated_at)
  SELECT N'HK2 2025-2026', id, '2026-01-05', GETDATE(), GETDATE() FROM nam_hoc WHERE ten_nam_hoc = N'2025-2026';

IF NOT EXISTS (SELECT 1 FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2026-2027')
  INSERT INTO hoc_ky (ten_hoc_ky, nam_hoc_id, ngay_bat_dau, created_at, updated_at)
  SELECT N'HK1 2026-2027', id, '2026-09-01', GETDATE(), GETDATE() FROM nam_hoc WHERE ten_nam_hoc = N'2026-2027';

IF NOT EXISTS (SELECT 1 FROM hoc_ky WHERE ten_hoc_ky = N'HK2 2026-2027')
  INSERT INTO hoc_ky (ten_hoc_ky, nam_hoc_id, ngay_bat_dau, created_at, updated_at)
  SELECT N'HK2 2026-2027', id, '2027-01-04', GETDATE(), GETDATE() FROM nam_hoc WHERE ten_nam_hoc = N'2026-2027';

-- ============================================================
-- BƯỚC 3: MÔN HỌC MỚI (dùng chung cho KTPM / HTTT / KHMT)
-- ============================================================
INSERT INTO mon_hoc (ma_mon, ten_mon, created_at, updated_at)
SELECT v.ma_mon, v.ten_mon, GETDATE(), GETDATE()
FROM (VALUES
  -- Đại cương / chính trị / thể chất — dùng chung mọi ngành
  (N'KTCT101',   N'Kinh tế chính trị Mác - Lênin'),
  (N'CNXH102',   N'Chủ nghĩa xã hội khoa học'),
  (N'LSD103',    N'Lịch sử Đảng Cộng sản Việt Nam'),
  (N'TTHCM104',  N'Tư tưởng Hồ Chí Minh'),
  (N'PLDC105',   N'Pháp luật đại cương'),
  (N'GDTC1',     N'Giáo dục thể chất 1'),
  (N'GDTC2',     N'Giáo dục thể chất 2'),
  (N'GDTC3',     N'Giáo dục thể chất 3'),
  (N'GDQP106',   N'Giáo dục Quốc phòng - An ninh'),
  (N'KNM107',    N'Kỹ năng mềm'),
  (N'TA202',     N'Tiếng Anh 2'),
  (N'TA303',     N'Tiếng Anh 3'),
  -- Chuyên ngành Kỹ thuật Phần mềm (KTPM)
  (N'KTMT201',    N'Kiến trúc máy tính'),
  (N'PTTKHT301',  N'Phân tích thiết kế hệ thống thông tin'),
  (N'LTSS402',    N'Lập trình song song và phân tán'),
  (N'MMTNC402',   N'Quản trị và an ninh mạng'),
  (N'PTUDDD403',  N'Phát triển ứng dụng đa nền tảng'),
  (N'ANM403',     N'An toàn ứng dụng Web'),
  (N'DEVOPS501',  N'DevOps và triển khai hệ thống'),
  (N'BIGDATA501', N'Big Data và phân tích dữ liệu'),
  -- Chuyên ngành Hệ thống Thông tin (HTTT)
  (N'HTTTQL401',    N'Hệ thống thông tin quản lý'),
  (N'QTCSDL402',    N'Quản trị cơ sở dữ liệu'),
  (N'KHODULIEU402', N'Kho dữ liệu và khai phá dữ liệu'),
  (N'TMDT401',      N'Thương mại điện tử'),
  (N'HTHTQD501',    N'Hệ hỗ trợ ra quyết định'),
  (N'ATHTTT501',    N'An toàn và bảo mật hệ thống thông tin'),
  (N'QTDACNTT501',  N'Quản trị dự án công nghệ thông tin'),
  (N'KTRUCDN501',   N'Kiến trúc doanh nghiệp'),
  (N'BI501',        N'Phân tích kinh doanh thông minh'),
  -- Chuyên ngành Khoa học Máy tính (KHMT)
  (N'LTTT301',      N'Lý thuyết tính toán'),
  (N'CTDLNC401',    N'Cấu trúc dữ liệu nâng cao'),
  (N'HOCMAY401',    N'Học máy'),
  (N'XLNNTN402',    N'Xử lý ngôn ngữ tự nhiên'),
  (N'XLANH402',     N'Xử lý ảnh số'),
  (N'TGMT403',      N'Thị giác máy tính'),
  (N'HDTT501',      N'Học sâu'),
  (N'TOIUUHOA401',  N'Tối ưu hóa'),
  (N'KHDL501',      N'Khoa học dữ liệu')
) AS v(ma_mon, ten_mon)
WHERE NOT EXISTS (SELECT 1 FROM mon_hoc m WHERE m.ma_mon = v.ma_mon);

-- ============================================================
-- BƯỚC 4: CTĐT KTPM2020 (khoá 2020, đã ra trường — HK1 2020-2021 → HK2 2023-2024)
-- Chương trình khung đầy đủ 45 môn / 131 tín chỉ
-- ============================================================
INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
SELECT c.id, m.id, h.id, v.so_tin_chi, v.tinh_diem_tb, GETDATE(), GETDATE()
FROM (VALUES
  -- Học kỳ 1
  (N'LTC101',   N'HK1 2020-2021', 3, 1), (N'GTTOAN',   N'HK1 2020-2021', 3, 1), (N'TTHOC',    N'HK1 2020-2021', 3, 0),
  (N'PLDC105',  N'HK1 2020-2021', 2, 1), (N'KNM107',   N'HK1 2020-2021', 2, 1), (N'GDTC1',    N'HK1 2020-2021', 1, 0),
  (N'KTCT101',  N'HK1 2020-2021', 2, 0),
  -- Học kỳ 2
  (N'CTDL201',  N'HK2 2020-2021', 4, 1), (N'DSTT102',  N'HK2 2020-2021', 3, 1), (N'NHCNTT',   N'HK2 2020-2021', 2, 0),
  (N'GDQP106',  N'HK2 2020-2021', 4, 0), (N'GDTC2',    N'HK2 2020-2021', 1, 0), (N'CNXH102',  N'HK2 2020-2021', 2, 0),
  -- Học kỳ 3
  (N'OOP301',   N'HK1 2021-2022', 3, 1), (N'CSDL301',  N'HK1 2021-2022', 4, 1), (N'XSTK201',  N'HK1 2021-2022', 3, 1),
  (N'LSD103',   N'HK1 2021-2022', 2, 0), (N'GDTC3',    N'HK1 2021-2022', 1, 0), (N'KTMT201',  N'HK1 2021-2022', 3, 1),
  (N'TA202',    N'HK1 2021-2022', 3, 1),
  -- Học kỳ 4
  (N'MMT301',   N'HK2 2021-2022', 3, 1), (N'HDH302',   N'HK2 2021-2022', 3, 1), (N'ATVT401',  N'HK2 2021-2022', 3, 1),
  (N'TTHCM104', N'HK2 2021-2022', 2, 0), (N'TA303',    N'HK2 2021-2022', 3, 1),
  -- Học kỳ 5
  (N'LTWEB401', N'HK1 2022-2023', 4, 1), (N'KTPM401',  N'HK1 2022-2023', 3, 1), (N'CNPM402',  N'HK1 2022-2023', 4, 1),
  (N'PTTKHT301',N'HK1 2022-2023', 3, 1),
  -- Học kỳ 6
  (N'MOBILE402',N'HK2 2022-2023', 3, 1), (N'TTNT402',  N'HK2 2022-2023', 3, 1), (N'KT_LT403', N'HK2 2022-2023', 3, 1),
  (N'LTSS402',  N'HK2 2022-2023', 3, 1), (N'MMTNC402', N'HK2 2022-2023', 3, 1),
  -- Học kỳ 7
  (N'KIEMTHU',  N'HK1 2023-2024', 3, 1), (N'QLDAPM',   N'HK1 2023-2024', 3, 1), (N'DDMAY501', N'HK1 2023-2024', 3, 1),
  (N'AVSV',     N'HK1 2023-2024', 2, 0), (N'PTUDDD403',N'HK1 2023-2024', 3, 1), (N'ANM403',   N'HK1 2023-2024', 3, 1),
  -- Học kỳ 8
  (N'THUCTAP',  N'HK2 2023-2024', 4, 0), (N'DOAN501',  N'HK2 2023-2024', 8, 1), (N'THAYDOI',  N'HK2 2023-2024', 2, 1),
  (N'DEVOPS501',N'HK2 2023-2024', 3, 1), (N'BIGDATA501',N'HK2 2023-2024',3, 1)
) AS v(ma_mon, ten_hoc_ky, so_tin_chi, tinh_diem_tb)
CROSS JOIN chuong_trinh_dt c
JOIN mon_hoc m ON m.ma_mon = v.ma_mon
JOIN hoc_ky h ON h.ten_hoc_ky = v.ten_hoc_ky
WHERE c.ma_ctdt = N'KTPM2020'
  AND NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt x WHERE x.ctdt_id = c.id AND x.mon_hoc_id = m.id);

-- ============================================================
-- BƯỚC 5: CTĐT KTPM2021 (đã có sẵn 25 môn / 82 TC — chỉ bổ sung 20 môn còn thiếu
-- để đạt 131 tín chỉ, dùng đúng các học kỳ HK1 2021-2022 → HK2 2024-2025 đã có sẵn)
-- ============================================================
INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
SELECT c.id, m.id, h.id, v.so_tin_chi, v.tinh_diem_tb, GETDATE(), GETDATE()
FROM (VALUES
  (N'PLDC105',  N'HK1 2021-2022', 2, 1), (N'KNM107',   N'HK1 2021-2022', 2, 1), (N'GDTC1',    N'HK1 2021-2022', 1, 0),
  (N'KTCT101',  N'HK1 2021-2022', 2, 0),
  (N'GDQP106',  N'HK2 2021-2022', 4, 0), (N'GDTC2',    N'HK2 2021-2022', 1, 0), (N'CNXH102',  N'HK2 2021-2022', 2, 0),
  (N'LSD103',   N'HK1 2022-2023', 2, 0), (N'GDTC3',    N'HK1 2022-2023', 1, 0), (N'KTMT201',  N'HK1 2022-2023', 3, 1),
  (N'TA202',    N'HK1 2022-2023', 3, 1),
  (N'TTHCM104', N'HK2 2022-2023', 2, 0), (N'TA303',    N'HK2 2022-2023', 3, 1),
  (N'PTTKHT301',N'HK1 2023-2024', 3, 1),
  (N'LTSS402',  N'HK2 2023-2024', 3, 1), (N'MMTNC402', N'HK2 2023-2024', 3, 1),
  (N'PTUDDD403',N'HK1 2024-2025', 3, 1), (N'ANM403',   N'HK1 2024-2025', 3, 1),
  (N'DEVOPS501',N'HK2 2024-2025', 3, 1), (N'BIGDATA501',N'HK2 2024-2025',3, 1)
) AS v(ma_mon, ten_hoc_ky, so_tin_chi, tinh_diem_tb)
CROSS JOIN chuong_trinh_dt c
JOIN mon_hoc m ON m.ma_mon = v.ma_mon
JOIN hoc_ky h ON h.ten_hoc_ky = v.ten_hoc_ky
WHERE c.ma_ctdt = N'KTPM2021'
  AND NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt x WHERE x.ctdt_id = c.id AND x.mon_hoc_id = m.id);

-- ============================================================
-- BƯỚC 6: CTĐT KTPM-K22 (khoá 2022-2026 — đã có sẵn 5 môn/16 TC cũ,
-- KHÔNG xoá vì đang bị lớp học phần tham chiếu. Bổ sung 40 môn còn lại
-- — trừ 5 môn trùng khái niệm với dữ liệu cũ (LTC101, GTTOAN, CTDL201,
-- CSDL301, LTWEB401 đã có bản tương đương INT101/MATH101/INT201/INT302/INT301)
-- → tổng đạt 129 tín chỉ)
-- ============================================================
INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
SELECT c.id, m.id, h.id, v.so_tin_chi, v.tinh_diem_tb, GETDATE(), GETDATE()
FROM (VALUES
  (N'TTHOC',    N'HK1 2022-2023', 3, 0), (N'PLDC105',  N'HK1 2022-2023', 2, 1), (N'KNM107',   N'HK1 2022-2023', 2, 1),
  (N'GDTC1',    N'HK1 2022-2023', 1, 0), (N'KTCT101',  N'HK1 2022-2023', 2, 0),
  (N'DSTT102',  N'HK2 2022-2023', 3, 1), (N'NHCNTT',   N'HK2 2022-2023', 2, 0), (N'GDQP106',  N'HK2 2022-2023', 4, 0),
  (N'GDTC2',    N'HK2 2022-2023', 1, 0), (N'CNXH102',  N'HK2 2022-2023', 2, 0),
  (N'OOP301',   N'HK1 2023-2024', 3, 1), (N'XSTK201',  N'HK1 2023-2024', 3, 1), (N'LSD103',   N'HK1 2023-2024', 2, 0),
  (N'GDTC3',    N'HK1 2023-2024', 1, 0), (N'KTMT201',  N'HK1 2023-2024', 3, 1), (N'TA202',    N'HK1 2023-2024', 3, 1),
  (N'MMT301',   N'HK2 2023-2024', 3, 1), (N'HDH302',   N'HK2 2023-2024', 3, 1), (N'ATVT401',  N'HK2 2023-2024', 3, 1),
  (N'TTHCM104', N'HK2 2023-2024', 2, 0), (N'TA303',    N'HK2 2023-2024', 3, 1),
  (N'KTPM401',  N'HK1 2024-2025', 3, 1), (N'CNPM402',  N'HK1 2024-2025', 4, 1), (N'PTTKHT301',N'HK1 2024-2025', 3, 1),
  (N'MOBILE402',N'HK2 2024-2025', 3, 1), (N'TTNT402',  N'HK2 2024-2025', 3, 1), (N'KT_LT403', N'HK2 2024-2025', 3, 1),
  (N'LTSS402',  N'HK2 2024-2025', 3, 1), (N'MMTNC402', N'HK2 2024-2025', 3, 1),
  (N'KIEMTHU',  N'HK1 2025-2026', 3, 1), (N'QLDAPM',   N'HK1 2025-2026', 3, 1), (N'DDMAY501', N'HK1 2025-2026', 3, 1),
  (N'AVSV',     N'HK1 2025-2026', 2, 0), (N'PTUDDD403',N'HK1 2025-2026', 3, 1), (N'ANM403',   N'HK1 2025-2026', 3, 1),
  (N'THUCTAP',  N'HK2 2025-2026', 4, 0), (N'DOAN501',  N'HK2 2025-2026', 8, 1), (N'THAYDOI',  N'HK2 2025-2026', 2, 1),
  (N'DEVOPS501',N'HK2 2025-2026', 3, 1), (N'BIGDATA501',N'HK2 2025-2026',3, 1)
) AS v(ma_mon, ten_hoc_ky, so_tin_chi, tinh_diem_tb)
CROSS JOIN chuong_trinh_dt c
JOIN mon_hoc m ON m.ma_mon = v.ma_mon
JOIN hoc_ky h ON h.ten_hoc_ky = v.ten_hoc_ky
WHERE c.ma_ctdt = N'KTPM-K22'
  AND NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt x WHERE x.ctdt_id = c.id AND x.mon_hoc_id = m.id);

-- ============================================================
-- BƯỚC 7: CTĐT KTPM2022 (khoá 2022, chưa có môn nào — chương trình đầy đủ 45 môn / 131 TC)
-- ============================================================
INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
SELECT c.id, m.id, h.id, v.so_tin_chi, v.tinh_diem_tb, GETDATE(), GETDATE()
FROM (VALUES
  (N'LTC101',   N'HK1 2022-2023', 3, 1), (N'GTTOAN',   N'HK1 2022-2023', 3, 1), (N'TTHOC',    N'HK1 2022-2023', 3, 0),
  (N'PLDC105',  N'HK1 2022-2023', 2, 1), (N'KNM107',   N'HK1 2022-2023', 2, 1), (N'GDTC1',    N'HK1 2022-2023', 1, 0),
  (N'KTCT101',  N'HK1 2022-2023', 2, 0),
  (N'CTDL201',  N'HK2 2022-2023', 4, 1), (N'DSTT102',  N'HK2 2022-2023', 3, 1), (N'NHCNTT',   N'HK2 2022-2023', 2, 0),
  (N'GDQP106',  N'HK2 2022-2023', 4, 0), (N'GDTC2',    N'HK2 2022-2023', 1, 0), (N'CNXH102',  N'HK2 2022-2023', 2, 0),
  (N'OOP301',   N'HK1 2023-2024', 3, 1), (N'CSDL301',  N'HK1 2023-2024', 4, 1), (N'XSTK201',  N'HK1 2023-2024', 3, 1),
  (N'LSD103',   N'HK1 2023-2024', 2, 0), (N'GDTC3',    N'HK1 2023-2024', 1, 0), (N'KTMT201',  N'HK1 2023-2024', 3, 1),
  (N'TA202',    N'HK1 2023-2024', 3, 1),
  (N'MMT301',   N'HK2 2023-2024', 3, 1), (N'HDH302',   N'HK2 2023-2024', 3, 1), (N'ATVT401',  N'HK2 2023-2024', 3, 1),
  (N'TTHCM104', N'HK2 2023-2024', 2, 0), (N'TA303',    N'HK2 2023-2024', 3, 1),
  (N'LTWEB401', N'HK1 2024-2025', 4, 1), (N'KTPM401',  N'HK1 2024-2025', 3, 1), (N'CNPM402',  N'HK1 2024-2025', 4, 1),
  (N'PTTKHT301',N'HK1 2024-2025', 3, 1),
  (N'MOBILE402',N'HK2 2024-2025', 3, 1), (N'TTNT402',  N'HK2 2024-2025', 3, 1), (N'KT_LT403', N'HK2 2024-2025', 3, 1),
  (N'LTSS402',  N'HK2 2024-2025', 3, 1), (N'MMTNC402', N'HK2 2024-2025', 3, 1),
  (N'KIEMTHU',  N'HK1 2025-2026', 3, 1), (N'QLDAPM',   N'HK1 2025-2026', 3, 1), (N'DDMAY501', N'HK1 2025-2026', 3, 1),
  (N'AVSV',     N'HK1 2025-2026', 2, 0), (N'PTUDDD403',N'HK1 2025-2026', 3, 1), (N'ANM403',   N'HK1 2025-2026', 3, 1),
  (N'THUCTAP',  N'HK2 2025-2026', 4, 0), (N'DOAN501',  N'HK2 2025-2026', 8, 1), (N'THAYDOI',  N'HK2 2025-2026', 2, 1),
  (N'DEVOPS501',N'HK2 2025-2026', 3, 1), (N'BIGDATA501',N'HK2 2025-2026',3, 1)
) AS v(ma_mon, ten_hoc_ky, so_tin_chi, tinh_diem_tb)
CROSS JOIN chuong_trinh_dt c
JOIN mon_hoc m ON m.ma_mon = v.ma_mon
JOIN hoc_ky h ON h.ten_hoc_ky = v.ten_hoc_ky
WHERE c.ma_ctdt = N'KTPM2022'
  AND NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt x WHERE x.ctdt_id = c.id AND x.mon_hoc_id = m.id);

-- ============================================================
-- BƯỚC 8: CTĐT KTPM2023 (khoá 2023, chưa có môn nào — chương trình đầy đủ 45 môn / 131 TC)
-- ============================================================
INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
SELECT c.id, m.id, h.id, v.so_tin_chi, v.tinh_diem_tb, GETDATE(), GETDATE()
FROM (VALUES
  (N'LTC101',   N'HK1 2023-2024', 3, 1), (N'GTTOAN',   N'HK1 2023-2024', 3, 1), (N'TTHOC',    N'HK1 2023-2024', 3, 0),
  (N'PLDC105',  N'HK1 2023-2024', 2, 1), (N'KNM107',   N'HK1 2023-2024', 2, 1), (N'GDTC1',    N'HK1 2023-2024', 1, 0),
  (N'KTCT101',  N'HK1 2023-2024', 2, 0),
  (N'CTDL201',  N'HK2 2023-2024', 4, 1), (N'DSTT102',  N'HK2 2023-2024', 3, 1), (N'NHCNTT',   N'HK2 2023-2024', 2, 0),
  (N'GDQP106',  N'HK2 2023-2024', 4, 0), (N'GDTC2',    N'HK2 2023-2024', 1, 0), (N'CNXH102',  N'HK2 2023-2024', 2, 0),
  (N'OOP301',   N'HK1 2024-2025', 3, 1), (N'CSDL301',  N'HK1 2024-2025', 4, 1), (N'XSTK201',  N'HK1 2024-2025', 3, 1),
  (N'LSD103',   N'HK1 2024-2025', 2, 0), (N'GDTC3',    N'HK1 2024-2025', 1, 0), (N'KTMT201',  N'HK1 2024-2025', 3, 1),
  (N'TA202',    N'HK1 2024-2025', 3, 1),
  (N'MMT301',   N'HK2 2024-2025', 3, 1), (N'HDH302',   N'HK2 2024-2025', 3, 1), (N'ATVT401',  N'HK2 2024-2025', 3, 1),
  (N'TTHCM104', N'HK2 2024-2025', 2, 0), (N'TA303',    N'HK2 2024-2025', 3, 1),
  (N'LTWEB401', N'HK1 2025-2026', 4, 1), (N'KTPM401',  N'HK1 2025-2026', 3, 1), (N'CNPM402',  N'HK1 2025-2026', 4, 1),
  (N'PTTKHT301',N'HK1 2025-2026', 3, 1),
  (N'MOBILE402',N'HK2 2025-2026', 3, 1), (N'TTNT402',  N'HK2 2025-2026', 3, 1), (N'KT_LT403', N'HK2 2025-2026', 3, 1),
  (N'LTSS402',  N'HK2 2025-2026', 3, 1), (N'MMTNC402', N'HK2 2025-2026', 3, 1),
  (N'KIEMTHU',  N'HK1 2026-2027', 3, 1), (N'QLDAPM',   N'HK1 2026-2027', 3, 1), (N'DDMAY501', N'HK1 2026-2027', 3, 1),
  (N'AVSV',     N'HK1 2026-2027', 2, 0), (N'PTUDDD403',N'HK1 2026-2027', 3, 1), (N'ANM403',   N'HK1 2026-2027', 3, 1),
  (N'THUCTAP',  N'HK2 2026-2027', 4, 0), (N'DOAN501',  N'HK2 2026-2027', 8, 1), (N'THAYDOI',  N'HK2 2026-2027', 2, 1),
  (N'DEVOPS501',N'HK2 2026-2027', 3, 1), (N'BIGDATA501',N'HK2 2026-2027',3, 1)
) AS v(ma_mon, ten_hoc_ky, so_tin_chi, tinh_diem_tb)
CROSS JOIN chuong_trinh_dt c
JOIN mon_hoc m ON m.ma_mon = v.ma_mon
JOIN hoc_ky h ON h.ten_hoc_ky = v.ten_hoc_ky
WHERE c.ma_ctdt = N'KTPM2023'
  AND NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt x WHERE x.ctdt_id = c.id AND x.mon_hoc_id = m.id);

-- ============================================================
-- BƯỚC 9: CTĐT HTTT2021 (khoá 2021, chưa có môn nào — chương trình đầy đủ 45 môn / 128 TC)
-- ============================================================
INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
SELECT c.id, m.id, h.id, v.so_tin_chi, v.tinh_diem_tb, GETDATE(), GETDATE()
FROM (VALUES
  (N'INT101',   N'HK1 2021-2022', 3, 1), (N'TTHOC',    N'HK1 2021-2022', 3, 0), (N'PLDC105',  N'HK1 2021-2022', 2, 1),
  (N'KNM107',   N'HK1 2021-2022', 2, 1), (N'GDTC1',    N'HK1 2021-2022', 1, 0),
  (N'GTTOAN',   N'HK2 2021-2022', 3, 1), (N'ENG101',   N'HK2 2021-2022', 3, 0), (N'GDQP106',  N'HK2 2021-2022', 4, 0),
  (N'GDTC2',    N'HK2 2021-2022', 1, 0), (N'KTCT101',  N'HK2 2021-2022', 2, 0),
  (N'DSTT102',  N'HK1 2022-2023', 3, 1), (N'NHCNTT',   N'HK1 2022-2023', 2, 0), (N'OOP301',   N'HK1 2022-2023', 3, 1),
  (N'CNXH102',  N'HK1 2022-2023', 2, 0), (N'GDTC3',    N'HK1 2022-2023', 1, 0),
  (N'CSDL301',  N'HK2 2022-2023', 4, 1), (N'XSTK201',  N'HK2 2022-2023', 3, 1), (N'LSD103',   N'HK2 2022-2023', 2, 0),
  (N'TA202',    N'HK2 2022-2023', 3, 1),
  (N'MMT301',   N'HK1 2023-2024', 3, 1), (N'HDH302',   N'HK1 2023-2024', 3, 1), (N'TTHCM104', N'HK1 2023-2024', 2, 0),
  (N'TA303',    N'HK1 2023-2024', 3, 1), (N'PTTKHT301',N'HK1 2023-2024', 3, 1),
  (N'ATVT401',  N'HK2 2023-2024', 3, 1), (N'KTMT201',  N'HK2 2023-2024', 3, 1), (N'HTTTQL401',N'HK2 2023-2024', 3, 1),
  (N'QTCSDL402',N'HK2 2023-2024', 3, 1), (N'KHODULIEU402',N'HK2 2023-2024',3, 1), (N'TTNT402',N'HK2 2023-2024', 3, 1),
  (N'TMDT401',  N'HK1 2024-2025', 3, 1), (N'HTHTQD501',N'HK1 2024-2025', 3, 1), (N'ATHTTT501',N'HK1 2024-2025', 3, 1),
  (N'AVSV',     N'HK1 2024-2025', 2, 0), (N'KIEMTHU',  N'HK1 2024-2025', 3, 1), (N'MMTNC402', N'HK1 2024-2025', 3, 1),
  (N'QLDAPM',   N'HK1 2024-2025', 3, 1),
  (N'THUCTAP',  N'HK2 2024-2025', 4, 0), (N'DOAN501',  N'HK2 2024-2025', 8, 1), (N'THAYDOI',  N'HK2 2024-2025', 2, 1),
  (N'QTDACNTT501',N'HK2 2024-2025',3, 1), (N'KTRUCDN501',N'HK2 2024-2025',3, 1), (N'BI501',   N'HK2 2024-2025', 3, 1),
  (N'DDMAY501', N'HK2 2024-2025', 3, 1), (N'BIGDATA501',N'HK2 2024-2025',3, 1)
) AS v(ma_mon, ten_hoc_ky, so_tin_chi, tinh_diem_tb)
CROSS JOIN chuong_trinh_dt c
JOIN mon_hoc m ON m.ma_mon = v.ma_mon
JOIN hoc_ky h ON h.ten_hoc_ky = v.ten_hoc_ky
WHERE c.ma_ctdt = N'HTTT2021'
  AND NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt x WHERE x.ctdt_id = c.id AND x.mon_hoc_id = m.id);

-- ============================================================
-- BƯỚC 10: CTĐT HTTT-K23 (khoá 2023-2027 — đã có sẵn 3 môn/9 TC cũ: INT101, INT302, ENG101.
-- Bổ sung phần còn lại của chương trình HTTT, bỏ CSDL301 vì trùng khái niệm
-- với INT302 đã có → tổng đạt 124 tín chỉ. INT101/ENG101 tự động không bị
-- trùng lặp nhờ điều kiện NOT EXISTS vì trùng đúng mon_hoc_id.)
-- ============================================================
INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
SELECT c.id, m.id, h.id, v.so_tin_chi, v.tinh_diem_tb, GETDATE(), GETDATE()
FROM (VALUES
  (N'INT101',   N'HK1 2023-2024', 3, 1), (N'TTHOC',    N'HK1 2023-2024', 3, 0), (N'PLDC105',  N'HK1 2023-2024', 2, 1),
  (N'KNM107',   N'HK1 2023-2024', 2, 1), (N'GDTC1',    N'HK1 2023-2024', 1, 0),
  (N'GTTOAN',   N'HK2 2023-2024', 3, 1), (N'ENG101',   N'HK2 2023-2024', 3, 0), (N'GDQP106',  N'HK2 2023-2024', 4, 0),
  (N'GDTC2',    N'HK2 2023-2024', 1, 0), (N'KTCT101',  N'HK2 2023-2024', 2, 0),
  (N'DSTT102',  N'HK1 2024-2025', 3, 1), (N'NHCNTT',   N'HK1 2024-2025', 2, 0), (N'OOP301',   N'HK1 2024-2025', 3, 1),
  (N'CNXH102',  N'HK1 2024-2025', 2, 0), (N'GDTC3',    N'HK1 2024-2025', 1, 0),
  (N'XSTK201',  N'HK2 2024-2025', 3, 1), (N'LSD103',   N'HK2 2024-2025', 2, 0), (N'TA202',    N'HK2 2024-2025', 3, 1),
  (N'MMT301',   N'HK1 2025-2026', 3, 1), (N'HDH302',   N'HK1 2025-2026', 3, 1), (N'TTHCM104', N'HK1 2025-2026', 2, 0),
  (N'TA303',    N'HK1 2025-2026', 3, 1), (N'PTTKHT301',N'HK1 2025-2026', 3, 1),
  (N'ATVT401',  N'HK2 2025-2026', 3, 1), (N'KTMT201',  N'HK2 2025-2026', 3, 1), (N'HTTTQL401',N'HK2 2025-2026', 3, 1),
  (N'QTCSDL402',N'HK2 2025-2026', 3, 1), (N'KHODULIEU402',N'HK2 2025-2026',3, 1), (N'TTNT402',N'HK2 2025-2026', 3, 1),
  (N'TMDT401',  N'HK1 2026-2027', 3, 1), (N'HTHTQD501',N'HK1 2026-2027', 3, 1), (N'ATHTTT501',N'HK1 2026-2027', 3, 1),
  (N'AVSV',     N'HK1 2026-2027', 2, 0), (N'KIEMTHU',  N'HK1 2026-2027', 3, 1), (N'MMTNC402', N'HK1 2026-2027', 3, 1),
  (N'QLDAPM',   N'HK1 2026-2027', 3, 1),
  (N'THUCTAP',  N'HK2 2026-2027', 4, 0), (N'DOAN501',  N'HK2 2026-2027', 8, 1), (N'THAYDOI',  N'HK2 2026-2027', 2, 1),
  (N'QTDACNTT501',N'HK2 2026-2027',3, 1), (N'KTRUCDN501',N'HK2 2026-2027',3, 1), (N'BI501',   N'HK2 2026-2027', 3, 1),
  (N'DDMAY501', N'HK2 2026-2027', 3, 1), (N'BIGDATA501',N'HK2 2026-2027',3, 1)
) AS v(ma_mon, ten_hoc_ky, so_tin_chi, tinh_diem_tb)
CROSS JOIN chuong_trinh_dt c
JOIN mon_hoc m ON m.ma_mon = v.ma_mon
JOIN hoc_ky h ON h.ten_hoc_ky = v.ten_hoc_ky
WHERE c.ma_ctdt = N'HTTT-K23'
  AND NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt x WHERE x.ctdt_id = c.id AND x.mon_hoc_id = m.id);

-- ============================================================
-- BƯỚC 11: CTĐT KHMT2021 (khoá 2021, chưa có môn nào — chương trình đầy đủ 45 môn / 127 TC)
-- ============================================================
INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
SELECT c.id, m.id, h.id, v.so_tin_chi, v.tinh_diem_tb, GETDATE(), GETDATE()
FROM (VALUES
  (N'INT101',   N'HK1 2021-2022', 3, 1), (N'TTHOC',    N'HK1 2021-2022', 3, 0), (N'PLDC105',  N'HK1 2021-2022', 2, 1),
  (N'KNM107',   N'HK1 2021-2022', 2, 1), (N'GDTC1',    N'HK1 2021-2022', 1, 0),
  (N'GTTOAN',   N'HK2 2021-2022', 3, 1), (N'ENG101',   N'HK2 2021-2022', 3, 0), (N'GDQP106',  N'HK2 2021-2022', 4, 0),
  (N'GDTC2',    N'HK2 2021-2022', 1, 0), (N'KTCT101',  N'HK2 2021-2022', 2, 0),
  (N'DSTT102',  N'HK1 2022-2023', 3, 1), (N'NHCNTT',   N'HK1 2022-2023', 2, 0), (N'OOP301',   N'HK1 2022-2023', 3, 1),
  (N'CNXH102',  N'HK1 2022-2023', 2, 0), (N'GDTC3',    N'HK1 2022-2023', 1, 0),
  (N'CSDL301',  N'HK2 2022-2023', 4, 1), (N'XSTK201',  N'HK2 2022-2023', 3, 1), (N'LSD103',   N'HK2 2022-2023', 2, 0),
  (N'TA202',    N'HK2 2022-2023', 3, 1),
  (N'MMT301',   N'HK1 2023-2024', 3, 1), (N'HDH302',   N'HK1 2023-2024', 3, 1), (N'TTHCM104', N'HK1 2023-2024', 2, 0),
  (N'TA303',    N'HK1 2023-2024', 3, 1), (N'PTTKHT301',N'HK1 2023-2024', 3, 1),
  (N'ATVT401',  N'HK2 2023-2024', 3, 1), (N'KTMT201',  N'HK2 2023-2024', 3, 1), (N'HOCMAY401',N'HK2 2023-2024', 3, 1),
  (N'XLNNTN402',N'HK2 2023-2024', 3, 1), (N'XLANH402', N'HK2 2023-2024', 3, 1), (N'TTNT402',  N'HK2 2023-2024', 3, 1),
  (N'TGMT403',  N'HK1 2024-2025', 3, 1), (N'HDTT501',  N'HK1 2024-2025', 3, 1), (N'TOIUUHOA401',N'HK1 2024-2025',3, 1),
  (N'AVSV',     N'HK1 2024-2025', 2, 0), (N'KIEMTHU',  N'HK1 2024-2025', 3, 1), (N'MMTNC402', N'HK1 2024-2025', 3, 1),
  (N'QLDAPM',   N'HK1 2024-2025', 3, 1),
  (N'THUCTAP',  N'HK2 2024-2025', 4, 0), (N'DOAN501',  N'HK2 2024-2025', 8, 1), (N'THAYDOI',  N'HK2 2024-2025', 2, 1),
  (N'LTTT301',  N'HK2 2024-2025', 3, 1), (N'CTDLNC401',N'HK2 2024-2025', 3, 1), (N'KHDL501',  N'HK2 2024-2025', 3, 1),
  (N'DDMAY501', N'HK2 2024-2025', 3, 1), (N'BIGDATA501',N'HK2 2024-2025',3, 1)
) AS v(ma_mon, ten_hoc_ky, so_tin_chi, tinh_diem_tb)
CROSS JOIN chuong_trinh_dt c
JOIN mon_hoc m ON m.ma_mon = v.ma_mon
JOIN hoc_ky h ON h.ten_hoc_ky = v.ten_hoc_ky
WHERE c.ma_ctdt = N'KHMT2021'
  AND NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt x WHERE x.ctdt_id = c.id AND x.mon_hoc_id = m.id);

-- ============================================================
-- BƯỚC 12: KIỂM TRA KẾT QUẢ
-- ============================================================
SELECT c.ma_ctdt, n.ten_nganh, c.khoa_hoc, COUNT(*) AS so_mon, SUM(ct.so_tin_chi) AS tong_tin_chi
FROM chi_tiet_ctdt ct
JOIN chuong_trinh_dt c ON c.id = ct.ctdt_id
JOIN nganh_hoc n ON n.id = c.nganh_id
WHERE c.ma_ctdt IN (N'KTPM2020', N'KTPM2021', N'KTPM-K22', N'KTPM2022', N'KTPM2023', N'HTTT2021', N'HTTT-K23', N'KHMT2021')
GROUP BY c.ma_ctdt, n.ten_nganh, c.khoa_hoc
ORDER BY c.ma_ctdt;

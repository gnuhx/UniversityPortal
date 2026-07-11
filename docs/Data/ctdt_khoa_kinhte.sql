-- ============================================================
-- KHOA KINH TẾ — CHUẨN HOÁ CHƯƠNG TRÌNH ĐÀO TẠO
-- Ngành Quản trị Kinh doanh (QTKD) hiện CHƯA có Chương trình đào tạo
-- nào trong hệ thống. Script này tạo CTĐT QTKD2021 (khoá 2021, dùng
-- các học kỳ HK1 2021-2022 → HK2 2024-2025 đã có sẵn — không cần thêm
-- năm học mới) và nạp đầy đủ 47 môn / 130 tín chỉ, 8 học kỳ.
--
-- File này độc lập với ctdt_khoa_cntt.sql (tự thêm mọi môn học cần
-- dùng, kể cả TMDT401 nếu chưa có). An toàn khi chạy nhiều lần nhờ
-- IF NOT EXISTS / NOT EXISTS ở mọi bước.
--
-- Chạy từng khối theo thứ tự từ trên xuống dưới.
-- ============================================================

-- ============================================================
-- BƯỚC 1: MÔN HỌC MỚI CHO NGÀNH QTKD
-- ============================================================
INSERT INTO mon_hoc (ma_mon, ten_mon, created_at, updated_at)
SELECT v.ma_mon, v.ten_mon, GETDATE(), GETDATE()
FROM (VALUES
  -- Đại cương / chính trị / thể chất (dùng chung, có thể trùng với file CNTT)
  (N'KTCT101',  N'Kinh tế chính trị Mác - Lênin'),
  (N'CNXH102',  N'Chủ nghĩa xã hội khoa học'),
  (N'LSD103',   N'Lịch sử Đảng Cộng sản Việt Nam'),
  (N'TTHCM104', N'Tư tưởng Hồ Chí Minh'),
  (N'PLDC105',  N'Pháp luật đại cương'),
  (N'GDTC1',    N'Giáo dục thể chất 1'),
  (N'GDTC2',    N'Giáo dục thể chất 2'),
  (N'GDTC3',    N'Giáo dục thể chất 3'),
  (N'GDQP106',  N'Giáo dục Quốc phòng - An ninh'),
  (N'KNM107',   N'Kỹ năng mềm'),
  (N'TA202',    N'Tiếng Anh 2'),
  (N'TA303',    N'Tiếng Anh 3'),
  (N'TOANKT',   N'Toán kinh tế'),
  (N'THDC',     N'Tin học ứng dụng trong kinh doanh'),
  -- Cơ sở khối ngành Kinh tế
  (N'KTEVM',    N'Kinh tế vi mô'),
  (N'KTEVX',    N'Kinh tế vĩ mô'),
  (N'NLKT',     N'Nguyên lý kế toán'),
  (N'QTH',      N'Quản trị học'),
  (N'NLTK',     N'Nguyên lý thống kê kinh tế'),
  (N'LUATKD',   N'Pháp luật kinh doanh'),
  (N'MKT101',   N'Marketing căn bản'),
  (N'TCTT',     N'Tài chính tiền tệ'),
  (N'KTLUONG',  N'Kinh tế lượng'),
  (N'HVTC',     N'Hành vi tổ chức'),
  -- Chuyên ngành Quản trị Kinh doanh
  (N'QTNL',     N'Quản trị nguồn nhân lực'),
  (N'QTTC',     N'Quản trị tài chính doanh nghiệp'),
  (N'QTSX',     N'Quản trị sản xuất và tác nghiệp'),
  (N'QTCL',     N'Quản trị chiến lược'),
  (N'QTMKT',    N'Quản trị Marketing'),
  (N'KTQT',     N'Kinh doanh quốc tế'),
  (N'TMDT401',  N'Thương mại điện tử'),
  (N'QTRR',     N'Quản trị rủi ro doanh nghiệp'),
  (N'VHDN',     N'Văn hóa doanh nghiệp và đạo đức kinh doanh'),
  (N'KTOANQT',  N'Kế toán quản trị'),
  -- Tự chọn / thực tế doanh nghiệp
  (N'DAMPHAN',  N'Kỹ năng đàm phán trong kinh doanh'),
  (N'QTBH',     N'Quản trị bán hàng'),
  (N'NCTT101',  N'Nghiên cứu thị trường'),
  (N'QTCUNGUNG',N'Quản trị chuỗi cung ứng'),
  (N'LAPKHDN',  N'Lập kế hoạch kinh doanh'),
  (N'TTQT',     N'Thanh toán quốc tế')
) AS v(ma_mon, ten_mon)
WHERE NOT EXISTS (SELECT 1 FROM mon_hoc m WHERE m.ma_mon = v.ma_mon);

-- ============================================================
-- BƯỚC 2: TẠO CHƯƠNG TRÌNH ĐÀO TẠO QTKD2021 (chưa tồn tại)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM chuong_trinh_dt WHERE ma_ctdt = N'QTKD2021')
  INSERT INTO chuong_trinh_dt (ma_ctdt, nganh_id, khoa_hoc, created_at, updated_at)
  SELECT N'QTKD2021', id, N'2021', GETDATE(), GETDATE()
  FROM nganh_hoc WHERE ma_nganh = N'QTKD';

-- ============================================================
-- BƯỚC 3: CHI TIẾT CTĐT QTKD2021 — 47 môn / 130 tín chỉ, 8 học kỳ
-- (HK1 2021-2022 → HK2 2024-2025)
-- ============================================================
INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
SELECT c.id, m.id, h.id, v.so_tin_chi, v.tinh_diem_tb, GETDATE(), GETDATE()
FROM (VALUES
  -- Học kỳ 1
  (N'TTHOC',    N'HK1 2021-2022', 3, 0), (N'PLDC105',  N'HK1 2021-2022', 2, 1), (N'KNM107',   N'HK1 2021-2022', 2, 1),
  (N'GDTC1',    N'HK1 2021-2022', 1, 0), (N'THDC',     N'HK1 2021-2022', 3, 1),
  -- Học kỳ 2
  (N'ENG101',   N'HK2 2021-2022', 3, 0), (N'KTCT101',  N'HK2 2021-2022', 2, 0), (N'GDTC2',    N'HK2 2021-2022', 1, 0),
  (N'GDQP106',  N'HK2 2021-2022', 4, 0), (N'MATH101',  N'HK2 2021-2022', 4, 0),
  -- Học kỳ 3
  (N'CNXH102',  N'HK1 2022-2023', 2, 0), (N'GDTC3',    N'HK1 2022-2023', 1, 0), (N'TOANKT',   N'HK1 2022-2023', 3, 1),
  (N'QTH',      N'HK1 2022-2023', 3, 1), (N'NLKT',     N'HK1 2022-2023', 3, 1),
  -- Học kỳ 4
  (N'LSD103',   N'HK2 2022-2023', 2, 0), (N'TA202',    N'HK2 2022-2023', 3, 1), (N'KTEVM',    N'HK2 2022-2023', 3, 1),
  (N'KTEVX',    N'HK2 2022-2023', 3, 1), (N'LUATKD',   N'HK2 2022-2023', 2, 1),
  -- Học kỳ 5
  (N'TTHCM104', N'HK1 2023-2024', 2, 0), (N'TA303',    N'HK1 2023-2024', 3, 1), (N'NLTK',     N'HK1 2023-2024', 3, 1),
  (N'MKT101',   N'HK1 2023-2024', 3, 1), (N'TCTT',     N'HK1 2023-2024', 3, 1),
  -- Học kỳ 6
  (N'AVSV',     N'HK2 2023-2024', 2, 0), (N'KTLUONG',  N'HK2 2023-2024', 3, 1), (N'HVTC',     N'HK2 2023-2024', 3, 1),
  (N'QTNL',     N'HK2 2023-2024', 3, 1), (N'QTTC',     N'HK2 2023-2024', 3, 1), (N'QTSX',     N'HK2 2023-2024', 3, 1),
  -- Học kỳ 7
  (N'QTCL',     N'HK1 2024-2025', 3, 1), (N'QTMKT',    N'HK1 2024-2025', 3, 1), (N'KTQT',     N'HK1 2024-2025', 3, 1),
  (N'TMDT401',  N'HK1 2024-2025', 3, 1), (N'QTRR',     N'HK1 2024-2025', 3, 1), (N'VHDN',     N'HK1 2024-2025', 2, 1),
  (N'KTOANQT',  N'HK1 2024-2025', 3, 1),
  -- Học kỳ 8
  (N'THUCTAP',  N'HK2 2024-2025', 4, 0), (N'DOAN501',  N'HK2 2024-2025', 8, 1), (N'THAYDOI',  N'HK2 2024-2025', 2, 1),
  (N'DAMPHAN',  N'HK2 2024-2025', 2, 1), (N'QTBH',     N'HK2 2024-2025', 3, 1), (N'NCTT101',  N'HK2 2024-2025', 3, 1),
  (N'QTCUNGUNG',N'HK2 2024-2025', 3, 1), (N'LAPKHDN',  N'HK2 2024-2025', 2, 1), (N'TTQT',     N'HK2 2024-2025', 3, 1)
) AS v(ma_mon, ten_hoc_ky, so_tin_chi, tinh_diem_tb)
CROSS JOIN chuong_trinh_dt c
JOIN mon_hoc m ON m.ma_mon = v.ma_mon
JOIN hoc_ky h ON h.ten_hoc_ky = v.ten_hoc_ky
WHERE c.ma_ctdt = N'QTKD2021'
  AND NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt x WHERE x.ctdt_id = c.id AND x.mon_hoc_id = m.id);

-- ============================================================
-- BƯỚC 4: KIỂM TRA KẾT QUẢ
-- ============================================================
SELECT c.ma_ctdt, n.ten_nganh, c.khoa_hoc, COUNT(*) AS so_mon, SUM(ct.so_tin_chi) AS tong_tin_chi
FROM chi_tiet_ctdt ct
JOIN chuong_trinh_dt c ON c.id = ct.ctdt_id
JOIN nganh_hoc n ON n.id = c.nganh_id
WHERE c.ma_ctdt = N'QTKD2021'
GROUP BY c.ma_ctdt, n.ten_nganh, c.khoa_hoc;

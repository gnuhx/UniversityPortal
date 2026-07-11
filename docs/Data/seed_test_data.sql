-- ============================================================
-- UNIVERSITY PORTAL — TEST DATA SEED SCRIPT
-- Password tất cả tài khoản test: 123456
-- Chạy trên SQL Server (idempotent — chạy lại an toàn)
-- ============================================================
SET NOCOUNT ON;
DECLARE @now  DATETIME2 = GETDATE();
DECLARE @hash NVARCHAR(255) = N'$2a$11$jquqmdmdmcSArYg8yvKfnuboDqa7VvdUn/xpOSG0NEXg5ytP11Dtu';

DECLARE @vt_sv  INT = (SELECT Id FROM vai_tro WHERE ten_vai_tro = N'Sinh viên');
DECLARE @vt_gv  INT = (SELECT Id FROM vai_tro WHERE ten_vai_tro = N'Giáo viên');

-- ============================================================
-- [1] NĂM HỌC
-- ============================================================
MERGE nam_hoc AS t USING (VALUES
    (N'2020-2021'),(N'2021-2022'),(N'2022-2023'),(N'2023-2024'),(N'2024-2025')
) AS s(ten) ON t.ten_nam_hoc = s.ten
WHEN NOT MATCHED THEN INSERT (ten_nam_hoc, created_at, updated_at) VALUES (s.ten, @now, @now);

-- ============================================================
-- [2] HỌC KỲ
-- ============================================================
MERGE hoc_ky AS t USING (VALUES
    (N'HK1 2020-2021', N'2020-2021', '2020-09-01'),
    (N'HK2 2020-2021', N'2020-2021', '2021-01-11'),
    (N'HK1 2021-2022', N'2021-2022', '2021-09-01'),
    (N'HK2 2021-2022', N'2021-2022', '2022-01-10'),
    (N'HK1 2022-2023', N'2022-2023', '2022-09-01'),
    (N'HK2 2022-2023', N'2022-2023', '2023-01-09'),
    (N'HK1 2023-2024', N'2023-2024', '2023-09-01'),
    (N'HK2 2023-2024', N'2023-2024', '2024-01-08'),
    (N'HK1 2024-2025', N'2024-2025', '2024-09-02'),
    (N'HK2 2024-2025', N'2024-2025', '2025-01-06')
) AS s(ten_hk, ten_nh, ngay) ON t.ten_hoc_ky = s.ten_hk
WHEN NOT MATCHED THEN INSERT (ten_hoc_ky, nam_hoc_id, ngay_bat_dau, created_at, updated_at)
    VALUES (s.ten_hk, (SELECT Id FROM nam_hoc WHERE ten_nam_hoc = s.ten_nh), s.ngay, @now, @now);

-- ============================================================
-- [3] NGÀNH HỌC
-- ============================================================
MERGE nganh_hoc AS t USING (VALUES
    (N'CNTT',   N'Công nghệ thông tin',         NULL),
    (N'KTPM',   N'Kỹ thuật phần mềm',           N'CNTT'),
    (N'HTTT',   N'Hệ thống thông tin',           N'CNTT'),
    (N'KHMT',   N'Khoa học máy tính',            N'CNTT')
) AS s(ma, ten, cha) ON t.ma_nganh = s.ma
WHEN NOT MATCHED THEN INSERT (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
    VALUES (s.ma, s.ten,
            (SELECT Id FROM nganh_hoc WHERE ma_nganh = s.cha),
            @now, @now);

-- ============================================================
-- [4] CHƯƠNG TRÌNH ĐÀO TẠO  (1 CTDT per ngành per khóa)
-- ============================================================
MERGE chuong_trinh_dt AS t USING (VALUES
    (N'KTPM2020', N'KTPM', N'2020'),
    (N'KTPM2021', N'KTPM', N'2021'),
    (N'KTPM2022', N'KTPM', N'2022'),
    (N'KTPM2023', N'KTPM', N'2023'),
    (N'HTTT2021', N'HTTT', N'2021'),
    (N'KHMT2021', N'KHMT', N'2021')
) AS s(ma, ma_nganh, khoa) ON t.ma_ctdt = s.ma
WHEN NOT MATCHED THEN INSERT (ma_ctdt, nganh_id, khoa_hoc, created_at, updated_at)
    VALUES (s.ma, (SELECT Id FROM nganh_hoc WHERE ma_nganh = s.ma_nganh), s.khoa, @now, @now);

-- ============================================================
-- [5] MÔN HỌC  (25 môn)
-- ============================================================
MERGE mon_hoc AS t USING (VALUES
    -- Năm 1
    (N'LTC101',   N'Lập trình căn bản'),
    (N'GTTOAN',   N'Giải tích'),
    (N'TTHOC',    N'Triết học Mác-Lênin'),
    (N'CTDL201',  N'Cấu trúc dữ liệu và giải thuật'),
    (N'DSTT102',  N'Đại số tuyến tính'),
    (N'NHCNTT',   N'Nhập môn Công nghệ thông tin'),
    -- Năm 2
    (N'OOP301',   N'Lập trình hướng đối tượng'),
    (N'CSDL301',  N'Cơ sở dữ liệu'),
    (N'XSTK201',  N'Xác suất thống kê'),
    (N'MMT301',   N'Mạng máy tính'),
    (N'HDH302',   N'Hệ điều hành'),
    (N'ATVT401',  N'An toàn thông tin'),
    -- Năm 3
    (N'LTWEB401', N'Lập trình Web'),
    (N'KTPM401',  N'Kiến trúc phần mềm'),
    (N'CNPM402',  N'Công nghệ phần mềm'),
    (N'MOBILE402',N'Phát triển ứng dụng di động'),
    (N'TTNT402',  N'Trí tuệ nhân tạo'),
    (N'KT_LT403', N'Kỹ thuật lập trình nâng cao'),
    -- Năm 4
    (N'KIEMTHU',  N'Kiểm thử phần mềm'),
    (N'QLDAPM',   N'Quản lý dự án phần mềm'),
    (N'DDMAY501', N'Điện toán đám mây'),
    (N'THAYDOI',  N'Khởi nghiệp và đổi mới sáng tạo'),
    (N'THUCTAP',  N'Thực tập tốt nghiệp'),
    (N'DOAN501',  N'Đồ án tốt nghiệp'),
    (N'AVSV',     N'Tiếng Anh chuyên ngành')
) AS s(ma, ten) ON t.ma_mon = s.ma
WHEN NOT MATCHED THEN INSERT (ma_mon, ten_mon, created_at, updated_at)
    VALUES (s.ma, s.ten, @now, @now);

-- ============================================================
-- [6] CHI TIẾT CTDT — KTPM2021 (dùng chung cho K2021, K2022, K2023 demo)
-- Bố cục: HK1-HK2 năm1 → HK1-HK2 năm2 → ... → HK1-HK2 năm4
-- ============================================================
DECLARE @ctdt INT = (SELECT Id FROM chuong_trinh_dt WHERE ma_ctdt = N'KTPM2021');

-- Các hoc_ky IDs
DECLARE @hk1_2122 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2021-2022');
DECLARE @hk2_2122 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK2 2021-2022');
DECLARE @hk1_2223 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2022-2023');
DECLARE @hk2_2223 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK2 2022-2023');
DECLARE @hk1_2324 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2023-2024');
DECLARE @hk2_2324 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK2 2023-2024');
DECLARE @hk1_2425 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2024-2025');
DECLARE @hk2_2425 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK2 2024-2025');

MERGE chi_tiet_ctdt AS t
USING (VALUES
    -- Năm 1 HK1
    (@ctdt, N'LTC101',   @hk1_2122, 3, 1),
    (@ctdt, N'GTTOAN',   @hk1_2122, 3, 1),
    (@ctdt, N'TTHOC',    @hk1_2122, 3, 0),
    -- Năm 1 HK2
    (@ctdt, N'CTDL201',  @hk2_2122, 4, 1),
    (@ctdt, N'DSTT102',  @hk2_2122, 3, 1),
    (@ctdt, N'NHCNTT',   @hk2_2122, 2, 0),
    -- Năm 2 HK1
    (@ctdt, N'OOP301',   @hk1_2223, 3, 1),
    (@ctdt, N'CSDL301',  @hk1_2223, 4, 1),
    (@ctdt, N'XSTK201',  @hk1_2223, 3, 1),
    -- Năm 2 HK2
    (@ctdt, N'MMT301',   @hk2_2223, 3, 1),
    (@ctdt, N'HDH302',   @hk2_2223, 3, 1),
    (@ctdt, N'ATVT401',  @hk2_2223, 3, 1),
    -- Năm 3 HK1
    (@ctdt, N'LTWEB401', @hk1_2324, 4, 1),
    (@ctdt, N'KTPM401',  @hk1_2324, 3, 1),
    (@ctdt, N'CNPM402',  @hk1_2324, 4, 1),
    -- Năm 3 HK2
    (@ctdt, N'MOBILE402',@hk2_2324, 3, 1),
    (@ctdt, N'TTNT402',  @hk2_2324, 3, 1),
    (@ctdt, N'KT_LT403', @hk2_2324, 3, 1),
    -- Năm 4 HK1
    (@ctdt, N'KIEMTHU',  @hk1_2425, 3, 1),
    (@ctdt, N'QLDAPM',   @hk1_2425, 3, 1),
    (@ctdt, N'DDMAY501', @hk1_2425, 3, 1),
    (@ctdt, N'AVSV',     @hk1_2425, 2, 0),
    -- Năm 4 HK2
    (@ctdt, N'THUCTAP',  @hk2_2425, 4, 0),
    (@ctdt, N'DOAN501',  @hk2_2425, 8, 1),
    (@ctdt, N'THAYDOI',  @hk2_2425, 2, 0)
) AS s(ctdt_id, ma_mon, hoc_ky_id, so_tc, tinh_dtb)
ON t.ctdt_id = s.ctdt_id
   AND t.mon_hoc_id = (SELECT Id FROM mon_hoc WHERE ma_mon = s.ma_mon)
   AND t.hoc_ky_id  = s.hoc_ky_id
WHEN NOT MATCHED THEN INSERT (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
    VALUES (s.ctdt_id,
            (SELECT Id FROM mon_hoc WHERE ma_mon = s.ma_mon),
            s.hoc_ky_id, s.so_tc, s.tinh_dtb, @now, @now);

-- ============================================================
-- [7] GIÁO VIÊN (thêm 3 GV demo nếu chưa có)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'gv.nguyen.tuan')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'gv.nguyen.tuan', @hash, @vt_gv, N'Nguyễn Văn Tuấn', N'tuan.nv@university.edu.vn', 1, @now, @now);
    INSERT INTO giao_vien (tai_khoan_id, ma_gv, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'GV001', @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'gv.tran.mai')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'gv.tran.mai', @hash, @vt_gv, N'Trần Thị Mai', N'mai.tt@university.edu.vn', 1, @now, @now);
    INSERT INTO giao_vien (tai_khoan_id, ma_gv, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'GV002', @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'gv.le.hung')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'gv.le.hung', @hash, @vt_gv, N'Lê Quốc Hùng', N'hung.lq@university.edu.vn', 1, @now, @now);
    INSERT INTO giao_vien (tai_khoan_id, ma_gv, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'GV003', @now, @now);
END

DECLARE @gv1 INT = (SELECT g.Id FROM giao_vien g JOIN tai_khoan t ON g.tai_khoan_id = t.Id WHERE t.ten_dang_nhap = N'gv.nguyen.tuan');
DECLARE @gv2 INT = (SELECT g.Id FROM giao_vien g JOIN tai_khoan t ON g.tai_khoan_id = t.Id WHERE t.ten_dang_nhap = N'gv.tran.mai');
DECLARE @gv3 INT = (SELECT g.Id FROM giao_vien g JOIN tai_khoan t ON g.tai_khoan_id = t.Id WHERE t.ten_dang_nhap = N'gv.le.hung');

-- ============================================================
-- [8] LỚP SINH HOẠT
-- ============================================================
DECLARE @ctdt2020 INT = (SELECT Id FROM chuong_trinh_dt WHERE ma_ctdt = N'KTPM2020');
DECLARE @ctdt2021 INT = (SELECT Id FROM chuong_trinh_dt WHERE ma_ctdt = N'KTPM2021');
DECLARE @ctdt2022 INT = (SELECT Id FROM chuong_trinh_dt WHERE ma_ctdt = N'KTPM2022');
DECLARE @ctdt2023 INT = (SELECT Id FROM chuong_trinh_dt WHERE ma_ctdt = N'KTPM2023');

MERGE lop_sinh_hoat AS t USING (VALUES
    (N'KTPM2020A', @gv1, @ctdt2020),
    (N'KTPM2021A', @gv1, @ctdt2021),
    (N'KTPM2022A', @gv2, @ctdt2022),
    (N'KTPM2023A', @gv3, @ctdt2023)
) AS s(ma_lop, gvcn, ctdt_id) ON t.ma_lop = s.ma_lop
WHEN NOT MATCHED THEN INSERT (ma_lop, gvcn_id, chuong_trinh_dt_id, created_at, updated_at)
    VALUES (s.ma_lop, s.gvcn, s.ctdt_id, @now, @now);

DECLARE @lop2020 INT = (SELECT Id FROM lop_sinh_hoat WHERE ma_lop = N'KTPM2020A');
DECLARE @lop2021 INT = (SELECT Id FROM lop_sinh_hoat WHERE ma_lop = N'KTPM2021A');
DECLARE @lop2022 INT = (SELECT Id FROM lop_sinh_hoat WHERE ma_lop = N'KTPM2022A');
DECLARE @lop2023 INT = (SELECT Id FROM lop_sinh_hoat WHERE ma_lop = N'KTPM2023A');

-- ============================================================
-- [9] SINH VIÊN — TÀI KHOẢN + HỒ SƠ
-- Các kịch bản: tốt nghiệp, năm 4, năm 3, năm 2, bị khóa
-- ============================================================

-- Helper: insert tai_khoan + sinh_vien nếu chưa tồn tại
-- K2020 — TỐT NGHIỆP (trang_thai=1, lop=NULL sau khi tốt nghiệp)
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2020.001')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2020.001', @hash, @vt_sv, N'Đinh Thị Hoa', N'hoa.dt.k2020@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2020001', @lop2020, @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2020.002')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2020.002', @hash, @vt_sv, N'Bùi Văn Hùng', N'hung.bv.k2020@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2020002', @lop2020, @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2020.003')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2020.003', @hash, @vt_sv, N'Cao Thị Lan', N'lan.ct.k2020@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2020003', @lop2020, @now, @now);
END

-- K2021 — NĂM 4 (đang học HK7)
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2021.001')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2021.001', @hash, @vt_sv, N'Nguyễn Văn An', N'an.nv.k2021@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2021001', @lop2021, @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2021.002')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2021.002', @hash, @vt_sv, N'Trần Thị Bình', N'binh.tt.k2021@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2021002', @lop2021, @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2021.003')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2021.003', @hash, @vt_sv, N'Lê Quang Cường', N'cuong.lq.k2021@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2021003', @lop2021, @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2021.004')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2021.004', @hash, @vt_sv, N'Phạm Thị Dung', N'dung.pt.k2021@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2021004', @lop2021, @now, @now);
END
-- K2021 bị khóa — nợ môn quá nhiều
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2021.005')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2021.005', @hash, @vt_sv, N'Hoàng Văn Em', N'em.hv.k2021@student.edu.vn', 0, @now, @now); -- KHÓA
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2021005', @lop2021, @now, @now);
END

-- K2022 — NĂM 3
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2022.001')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2022.001', @hash, @vt_sv, N'Nguyễn Thị Phương', N'phuong.nt.k2022@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2022001', @lop2022, @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2022.002')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2022.002', @hash, @vt_sv, N'Trần Văn Quân', N'quan.tv.k2022@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2022002', @lop2022, @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2022.003')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2022.003', @hash, @vt_sv, N'Đinh Quốc Khải', N'khai.dq.k2022@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2022003', @lop2022, @now, @now);
END

-- K2023 — NĂM 2
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2023.001')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2023.001', @hash, @vt_sv, N'Phạm Thị Minh', N'minh.pt.k2023@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2023001', @lop2023, @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2023.002')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2023.002', @hash, @vt_sv, N'Bùi Văn Nam', N'nam.bv.k2023@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2023002', @lop2023, @now, @now);
END
IF NOT EXISTS (SELECT 1 FROM tai_khoan WHERE ten_dang_nhap = N'sv.k2023.003')
BEGIN
    INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, vai_tro_id, ho_ten, email, trang_thai, created_at, updated_at)
    VALUES (N'sv.k2023.003', @hash, @vt_sv, N'Nguyễn Thị Oanh', N'oanh.nt.k2023@student.edu.vn', 1, @now, @now);
    INSERT INTO sinh_vien (tai_khoan_id, mssv, lop_id, created_at, updated_at)
    VALUES (SCOPE_IDENTITY(), N'2023003', @lop2023, @now, @now);
END

-- Sinh viên IDs
DECLARE @sv_2020_1 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2020.001');
DECLARE @sv_2020_2 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2020.002');
DECLARE @sv_2020_3 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2020.003');
DECLARE @sv_2021_1 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2021.001');
DECLARE @sv_2021_2 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2021.002');
DECLARE @sv_2021_3 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2021.003');
DECLARE @sv_2021_4 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2021.004');
DECLARE @sv_2021_5 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2021.005');
DECLARE @sv_2022_1 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2022.001');
DECLARE @sv_2022_2 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2022.002');
DECLARE @sv_2022_3 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2022.003');
DECLARE @sv_2023_1 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2023.001');
DECLARE @sv_2023_2 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2023.002');
DECLARE @sv_2023_3 INT = (SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2023.003');

-- ============================================================
-- [10] LỚP HỌC PHẦN — tạo 1 section per môn per học kỳ thực tế
-- khoa_bang_diem=1 và trang_thai_ket_thuc=1 cho các HK đã kết thúc
-- ============================================================
-- Macro helper: dùng CTE để upsert
-- ma_lop_hp = [ma_mon]_[ten_hk short]  ví dụ: LTC101_HK1_2122

-- HK1 2021-2022 (năm 1 HK1 — K2021 đã hoàn thành)
MERGE lop_hoc_phan AS t USING (VALUES
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'LTC101')  AND hoc_ky_id=@hk1_2122), @hk1_2122, @gv1, N'LTC101_2122_1', 1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'GTTOAN')  AND hoc_ky_id=@hk1_2122), @hk1_2122, @gv2, N'GTTOAN_2122_1', 1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'TTHOC')   AND hoc_ky_id=@hk1_2122), @hk1_2122, @gv3, N'TTHOC_2122_1',  1, 1)
) AS s(ct_id, hk_id, gv_id, ma, khoa, ket_thuc) ON t.ma_lop_hp = s.ma
WHEN NOT MATCHED THEN INSERT (chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at)
    VALUES (s.ct_id, s.hk_id, s.gv_id, s.ma, s.khoa, s.ket_thuc, @now, @now);

-- HK2 2021-2022
MERGE lop_hoc_phan AS t USING (VALUES
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'CTDL201') AND hoc_ky_id=@hk2_2122), @hk2_2122, @gv1, N'CTDL201_2122_2', 1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'DSTT102') AND hoc_ky_id=@hk2_2122), @hk2_2122, @gv2, N'DSTT102_2122_2', 1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'NHCNTT')  AND hoc_ky_id=@hk2_2122), @hk2_2122, @gv3, N'NHCNTT_2122_2',  1, 1)
) AS s(ct_id, hk_id, gv_id, ma, khoa, ket_thuc) ON t.ma_lop_hp = s.ma
WHEN NOT MATCHED THEN INSERT (chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at)
    VALUES (s.ct_id, s.hk_id, s.gv_id, s.ma, s.khoa, s.ket_thuc, @now, @now);

-- HK1 2022-2023  (K2021 năm 2, K2022 năm 1)
MERGE lop_hoc_phan AS t USING (VALUES
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'OOP301')  AND hoc_ky_id=@hk1_2223), @hk1_2223, @gv1, N'OOP301_2223_1',  1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'CSDL301') AND hoc_ky_id=@hk1_2223), @hk1_2223, @gv2, N'CSDL301_2223_1', 1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'XSTK201') AND hoc_ky_id=@hk1_2223), @hk1_2223, @gv3, N'XSTK201_2223_1', 1, 1)
) AS s(ct_id, hk_id, gv_id, ma, khoa, ket_thuc) ON t.ma_lop_hp = s.ma
WHEN NOT MATCHED THEN INSERT (chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at)
    VALUES (s.ct_id, s.hk_id, s.gv_id, s.ma, s.khoa, s.ket_thuc, @now, @now);

-- HK2 2022-2023
MERGE lop_hoc_phan AS t USING (VALUES
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'MMT301')  AND hoc_ky_id=@hk2_2223), @hk2_2223, @gv1, N'MMT301_2223_2',  1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'HDH302')  AND hoc_ky_id=@hk2_2223), @hk2_2223, @gv2, N'HDH302_2223_2',  1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'ATVT401') AND hoc_ky_id=@hk2_2223), @hk2_2223, @gv3, N'ATVT401_2223_2', 1, 1)
) AS s(ct_id, hk_id, gv_id, ma, khoa, ket_thuc) ON t.ma_lop_hp = s.ma
WHEN NOT MATCHED THEN INSERT (chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at)
    VALUES (s.ct_id, s.hk_id, s.gv_id, s.ma, s.khoa, s.ket_thuc, @now, @now);

-- HK1 2023-2024  (K2021 năm 3, K2022 năm 2, K2023 năm 1)
MERGE lop_hoc_phan AS t USING (VALUES
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'LTWEB401') AND hoc_ky_id=@hk1_2324), @hk1_2324, @gv1, N'LTWEB401_2324_1', 1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'KTPM401')  AND hoc_ky_id=@hk1_2324), @hk1_2324, @gv2, N'KTPM401_2324_1',  1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'CNPM402')  AND hoc_ky_id=@hk1_2324), @hk1_2324, @gv3, N'CNPM402_2324_1',  1, 1)
) AS s(ct_id, hk_id, gv_id, ma, khoa, ket_thuc) ON t.ma_lop_hp = s.ma
WHEN NOT MATCHED THEN INSERT (chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at)
    VALUES (s.ct_id, s.hk_id, s.gv_id, s.ma, s.khoa, s.ket_thuc, @now, @now);

-- HK2 2023-2024
MERGE lop_hoc_phan AS t USING (VALUES
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'MOBILE402') AND hoc_ky_id=@hk2_2324), @hk2_2324, @gv1, N'MOBILE402_2324_2', 1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'TTNT402')   AND hoc_ky_id=@hk2_2324), @hk2_2324, @gv2, N'TTNT402_2324_2',   1, 1),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'KT_LT403')  AND hoc_ky_id=@hk2_2324), @hk2_2324, @gv3, N'KTLT403_2324_2',   1, 1)
) AS s(ct_id, hk_id, gv_id, ma, khoa, ket_thuc) ON t.ma_lop_hp = s.ma
WHEN NOT MATCHED THEN INSERT (chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at)
    VALUES (s.ct_id, s.hk_id, s.gv_id, s.ma, s.khoa, s.ket_thuc, @now, @now);

-- HK1 2024-2025  (K2021 năm 4, K2022 năm 3, K2023 năm 2) — ĐANG HỌC
MERGE lop_hoc_phan AS t USING (VALUES
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'KIEMTHU')  AND hoc_ky_id=@hk1_2425), @hk1_2425, @gv1, N'KIEMTHU_2425_1',  0, 0),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'QLDAPM')   AND hoc_ky_id=@hk1_2425), @hk1_2425, @gv2, N'QLDAPM_2425_1',   0, 0),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'DDMAY501') AND hoc_ky_id=@hk1_2425), @hk1_2425, @gv3, N'DDMAY501_2425_1', 0, 0),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'MOBILE402') AND hoc_ky_id=@hk2_2324), @hk1_2425, @gv1, N'MOBILE402_2425_1',0, 0),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'TTNT402')   AND hoc_ky_id=@hk2_2324), @hk1_2425, @gv2, N'TTNT402_2425_1',  0, 0),
    ((SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id=@ctdt AND mon_hoc_id=(SELECT Id FROM mon_hoc WHERE ma_mon=N'OOP301')   AND hoc_ky_id=@hk1_2223), @hk1_2425, @gv3, N'OOP301_2425_1',   0, 0)
) AS s(ct_id, hk_id, gv_id, ma, khoa, ket_thuc) ON t.ma_lop_hp = s.ma
WHEN NOT MATCHED THEN INSERT (chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at)
    VALUES (s.ct_id, s.hk_id, s.gv_id, s.ma, s.khoa, s.ket_thuc, @now, @now);

-- ============================================================
-- [11] ĐĂNG KÝ & BẢNG ĐIỂM (danh_sach_lop_hp)
-- Công thức: DiemTongKet = Qt1*0.15 + Qt2*0.15 + Thi*0.70
-- ============================================================
-- Helper để insert nếu chưa có
-- K2021 SV1 (Nguyễn Văn An — Khá, GPA ~7.8)
MERGE danh_sach_lop_hp AS t USING (VALUES
    -- HK1 2021-22
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTC101_2122_1'), N'Chính quy', N'Đã duyệt', 8.0, 7.5, 7.5, ROUND(8.0*0.15+7.5*0.15+7.5*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'GTTOAN_2122_1'), N'Chính quy', N'Đã duyệt', 7.0, 7.5, 7.0, ROUND(7.0*0.15+7.5*0.15+7.0*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTHOC_2122_1'),  N'Chính quy', N'Đã duyệt', 9.0, 8.5, 8.0, ROUND(9.0*0.15+8.5*0.15+8.0*0.70,2)),
    -- HK2 2021-22
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CTDL201_2122_2'),N'Chính quy', N'Đã duyệt', 7.0, 8.0, 7.5, ROUND(7.0*0.15+8.0*0.15+7.5*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DSTT102_2122_2'),N'Chính quy', N'Đã duyệt', 6.5, 7.0, 7.0, ROUND(6.5*0.15+7.0*0.15+7.0*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'NHCNTT_2122_2'), N'Chính quy', N'Đã duyệt', 9.0, 9.0, 8.5, ROUND(9.0*0.15+9.0*0.15+8.5*0.70,2)),
    -- HK1 2022-23
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'OOP301_2223_1'),  N'Chính quy', N'Đã duyệt', 8.0, 8.5, 8.0, ROUND(8.0*0.15+8.5*0.15+8.0*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CSDL301_2223_1'), N'Chính quy', N'Đã duyệt', 7.5, 8.0, 7.5, ROUND(7.5*0.15+8.0*0.15+7.5*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'XSTK201_2223_1'), N'Chính quy', N'Đã duyệt', 6.0, 7.0, 6.5, ROUND(6.0*0.15+7.0*0.15+6.5*0.70,2)),
    -- HK2 2022-23
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MMT301_2223_2'),   N'Chính quy', N'Đã duyệt', 7.5, 8.0, 7.5, ROUND(7.5*0.15+8.0*0.15+7.5*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'HDH302_2223_2'),   N'Chính quy', N'Đã duyệt', 8.0, 7.5, 7.0, ROUND(8.0*0.15+7.5*0.15+7.0*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'ATVT401_2223_2'),  N'Chính quy', N'Đã duyệt', 7.0, 7.5, 7.5, ROUND(7.0*0.15+7.5*0.15+7.5*0.70,2)),
    -- HK1 2023-24
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTWEB401_2324_1'), N'Chính quy', N'Đã duyệt', 8.5, 9.0, 8.5, ROUND(8.5*0.15+9.0*0.15+8.5*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KTPM401_2324_1'),  N'Chính quy', N'Đã duyệt', 7.0, 8.0, 7.5, ROUND(7.0*0.15+8.0*0.15+7.5*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CNPM402_2324_1'),  N'Chính quy', N'Đã duyệt', 7.5, 7.0, 7.5, ROUND(7.5*0.15+7.0*0.15+7.5*0.70,2)),
    -- HK2 2023-24
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MOBILE402_2324_2'),N'Chính quy', N'Đã duyệt', 8.0, 8.5, 8.0, ROUND(8.0*0.15+8.5*0.15+8.0*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTNT402_2324_2'),  N'Chính quy', N'Đã duyệt', 7.5, 7.0, 7.0, ROUND(7.5*0.15+7.0*0.15+7.0*0.70,2)),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KTLT403_2324_2'),  N'Chính quy', N'Đã duyệt', 7.0, 7.5, 7.5, ROUND(7.0*0.15+7.5*0.15+7.5*0.70,2)),
    -- HK1 2024-25 (đang học — có Qt1)
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KIEMTHU_2425_1'),  N'Chính quy', N'Đã duyệt', 8.0, NULL, NULL, NULL),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'QLDAPM_2425_1'),   N'Chính quy', N'Đã duyệt', 7.5, NULL, NULL, NULL),
    (@sv_2021_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DDMAY501_2425_1'), N'Chính quy', N'Đã duyệt', NULL,NULL, NULL, NULL)
) AS s(sv_id, lhp_id, loai, tt, qt1, qt2, thi, tong)
ON t.sinh_vien_id = s.sv_id AND t.lop_hp_id = s.lhp_id
WHEN NOT MATCHED THEN INSERT (sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet, diem_qt1, diem_qt2, diem_thi, diem_tong_ket, created_at, updated_at)
    VALUES (s.sv_id, s.lhp_id, s.loai, s.tt, s.qt1, s.qt2, s.thi, s.tong, @now, @now);

-- K2021 SV2 (Trần Thị Bình — Giỏi, GPA ~9.0)
MERGE danh_sach_lop_hp AS t USING (VALUES
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTC101_2122_1'), N'Chính quy', N'Đã duyệt', 9.5, 9.0, 9.5, ROUND(9.5*0.15+9.0*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'GTTOAN_2122_1'), N'Chính quy', N'Đã duyệt', 9.0, 9.5, 9.0, ROUND(9.0*0.15+9.5*0.15+9.0*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTHOC_2122_1'),  N'Chính quy', N'Đã duyệt',10.0, 9.5, 9.5, ROUND(10.0*0.15+9.5*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CTDL201_2122_2'),N'Chính quy', N'Đã duyệt', 9.0, 9.5, 9.0, ROUND(9.0*0.15+9.5*0.15+9.0*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DSTT102_2122_2'),N'Chính quy', N'Đã duyệt', 9.0, 8.5, 9.5, ROUND(9.0*0.15+8.5*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'NHCNTT_2122_2'), N'Chính quy', N'Đã duyệt',10.0,10.0, 9.5, ROUND(10.0*0.15+10.0*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'OOP301_2223_1'),  N'Chính quy', N'Đã duyệt', 9.5, 9.0, 9.5, ROUND(9.5*0.15+9.0*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CSDL301_2223_1'), N'Chính quy', N'Đã duyệt', 9.0, 9.5, 9.0, ROUND(9.0*0.15+9.5*0.15+9.0*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'XSTK201_2223_1'), N'Chính quy', N'Đã duyệt', 8.5, 9.0, 9.0, ROUND(8.5*0.15+9.0*0.15+9.0*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MMT301_2223_2'),   N'Chính quy', N'Đã duyệt', 9.0, 9.0, 9.5, ROUND(9.0*0.15+9.0*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'HDH302_2223_2'),   N'Chính quy', N'Đã duyệt', 9.5, 9.0, 9.0, ROUND(9.5*0.15+9.0*0.15+9.0*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'ATVT401_2223_2'),  N'Chính quy', N'Đã duyệt', 9.0, 9.5, 9.5, ROUND(9.0*0.15+9.5*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTWEB401_2324_1'), N'Chính quy', N'Đã duyệt', 9.5,10.0, 9.5, ROUND(9.5*0.15+10.0*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KTPM401_2324_1'),  N'Chính quy', N'Đã duyệt', 9.0, 9.0, 9.5, ROUND(9.0*0.15+9.0*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CNPM402_2324_1'),  N'Chính quy', N'Đã duyệt', 9.5, 9.0, 9.0, ROUND(9.5*0.15+9.0*0.15+9.0*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MOBILE402_2324_2'),N'Chính quy', N'Đã duyệt', 9.0, 9.5, 9.0, ROUND(9.0*0.15+9.5*0.15+9.0*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTNT402_2324_2'),  N'Chính quy', N'Đã duyệt', 9.5, 9.0, 9.5, ROUND(9.5*0.15+9.0*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KTLT403_2324_2'),  N'Chính quy', N'Đã duyệt',10.0, 9.5, 9.5, ROUND(10.0*0.15+9.5*0.15+9.5*0.70,2)),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KIEMTHU_2425_1'),  N'Chính quy', N'Đã duyệt', 9.5, NULL, NULL, NULL),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'QLDAPM_2425_1'),   N'Chính quy', N'Đã duyệt', 9.0, NULL, NULL, NULL),
    (@sv_2021_2,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DDMAY501_2425_1'), N'Chính quy', N'Đã duyệt', 9.5, NULL, NULL, NULL)
) AS s(sv_id, lhp_id, loai, tt, qt1, qt2, thi, tong)
ON t.sinh_vien_id = s.sv_id AND t.lop_hp_id = s.lhp_id
WHEN NOT MATCHED THEN INSERT (sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet, diem_qt1, diem_qt2, diem_thi, diem_tong_ket, created_at, updated_at)
    VALUES (s.sv_id, s.lhp_id, s.loai, s.tt, s.qt1, s.qt2, s.thi, s.tong, @now, @now);

-- K2021 SV3 (Lê Quang Cường — Trung bình, 1 môn thi lại)
MERGE danh_sach_lop_hp AS t USING (VALUES
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTC101_2122_1'), N'Chính quy', N'Đã duyệt', 6.0, 5.5, 5.5, ROUND(6.0*0.15+5.5*0.15+5.5*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'GTTOAN_2122_1'), N'Chính quy', N'Đã duyệt', 5.5, 5.0, 4.5, ROUND(5.5*0.15+5.0*0.15+4.5*0.70,2)), -- Rớt
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTHOC_2122_1'),  N'Chính quy', N'Đã duyệt', 7.0, 7.5, 6.5, ROUND(7.0*0.15+7.5*0.15+6.5*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CTDL201_2122_2'),N'Chính quy', N'Đã duyệt', 6.5, 6.0, 5.5, ROUND(6.5*0.15+6.0*0.15+5.5*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DSTT102_2122_2'),N'Chính quy', N'Đã duyệt', 5.0, 5.5, 5.5, ROUND(5.0*0.15+5.5*0.15+5.5*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'NHCNTT_2122_2'), N'Chính quy', N'Đã duyệt', 7.5, 8.0, 7.0, ROUND(7.5*0.15+8.0*0.15+7.0*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'OOP301_2223_1'),  N'Chính quy', N'Đã duyệt', 6.0, 6.5, 6.0, ROUND(6.0*0.15+6.5*0.15+6.0*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CSDL301_2223_1'), N'Chính quy', N'Đã duyệt', 5.5, 6.0, 5.5, ROUND(5.5*0.15+6.0*0.15+5.5*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'XSTK201_2223_1'), N'Chính quy', N'Đã duyệt', 6.5, 6.0, 5.5, ROUND(6.5*0.15+6.0*0.15+5.5*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MMT301_2223_2'),   N'Chính quy', N'Đã duyệt', 6.0, 6.5, 6.0, ROUND(6.0*0.15+6.5*0.15+6.0*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'HDH302_2223_2'),   N'Chính quy', N'Đã duyệt', 5.5, 5.0, 5.5, ROUND(5.5*0.15+5.0*0.15+5.5*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'ATVT401_2223_2'),  N'Chính quy', N'Đã duyệt', 6.5, 7.0, 6.0, ROUND(6.5*0.15+7.0*0.15+6.0*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTWEB401_2324_1'), N'Chính quy', N'Đã duyệt', 6.0, 6.5, 6.0, ROUND(6.0*0.15+6.5*0.15+6.0*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KTPM401_2324_1'),  N'Chính quy', N'Đã duyệt', 5.5, 6.0, 5.5, ROUND(5.5*0.15+6.0*0.15+5.5*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CNPM402_2324_1'),  N'Chính quy', N'Đã duyệt', 6.5, 6.0, 6.0, ROUND(6.5*0.15+6.0*0.15+6.0*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MOBILE402_2324_2'),N'Chính quy', N'Đã duyệt', 6.0, 6.5, 6.0, ROUND(6.0*0.15+6.5*0.15+6.0*0.70,2)),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTNT402_2324_2'),  N'Chính quy', N'Đã duyệt', 5.0, 5.5, 4.5, ROUND(5.0*0.15+5.5*0.15+4.5*0.70,2)), -- Rớt
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KTLT403_2324_2'),  N'Chính quy', N'Đã duyệt', 6.5, 6.0, 6.0, ROUND(6.5*0.15+6.0*0.15+6.0*0.70,2)),
    -- HK1 2024-25: đang học + học lại TTNT
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KIEMTHU_2425_1'),  N'Chính quy', N'Đã duyệt', 6.5, NULL, NULL, NULL),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'QLDAPM_2425_1'),   N'Chính quy', N'Đã duyệt', NULL,NULL, NULL, NULL),
    (@sv_2021_3,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTNT402_2425_1'),  N'Học lại',   N'Đã duyệt', 6.0, NULL, NULL, NULL)
) AS s(sv_id, lhp_id, loai, tt, qt1, qt2, thi, tong)
ON t.sinh_vien_id = s.sv_id AND t.lop_hp_id = s.lhp_id
WHEN NOT MATCHED THEN INSERT (sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet, diem_qt1, diem_qt2, diem_thi, diem_tong_ket, created_at, updated_at)
    VALUES (s.sv_id, s.lhp_id, s.loai, s.tt, s.qt1, s.qt2, s.thi, s.tong, @now, @now);

-- K2022 SV1 (Nguyễn Thị Phương — Khá, năm 3)
MERGE danh_sach_lop_hp AS t USING (VALUES
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTC101_2122_1'), N'Chính quy', N'Đã duyệt', 8.0, 8.5, 8.0, ROUND(8.0*0.15+8.5*0.15+8.0*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'GTTOAN_2122_1'), N'Chính quy', N'Đã duyệt', 7.5, 8.0, 7.5, ROUND(7.5*0.15+8.0*0.15+7.5*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTHOC_2122_1'),  N'Chính quy', N'Đã duyệt', 9.0, 8.5, 8.5, ROUND(9.0*0.15+8.5*0.15+8.5*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CTDL201_2122_2'),N'Chính quy', N'Đã duyệt', 8.0, 8.0, 7.5, ROUND(8.0*0.15+8.0*0.15+7.5*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DSTT102_2122_2'),N'Chính quy', N'Đã duyệt', 7.0, 7.5, 7.0, ROUND(7.0*0.15+7.5*0.15+7.0*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'NHCNTT_2122_2'), N'Chính quy', N'Đã duyệt', 9.5, 9.0, 9.0, ROUND(9.5*0.15+9.0*0.15+9.0*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'OOP301_2223_1'),  N'Chính quy', N'Đã duyệt', 8.0, 8.5, 8.0, ROUND(8.0*0.15+8.5*0.15+8.0*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CSDL301_2223_1'), N'Chính quy', N'Đã duyệt', 8.5, 8.0, 8.0, ROUND(8.5*0.15+8.0*0.15+8.0*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'XSTK201_2223_1'), N'Chính quy', N'Đã duyệt', 7.5, 7.0, 7.5, ROUND(7.5*0.15+7.0*0.15+7.5*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MMT301_2223_2'),   N'Chính quy', N'Đã duyệt', 8.0, 7.5, 8.0, ROUND(8.0*0.15+7.5*0.15+8.0*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'HDH302_2223_2'),   N'Chính quy', N'Đã duyệt', 7.5, 8.0, 7.5, ROUND(7.5*0.15+8.0*0.15+7.5*0.70,2)),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'ATVT401_2223_2'),  N'Chính quy', N'Đã duyệt', 8.0, 8.5, 8.0, ROUND(8.0*0.15+8.5*0.15+8.0*0.70,2)),
    -- Năm 3 HK1 (đang học)
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MOBILE402_2425_1'), N'Chính quy', N'Đã duyệt', 8.5, NULL, NULL, NULL),
    (@sv_2022_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTNT402_2425_1'),   N'Chính quy', N'Đã duyệt', 8.0, NULL, NULL, NULL)
) AS s(sv_id, lhp_id, loai, tt, qt1, qt2, thi, tong)
ON t.sinh_vien_id = s.sv_id AND t.lop_hp_id = s.lhp_id
WHEN NOT MATCHED THEN INSERT (sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet, diem_qt1, diem_qt2, diem_thi, diem_tong_ket, created_at, updated_at)
    VALUES (s.sv_id, s.lhp_id, s.loai, s.tt, s.qt1, s.qt2, s.thi, s.tong, @now, @now);

-- K2023 SV1 (Phạm Thị Minh — Khá, năm 2)
MERGE danh_sach_lop_hp AS t USING (VALUES
    (@sv_2023_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTC101_2122_1'), N'Chính quy', N'Đã duyệt', 8.5, 8.0, 8.0, ROUND(8.5*0.15+8.0*0.15+8.0*0.70,2)),
    (@sv_2023_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'GTTOAN_2122_1'), N'Chính quy', N'Đã duyệt', 7.5, 7.0, 7.5, ROUND(7.5*0.15+7.0*0.15+7.5*0.70,2)),
    (@sv_2023_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTHOC_2122_1'),  N'Chính quy', N'Đã duyệt', 8.0, 8.5, 8.0, ROUND(8.0*0.15+8.5*0.15+8.0*0.70,2)),
    (@sv_2023_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CTDL201_2122_2'),N'Chính quy', N'Đã duyệt', 8.0, 7.5, 7.5, ROUND(8.0*0.15+7.5*0.15+7.5*0.70,2)),
    (@sv_2023_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DSTT102_2122_2'),N'Chính quy', N'Đã duyệt', 7.0, 7.5, 7.0, ROUND(7.0*0.15+7.5*0.15+7.0*0.70,2)),
    (@sv_2023_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'NHCNTT_2122_2'), N'Chính quy', N'Đã duyệt', 9.0, 9.5, 9.0, ROUND(9.0*0.15+9.5*0.15+9.0*0.70,2)),
    -- Năm 2 (đang học)
    (@sv_2023_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'OOP301_2425_1'),  N'Chính quy', N'Đã duyệt', 8.0, NULL, NULL, NULL)
) AS s(sv_id, lhp_id, loai, tt, qt1, qt2, thi, tong)
ON t.sinh_vien_id = s.sv_id AND t.lop_hp_id = s.lhp_id
WHEN NOT MATCHED THEN INSERT (sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet, diem_qt1, diem_qt2, diem_thi, diem_tong_ket, created_at, updated_at)
    VALUES (s.sv_id, s.lhp_id, s.loai, s.tt, s.qt1, s.qt2, s.thi, s.tong, @now, @now);

-- K2020 SV1 (Đinh Thị Hoa — Tốt nghiệp loại Giỏi, GPA ~8.5)
MERGE danh_sach_lop_hp AS t USING (VALUES
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTC101_2122_1'), N'Chính quy', N'Đã duyệt', 9.0, 8.5, 8.5, ROUND(9.0*0.15+8.5*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'GTTOAN_2122_1'), N'Chính quy', N'Đã duyệt', 8.5, 8.0, 8.5, ROUND(8.5*0.15+8.0*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTHOC_2122_1'),  N'Chính quy', N'Đã duyệt', 9.5, 9.0, 9.0, ROUND(9.5*0.15+9.0*0.15+9.0*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CTDL201_2122_2'),N'Chính quy', N'Đã duyệt', 8.5, 9.0, 8.5, ROUND(8.5*0.15+9.0*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DSTT102_2122_2'),N'Chính quy', N'Đã duyệt', 8.0, 8.5, 8.5, ROUND(8.0*0.15+8.5*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'NHCNTT_2122_2'), N'Chính quy', N'Đã duyệt', 9.0, 9.5, 9.5, ROUND(9.0*0.15+9.5*0.15+9.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'OOP301_2223_1'),  N'Chính quy', N'Đã duyệt', 8.5, 9.0, 8.5, ROUND(8.5*0.15+9.0*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CSDL301_2223_1'), N'Chính quy', N'Đã duyệt', 9.0, 8.5, 8.5, ROUND(9.0*0.15+8.5*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'XSTK201_2223_1'), N'Chính quy', N'Đã duyệt', 8.0, 8.5, 8.5, ROUND(8.0*0.15+8.5*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MMT301_2223_2'),   N'Chính quy', N'Đã duyệt', 8.5, 8.0, 8.5, ROUND(8.5*0.15+8.0*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'HDH302_2223_2'),   N'Chính quy', N'Đã duyệt', 9.0, 8.5, 8.0, ROUND(9.0*0.15+8.5*0.15+8.0*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'ATVT401_2223_2'),  N'Chính quy', N'Đã duyệt', 8.5, 9.0, 8.5, ROUND(8.5*0.15+9.0*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTWEB401_2324_1'), N'Chính quy', N'Đã duyệt', 9.0, 9.5, 9.0, ROUND(9.0*0.15+9.5*0.15+9.0*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KTPM401_2324_1'),  N'Chính quy', N'Đã duyệt', 8.5, 8.0, 8.5, ROUND(8.5*0.15+8.0*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CNPM402_2324_1'),  N'Chính quy', N'Đã duyệt', 8.0, 8.5, 8.5, ROUND(8.0*0.15+8.5*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'MOBILE402_2324_2'),N'Chính quy', N'Đã duyệt', 9.0, 8.5, 8.5, ROUND(9.0*0.15+8.5*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTNT402_2324_2'),  N'Chính quy', N'Đã duyệt', 8.5, 9.0, 8.5, ROUND(8.5*0.15+9.0*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KTLT403_2324_2'),  N'Chính quy', N'Đã duyệt', 9.0, 8.5, 9.0, ROUND(9.0*0.15+8.5*0.15+9.0*0.70,2)),
    -- Năm 4 (đã hoàn thành — coi như đã nộp)
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KIEMTHU_2425_1'),  N'Chính quy', N'Đã duyệt', 9.0, 9.0, 9.0, ROUND(9.0*0.15+9.0*0.15+9.0*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'QLDAPM_2425_1'),   N'Chính quy', N'Đã duyệt', 8.5, 9.0, 8.5, ROUND(8.5*0.15+9.0*0.15+8.5*0.70,2)),
    (@sv_2020_1,(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DDMAY501_2425_1'), N'Chính quy', N'Đã duyệt', 9.0, 8.5, 9.0, ROUND(9.0*0.15+8.5*0.15+9.0*0.70,2))
) AS s(sv_id, lhp_id, loai, tt, qt1, qt2, thi, tong)
ON t.sinh_vien_id = s.sv_id AND t.lop_hp_id = s.lhp_id
WHEN NOT MATCHED THEN INSERT (sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet, diem_qt1, diem_qt2, diem_thi, diem_tong_ket, created_at, updated_at)
    VALUES (s.sv_id, s.lhp_id, s.loai, s.tt, s.qt1, s.qt2, s.thi, s.tong, @now, @now);

-- ============================================================
-- [12] HỌC PHÍ  (450,000 VND/tín chỉ)
-- K2021: 6 học kỳ đã đóng (HK1-HK6) + HK7 chưa đóng
-- K2022: 4 học kỳ đã đóng + HK5 chưa đóng
-- K2023: 2 học kỳ đã đóng
-- K2020: tất cả đã đóng (tốt nghiệp)
-- ============================================================
MERGE hoc_phi AS t USING (VALUES
    -- K2020 SV1 — đã tốt nghiệp, đóng hết
    (@sv_2020_1, @hk1_2122, 3600000, N'Đã đóng'),
    (@sv_2020_1, @hk2_2122, 4050000, N'Đã đóng'),
    (@sv_2020_1, @hk1_2223, 4500000, N'Đã đóng'),
    (@sv_2020_1, @hk2_2223, 4050000, N'Đã đóng'),
    (@sv_2020_1, @hk1_2324, 4950000, N'Đã đóng'),
    (@sv_2020_1, @hk2_2324, 4050000, N'Đã đóng'),
    (@sv_2020_1, @hk1_2425, 4050000, N'Đã đóng'),
    -- K2021 SV1 — HK1-HK6 đã đóng, HK7 chưa
    (@sv_2021_1, @hk1_2122, 4050000, N'Đã đóng'),
    (@sv_2021_1, @hk2_2122, 4050000, N'Đã đóng'),
    (@sv_2021_1, @hk1_2223, 4500000, N'Đã đóng'),
    (@sv_2021_1, @hk2_2223, 4050000, N'Đã đóng'),
    (@sv_2021_1, @hk1_2324, 4950000, N'Đã đóng'),
    (@sv_2021_1, @hk2_2324, 4050000, N'Đã đóng'),
    (@sv_2021_1, @hk1_2425, 4050000, N'Chưa đóng'),
    -- K2021 SV2
    (@sv_2021_2, @hk1_2122, 4050000, N'Đã đóng'),
    (@sv_2021_2, @hk2_2122, 4050000, N'Đã đóng'),
    (@sv_2021_2, @hk1_2223, 4500000, N'Đã đóng'),
    (@sv_2021_2, @hk2_2223, 4050000, N'Đã đóng'),
    (@sv_2021_2, @hk1_2324, 4950000, N'Đã đóng'),
    (@sv_2021_2, @hk2_2324, 4050000, N'Đã đóng'),
    (@sv_2021_2, @hk1_2425, 4050000, N'Đã đóng'),
    -- K2021 SV3 — HK5 miễn giảm
    (@sv_2021_3, @hk1_2122, 4050000, N'Đã đóng'),
    (@sv_2021_3, @hk2_2122, 4050000, N'Đã đóng'),
    (@sv_2021_3, @hk1_2223, 4500000, N'Đã đóng'),
    (@sv_2021_3, @hk2_2223, 4050000, N'Đã đóng'),
    (@sv_2021_3, @hk1_2324, 4950000, N'Miễn giảm'),
    (@sv_2021_3, @hk2_2324, 4050000, N'Đã đóng'),
    (@sv_2021_3, @hk1_2425, 4950000, N'Chưa đóng'),
    -- K2022 SV1 — 4 HK đã đóng, HK5 chưa
    (@sv_2022_1, @hk1_2122, 4050000, N'Đã đóng'),
    (@sv_2022_1, @hk2_2122, 4050000, N'Đã đóng'),
    (@sv_2022_1, @hk1_2223, 4500000, N'Đã đóng'),
    (@sv_2022_1, @hk2_2223, 4050000, N'Đã đóng'),
    (@sv_2022_1, @hk1_2425, 4050000, N'Chưa đóng'),
    -- K2023 SV1 — 2 HK đã đóng
    (@sv_2023_1, @hk1_2122, 4050000, N'Đã đóng'),
    (@sv_2023_1, @hk2_2122, 4050000, N'Đã đóng'),
    (@sv_2023_1, @hk1_2425, 4050000, N'Chưa đóng')
) AS s(sv_id, hk_id, so_tien, tt)
ON t.sinh_vien_id = s.sv_id AND t.hoc_ky_id = s.hk_id
WHEN NOT MATCHED THEN INSERT (sinh_vien_id, hoc_ky_id, so_tien, trang_thai_dong, created_at, updated_at)
    VALUES (s.sv_id, s.hk_id, s.so_tien, s.tt, @now, @now);

-- ============================================================
-- DONE — Kiểm tra kết quả
-- ============================================================
SELECT 'nam_hoc'          AS bang, COUNT(*) AS so_luong FROM nam_hoc
UNION ALL SELECT 'hoc_ky',         COUNT(*) FROM hoc_ky
UNION ALL SELECT 'nganh_hoc',      COUNT(*) FROM nganh_hoc
UNION ALL SELECT 'chuong_trinh_dt',COUNT(*) FROM chuong_trinh_dt
UNION ALL SELECT 'mon_hoc',        COUNT(*) FROM mon_hoc
UNION ALL SELECT 'chi_tiet_ctdt',  COUNT(*) FROM chi_tiet_ctdt
UNION ALL SELECT 'lop_sinh_hoat',  COUNT(*) FROM lop_sinh_hoat
UNION ALL SELECT 'tai_khoan (SV)', COUNT(*) FROM tai_khoan WHERE vai_tro_id = (SELECT Id FROM vai_tro WHERE ten_vai_tro = N'Sinh viên')
UNION ALL SELECT 'giao_vien',      COUNT(*) FROM giao_vien
UNION ALL SELECT 'lop_hoc_phan',   COUNT(*) FROM lop_hoc_phan
UNION ALL SELECT 'danh_sach_lop_hp',COUNT(*) FROM danh_sach_lop_hp
UNION ALL SELECT 'hoc_phi',        COUNT(*) FROM hoc_phi;

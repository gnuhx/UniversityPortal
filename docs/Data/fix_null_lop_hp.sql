-- ============================================================
-- FIX: lop_hoc_phan + danh_sach_lop_hp for HK1 2024-2025
-- Root cause: MERGE VALUES with subqueries caused NULL chi_tiet_ctdt_id
-- This script uses pre-computed variables — safe to run multiple times
-- ============================================================
SET NOCOUNT ON;
DECLARE @now DATETIME2 = GETDATE();

-- ===== HỌC KỲ =====
DECLARE @hk1_2122 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2021-2022');
DECLARE @hk1_2223 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2022-2023');
DECLARE @hk2_2223 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK2 2022-2023');
DECLARE @hk1_2324 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2023-2024');
DECLARE @hk2_2324 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK2 2023-2024');
DECLARE @hk1_2425 INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = N'HK1 2024-2025');

-- ===== CTDT =====
DECLARE @ctdt INT = (SELECT Id FROM chuong_trinh_dt WHERE ma_ctdt = N'KTPM2021');

-- ===== GIÁO VIÊN =====
DECLARE @gv1 INT = (SELECT g.Id FROM giao_vien g JOIN tai_khoan t ON g.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'gv.nguyen.tuan');
DECLARE @gv2 INT = (SELECT g.Id FROM giao_vien g JOIN tai_khoan t ON g.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'gv.tran.mai');
DECLARE @gv3 INT = (SELECT g.Id FROM giao_vien g JOIN tai_khoan t ON g.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'gv.le.hung');

-- ===== CHI TIẾT CTDT IDs (pre-computed, no subquery in VALUES) =====
DECLARE @ct_kiemthu INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'KIEMTHU'   AND ct.hoc_ky_id=@hk1_2425);
DECLARE @ct_qldapm  INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'QLDAPM'    AND ct.hoc_ky_id=@hk1_2425);
DECLARE @ct_ddmay   INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'DDMAY501'  AND ct.hoc_ky_id=@hk1_2425);
DECLARE @ct_ltweb   INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'LTWEB401'  AND ct.hoc_ky_id=@hk1_2324);
DECLARE @ct_ktpm401 INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'KTPM401'   AND ct.hoc_ky_id=@hk1_2324);
DECLARE @ct_cnpm    INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'CNPM402'   AND ct.hoc_ky_id=@hk1_2324);
DECLARE @ct_oop     INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'OOP301'    AND ct.hoc_ky_id=@hk1_2223);
DECLARE @ct_csdl    INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'CSDL301'   AND ct.hoc_ky_id=@hk1_2223);
DECLARE @ct_xstk    INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'XSTK201'   AND ct.hoc_ky_id=@hk1_2223);
DECLARE @ct_ttnt    INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'TTNT402'   AND ct.hoc_ky_id=@hk2_2324);
DECLARE @ct_atvt    INT = (SELECT ct.Id FROM chi_tiet_ctdt ct JOIN mon_hoc m ON ct.mon_hoc_id=m.Id WHERE ct.ctdt_id=@ctdt AND m.ma_mon=N'ATVT401'   AND ct.hoc_ky_id=@hk2_2223);

PRINT N'=== chi_tiet_ctdt IDs (NULL = chưa tồn tại, chạy seed_test_data.sql trước) ===';
PRINT N'KIEMTHU  @hk1_2425: ' + ISNULL(CAST(@ct_kiemthu AS NVARCHAR(10)), N'NULL');
PRINT N'QLDAPM   @hk1_2425: ' + ISNULL(CAST(@ct_qldapm  AS NVARCHAR(10)), N'NULL');
PRINT N'DDMAY501 @hk1_2425: ' + ISNULL(CAST(@ct_ddmay   AS NVARCHAR(10)), N'NULL');
PRINT N'LTWEB401 @hk1_2324: ' + ISNULL(CAST(@ct_ltweb   AS NVARCHAR(10)), N'NULL');
PRINT N'KTPM401  @hk1_2324: ' + ISNULL(CAST(@ct_ktpm401 AS NVARCHAR(10)), N'NULL');
PRINT N'CNPM402  @hk1_2324: ' + ISNULL(CAST(@ct_cnpm    AS NVARCHAR(10)), N'NULL');
PRINT N'OOP301   @hk1_2223: ' + ISNULL(CAST(@ct_oop     AS NVARCHAR(10)), N'NULL');
PRINT N'CSDL301  @hk1_2223: ' + ISNULL(CAST(@ct_csdl    AS NVARCHAR(10)), N'NULL');
PRINT N'XSTK201  @hk1_2223: ' + ISNULL(CAST(@ct_xstk    AS NVARCHAR(10)), N'NULL');
PRINT N'TTNT402  @hk2_2324: ' + ISNULL(CAST(@ct_ttnt    AS NVARCHAR(10)), N'NULL');

-- ===== [1] LỚP HỌC PHẦN — HK1 2024-2025 =====
-- K2021 năm 4
IF @ct_kiemthu IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'KIEMTHU_2425_1')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_kiemthu,@hk1_2425,@gv1,N'KIEMTHU_2425_1',0,0,@now,@now);

IF @ct_qldapm IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'QLDAPM_2425_1')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_qldapm,@hk1_2425,@gv2,N'QLDAPM_2425_1',0,0,@now,@now);

IF @ct_ddmay IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'DDMAY501_2425_1')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_ddmay,@hk1_2425,@gv3,N'DDMAY501_2425_1',0,0,@now,@now);

-- K2022 năm 3 (year 3 HK1 subjects)
IF @ct_ltweb IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'LTWEB401_K22_2425')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_ltweb,@hk1_2425,@gv1,N'LTWEB401_K22_2425',0,0,@now,@now);

IF @ct_ktpm401 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'KTPM401_K22_2425')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_ktpm401,@hk1_2425,@gv2,N'KTPM401_K22_2425',0,0,@now,@now);

IF @ct_cnpm IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'CNPM402_K22_2425')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_cnpm,@hk1_2425,@gv3,N'CNPM402_K22_2425',0,0,@now,@now);

-- K2023 năm 2 (year 2 HK1 subjects)
IF @ct_oop IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'OOP301_K23_2425')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_oop,@hk1_2425,@gv1,N'OOP301_K23_2425',0,0,@now,@now);

IF @ct_csdl IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'CSDL301_K23_2425')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_csdl,@hk1_2425,@gv2,N'CSDL301_K23_2425',0,0,@now,@now);

IF @ct_xstk IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'XSTK201_K23_2425')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_xstk,@hk1_2425,@gv3,N'XSTK201_K23_2425',0,0,@now,@now);

-- K2021 SV3 học lại TTNT402
IF @ct_ttnt IS NOT NULL AND NOT EXISTS(SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp=N'TTNT402_HL_2425')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id,hoc_ky_id,giao_vien_id,ma_lop_hp,khoa_bang_diem,trang_thai_ket_thuc,created_at,updated_at)
    VALUES (@ct_ttnt,@hk1_2425,@gv2,N'TTNT402_HL_2425',0,0,@now,@now);

-- ===== [2] SINH VIÊN IDs =====
DECLARE @sv_2021_1 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2021.001');
DECLARE @sv_2021_2 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2021.002');
DECLARE @sv_2021_3 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2021.003');
DECLARE @sv_2021_4 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2021.004');
DECLARE @sv_2022_1 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2022.001');
DECLARE @sv_2022_2 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2022.002');
DECLARE @sv_2022_3 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2022.003');
DECLARE @sv_2023_1 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2023.001');
DECLARE @sv_2023_2 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2023.002');
DECLARE @sv_2023_3 INT=(SELECT sv.Id FROM sinh_vien sv JOIN tai_khoan t ON sv.tai_khoan_id=t.Id WHERE t.ten_dang_nhap=N'sv.k2023.003');

-- ===== [3] LỚP HỌC PHẦN IDs (sau khi insert xong) =====
DECLARE @lhp_kiemthu   INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KIEMTHU_2425_1');
DECLARE @lhp_qldapm    INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'QLDAPM_2425_1');
DECLARE @lhp_ddmay     INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'DDMAY501_2425_1');
DECLARE @lhp_ltweb_k22 INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'LTWEB401_K22_2425');
DECLARE @lhp_ktpm_k22  INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'KTPM401_K22_2425');
DECLARE @lhp_cnpm_k22  INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CNPM402_K22_2425');
DECLARE @lhp_oop_k23   INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'OOP301_K23_2425');
DECLARE @lhp_csdl_k23  INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'CSDL301_K23_2425');
DECLARE @lhp_xstk_k23  INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'XSTK201_K23_2425');
DECLARE @lhp_ttnt_hl   INT=(SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp=N'TTNT402_HL_2425');

-- Helper macro — insert danh_sach_lop_hp if (sv, lhp) not already present
-- K2021 SV1 — HK1 2024-2025
IF @lhp_kiemthu IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_1 AND lop_hp_id=@lhp_kiemthu)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2021_1,@lhp_kiemthu,N'Chính quy',N'Đã duyệt',8.0,@now,@now);

IF @lhp_qldapm IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_1 AND lop_hp_id=@lhp_qldapm)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2021_1,@lhp_qldapm,N'Chính quy',N'Đã duyệt',7.5,@now,@now);

IF @lhp_ddmay IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_1 AND lop_hp_id=@lhp_ddmay)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,created_at,updated_at)
    VALUES(@sv_2021_1,@lhp_ddmay,N'Chính quy',N'Đã duyệt',@now,@now);

-- K2021 SV2 — HK1 2024-2025
IF @lhp_kiemthu IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_2 AND lop_hp_id=@lhp_kiemthu)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2021_2,@lhp_kiemthu,N'Chính quy',N'Đã duyệt',9.5,@now,@now);

IF @lhp_qldapm IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_2 AND lop_hp_id=@lhp_qldapm)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2021_2,@lhp_qldapm,N'Chính quy',N'Đã duyệt',9.0,@now,@now);

IF @lhp_ddmay IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_2 AND lop_hp_id=@lhp_ddmay)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2021_2,@lhp_ddmay,N'Chính quy',N'Đã duyệt',9.5,@now,@now);

-- K2021 SV3 — HK1 2024-2025 + học lại TTNT402
IF @lhp_kiemthu IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_3 AND lop_hp_id=@lhp_kiemthu)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2021_3,@lhp_kiemthu,N'Chính quy',N'Đã duyệt',6.5,@now,@now);

IF @lhp_qldapm IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_3 AND lop_hp_id=@lhp_qldapm)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,created_at,updated_at)
    VALUES(@sv_2021_3,@lhp_qldapm,N'Chính quy',N'Đã duyệt',@now,@now);

IF @lhp_ttnt_hl IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_3 AND lop_hp_id=@lhp_ttnt_hl)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2021_3,@lhp_ttnt_hl,N'Học lại',N'Đã duyệt',6.0,@now,@now);

-- K2021 SV4 — HK1 2024-2025
IF @lhp_kiemthu IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_4 AND lop_hp_id=@lhp_kiemthu)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2021_4,@lhp_kiemthu,N'Chính quy',N'Đã duyệt',7.0,@now,@now);

IF @lhp_qldapm IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_4 AND lop_hp_id=@lhp_qldapm)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2021_4,@lhp_qldapm,N'Chính quy',N'Đã duyệt',7.5,@now,@now);

IF @lhp_ddmay IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2021_4 AND lop_hp_id=@lhp_ddmay)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,created_at,updated_at)
    VALUES(@sv_2021_4,@lhp_ddmay,N'Chính quy',N'Đã duyệt',@now,@now);

-- K2022 SV1 — HK1 2024-2025 (năm 3 HK1)
IF @lhp_ltweb_k22 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2022_1 AND lop_hp_id=@lhp_ltweb_k22)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2022_1,@lhp_ltweb_k22,N'Chính quy',N'Đã duyệt',8.5,@now,@now);

IF @lhp_ktpm_k22 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2022_1 AND lop_hp_id=@lhp_ktpm_k22)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2022_1,@lhp_ktpm_k22,N'Chính quy',N'Đã duyệt',8.0,@now,@now);

IF @lhp_cnpm_k22 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2022_1 AND lop_hp_id=@lhp_cnpm_k22)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,created_at,updated_at)
    VALUES(@sv_2022_1,@lhp_cnpm_k22,N'Chính quy',N'Đã duyệt',@now,@now);

-- K2022 SV2, SV3 — HK1 2024-2025
IF @lhp_ltweb_k22 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2022_2 AND lop_hp_id=@lhp_ltweb_k22)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2022_2,@lhp_ltweb_k22,N'Chính quy',N'Đã duyệt',7.5,@now,@now);

IF @lhp_ktpm_k22 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2022_2 AND lop_hp_id=@lhp_ktpm_k22)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,created_at,updated_at)
    VALUES(@sv_2022_2,@lhp_ktpm_k22,N'Chính quy',N'Đã duyệt',@now,@now);

IF @lhp_ltweb_k22 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2022_3 AND lop_hp_id=@lhp_ltweb_k22)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2022_3,@lhp_ltweb_k22,N'Chính quy',N'Đã duyệt',7.0,@now,@now);

IF @lhp_ktpm_k22 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2022_3 AND lop_hp_id=@lhp_ktpm_k22)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,created_at,updated_at)
    VALUES(@sv_2022_3,@lhp_ktpm_k22,N'Chính quy',N'Đã duyệt',@now,@now);

-- K2023 SV1, SV2, SV3 — HK1 2024-2025 (năm 2 HK1)
IF @lhp_oop_k23 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2023_1 AND lop_hp_id=@lhp_oop_k23)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2023_1,@lhp_oop_k23,N'Chính quy',N'Đã duyệt',8.0,@now,@now);

IF @lhp_csdl_k23 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2023_1 AND lop_hp_id=@lhp_csdl_k23)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2023_1,@lhp_csdl_k23,N'Chính quy',N'Đã duyệt',8.5,@now,@now);

IF @lhp_xstk_k23 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2023_1 AND lop_hp_id=@lhp_xstk_k23)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,created_at,updated_at)
    VALUES(@sv_2023_1,@lhp_xstk_k23,N'Chính quy',N'Đã duyệt',@now,@now);

IF @lhp_oop_k23 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2023_2 AND lop_hp_id=@lhp_oop_k23)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2023_2,@lhp_oop_k23,N'Chính quy',N'Đã duyệt',7.0,@now,@now);

IF @lhp_csdl_k23 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2023_2 AND lop_hp_id=@lhp_csdl_k23)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,created_at,updated_at)
    VALUES(@sv_2023_2,@lhp_csdl_k23,N'Chính quy',N'Đã duyệt',@now,@now);

IF @lhp_oop_k23 IS NOT NULL AND NOT EXISTS(SELECT 1 FROM danh_sach_lop_hp WHERE sinh_vien_id=@sv_2023_3 AND lop_hp_id=@lhp_oop_k23)
    INSERT INTO danh_sach_lop_hp(sinh_vien_id,lop_hp_id,loai_dang_ky,trang_thai_duyet,diem_qt1,created_at,updated_at)
    VALUES(@sv_2023_3,@lhp_oop_k23,N'Chính quy',N'Đã duyệt',7.5,@now,@now);

-- ===== [4] HỌC PHÍ HK1 2024-2025 (nếu chưa có) =====
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2021_1 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2021_1,@hk1_2425,4050000,N'Chưa đóng',@now,@now);
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2021_2 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2021_2,@hk1_2425,4050000,N'Đã đóng',@now,@now);
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2021_3 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2021_3,@hk1_2425,4950000,N'Chưa đóng',@now,@now);
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2021_4 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2021_4,@hk1_2425,4050000,N'Chưa đóng',@now,@now);
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2022_1 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2022_1,@hk1_2425,4950000,N'Chưa đóng',@now,@now);
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2022_2 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2022_2,@hk1_2425,4500000,N'Đã đóng',@now,@now);
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2022_3 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2022_3,@hk1_2425,4500000,N'Chưa đóng',@now,@now);
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2023_1 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2023_1,@hk1_2425,4050000,N'Chưa đóng',@now,@now);
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2023_2 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2023_2,@hk1_2425,3600000,N'Đã đóng',@now,@now);
IF NOT EXISTS(SELECT 1 FROM hoc_phi WHERE sinh_vien_id=@sv_2023_3 AND hoc_ky_id=@hk1_2425)
    INSERT INTO hoc_phi(sinh_vien_id,hoc_ky_id,so_tien,trang_thai_dong,created_at,updated_at) VALUES(@sv_2023_3,@hk1_2425,3600000,N'Chưa đóng',@now,@now);

-- ===== VERIFY =====
PRINT N'';
PRINT N'=== lop_hoc_phan HK1 2024-2025 ===';
SELECT ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc
FROM lop_hoc_phan WHERE hoc_ky_id=@hk1_2425 ORDER BY ma_lop_hp;

PRINT N'=== danh_sach_lop_hp HK1 2024-2025 ===';
SELECT t.ten_dang_nhap, lhp.ma_lop_hp, ds.loai_dang_ky, ds.diem_qt1
FROM danh_sach_lop_hp ds
JOIN sinh_vien sv ON ds.sinh_vien_id=sv.Id
JOIN tai_khoan t  ON sv.tai_khoan_id=t.Id
JOIN lop_hoc_phan lhp ON ds.lop_hp_id=lhp.Id
WHERE lhp.hoc_ky_id=@hk1_2425
ORDER BY t.ten_dang_nhap, lhp.ma_lop_hp;

PRINT N'=== hoc_phi HK1 2024-2025 ===';
SELECT t.ten_dang_nhap, hp.so_tien, hp.trang_thai_dong
FROM hoc_phi hp
JOIN sinh_vien sv ON hp.sinh_vien_id=sv.Id
JOIN tai_khoan t  ON sv.tai_khoan_id=t.Id
WHERE hp.hoc_ky_id=@hk1_2425
ORDER BY t.ten_dang_nhap;

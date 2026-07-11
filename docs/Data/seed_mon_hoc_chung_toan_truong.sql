-- ============================================================
-- Thêm 1 môn học "chung" bắt buộc toàn trường (Giáo dục Quốc phòng
-- và An ninh) vào Học kỳ + Tuần học "hiện tại" — tính ĐỘNG theo
-- GETDATE() tại thời điểm chạy script (không hard-code ngày tháng),
-- và 1 buổi học (thời khoá biểu) rơi đúng vào ngày chạy script, để
-- khi mở trang "Thời khoá biểu" sẽ thấy ngay một buổi học ở đúng
-- ngày hôm nay trong học kỳ hiện tại.
--
-- Idempotent: chạy lại nhiều lần không tạo trùng dữ liệu (mỗi bước
-- đều kiểm tra NOT EXISTS trước khi insert).
--
-- Xem docs/features/05_todo_seed_mon_hoc_chung_toan_truong.md để
-- biết bối cảnh và các giả định (vd: gắn tạm vào 1 CTĐT bất kỳ chỉ
-- để thoả khoá ngoại bắt buộc của chi_tiet_ctdt).
-- ============================================================

SET NOCOUNT ON;
DECLARE @now DATETIME = GETDATE();

-- Thứ Hai của tuần hiện tại: ngày 0 (1900-01-01) luôn là thứ Hai
-- trong SQL Server, nên công thức này không phụ thuộc @@DATEFIRST
-- hay ngôn ngữ/locale của server.
DECLARE @ngayBatDauTuan DATE = DATEADD(day, DATEDIFF(day, 0, @now) / 7 * 7, 0);
DECLARE @ngayKetThucTuan DATE = DATEADD(day, 6, @ngayBatDauTuan);

-- Năm học kiểu "2025-2026" (quy ước năm học VN bắt đầu từ tháng 9)
DECLARE @namBatDau INT = CASE WHEN MONTH(@now) >= 9 THEN YEAR(@now) ELSE YEAR(@now) - 1 END;
DECLARE @tenNamHoc NVARCHAR(20) = CONCAT(@namBatDau, N'-', @namBatDau + 1);
DECLARE @tenHocKy NVARCHAR(50) = CONCAT(N'Học kỳ hiện tại (', @tenNamHoc, N')');

-- ── 1. Năm học ──────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM nam_hoc WHERE ten_nam_hoc = @tenNamHoc)
    INSERT INTO nam_hoc (ten_nam_hoc, created_at, updated_at)
    VALUES (@tenNamHoc, @now, @now);
DECLARE @namHocId INT = (SELECT Id FROM nam_hoc WHERE ten_nam_hoc = @tenNamHoc);

-- ── 2. Học kỳ hiện tại (ngay_bat_dau = thứ Hai tuần này) ────
IF NOT EXISTS (SELECT 1 FROM hoc_ky WHERE ten_hoc_ky = @tenHocKy)
    INSERT INTO hoc_ky (ten_hoc_ky, nam_hoc_id, ngay_bat_dau, created_at, updated_at)
    VALUES (@tenHocKy, @namHocId, @ngayBatDauTuan, @now, @now);
DECLARE @hocKyId INT = (SELECT Id FROM hoc_ky WHERE ten_hoc_ky = @tenHocKy);

-- ── 3. Tuần học hiện tại ────────────────────────────────────
DECLARE @maTuan NVARCHAR(20) = CONCAT(N'CUR_', FORMAT(@ngayBatDauTuan, 'yyyyMMdd'));
IF NOT EXISTS (SELECT 1 FROM tuan_hoc WHERE ma_tuan = @maTuan)
    INSERT INTO tuan_hoc (nam_hoc_id, ma_tuan, so_thu_tu_tuan, ngay_bat_dau, ngay_ket_thuc, created_at, updated_at)
    VALUES (@namHocId, @maTuan, 1, @ngayBatDauTuan, @ngayKetThucTuan, @now, @now);
DECLARE @tuanHocId INT = (SELECT Id FROM tuan_hoc WHERE ma_tuan = @maTuan);

-- ── 4. Môn học chung ────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM mon_hoc WHERE ma_mon = N'GDQPAN')
    INSERT INTO mon_hoc (ma_mon, ten_mon, created_at, updated_at)
    VALUES (N'GDQPAN', N'Giáo dục Quốc phòng và An ninh', @now, @now);
DECLARE @monHocId INT = (SELECT Id FROM mon_hoc WHERE ma_mon = N'GDQPAN');

-- ── 5. Gắn môn vào 1 CTĐT bất kỳ để thoả FK bắt buộc của
--      chi_tiet_ctdt (ctdt_id NOT NULL). Môn này thực chất áp dụng
--      cho TOÀN TRƯỜNG, không thuộc riêng ngành/CTĐT nào — CTĐT ở
--      đây chỉ là "chỗ neo" kỹ thuật, không ảnh hưởng việc ghi danh
--      ở bước 7 (ghi danh trực tiếp theo sinh_vien, không qua CTĐT).
DECLARE @ctdtId INT = (SELECT TOP 1 Id FROM chuong_trinh_dt ORDER BY Id);
IF NOT EXISTS (SELECT 1 FROM chi_tiet_ctdt WHERE ctdt_id = @ctdtId AND mon_hoc_id = @monHocId AND hoc_ky_id = @hocKyId)
    INSERT INTO chi_tiet_ctdt (ctdt_id, mon_hoc_id, hoc_ky_id, so_tin_chi, tinh_diem_tb, created_at, updated_at)
    VALUES (@ctdtId, @monHocId, @hocKyId, 3, 0, @now, @now);
DECLARE @chiTietCtdtId INT = (SELECT Id FROM chi_tiet_ctdt WHERE ctdt_id = @ctdtId AND mon_hoc_id = @monHocId AND hoc_ky_id = @hocKyId);

-- ── 6. Lớp học phần chung (1 lớp duy nhất, toàn trường học chung) ──
DECLARE @giaoVienId INT = (SELECT TOP 1 Id FROM giao_vien ORDER BY Id);
IF NOT EXISTS (SELECT 1 FROM lop_hoc_phan WHERE ma_lop_hp = N'GDQPAN-TOANTRUONG')
    INSERT INTO lop_hoc_phan (chi_tiet_ctdt_id, hoc_ky_id, giao_vien_id, ma_lop_hp, khoa_bang_diem, trang_thai_ket_thuc, created_at, updated_at)
    VALUES (@chiTietCtdtId, @hocKyId, @giaoVienId, N'GDQPAN-TOANTRUONG', 0, 0, @now, @now);
DECLARE @lopHpId INT = (SELECT Id FROM lop_hoc_phan WHERE ma_lop_hp = N'GDQPAN-TOANTRUONG');

-- ── 7. Ghi danh MỌI sinh viên của trường vào lớp này (bất kể ngành/CTĐT) ──
INSERT INTO danh_sach_lop_hp (sinh_vien_id, lop_hp_id, loai_dang_ky, trang_thai_duyet, created_at, updated_at)
SELECT sv.Id, @lopHpId, N'Học chính', N'Đã duyệt', @now, @now
FROM sinh_vien sv
WHERE NOT EXISTS (
    SELECT 1 FROM danh_sach_lop_hp d WHERE d.sinh_vien_id = sv.Id AND d.lop_hp_id = @lopHpId
);

-- ── 8. Buổi học rơi đúng vào NGÀY CHẠY SCRIPT (quy ước thu = 2..8) ──
DECLARE @thu INT = 2 + DATEDIFF(day, @ngayBatDauTuan, CAST(@now AS date));
IF NOT EXISTS (
    SELECT 1 FROM thoi_khoa_bieu
    WHERE lop_hp_id = @lopHpId AND tuan_hoc_id = @tuanHocId AND thu = @thu
)
    INSERT INTO thoi_khoa_bieu (lop_hp_id, tuan_hoc_id, thu, tiet_bat_dau, tiet_ket_thuc, phong_hoc, created_at, updated_at)
    VALUES (@lopHpId, @tuanHocId, @thu, 1, 3, N'Sân vận động', @now, @now);

-- ── Đối chiếu kết quả ────────────────────────────────────────
SELECT
    @tenNamHoc AS nam_hoc, @tenHocKy AS hoc_ky, @maTuan AS tuan_hoc,
    @ngayBatDauTuan AS ngay_bat_dau_tuan, @thu AS thu_hoc,
    DATEADD(day, @thu - 2, @ngayBatDauTuan) AS ngay_hoc_thuc_te,
    (SELECT COUNT(*) FROM danh_sach_lop_hp WHERE lop_hp_id = @lopHpId) AS so_sinh_vien_da_ghi_danh;

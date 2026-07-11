-- ============================================================
-- MỞ RỘNG CƠ CẤU KHOA — NGÀNH (ngành cha / ngành con)
-- Thêm 4 khoa kỹ thuật mới (Cơ khí, Điện – Điện tử, Cơ điện tử –
-- Tự động hoá, Ô tô) và bổ sung 2 ngành con cho Khoa CNTT hiện có.
--
-- Lưu ý: bảng nganh_hoc KHÔNG có khoá ngoại tới phong_ban — quan hệ
-- Khoa/Ngành ở đây chỉ mang tính tổ chức (đặt tên phong_ban tương
-- ứng), không được model cứng trong DB. Script chỉ thêm phong_ban
-- (để hiển thị đúng tên khoa khi gán tài khoản) và nganh_hoc
-- (ngành cha/con dùng cho Chương trình đào tạo).
--
-- An toàn khi chạy nhiều lần: mọi INSERT đều có IF NOT EXISTS.
-- Chưa tạo chương trình đào tạo / môn học cho các ngành mới —
-- sẽ làm ở bước sau nếu cần (giống ctdt_khoa_cntt.sql).
-- ============================================================

-- ============================================================
-- BƯỚC 1: THÊM PHÒNG BAN (KHOA) MỚI
-- ============================================================
INSERT INTO phong_ban (ten_phong_ban, created_at, updated_at)
SELECT v.ten_phong_ban, GETDATE(), GETDATE()
FROM (VALUES
  (N'Khoa Cơ khí'),
  (N'Khoa Điện – Điện tử'),
  (N'Khoa Cơ điện tử – Tự động hóa'),
  (N'Khoa Ô tô')
) AS v(ten_phong_ban)
WHERE NOT EXISTS (SELECT 1 FROM phong_ban p WHERE p.ten_phong_ban = v.ten_phong_ban);

-- ============================================================
-- BƯỚC 2: BỔ SUNG NGÀNH CON CHO KHOA CNTT (ngành cha CNTT đã có sẵn)
-- ============================================================
INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
SELECT v.ma_nganh, v.ten_nganh, p.id, GETDATE(), GETDATE()
FROM (VALUES
  (N'QTMMT',  N'Quản trị mạng máy tính'),
  (N'SCLRMT', N'Kỹ thuật sửa chữa, lắp ráp máy tính')
) AS v(ma_nganh, ten_nganh)
CROSS JOIN nganh_hoc p
WHERE p.ma_nganh = N'CNTT'
  AND NOT EXISTS (SELECT 1 FROM nganh_hoc x WHERE x.ma_nganh = v.ma_nganh);

-- ============================================================
-- BƯỚC 3: KHOA CƠ KHÍ
-- Ngành cha: Công nghệ Kỹ thuật Cơ khí
-- Ngành con: Cơ khí chế tạo (Cắt gọt kim loại), Hàn, Sửa chữa cơ khí
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM nganh_hoc WHERE ma_nganh = N'CKI')
  INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
  VALUES (N'CKI', N'Công nghệ Kỹ thuật Cơ khí', NULL, GETDATE(), GETDATE());

INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
SELECT v.ma_nganh, v.ten_nganh, p.id, GETDATE(), GETDATE()
FROM (VALUES
  (N'CKCT', N'Cơ khí chế tạo (Cắt gọt kim loại)'),
  (N'HAN',  N'Hàn'),
  (N'SCCK', N'Sửa chữa cơ khí')
) AS v(ma_nganh, ten_nganh)
CROSS JOIN nganh_hoc p
WHERE p.ma_nganh = N'CKI'
  AND NOT EXISTS (SELECT 1 FROM nganh_hoc x WHERE x.ma_nganh = v.ma_nganh);

-- ============================================================
-- BƯỚC 4: KHOA ĐIỆN – ĐIỆN TỬ
-- Ngành cha: Công nghệ Kỹ thuật Điện – Điện tử
-- Ngành con: Điện công nghiệp, Điện tử công nghiệp, Điện tử – Viễn thông
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM nganh_hoc WHERE ma_nganh = N'DDT')
  INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
  VALUES (N'DDT', N'Công nghệ Kỹ thuật Điện – Điện tử', NULL, GETDATE(), GETDATE());

INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
SELECT v.ma_nganh, v.ten_nganh, p.id, GETDATE(), GETDATE()
FROM (VALUES
  (N'DCN',  N'Điện công nghiệp'),
  (N'DTCN', N'Điện tử công nghiệp'),
  (N'DTVT', N'Điện tử – Viễn thông')
) AS v(ma_nganh, ten_nganh)
CROSS JOIN nganh_hoc p
WHERE p.ma_nganh = N'DDT'
  AND NOT EXISTS (SELECT 1 FROM nganh_hoc x WHERE x.ma_nganh = v.ma_nganh);

-- ============================================================
-- BƯỚC 5: KHOA CƠ ĐIỆN TỬ – TỰ ĐỘNG HÓA
-- 2 ngành cha: Công nghệ Kỹ thuật Cơ điện tử; Công nghệ Kỹ thuật
-- Điều khiển và Tự động hóa. Chương trình về robot là ngành con
-- của ngành Điều khiển và Tự động hóa.
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM nganh_hoc WHERE ma_nganh = N'CDT')
  INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
  VALUES (N'CDT', N'Công nghệ Kỹ thuật Cơ điện tử', NULL, GETDATE(), GETDATE());

IF NOT EXISTS (SELECT 1 FROM nganh_hoc WHERE ma_nganh = N'DKTDH')
  INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
  VALUES (N'DKTDH', N'Công nghệ Kỹ thuật Điều khiển và Tự động hóa', NULL, GETDATE(), GETDATE());

INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
SELECT N'TDHRB', N'Tự động hóa và Robot', p.id, GETDATE(), GETDATE()
FROM nganh_hoc p
WHERE p.ma_nganh = N'DKTDH'
  AND NOT EXISTS (SELECT 1 FROM nganh_hoc x WHERE x.ma_nganh = N'TDHRB');

-- ============================================================
-- BƯỚC 6: KHOA Ô TÔ
-- Ngành cha: Công nghệ Kỹ thuật Ô tô
-- Ngành con: Bảo trì, sửa chữa Ô tô
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM nganh_hoc WHERE ma_nganh = N'OTO')
  INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
  VALUES (N'OTO', N'Công nghệ Kỹ thuật Ô tô', NULL, GETDATE(), GETDATE());

INSERT INTO nganh_hoc (ma_nganh, ten_nganh, nganh_cha_id, created_at, updated_at)
SELECT N'BTSCOT', N'Bảo trì, sửa chữa Ô tô', p.id, GETDATE(), GETDATE()
FROM nganh_hoc p
WHERE p.ma_nganh = N'OTO'
  AND NOT EXISTS (SELECT 1 FROM nganh_hoc x WHERE x.ma_nganh = N'BTSCOT');

-- ============================================================
-- BƯỚC 7: KIỂM TRA KẾT QUẢ
-- ============================================================
SELECT cha.ten_nganh AS khoi_nganh_cha, con.ma_nganh, con.ten_nganh AS nganh_con
FROM nganh_hoc con
LEFT JOIN nganh_hoc cha ON cha.id = con.nganh_cha_id
ORDER BY ISNULL(cha.ten_nganh, con.ten_nganh), CASE WHEN con.nganh_cha_id IS NULL THEN 0 ELSE 1 END, con.ten_nganh;

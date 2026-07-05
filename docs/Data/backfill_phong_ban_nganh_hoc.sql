-- ============================================================
-- BACKFILL phong_ban_id CHO nganh_hoc
-- Chạy SAU KHI migration AddPhongBanToNganhHoc đã được áp dụng
-- (cột nganh_hoc.phong_ban_id đã tồn tại).
--
-- Gán Khoa cho 7 ngành cha hiện có, sau đó lan truyền xuống ngành con
-- (ngành con kế thừa phong_ban_id từ ngành cha trực tiếp).
--
-- An toàn khi chạy nhiều lần: mọi UPDATE đều có điều kiện so sánh giá
-- trị hiện tại, không có tác dụng phụ nếu chạy lại.
-- ============================================================

-- ============================================================
-- BƯỚC 1: GÁN KHOA CHO CÁC NGÀNH CHA (root)
-- ============================================================
UPDATE n SET n.phong_ban_id = pb.id, n.updated_at = GETDATE()
FROM nganh_hoc n
JOIN phong_ban pb ON pb.ten_phong_ban = N'Khoa Công nghệ Thông tin'
WHERE n.ma_nganh = N'CNTT' AND (n.phong_ban_id IS NULL OR n.phong_ban_id <> pb.id);

UPDATE n SET n.phong_ban_id = pb.id, n.updated_at = GETDATE()
FROM nganh_hoc n
JOIN phong_ban pb ON pb.ten_phong_ban = N'Khoa Kinh tế'
WHERE n.ma_nganh = N'QTKD' AND (n.phong_ban_id IS NULL OR n.phong_ban_id <> pb.id);

UPDATE n SET n.phong_ban_id = pb.id, n.updated_at = GETDATE()
FROM nganh_hoc n
JOIN phong_ban pb ON pb.ten_phong_ban = N'Khoa Cơ khí'
WHERE n.ma_nganh = N'CKI' AND (n.phong_ban_id IS NULL OR n.phong_ban_id <> pb.id);

UPDATE n SET n.phong_ban_id = pb.id, n.updated_at = GETDATE()
FROM nganh_hoc n
JOIN phong_ban pb ON pb.ten_phong_ban = N'Khoa Điện – Điện tử'
WHERE n.ma_nganh = N'DDT' AND (n.phong_ban_id IS NULL OR n.phong_ban_id <> pb.id);

UPDATE n SET n.phong_ban_id = pb.id, n.updated_at = GETDATE()
FROM nganh_hoc n
JOIN phong_ban pb ON pb.ten_phong_ban = N'Khoa Cơ điện tử – Tự động hóa'
WHERE n.ma_nganh IN (N'CDT', N'DKTDH') AND (n.phong_ban_id IS NULL OR n.phong_ban_id <> pb.id);

UPDATE n SET n.phong_ban_id = pb.id, n.updated_at = GETDATE()
FROM nganh_hoc n
JOIN phong_ban pb ON pb.ten_phong_ban = N'Khoa Ô tô'
WHERE n.ma_nganh = N'OTO' AND (n.phong_ban_id IS NULL OR n.phong_ban_id <> pb.id);

-- ============================================================
-- BƯỚC 2: LAN TRUYỀN XUỐNG NGÀNH CON (kế thừa từ ngành cha trực tiếp)
-- ============================================================
UPDATE con SET con.phong_ban_id = cha.phong_ban_id, con.updated_at = GETDATE()
FROM nganh_hoc con
JOIN nganh_hoc cha ON cha.id = con.nganh_cha_id
WHERE cha.phong_ban_id IS NOT NULL
  AND (con.phong_ban_id IS NULL OR con.phong_ban_id <> cha.phong_ban_id);

-- ============================================================
-- BƯỚC 3: KIỂM TRA KẾT QUẢ
-- ============================================================
SELECT n.ma_nganh, n.ten_nganh, cha.ten_nganh AS nganh_cha, pb.ten_phong_ban AS khoa
FROM nganh_hoc n
LEFT JOIN nganh_hoc cha ON cha.id = n.nganh_cha_id
LEFT JOIN phong_ban pb ON pb.id = n.phong_ban_id
ORDER BY pb.ten_phong_ban, n.ma_nganh;

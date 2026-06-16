export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string | null;
  errors?: string[] | null;
}

export interface PagedResult<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface UserInfo {
  id: number;
  hoTen: string;
  email: string;
  vaiTro: string;
  anhDaiDien?: string | null;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  userInfo: UserInfo;
}

// ===== Sinh vien =====
export interface SinhVien {
  id: number;
  taiKhoanId: number;
  mssv: string;
  lopId?: number | null;
  tenLop?: string | null;
  hoTen: string;
  email: string;
  anhDaiDien?: string | null;
  trangThai: boolean;
  createdAt: string;
}

export interface CreateSinhVien {
  tenDangNhap: string;
  matKhau: string;
  hoTen: string;
  email: string;
  mssv: string;
  lopId?: number | null;
}

export interface UpdateSinhVien {
  hoTen: string;
  email: string;
  lopId?: number | null;
  trangThai: boolean;
}

// ===== Giao vien =====
export interface GiaoVien {
  id: number;
  taiKhoanId: number;
  maGv: string;
  hoTen: string;
  email: string;
  anhDaiDien?: string | null;
  phongBanId?: number | null;
  tenPhongBan?: string | null;
  trangThai: boolean;
  createdAt: string;
}

export interface CreateGiaoVien {
  tenDangNhap: string;
  matKhau: string;
  hoTen: string;
  email: string;
  phongBanId?: number | null;
  maGv: string;
}

export interface UpdateGiaoVien {
  hoTen: string;
  email: string;
  maGv: string;
  phongBanId?: number | null;
  trangThai: boolean;
}

// ===== Lop sinh hoat =====
export interface LopSinhHoat {
  id: number;
  maLop: string;
  gvcnId: number;
  tenGvcn: string;
  thuKyId?: number | null;
  tenThuKy?: string | null;
  chuongTrinhDtId: number;
  maCtdt: string;
  soSinhVien: number;
  createdAt: string;
}

export interface CreateLopSinhHoat {
  maLop: string;
  gvcnId: number;
  chuongTrinhDtId: number;
}

export interface UpdateLopSinhHoat {
  maLop: string;
  gvcnId: number;
  thuKyId?: number | null;
  chuongTrinhDtId: number;
}

// ===== Nganh hoc =====
export interface NganhHoc {
  id: number;
  maNganh: string;
  tenNganh: string;
  nganhChaId?: number | null;
  tenNganhCha?: string | null;
  createdAt: string;
}

export interface UpsertNganhHoc {
  maNganh: string;
  tenNganh: string;
  nganhChaId?: number | null;
}

// ===== Chuong trinh dao tao =====
export interface ChuongTrinhDT {
  id: number;
  maCtdt: string;
  nganhId: number;
  tenNganh: string;
  khoaHoc: string;
  createdAt: string;
}

export interface UpsertChuongTrinhDT {
  maCtdt: string;
  nganhId: number;
  khoaHoc: string;
}

// ===== Chi tiet CTDT =====
export interface ChiTietCTDT {
  id: number;
  ctdtId: number;
  maCtdt: string;
  monHocId: number;
  maMon: string;
  tenMon: string;
  hocKyId: number;
  tenHocKy: string;
  soTinChi: number;
  tinhDiemTb: boolean;
  createdAt: string;
}

export interface CreateChiTietCTDT {
  ctdtId: number;
  monHocId: number;
  hocKyId: number;
  soTinChi: number;
  tinhDiemTb: boolean;
}

export interface UpdateChiTietCTDT {
  soTinChi: number;
  tinhDiemTb: boolean;
}

// ===== Mon hoc =====
export interface MonHoc {
  id: number;
  maMon: string;
  tenMon: string;
  createdAt: string;
}

export interface UpsertMonHoc {
  maMon: string;
  tenMon: string;
}

// ===== Tai khoan =====
export interface TaiKhoan {
  id: number;
  tenDangNhap: string;
  vaiTroId: number;
  tenVaiTro: string;
  phongBanId?: number | null;
  tenPhongBan?: string | null;
  hoTen: string;
  email: string;
  anhDaiDien?: string | null;
  trangThai: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaiKhoan {
  tenDangNhap: string;
  matKhau: string;
  vaiTroId: number;
  phongBanId?: number | null;
  hoTen: string;
  email: string;
}

export interface UpdateTaiKhoan {
  vaiTroId: number;
  phongBanId?: number | null;
  hoTen: string;
  email: string;
  trangThai: boolean;
}

export const VAI_TRO_OPTIONS = [
  { id: 1, label: "Admin" },
  { id: 2, label: "Giáo viên" },
  { id: 3, label: "Sinh viên" },
  { id: 4, label: "Giáo vụ" },
];

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
  phongBanId?: number | null;
  tenPhongBan?: string | null;
  createdAt: string;
}

export interface UpsertNganhHoc {
  maNganh: string;
  tenNganh: string;
  nganhChaId?: number | null;
  phongBanId?: number | null;
}

// ===== Phong ban =====
export interface PhongBan {
  id: number;
  tenPhongBan: string;
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

// ===== Danh sach lop hoc phan (bang diem) =====
export interface DanhSachLopHP {
  id: number;
  lopHpId: number;
  maLopHp: string;
  maMon: string;
  tenMon: string;
  hocKyId: number;
  tenHocKy: string;
  tenGiaoVien: string;
  loaiDangKy: string;
  trangThaiDuyet: string;
  diemQt1?: number | null;
  diemQt2?: number | null;
  diemThi?: number | null;
  diemTongKet?: number | null;
  soTienPhaiDong?: number | null;
  trangThaiDongTien?: string | null;
  khoaBangDiem: boolean;
  tenSinhVien?: string | null;
  mssv?: string | null;
}

// ===== Kiem tra dieu kien tot nghiep =====
export interface MonHocConThieu {
  maMon: string;
  tenMon: string;
  soTinChi: number;
  tenHocKy: string;
}

export interface TotNghiep {
  duDieuKienTotNghiep: boolean;
  tongSoTinChiYeuCau: number;
  tongSoTinChiDaTichLuy: number;
  monHocConThieu: MonHocConThieu[];
}

// ===== Lop hoc phan =====
export interface LopHocPhan {
  id: number;
  maLopHp: string;
  chiTietCtdtId: number;
  maMon: string;
  tenMon: string;
  hocKyId: number;
  tenHocKy: string;
  giaoVienId: number;
  tenGiaoVien: string;
  soSinhVien: number;
  khoaBangDiem: boolean;
  trangThaiKetThuc: boolean;
}

export interface NhapDiem {
  diemQt1?: number | null;
  diemQt2?: number | null;
  diemThi?: number | null;
}

// ===== Thong bao =====
export interface ThongBao {
  id: number;
  tieuDe: string;
  noiDung: string;
  loaiThongBao: string;
  mucDo: string;
  nguoiTaoId: number;
  tenNguoiTao: string;
  lopNhanId?: number | null;
  tenLopNhan?: string | null;
  ngayTao: string;
  daDoc?: boolean | null;
}

export interface CreateThongBao {
  tieuDe: string;
  noiDung: string;
  loaiThongBao: string;
  mucDo: string;
  lopNhanId?: number | null;
}

// ===== Hoc ky =====
export interface HocKy {
  id: number;
  tenHocKy: string;
  ngayBatDau: string;
  tenNamHoc: string;
}

// ===== Tuan hoc =====
export interface TuanHoc {
  id: number;
  maTuan: string;
  soThuTuTuan: number;
  ngayBatDau: string;
  ngayKetThuc: string;
  tenNamHoc: string;
}

// ===== Thoi khoa bieu =====
export interface ThoiKhoaBieu {
  id: number;
  lopHpId: number;
  maLopHp: string;
  maMon: string;
  tenMon: string;
  tenGiaoVien: string;
  hocKyId: number;
  tenHocKy: string;
  tuanHocId: number;
  maTuan: string;
  soThuTuTuan: number;
  /** 2 = Thứ Hai ... 7 = Thứ Bảy, 8 = Chủ nhật */
  thu: number;
  tietBatDau: number;
  tietKetThuc: number;
  phongHoc: string;
  ngayHoc: string;
}

export interface CreateThoiKhoaBieu {
  lopHpId: number;
  tuanHocId: number;
  thu: number;
  tietBatDau: number;
  tietKetThuc: number;
  phongHoc: string;
}

export interface UpdateThoiKhoaBieu {
  tuanHocId: number;
  thu: number;
  tietBatDau: number;
  tietKetThuc: number;
  phongHoc: string;
}

// ===== Hoc phi =====
export interface GenerateHocPhi {
  hocKyId: number;
  tienMotTinChi: number;
}

export interface GenerateHocPhiResult {
  created: number;
  skipped: number;
  message: string;
}

export interface HocPhi {
  id: number;
  sinhVienId: number;
  tenSinhVien: string;
  mssv: string;
  hocKyId: number;
  tenHocKy: string;
  soTien: number;
  trangThaiDong: string;
  createdAt: string;
}

export interface CreateHocPhi {
  sinhVienId: number;
  hocKyId: number;
  soTien: number;
}

// ===== Yeu cau hanh chinh =====
export interface YeuCauHanhChinh {
  id: number;
  sinhVienId: number;
  tenSinhVien: string;
  mssv: string;
  loaiYeuCau: string;
  noiDung: string;
  fileDinhKem?: string | null;
  trangThai: string;
  nguoiDuyetId?: number | null;
  tenNguoiDuyet?: string | null;
  ngayTao: string;
  createdAt: string;
}

export interface CreateYeuCauHanhChinh {
  loaiYeuCau: string;
  noiDung: string;
  fileDinhKem?: string | null;
}

export interface DuyetYeuCauHanhChinh {
  trangThai: string;
  ghiChu?: string | null;
}

// ===== Yeu cau sua diem =====
export interface YeuCauSuaDiem {
  id: number;
  lopHpId: number;
  maLopHp: string;
  tenMon: string;
  tenHocKy: string;
  giaoVienId: number;
  tenGiaoVien: string;
  lyDo: string;
  trangThai: string;
  nguoiDuyetId?: number | null;
  tenNguoiDuyet?: string | null;
  createdAt: string;
}

export interface CreateYeuCauSuaDiem {
  lopHpId: number;
  lyDo: string;
}

export interface DuyetYeuCauSuaDiem {
  trangThai: string;
}

export const VAI_TRO_OPTIONS = [
  { id: 1, label: "Admin" },
  { id: 2, label: "Giáo viên" },
  { id: 3, label: "Sinh viên" },
  { id: 4, label: "Giáo vụ" },
];

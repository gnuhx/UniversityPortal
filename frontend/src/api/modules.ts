import { createCrudApi } from "./crud";
import { apiClient } from "./client";
import type {
  SinhVien, CreateSinhVien, UpdateSinhVien,
  GiaoVien, CreateGiaoVien, UpdateGiaoVien,
  LopSinhHoat, CreateLopSinhHoat, UpdateLopSinhHoat,
  NganhHoc, UpsertNganhHoc,
  ChuongTrinhDT, UpsertChuongTrinhDT,
  ChiTietCTDT, CreateChiTietCTDT, UpdateChiTietCTDT,
  MonHoc, UpsertMonHoc,
  TaiKhoan, CreateTaiKhoan, UpdateTaiKhoan,
  DanhSachLopHP,
  LopHocPhan, NhapDiem,
  ThongBao, CreateThongBao,
  HocPhi, CreateHocPhi,
  ApiResponse,
} from "../types";

export const sinhVienApi = createCrudApi<SinhVien, CreateSinhVien, UpdateSinhVien>("/sinh-vien");
export const giaoVienApi = createCrudApi<GiaoVien, CreateGiaoVien, UpdateGiaoVien>("/giao-vien");
export const lopSinhHoatApi = createCrudApi<LopSinhHoat, CreateLopSinhHoat, UpdateLopSinhHoat>("/lop-sinh-hoat");
export const nganhHocApi = createCrudApi<NganhHoc, UpsertNganhHoc>("/nganh-hoc");
export const chuongTrinhDTApi = createCrudApi<ChuongTrinhDT, UpsertChuongTrinhDT>("/chuong-trinh-dt");
export const chiTietCTDTApi = createCrudApi<ChiTietCTDT, CreateChiTietCTDT, UpdateChiTietCTDT>("/chi-tiet-ctdt");
export const monHocApi = createCrudApi<MonHoc, UpsertMonHoc>("/mon-hoc");
export const taiKhoanApi = createCrudApi<TaiKhoan, CreateTaiKhoan, UpdateTaiKhoan>("/tai-khoan");

export const sinhVienMeApi = {
  async getMe() {
    const res = await apiClient.get<ApiResponse<SinhVien>>("/sinh-vien/me");
    return res.data.data;
  },
};

export const danhSachLopHPApi = {
  async getMe() {
    const res = await apiClient.get<ApiResponse<DanhSachLopHP[]>>("/danh-sach-lop-hp/me");
    return res.data.data;
  },
  async getByLopHocPhan(lopHpId: number) {
    const res = await apiClient.get<ApiResponse<DanhSachLopHP[]>>("/danh-sach-lop-hp", { params: { lopHpId } });
    return res.data.data;
  },
  async nhapDiem(id: number, dto: NhapDiem) {
    const res = await apiClient.put<ApiResponse<DanhSachLopHP>>(`/danh-sach-lop-hp/${id}/diem`, dto);
    return res.data.data;
  },
};

export const lopHocPhanApi = {
  async getMe() {
    const res = await apiClient.get<ApiResponse<LopHocPhan[]>>("/lop-hoc-phan/me");
    return res.data.data;
  },
  async khoaBangDiem(id: number) {
    const res = await apiClient.put<ApiResponse<null>>(`/lop-hoc-phan/${id}/khoa-bang-diem`, {});
    return res.data;
  },
  async moBangDiem(id: number) {
    const res = await apiClient.put<ApiResponse<null>>(`/lop-hoc-phan/${id}/mo-bang-diem`, {});
    return res.data;
  },
};

export const thongBaoApi = {
  async getAll() {
    const res = await apiClient.get<ApiResponse<ThongBao[]>>("/thong-bao");
    return res.data.data;
  },
  async getMe() {
    const res = await apiClient.get<ApiResponse<ThongBao[]>>("/thong-bao/me");
    return res.data.data;
  },
  async create(dto: CreateThongBao) {
    const res = await apiClient.post<ApiResponse<ThongBao>>("/thong-bao", dto);
    return res.data.data;
  },
  async markAsRead(id: number) {
    const res = await apiClient.put<ApiResponse<null>>(`/thong-bao/${id}/da-doc`, {});
    return res.data;
  },
  async remove(id: number) {
    const res = await apiClient.delete<ApiResponse<null>>(`/thong-bao/${id}`);
    return res.data;
  },
};

export const hocPhiApi = {
  async getMe() {
    const res = await apiClient.get<ApiResponse<HocPhi[]>>("/hoc-phi/me");
    return res.data.data;
  },
  async getByHocKy(hocKyId: number) {
    const res = await apiClient.get<ApiResponse<HocPhi[]>>("/hoc-phi", { params: { hocKyId } });
    return res.data.data;
  },
  async create(dto: CreateHocPhi) {
    const res = await apiClient.post<ApiResponse<HocPhi>>("/hoc-phi", dto);
    return res.data.data;
  },
  async updateTrangThai(id: number, trangThai: string) {
    const res = await apiClient.put<ApiResponse<HocPhi>>(`/hoc-phi/${id}/trang-thai`, { trangThai });
    return res.data.data;
  },
};

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
};

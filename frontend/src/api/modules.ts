import { createCrudApi } from "./crud";
import { apiClient } from "./client";
import type {
  SinhVien, CreateSinhVien, UpdateSinhVien,
  GiaoVien, CreateGiaoVien, UpdateGiaoVien,
  LopSinhHoat, CreateLopSinhHoat, UpdateLopSinhHoat,
  NganhHoc, UpsertNganhHoc,
  PhongBan,
  ChuongTrinhDT, UpsertChuongTrinhDT, CloneChuongTrinhDT, CloneChuongTrinhDTResult,
  ChiTietCTDT, CreateChiTietCTDT, UpdateChiTietCTDT,
  MonHoc, UpsertMonHoc,
  TaiKhoan, CreateTaiKhoan, UpdateTaiKhoan,
  DanhSachLopHP,
  LopHocPhan, NhapDiem,
  ThongBao, CreateThongBao,
  NamHoc, UpsertNamHoc,
  HocKy, UpsertHocKy, HocPhi, CreateHocPhi, GenerateHocPhi, GenerateHocPhiResult,
  TuanHoc, ThoiKhoaBieu, CreateThoiKhoaBieu, UpdateThoiKhoaBieu, GenerateThoiKhoaBieu, GenerateThoiKhoaBieuResult,
  YeuCauHanhChinh, CreateYeuCauHanhChinh, DuyetYeuCauHanhChinh,
  YeuCauSuaDiem, CreateYeuCauSuaDiem, DuyetYeuCauSuaDiem,
  TotNghiep,
  NoiDungTinh, UpsertNoiDungTinh,
  BienBanSHCN, BienBanSHCNSinhVien, CreateBienBanSHCN,
  ApiResponse, PagedResult,
} from "../types";

export const sinhVienApi = createCrudApi<SinhVien, CreateSinhVien, UpdateSinhVien>("/sinh-vien");
export const giaoVienApi = createCrudApi<GiaoVien, CreateGiaoVien, UpdateGiaoVien>("/giao-vien");
export const lopSinhHoatApi = {
  ...createCrudApi<LopSinhHoat, CreateLopSinhHoat, UpdateLopSinhHoat>("/lop-sinh-hoat"),
  /** Sinh viên: chi tiết lớp sinh hoạt của mình, kèm roster bạn cùng lớp. */
  async getMe() {
    const res = await apiClient.get<ApiResponse<LopSinhHoat>>("/lop-sinh-hoat/me");
    return res.data.data;
  },
  /** Giáo viên: (các) lớp mình là GVCN. */
  async getMeGvcn() {
    const res = await apiClient.get<ApiResponse<LopSinhHoat[]>>("/lop-sinh-hoat/me-gvcn");
    return res.data.data;
  },
};
export const nganhHocApi = createCrudApi<NganhHoc, UpsertNganhHoc>("/nganh-hoc");
export const phongBanApi = createCrudApi<PhongBan, unknown>("/phong-ban");
export const chuongTrinhDTApi = {
  ...createCrudApi<ChuongTrinhDT, UpsertChuongTrinhDT>("/chuong-trinh-dt"),
  async clone(dto: CloneChuongTrinhDT) {
    const res = await apiClient.post<ApiResponse<CloneChuongTrinhDTResult>>("/chuong-trinh-dt/clone", dto);
    return res.data.data;
  },
};
export const chiTietCTDTApi = createCrudApi<ChiTietCTDT, CreateChiTietCTDT, UpdateChiTietCTDT>("/chi-tiet-ctdt");
export const monHocApi = createCrudApi<MonHoc, UpsertMonHoc>("/mon-hoc");
export const taiKhoanApi = createCrudApi<TaiKhoan, CreateTaiKhoan, UpdateTaiKhoan>("/tai-khoan");
export const namHocApi = createCrudApi<NamHoc, UpsertNamHoc>("/nam-hoc");

export const noiDungTinhApi = {
  ...createCrudApi<NoiDungTinh, UpsertNoiDungTinh>("/noi-dung-tinh"),
  async getByKhuVuc(khuVuc: string) {
    const res = await apiClient.get<ApiResponse<NoiDungTinh[]>>("/noi-dung-tinh", { params: { khuVuc } });
    return res.data.data;
  },
};

export const sinhVienMeApi = {
  async getMe() {
    const res = await apiClient.get<ApiResponse<SinhVien>>("/sinh-vien/me");
    return res.data.data;
  },
  async getTotNghiep() {
    const res = await apiClient.get<ApiResponse<TotNghiep>>("/sinh-vien/me/tot-nghiep");
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
  async getPaged(params: Record<string, unknown> = {}) {
    const res = await apiClient.get<ApiResponse<PagedResult<LopHocPhan>>>("/lop-hoc-phan", { params });
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

export const hocKyApi = {
  ...createCrudApi<HocKy, UpsertHocKy>("/hoc-ky"),
  async getMe() {
    const res = await apiClient.get<ApiResponse<HocKy[]>>("/hoc-ky/me");
    return res.data.data;
  },
};

export const tuanHocApi = {
  async getAll() {
    const res = await apiClient.get<ApiResponse<TuanHoc[]>>("/tuan-hoc/all");
    return res.data.data;
  },
};

export const thoiKhoaBieuApi = {
  ...createCrudApi<ThoiKhoaBieu, CreateThoiKhoaBieu, UpdateThoiKhoaBieu>("/thoi-khoa-bieu"),
  async getMe(hocKyId?: number) {
    const res = await apiClient.get<ApiResponse<ThoiKhoaBieu[]>>("/thoi-khoa-bieu/me", {
      params: hocKyId ? { hocKyId } : undefined,
    });
    return res.data.data;
  },
  async generate(dto: GenerateThoiKhoaBieu) {
    const res = await apiClient.post<ApiResponse<GenerateThoiKhoaBieuResult>>("/thoi-khoa-bieu/generate", dto);
    return res.data.data;
  },
};

export const hocPhiApi = {
  async getMe() {
    const res = await apiClient.get<ApiResponse<HocPhi[]>>("/hoc-phi/me");
    return res.data.data;
  },
  async getAll(hocKyId?: number) {
    const res = await apiClient.get<ApiResponse<HocPhi[]>>("/hoc-phi", {
      params: hocKyId ? { hocKyId } : undefined,
    });
    return res.data.data;
  },
  async create(dto: CreateHocPhi) {
    const res = await apiClient.post<ApiResponse<HocPhi>>("/hoc-phi", dto);
    return res.data.data;
  },
  async generate(dto: GenerateHocPhi) {
    const res = await apiClient.post<ApiResponse<GenerateHocPhiResult>>("/hoc-phi/generate", dto);
    return res.data.data;
  },
  async updateTrangThai(id: number, trangThai: string) {
    const res = await apiClient.put<ApiResponse<HocPhi>>(`/hoc-phi/${id}/trang-thai`, { trangThai });
    return res.data.data;
  },
};

export const yeuCauHanhChinhApi = {
  async getMe() {
    const res = await apiClient.get<ApiResponse<YeuCauHanhChinh[]>>("/yeu-cau-hanh-chinh/me");
    return res.data.data;
  },
  async getAll(page = 1, pageSize = 20, trangThai?: string) {
    const res = await apiClient.get<ApiResponse<PagedResult<YeuCauHanhChinh>>>("/yeu-cau-hanh-chinh", {
      params: { page, pageSize, trangThai },
    });
    return res.data.data;
  },
  async create(dto: CreateYeuCauHanhChinh) {
    const res = await apiClient.post<ApiResponse<YeuCauHanhChinh>>("/yeu-cau-hanh-chinh", dto);
    return res.data.data;
  },
  async duyet(id: number, dto: DuyetYeuCauHanhChinh) {
    const res = await apiClient.put<ApiResponse<YeuCauHanhChinh>>(`/yeu-cau-hanh-chinh/${id}/duyet`, dto);
    return res.data.data;
  },
};

export const yeuCauSuaDiemApi = {
  async getMe() {
    const res = await apiClient.get<ApiResponse<YeuCauSuaDiem[]>>("/yeu-cau-sua-diem/me");
    return res.data.data;
  },
  async getAll(page = 1, pageSize = 20, trangThai?: string) {
    const res = await apiClient.get<ApiResponse<PagedResult<YeuCauSuaDiem>>>("/yeu-cau-sua-diem", {
      params: { page, pageSize, trangThai },
    });
    return res.data.data;
  },
  async create(dto: CreateYeuCauSuaDiem) {
    const res = await apiClient.post<ApiResponse<YeuCauSuaDiem>>("/yeu-cau-sua-diem", dto);
    return res.data.data;
  },
  async duyet(id: number, dto: DuyetYeuCauSuaDiem) {
    const res = await apiClient.put<ApiResponse<YeuCauSuaDiem>>(`/yeu-cau-sua-diem/${id}/duyet`, dto);
    return res.data.data;
  },
};

export const bienBanShcnApi = {
  /** Admin/Giáo vụ: danh sách phân trang, lọc theo lớp (bỏ trống = tất cả lớp). */
  async getPaged(params: Record<string, unknown> = {}) {
    const res = await apiClient.get<ApiResponse<PagedResult<BienBanSHCN>>>("/bien-ban-shcn", { params });
    return res.data.data;
  },
  /** Admin/Giáo vụ: chi tiết 1 biên bản. */
  async getById(id: number) {
    const res = await apiClient.get<ApiResponse<BienBanSHCN>>(`/bien-ban-shcn/${id}`);
    return res.data.data;
  },
  /** Giáo viên: danh sách phân trang của 1 lớp mình chủ nhiệm. */
  async getPagedForGvcn(lopId: number, page = 1, pageSize = 20) {
    const res = await apiClient.get<ApiResponse<PagedResult<BienBanSHCN>>>("/bien-ban-shcn/me-gvcn", {
      params: { lopId, page, pageSize },
    });
    return res.data.data;
  },
  /** Giáo viên: chi tiết 1 biên bản của lớp mình chủ nhiệm. */
  async getDetailForGvcn(id: number) {
    const res = await apiClient.get<ApiResponse<BienBanSHCN>>(`/bien-ban-shcn/me-gvcn/${id}`);
    return res.data.data;
  },
  /** Giáo viên: tạo biên bản mới cho lớp mình chủ nhiệm. */
  async create(dto: CreateBienBanSHCN) {
    const res = await apiClient.post<ApiResponse<BienBanSHCN>>("/bien-ban-shcn", dto);
    return res.data.data;
  },
  /** Sinh viên: danh sách biên bản của lớp mình (ẩn lý do vắng của bạn khác). */
  async getMe() {
    const res = await apiClient.get<ApiResponse<BienBanSHCNSinhVien[]>>("/bien-ban-shcn/me");
    return res.data.data;
  },
};

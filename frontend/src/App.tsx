import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AppLayout } from "./components/AppLayout";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { LoginPage } from "./pages/LoginPage";
import { DashboardPage } from "./pages/DashboardPage";
import { HoSoPage } from "./pages/HoSoPage";
import { BangDiemPage } from "./pages/BangDiemPage";
import { SinhVienPage } from "./pages/SinhVienPage";
import { GiaoVienPage } from "./pages/GiaoVienPage";
import { LopSinhHoatPage } from "./pages/LopSinhHoatPage";
import { NganhHocPage } from "./pages/NganhHocPage";
import { NamHocPage } from "./pages/NamHocPage";
import { HocKyPage } from "./pages/HocKyPage";
import { ChiTietCTDTPage } from "./pages/ChiTietCTDTPage";
import { MonHocPage } from "./pages/MonHocPage";
import { TaiKhoanPage } from "./pages/TaiKhoanPage";
import { LopHocPhanPage } from "./pages/LopHocPhanPage";
import { ThongBaoPage } from "./pages/ThongBaoPage";
import { HocPhiPage } from "./pages/HocPhiPage";
import { YeuCauHanhChinhPage } from "./pages/YeuCauHanhChinhPage";
import { YeuCauSuaDiemPage } from "./pages/YeuCauSuaDiemPage";
import { ThoiKhoaBieuPage } from "./pages/ThoiKhoaBieuPage";
import { ROLES } from "./constants/roles";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route element={<ProtectedRoute />}>
          <Route element={<AppLayout />}>
            <Route path="/" element={<DashboardPage />} />
            <Route path="/nganh-hoc" element={<NganhHocPage />} />
            <Route path="/nam-hoc" element={<NamHocPage />} />
            <Route path="/nam-hoc/:id" element={<HocKyPage />} />
            {/* Trang danh sách CTĐT đã gộp vào /nganh-hoc (task #11) — redirect để không vỡ link cũ */}
            <Route path="/chuong-trinh-dt" element={<Navigate to="/nganh-hoc" replace />} />
            <Route path="/chuong-trinh-dt/:id" element={<ChiTietCTDTPage />} />
            <Route path="/mon-hoc" element={<MonHocPage />} />
            {/* ThoiKhoaBieuPage tự phân nhánh theo vai trò bên trong, không cần giới hạn allowedRoles ở route */}
            <Route path="/thoi-khoa-bieu" element={<ThoiKhoaBieuPage />} />

            {/* Sinh viên */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.SINH_VIEN]} />}>
              <Route path="/ho-so" element={<HoSoPage />} />
              <Route path="/bang-diem" element={<BangDiemPage />} />
              <Route path="/hoc-phi" element={<HocPhiPage />} />
              <Route path="/thong-bao" element={<ThongBaoPage />} />
              <Route path="/yeu-cau-hanh-chinh" element={<YeuCauHanhChinhPage />} />
            </Route>

            {/* Giáo viên */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.GIAO_VIEN]} />}>
              <Route path="/lop-hoc-phan" element={<LopHocPhanPage />} />
              <Route path="/yeu-cau-sua-diem" element={<YeuCauSuaDiemPage />} />
            </Route>

            {/* Admin / Giáo vụ */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN, ROLES.GIAO_VU]} />}>
              <Route path="/sinh-vien" element={<SinhVienPage />} />
              <Route path="/giao-vien" element={<GiaoVienPage />} />
              <Route path="/lop-sinh-hoat" element={<LopSinhHoatPage />} />
              <Route path="/tai-khoan" element={<TaiKhoanPage />} />
              <Route path="/thong-bao" element={<ThongBaoPage />} />
              <Route path="/hoc-phi" element={<HocPhiPage />} />
              <Route path="/yeu-cau-hanh-chinh" element={<YeuCauHanhChinhPage />} />
            </Route>

            {/* Admin only */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN]} />}>
              <Route path="/yeu-cau-sua-diem" element={<YeuCauSuaDiemPage />} />
            </Route>
          </Route>
        </Route>

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

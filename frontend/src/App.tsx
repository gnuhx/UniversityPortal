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
import { ChuongTrinhDTPage } from "./pages/ChuongTrinhDTPage";
import { ChiTietCTDTPage } from "./pages/ChiTietCTDTPage";
import { MonHocPage } from "./pages/MonHocPage";
import { TaiKhoanPage } from "./pages/TaiKhoanPage";
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
            <Route path="/chuong-trinh-dt" element={<ChuongTrinhDTPage />} />
            <Route path="/chuong-trinh-dt/:id" element={<ChiTietCTDTPage />} />
            <Route path="/mon-hoc" element={<MonHocPage />} />

            {/* Sinh viên */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.SINH_VIEN]} />}>
              <Route path="/ho-so" element={<HoSoPage />} />
              <Route path="/bang-diem" element={<BangDiemPage />} />
            </Route>

            {/* Admin / Giáo vụ */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN, ROLES.GIAO_VU]} />}>
              <Route path="/sinh-vien" element={<SinhVienPage />} />
              <Route path="/giao-vien" element={<GiaoVienPage />} />
              <Route path="/lop-sinh-hoat" element={<LopSinhHoatPage />} />
              <Route path="/tai-khoan" element={<TaiKhoanPage />} />
            </Route>
          </Route>
        </Route>

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

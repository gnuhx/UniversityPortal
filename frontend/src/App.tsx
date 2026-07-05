import { BrowserRouter, Routes, Route, Navigate, useParams } from "react-router-dom";
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

/** Task #12: /nam-hoc/:id gộp vào trang Học kỳ độc lập — redirect giữ link cũ không vỡ. */
function RedirectToHocKy() {
  const { id } = useParams<{ id: string }>();
  return <Navigate to={`/hoc-ky?namHocId=${id}`} replace />;
}

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
            {/* Trang Học kỳ theo năm học đã gộp vào /hoc-ky (task #12) — redirect để không vỡ link cũ */}
            <Route path="/nam-hoc/:id" element={<RedirectToHocKy />} />
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
            </Route>

            {/* Giáo viên */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.GIAO_VIEN]} />}>
              <Route path="/lop-hoc-phan" element={<LopHocPhanPage />} />
            </Route>

            {/* Admin / Giáo vụ */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN, ROLES.GIAO_VU]} />}>
              <Route path="/sinh-vien" element={<SinhVienPage />} />
              <Route path="/giao-vien" element={<GiaoVienPage />} />
              <Route path="/lop-sinh-hoat" element={<LopSinhHoatPage />} />
              <Route path="/tai-khoan" element={<TaiKhoanPage />} />
            </Route>

            {/* Admin only */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN]} />}>
              <Route path="/hoc-ky" element={<HocKyPage />} />
            </Route>

            {/*
              ThongBaoPage/HocPhiPage/YeuCauHanhChinhPage/YeuCauSuaDiemPage tự phân nhánh nội dung
              theo vai trò bên trong (giống ThoiKhoaBieuPage) — mỗi path chỉ được khai báo 1 lần với
              đúng tập vai trò được phép, tránh lặp path dưới nhiều ProtectedRoute khác nhau (React
              Router chỉ khớp route đầu tiên trùng path, khiến các vai trò ở route trùng phía sau
              không bao giờ tới được — đây chính là lỗi Admin/Giáo vụ bị đá về "/" khi vào các trang
              này trước khi sửa).
            */}
            <Route element={<ProtectedRoute allowedRoles={[ROLES.SINH_VIEN, ROLES.ADMIN, ROLES.GIAO_VU]} />}>
              <Route path="/thong-bao" element={<ThongBaoPage />} />
              <Route path="/hoc-phi" element={<HocPhiPage />} />
              <Route path="/yeu-cau-hanh-chinh" element={<YeuCauHanhChinhPage />} />
            </Route>
            <Route element={<ProtectedRoute allowedRoles={[ROLES.GIAO_VIEN, ROLES.ADMIN]} />}>
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

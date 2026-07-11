import { Layout, Menu, Avatar, Dropdown, Space, Badge } from "antd";
import {
  DashboardOutlined,
  UserOutlined,
  TeamOutlined,
  ApartmentOutlined,
  BookOutlined,
  IdcardOutlined,
  BankOutlined,
  LogoutOutlined,
  ReadOutlined,
  FileTextOutlined,
  ScheduleOutlined,
  BellOutlined,
  DollarOutlined,
  CalendarOutlined,
  FolderOutlined,
  SolutionOutlined,
  SettingOutlined,
} from "@ant-design/icons";
import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { GlobalLoader } from "./GlobalLoader";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import { logout as logoutApi } from "../api/auth";
import { hocPhiApi, yeuCauHanhChinhApi, yeuCauSuaDiemApi } from "../api/modules";

const { Header, Sider, Content } = Layout;

const allMenuItems = [
  { key: "/", icon: <DashboardOutlined />, label: "Tổng quan", roles: undefined },

  // Sinh viên
  { key: "/ho-so", icon: <UserOutlined />, label: "Hồ sơ cá nhân", roles: [ROLES.SINH_VIEN] },
  { key: "/bang-diem", icon: <ReadOutlined />, label: "Bảng điểm", roles: [ROLES.SINH_VIEN] },
  { key: "/hoc-phi", icon: <DollarOutlined />, label: "Học phí", roles: [ROLES.SINH_VIEN] },
  { key: "/thong-bao", icon: <BellOutlined />, label: "Thông báo", roles: [ROLES.SINH_VIEN] },
  { key: "/yeu-cau-hanh-chinh", icon: <FileTextOutlined />, label: "Yêu cầu hành chính", roles: [ROLES.SINH_VIEN] },

  // Giáo viên
  { key: "/lop-hoc-phan", icon: <ScheduleOutlined />, label: "Lớp học phần", roles: [ROLES.GIAO_VIEN] },
  { key: "/yeu-cau-sua-diem", icon: <FileTextOutlined />, label: "Yêu cầu sửa điểm", roles: [ROLES.GIAO_VIEN] },

  // Admin / Giáo vụ
  { key: "/sinh-vien", icon: <UserOutlined />, label: "Sinh viên", roles: [ROLES.ADMIN, ROLES.GIAO_VU] },
  { key: "/giao-vien", icon: <IdcardOutlined />, label: "Giáo viên", roles: [ROLES.ADMIN, ROLES.GIAO_VU] },
  { key: "/tai-khoan", icon: <BankOutlined />, label: "Tài khoản", roles: [ROLES.ADMIN, ROLES.GIAO_VU] },
  { key: "/thong-bao", icon: <BellOutlined />, label: "Thông báo", roles: [ROLES.ADMIN, ROLES.GIAO_VU] },
  { key: "/hoc-phi", icon: <DollarOutlined />, label: "Học phí", roles: [ROLES.ADMIN, ROLES.GIAO_VU] },
  { key: "/yeu-cau-hanh-chinh", icon: <FileTextOutlined />, label: "Yêu cầu hành chính", roles: [ROLES.ADMIN, ROLES.GIAO_VU] },
  { key: "/yeu-cau-sua-diem", icon: <FileTextOutlined />, label: "Yêu cầu sửa điểm", roles: [ROLES.ADMIN] },

  // Tất cả roles
  {
    key: "/thoi-khoa-bieu",
    icon: <CalendarOutlined />,
    label: "Thời khoá biểu",
    roles: [ROLES.SINH_VIEN, ROLES.GIAO_VIEN, ROLES.ADMIN, ROLES.GIAO_VU],
  },
  {
    key: "/lop-sinh-hoat",
    icon: <TeamOutlined />,
    label: "Lớp sinh hoạt",
    roles: [ROLES.SINH_VIEN, ROLES.GIAO_VIEN, ROLES.ADMIN, ROLES.GIAO_VU],
  },
  { key: "/nganh-hoc", icon: <ApartmentOutlined />, label: "Ngành học & CTĐT", roles: undefined },
  { key: "/nam-hoc", icon: <CalendarOutlined />, label: "Năm học", roles: [ROLES.ADMIN, ROLES.GIAO_VU, ROLES.GIAO_VIEN] },
  { key: "/hoc-ky", icon: <CalendarOutlined />, label: "Học kỳ", roles: [ROLES.ADMIN] },
  { key: "/mon-hoc", icon: <BookOutlined />, label: "Môn học", roles: [ROLES.ADMIN, ROLES.GIAO_VU, ROLES.GIAO_VIEN] },
  { key: "/thu-vien", icon: <FolderOutlined />, label: "Thư viện", roles: undefined },
  { key: "/hoc-vu", icon: <SolutionOutlined />, label: "Học Vụ", roles: undefined },
  { key: "/noi-dung-tinh", icon: <SettingOutlined />, label: "Quản lý Thư viện/Học Vụ", roles: [ROLES.ADMIN] },
];

const isSinhVien = (vaiTro?: string) => vaiTro === ROLES.SINH_VIEN;
const isAdminOrGiaoVu = (vaiTro?: string) => vaiTro === ROLES.ADMIN || vaiTro === ROLES.GIAO_VU;
const isAdmin = (vaiTro?: string) => vaiTro === ROLES.ADMIN;
const isGiaoVien = (vaiTro?: string) => vaiTro === ROLES.GIAO_VIEN;

export function AppLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuthStore();

  // Badge số lượng trên menu (task #21) — mỗi query chỉ bật đúng 1 vai trò
  // liên quan; queryKey trùng với trang đích để dùng chung cache khi cả 2
  // cùng mount (vd Sinh viên đang đứng ở /hoc-phi).
  const { data: hocPhiMe } = useQuery({
    queryKey: ["hoc-phi-me"],
    queryFn: () => hocPhiApi.getMe(),
    enabled: isSinhVien(user?.vaiTro),
    refetchInterval: 60_000,
  });
  const { data: yeuCauHanhChinhMe } = useQuery({
    queryKey: ["yeu-cau-hanh-chinh-me"],
    queryFn: () => yeuCauHanhChinhApi.getMe(),
    enabled: isSinhVien(user?.vaiTro),
    refetchInterval: 60_000,
  });
  const { data: yeuCauHanhChinhPending } = useQuery({
    queryKey: ["yeu-cau-hanh-chinh-pending-count"],
    queryFn: () => yeuCauHanhChinhApi.getAll(1, 1, "Chờ duyệt"),
    enabled: isAdminOrGiaoVu(user?.vaiTro),
    refetchInterval: 60_000,
  });
  const { data: yeuCauSuaDiemMe } = useQuery({
    queryKey: ["yeu-cau-sua-diem-me"],
    queryFn: () => yeuCauSuaDiemApi.getMe(),
    enabled: isGiaoVien(user?.vaiTro),
    refetchInterval: 60_000,
  });
  const { data: yeuCauSuaDiemPending } = useQuery({
    queryKey: ["yeu-cau-sua-diem-pending-count"],
    queryFn: () => yeuCauSuaDiemApi.getAll(1, 1, "Chờ duyệt"),
    enabled: isAdmin(user?.vaiTro),
    refetchInterval: 60_000,
  });

  const badgeCounts: Record<string, number> = {
    "/hoc-phi": (hocPhiMe ?? []).filter((x) => x.trangThaiDong === "Chưa đóng").length,
    "/yeu-cau-hanh-chinh": isSinhVien(user?.vaiTro)
      ? (yeuCauHanhChinhMe ?? []).filter((x) => x.trangThai === "Chờ duyệt").length
      : (yeuCauHanhChinhPending?.total ?? 0),
    "/yeu-cau-sua-diem": isGiaoVien(user?.vaiTro)
      ? (yeuCauSuaDiemMe ?? []).filter((x) => x.trangThai === "Chờ duyệt").length
      : (yeuCauSuaDiemPending?.total ?? 0),
  };

  const handleLogout = async () => {
    try {
      await logoutApi();
    } catch {
      // ignore - clear local session regardless
    }
    logout();
    navigate("/login");
  };

  const visibleItems = allMenuItems.filter(
    (item) => !item.roles || (user && (item.roles as string[]).includes(user.vaiTro)),
  );

  const selectedKey =
    visibleItems.find((item) => item.key !== "/" && location.pathname.startsWith(item.key))?.key || "/";

  return (
    <>
    <GlobalLoader />
    <Layout style={{ minHeight: "100vh" }}>
      <Sider breakpoint="lg" collapsedWidth="0">
        <div style={{ color: "#fff", textAlign: "center", padding: 16, fontWeight: "bold", fontSize: 16 }}>
          University Portal
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[selectedKey]}
          items={visibleItems.map(({ key, icon, label }) => ({
            key,
            icon: badgeCounts[key] ? <Badge size="small" count={badgeCounts[key]}>{icon}</Badge> : icon,
            label,
          }))}
          onClick={({ key }) => navigate(key)}
        />
      </Sider>
      <Layout>
        <Header style={{ background: "#fff", padding: "0 24px", display: "flex", justifyContent: "flex-end", alignItems: "center" }}>
          <Dropdown
            menu={{
              items: [{ key: "logout", icon: <LogoutOutlined />, label: "Đăng xuất", onClick: handleLogout }],
            }}
          >
            <Space style={{ cursor: "pointer" }}>
              <Avatar src={user?.anhDaiDien || undefined} icon={<UserOutlined />} />
              <span>
                {user?.hoTen} ({user?.vaiTro})
              </span>
            </Space>
          </Dropdown>
        </Header>
        <Content style={{ margin: 24, background: "#fff", padding: 24 }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
    </>
  );
}

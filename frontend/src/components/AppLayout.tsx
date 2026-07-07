import { Layout, Menu, Avatar, Dropdown, Space } from "antd";
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
import { GlobalLoader } from "./GlobalLoader";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import { logout as logoutApi } from "../api/auth";

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
  { key: "/lop-sinh-hoat", icon: <TeamOutlined />, label: "Lớp sinh hoạt", roles: [ROLES.ADMIN, ROLES.GIAO_VU] },
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
  { key: "/nganh-hoc", icon: <ApartmentOutlined />, label: "Ngành học & CTĐT", roles: undefined },
  { key: "/nam-hoc", icon: <CalendarOutlined />, label: "Năm học", roles: undefined },
  { key: "/hoc-ky", icon: <CalendarOutlined />, label: "Học kỳ", roles: [ROLES.ADMIN] },
  { key: "/mon-hoc", icon: <BookOutlined />, label: "Môn học", roles: undefined },
  { key: "/thu-vien", icon: <FolderOutlined />, label: "Thư viện", roles: undefined },
  { key: "/hoc-vu", icon: <SolutionOutlined />, label: "Học Vụ", roles: undefined },
  { key: "/noi-dung-tinh", icon: <SettingOutlined />, label: "Quản lý Thư viện/Học Vụ", roles: [ROLES.ADMIN] },
];

export function AppLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuthStore();

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
          items={visibleItems.map(({ key, icon, label }) => ({ key, icon, label }))}
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

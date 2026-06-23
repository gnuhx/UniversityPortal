import { Button, Card, Col, Row, Statistic, Tag } from "antd";
import { useQuery } from "@tanstack/react-query";
import {
  UserOutlined, IdcardOutlined, TeamOutlined, BookOutlined,
  ApartmentOutlined, ReadOutlined,
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import { sinhVienApi, giaoVienApi, lopSinhHoatApi, nganhHocApi, monHocApi, sinhVienMeApi, danhSachLopHPApi } from "../api/modules";

function StudentDashboard() {
  const navigate = useNavigate();

  const { data: sv } = useQuery({
    queryKey: ["sinh-vien-me"],
    queryFn: () => sinhVienMeApi.getMe(),
  });

  const { data: bangDiem } = useQuery({
    queryKey: ["danh-sach-lop-hp-me"],
    queryFn: () => danhSachLopHPApi.getMe(),
  });

  const soMon = bangDiem?.length ?? 0;
  const coGpa = bangDiem?.filter((x) => x.diemTongKet != null) ?? [];
  const gpa = coGpa.length > 0
    ? (coGpa.reduce((s, x) => s + (x.diemTongKet ?? 0), 0) / coGpa.length).toFixed(2)
    : "—";

  return (
    <div>
      <h2>Xin chào, {sv?.hoTen ?? "Sinh viên"}</h2>
      <p style={{ color: "#888" }}>
        MSSV: <strong>{sv?.mssv ?? "—"}</strong>
        {" · "}
        Lớp: <strong>{sv?.tenLop ?? "—"}</strong>
        {" · "}
        {sv?.trangThai ? <Tag color="green">Đang học</Tag> : <Tag color="red">Đã khoá</Tag>}
      </p>

      <Row gutter={16} style={{ marginTop: 24 }}>
        <Col xs={12} md={6}>
          <Card>
            <Statistic title="Số môn đã học" value={soMon} prefix={<BookOutlined />} />
          </Card>
        </Col>
        <Col xs={12} md={6}>
          <Card>
            <Statistic title="GPA tích lũy" value={gpa} prefix={<ReadOutlined />} />
          </Card>
        </Col>
      </Row>

      <Row gutter={16} style={{ marginTop: 24 }}>
        <Col xs={24} md={8}>
          <Card
            title="Bảng điểm"
            extra={<Button type="link" onClick={() => navigate("/bang-diem")}>Xem</Button>}
          >
            Xem điểm tất cả môn học theo từng học kỳ.
          </Card>
        </Col>
        <Col xs={24} md={8}>
          <Card
            title="Hồ sơ cá nhân"
            extra={<Button type="link" onClick={() => navigate("/ho-so")}>Xem</Button>}
          >
            Thông tin MSSV, lớp, email và trạng thái học.
          </Card>
        </Col>
        <Col xs={24} md={8}>
          <Card
            title="Yêu cầu hành chính"
            extra={<Button type="link" onClick={() => navigate("/yeu-cau-hanh-chinh")}>Xem</Button>}
            style={{ opacity: 0.6 }}
          >
            Tạo và theo dõi yêu cầu xác nhận, bảo lưu... <em>(sắp có)</em>
          </Card>
        </Col>
      </Row>
    </div>
  );
}

export function DashboardPage() {
  const user = useAuthStore((s) => s.user);
  const isAdminOrGiaoVu = user?.vaiTro === ROLES.ADMIN || user?.vaiTro === ROLES.GIAO_VU;
  const isSinhVien = user?.vaiTro === ROLES.SINH_VIEN;

  const sinhVienCount = useQuery({
    queryKey: ["sinh-vien-count"],
    queryFn: () => sinhVienApi.getPaged({ page: 1, pageSize: 1 }),
    enabled: isAdminOrGiaoVu,
  });
  const giaoVienCount = useQuery({
    queryKey: ["giao-vien-count"],
    queryFn: () => giaoVienApi.getPaged({ page: 1, pageSize: 1 }),
    enabled: isAdminOrGiaoVu,
  });
  const lopCount = useQuery({
    queryKey: ["lop-sinh-hoat-all"],
    queryFn: () => lopSinhHoatApi.getAll(),
    enabled: isAdminOrGiaoVu,
  });
  const nganhCount = useQuery({
    queryKey: ["nganh-hoc-all"],
    queryFn: () => nganhHocApi.getAll(),
    enabled: isAdminOrGiaoVu,
  });
  const monHocCount = useQuery({
    queryKey: ["mon-hoc-count"],
    queryFn: () => monHocApi.getPaged({ page: 1, pageSize: 1 }),
    enabled: isAdminOrGiaoVu,
  });

  if (isSinhVien) return <StudentDashboard />;

  return (
    <div>
      <h2>Xin chào, {user?.hoTen}</h2>
      <p>Vai trò: {user?.vaiTro}</p>
      <Row gutter={16} style={{ marginTop: 24 }}>
        <Col span={6}>
          <Card>
            <Statistic title="Sinh viên" value={sinhVienCount.data?.total ?? "-"} prefix={<UserOutlined />} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Giáo viên" value={giaoVienCount.data?.total ?? "-"} prefix={<IdcardOutlined />} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Lớp sinh hoạt" value={lopCount.data?.length ?? "-"} prefix={<TeamOutlined />} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Ngành học" value={nganhCount.data?.length ?? "-"} prefix={<ApartmentOutlined />} />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic title="Môn học" value={monHocCount.data?.total ?? "-"} prefix={<BookOutlined />} />
          </Card>
        </Col>
      </Row>
    </div>
  );
}

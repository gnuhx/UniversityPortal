import { Card, Col, Row, Statistic } from "antd";
import { useQuery } from "@tanstack/react-query";
import { UserOutlined, IdcardOutlined, TeamOutlined, BookOutlined, ApartmentOutlined } from "@ant-design/icons";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import { sinhVienApi, giaoVienApi, lopSinhHoatApi, nganhHocApi, monHocApi } from "../api/modules";

export function DashboardPage() {
  const user = useAuthStore((s) => s.user);
  const isAdminOrGiaoVu = user?.vaiTro === ROLES.ADMIN || user?.vaiTro === ROLES.GIAO_VU;

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
  });
  const nganhCount = useQuery({
    queryKey: ["nganh-hoc-all"],
    queryFn: () => nganhHocApi.getAll(),
  });
  const monHocCount = useQuery({
    queryKey: ["mon-hoc-count"],
    queryFn: () => monHocApi.getPaged({ page: 1, pageSize: 1 }),
  });

  return (
    <div>
      <h2>Xin chào, {user?.hoTen}</h2>
      <p>Vai trò: {user?.vaiTro}</p>
      <Row gutter={16} style={{ marginTop: 24 }}>
        {isAdminOrGiaoVu && (
          <Col span={6}>
            <Card>
              <Statistic title="Sinh viên" value={sinhVienCount.data?.total ?? "-"} prefix={<UserOutlined />} />
            </Card>
          </Col>
        )}
        {isAdminOrGiaoVu && (
          <Col span={6}>
            <Card>
              <Statistic title="Giáo viên" value={giaoVienCount.data?.total ?? "-"} prefix={<IdcardOutlined />} />
            </Card>
          </Col>
        )}
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

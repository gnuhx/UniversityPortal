import { Avatar, Card, Col, Descriptions, Row, Spin, Tag } from "antd";
import { UserOutlined } from "@ant-design/icons";
import { useQuery } from "@tanstack/react-query";
import { sinhVienMeApi } from "../api/modules";

export function HoSoPage() {
  const { data: sv, isLoading } = useQuery({
    queryKey: ["sinh-vien-me"],
    queryFn: () => sinhVienMeApi.getMe(),
  });

  if (isLoading) return <Spin />;
  if (!sv) return null;

  return (
    <div>
      <h2>Hồ sơ cá nhân</h2>
      <Row gutter={24} style={{ marginTop: 16 }}>
        <Col xs={24} md={6} style={{ textAlign: "center", marginBottom: 24 }}>
          <Card>
            <Avatar
              size={120}
              src={sv.anhDaiDien || undefined}
              icon={<UserOutlined />}
              style={{ marginBottom: 16 }}
            />
            <div style={{ fontWeight: "bold", fontSize: 16 }}>{sv.hoTen}</div>
            <div style={{ color: "#888", marginTop: 4 }}>Sinh viên</div>
            <div style={{ marginTop: 8 }}>
              {sv.trangThai ? (
                <Tag color="green">Đang học</Tag>
              ) : (
                <Tag color="red">Đã khoá</Tag>
              )}
            </div>
          </Card>
        </Col>

        <Col xs={24} md={18}>
          <Card title="Thông tin sinh viên">
            <Descriptions column={{ xs: 1, sm: 2 }} bordered size="middle">
              <Descriptions.Item label="MSSV">{sv.mssv}</Descriptions.Item>
              <Descriptions.Item label="Họ và tên">{sv.hoTen}</Descriptions.Item>
              <Descriptions.Item label="Email">{sv.email}</Descriptions.Item>
              <Descriptions.Item label="Lớp sinh hoạt">
                {sv.tenLop ?? <span style={{ color: "#aaa" }}>Chưa phân lớp</span>}
              </Descriptions.Item>
              <Descriptions.Item label="Ngày vào trường">
                {new Date(sv.createdAt).toLocaleDateString("vi-VN")}
              </Descriptions.Item>
              <Descriptions.Item label="Trạng thái">
                {sv.trangThai ? (
                  <Tag color="green">Đang học</Tag>
                ) : (
                  <Tag color="red">Đã khoá</Tag>
                )}
              </Descriptions.Item>
            </Descriptions>
          </Card>
        </Col>
      </Row>
    </div>
  );
}

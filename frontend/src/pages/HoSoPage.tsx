import { Alert, Avatar, Card, Col, Descriptions, Row, Spin, Table, Tag } from "antd";
import { CheckCircleOutlined, UserOutlined, WarningOutlined } from "@ant-design/icons";
import { useQuery } from "@tanstack/react-query";
import { sinhVienMeApi } from "../api/modules";

export function HoSoPage() {
  const { data: sv, isLoading } = useQuery({
    queryKey: ["sinh-vien-me"],
    queryFn: () => sinhVienMeApi.getMe(),
  });

  const { data: totNghiep, isLoading: isLoadingTotNghiep, isError: isErrorTotNghiep } = useQuery({
    queryKey: ["sinh-vien-me-tot-nghiep"],
    queryFn: () => sinhVienMeApi.getTotNghiep(),
    retry: false,
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

        <Col xs={24}>
          <Card title="Tình trạng tốt nghiệp">
            {isLoadingTotNghiep ? (
              <Spin />
            ) : isErrorTotNghiep || !totNghiep ? (
              <Alert
                type="info"
                showIcon
                message="Chưa thể xác định tình trạng tốt nghiệp"
                description="Không tìm thấy chương trình đào tạo áp dụng cho tài khoản của bạn. Vui lòng liên hệ phòng đào tạo để được hỗ trợ."
              />
            ) : totNghiep.duDieuKienTotNghiep ? (
              <Alert
                type="success"
                showIcon
                icon={<CheckCircleOutlined />}
                message="Đủ điều kiện tốt nghiệp"
                description={`Bạn đã hoàn thành ${totNghiep.tongSoTinChiDaTichLuy}/${totNghiep.tongSoTinChiYeuCau} tín chỉ theo chương trình đào tạo.`}
              />
            ) : (
              <>
                <Alert
                  type="warning"
                  showIcon
                  icon={<WarningOutlined />}
                  message="Chưa đủ điều kiện tốt nghiệp"
                  description={`Đã tích luỹ ${totNghiep.tongSoTinChiDaTichLuy}/${totNghiep.tongSoTinChiYeuCau} tín chỉ. Còn ${totNghiep.monHocConThieu.length} môn học cần hoàn thành:`}
                  style={{ marginBottom: 16 }}
                />
                <Table
                  rowKey="maMon"
                  size="small"
                  pagination={false}
                  dataSource={totNghiep.monHocConThieu}
                  columns={[
                    { title: "Mã môn", dataIndex: "maMon", key: "maMon" },
                    { title: "Tên môn", dataIndex: "tenMon", key: "tenMon" },
                    { title: "Số tín chỉ", dataIndex: "soTinChi", key: "soTinChi" },
                    { title: "Học kỳ dự kiến", dataIndex: "tenHocKy", key: "tenHocKy" },
                  ]}
                />
              </>
            )}
          </Card>
        </Col>
      </Row>
    </div>
  );
}

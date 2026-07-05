import { useState, type ReactNode } from "react";
import {
  Table, Tag, Card, Row, Col, Statistic, Select, Space, Button,
  Modal, Form, InputNumber, message, Alert, Empty, Spin, Typography,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  PlusOutlined, ThunderboltOutlined, CheckCircleOutlined,
  ClockCircleOutlined, GiftOutlined, WalletOutlined,
} from "@ant-design/icons";
import { hocPhiApi, hocKyApi } from "../api/modules";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import type { HocPhi, CreateHocPhi, GenerateHocPhi } from "../types";

const { Text } = Typography;
const TRANG_THAI_OPTIONS = ["Chưa đóng", "Đã đóng", "Miễn giảm"];

// Màu trạng thái tra từ palette đã validate (references/palette.md của skill dataviz) —
// luôn đi kèm icon + nhãn chữ, không dùng màu làm dấu hiệu duy nhất.
const TRANG_THAI_META: Record<string, { color: string; icon: ReactNode; accent: string }> = {
  "Đã đóng": { color: "success", icon: <CheckCircleOutlined />, accent: "#0ca30c" },
  "Chưa đóng": { color: "warning", icon: <ClockCircleOutlined />, accent: "#fab219" },
  "Miễn giảm": { color: "processing", icon: <GiftOutlined />, accent: "#2a78d6" },
};

function TrangThaiTag({ trangThai }: { trangThai: string }) {
  const meta = TRANG_THAI_META[trangThai];
  return (
    <Tag color={meta?.color} icon={meta?.icon}>
      {trangThai}
    </Tag>
  );
}

function fmtVnd(v: number) {
  return v.toLocaleString("vi-VN", { style: "currency", currency: "VND" });
}

/** Khu vực riêng cho 1 học kỳ — thay bảng phẳng để sinh viên thấy rõ từng kỳ. */
function SemesterFeeCard({ hocPhi }: { hocPhi: HocPhi }) {
  const meta = TRANG_THAI_META[hocPhi.trangThaiDong];
  return (
    <Card
      size="small"
      style={{
        borderLeft: `4px solid ${meta?.accent ?? "#c3c2b7"}`,
        height: "100%",
      }}
    >
      <Text type="secondary" style={{ fontSize: 12 }}>
        {hocPhi.tenHocKy}
      </Text>
      <div style={{ fontSize: 22, fontWeight: 600, margin: "4px 0 8px" }}>
        {fmtVnd(hocPhi.soTien)}
      </div>
      <Space style={{ width: "100%", justifyContent: "space-between" }}>
        <TrangThaiTag trangThai={hocPhi.trangThaiDong} />
        <Text type="secondary" style={{ fontSize: 12 }}>
          {new Date(hocPhi.createdAt).toLocaleDateString("vi-VN")}
        </Text>
      </Space>
    </Card>
  );
}

export function HocPhiPage() {
  const { user } = useAuthStore();
  const qc = useQueryClient();
  const isAdmin = user?.vaiTro === ROLES.ADMIN || user?.vaiTro === ROLES.GIAO_VU;

  const [filterHocKyId, setFilterHocKyId] = useState<number | undefined>();
  const [createOpen, setCreateOpen] = useState(false);
  const [generateOpen, setGenerateOpen] = useState(false);
  const [createForm] = Form.useForm<CreateHocPhi>();
  const [generateForm] = Form.useForm<GenerateHocPhi>();

  // Semester list for dropdowns
  const { data: hocKys = [] } = useQuery({
    queryKey: ["hoc-ky"],
    queryFn: () => hocKyApi.getAll(),
    enabled: isAdmin,
  });

  // Student: own bills
  const {
    data: myData,
    isLoading: myLoading,
    isError: myError,
  } = useQuery({
    queryKey: ["hoc-phi-me"],
    queryFn: () => hocPhiApi.getMe(),
    enabled: !isAdmin,
  });

  // Admin: all bills (optionally filtered by semester)
  const {
    data: allData,
    isLoading: allLoading,
  } = useQuery({
    queryKey: ["hoc-phi-admin", filterHocKyId],
    queryFn: () => hocPhiApi.getAll(filterHocKyId),
    enabled: isAdmin,
  });

  const items: HocPhi[] = isAdmin ? (allData ?? []) : (myData ?? []);
  const isLoading = isAdmin ? allLoading : myLoading;

  const createMutation = useMutation({
    mutationFn: (dto: CreateHocPhi) => hocPhiApi.create(dto),
    onSuccess: () => {
      message.success("Tạo học phí thành công.");
      qc.invalidateQueries({ queryKey: ["hoc-phi-admin"] });
      setCreateOpen(false);
      createForm.resetFields();
    },
    onError: (e: any) => message.error(e?.response?.data?.message ?? "Tạo học phí thất bại."),
  });

  const generateMutation = useMutation({
    mutationFn: (dto: GenerateHocPhi) => hocPhiApi.generate(dto),
    onSuccess: (result) => {
      message.success(result.message);
      qc.invalidateQueries({ queryKey: ["hoc-phi-admin"] });
      setGenerateOpen(false);
      generateForm.resetFields();
    },
    onError: (e: any) => message.error(e?.response?.data?.message ?? "Tạo hàng loạt thất bại."),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, trangThai }: { id: number; trangThai: string }) =>
      hocPhiApi.updateTrangThai(id, trangThai),
    onSuccess: () => {
      message.success("Cập nhật trạng thái thành công.");
      qc.invalidateQueries({ queryKey: ["hoc-phi-admin"] });
    },
    onError: () => message.error("Cập nhật thất bại."),
  });

  const total = items.reduce((s, x) => s + x.soTien, 0);
  const unpaid = items.filter((x) => x.trangThaiDong === "Chưa đóng").reduce((s, x) => s + x.soTien, 0);
  const paid = items.filter((x) => x.trangThaiDong === "Đã đóng").reduce((s, x) => s + x.soTien, 0);

  const adminColumns: ColumnsType<HocPhi> = [
    { title: "Sinh viên", dataIndex: "tenSinhVien", width: 160 },
    { title: "MSSV", dataIndex: "mssv", width: 110 },
    { title: "Học kỳ", dataIndex: "tenHocKy", width: 160 },
    {
      title: "Số tiền", dataIndex: "soTien", width: 160, align: "right",
      render: (v: number) => fmtVnd(v),
    },
    {
      title: "Trạng thái", dataIndex: "trangThaiDong", width: 130,
      render: (v: string) => <TrangThaiTag trangThai={v} />,
    },
    {
      title: "Cập nhật", width: 160,
      render: (_: unknown, row: HocPhi) => (
        <Select
          size="small"
          style={{ width: 140 }}
          value={row.trangThaiDong}
          options={TRANG_THAI_OPTIONS.map((v) => ({ value: v, label: v }))}
          onChange={(val) => updateMutation.mutate({ id: row.id, trangThai: val })}
        />
      ),
    },
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16 }} align="center" wrap>
        <h2 style={{ margin: 0 }}>Học phí</h2>
        {isAdmin && (
          <>
            <Button
              type="primary"
              icon={<ThunderboltOutlined />}
              onClick={() => setGenerateOpen(true)}
            >
              Tạo hàng loạt theo học kỳ
            </Button>
            <Button icon={<PlusOutlined />} onClick={() => setCreateOpen(true)}>
              Tạo đơn lẻ
            </Button>
            <Select
              allowClear
              placeholder="Lọc theo học kỳ"
              style={{ width: 220 }}
              options={hocKys.map((hk) => ({ value: hk.id, label: hk.tenHocKy }))}
              onChange={(v) => setFilterHocKyId(v)}
            />
          </>
        )}
      </Space>

      {/* Summary cards */}
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col xs={24} sm={8} md={6}>
          <Card size="small">
            <Statistic
              title="Tổng học phí"
              value={total}
              prefix={<WalletOutlined />}
              suffix="₫"
              formatter={(v) => Number(v).toLocaleString("vi-VN")}
            />
          </Card>
        </Col>
        {!isAdmin && (
          <>
            <Col xs={24} sm={8} md={6}>
              <Card size="small">
                <Statistic
                  title="Đã đóng"
                  value={paid}
                  prefix={<CheckCircleOutlined />}
                  suffix="₫"
                  valueStyle={{ color: TRANG_THAI_META["Đã đóng"].accent }}
                  formatter={(v) => Number(v).toLocaleString("vi-VN")}
                />
              </Card>
            </Col>
            <Col xs={24} sm={8} md={6}>
              <Card size="small">
                <Statistic
                  title="Chưa đóng"
                  value={unpaid}
                  prefix={<ClockCircleOutlined />}
                  suffix="₫"
                  valueStyle={{ color: unpaid > 0 ? "#d03b3b" : TRANG_THAI_META["Đã đóng"].accent }}
                  formatter={(v) => Number(v).toLocaleString("vi-VN")}
                />
              </Card>
            </Col>
          </>
        )}
      </Row>

      {/* Student error state */}
      {!isAdmin && myError && (
        <Alert
          type="error"
          showIcon
          message="Không thể tải thông tin học phí. Vui lòng thử lại sau."
          style={{ marginBottom: 16 }}
        />
      )}

      {/* Student empty state */}
      {!isAdmin && !myLoading && !myError && items.length === 0 && (
        <Empty
          description={
            <Text type="secondary">
              Chưa có thông tin học phí. Vui lòng liên hệ phòng giáo vụ.
            </Text>
          }
        />
      )}

      {/* Sinh viên: chia riêng từng học kỳ thành 1 thẻ thay vì bảng phẳng */}
      {!isAdmin && !myError && (myLoading || items.length > 0) && (
        <Spin spinning={isLoading}>
          <Row gutter={[16, 16]}>
            {items.map((hp) => (
              <Col key={hp.id} xs={24} sm={12} md={8} lg={6}>
                <SemesterFeeCard hocPhi={hp} />
              </Col>
            ))}
          </Row>
        </Spin>
      )}

      {/* Admin: bảng tổng hợp toàn trường, vẫn cần dạng bảng để tra cứu nhiều sinh viên */}
      {isAdmin && (
        <Spin spinning={isLoading}>
          <Table<HocPhi>
            rowKey="id"
            columns={adminColumns}
            dataSource={items}
            loading={false}
            pagination={{ pageSize: 20 }}
            scroll={{ x: 700 }}
            size="middle"
          />
        </Spin>
      )}

      {/* Generate modal (admin) */}
      <Modal
        title="Tạo học phí hàng loạt theo học kỳ"
        open={generateOpen}
        onCancel={() => { setGenerateOpen(false); generateForm.resetFields(); }}
        onOk={() => generateForm.submit()}
        okText="Tạo hàng loạt"
        cancelText="Huỷ"
        confirmLoading={generateMutation.isPending}
      >
        <Alert
          type="info"
          showIcon
          style={{ marginBottom: 16 }}
          message="Hệ thống sẽ tính học phí dựa trên số tín chỉ mỗi sinh viên đã đăng ký trong học kỳ. Sinh viên đã có học phí sẽ bị bỏ qua."
        />
        <Form form={generateForm} layout="vertical" onFinish={(v) => generateMutation.mutate(v)}>
          <Form.Item name="hocKyId" label="Học kỳ" rules={[{ required: true, message: "Chọn học kỳ" }]}>
            <Select
              placeholder="Chọn học kỳ"
              options={hocKys.map((hk) => ({
                value: hk.id,
                label: `${hk.tenHocKy} — ${hk.tenNamHoc}`,
              }))}
            />
          </Form.Item>
          <Form.Item
            name="tienMotTinChi"
            label="Tiền mỗi tín chỉ (VNĐ)"
            rules={[{ required: true, message: "Nhập tiền mỗi tín chỉ" }]}
          >
            <InputNumber
              min={0}
              style={{ width: "100%" }}
              placeholder="Ví dụ: 450000"
              formatter={(v) => `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",")}
              parser={(v) => (Number(v?.replace(/,/g, "") ?? 0) as unknown as 0)}
            />
          </Form.Item>
        </Form>
      </Modal>

      {/* Create single modal (admin) */}
      <Modal
        title="Tạo học phí đơn lẻ"
        open={createOpen}
        onCancel={() => { setCreateOpen(false); createForm.resetFields(); }}
        onOk={() => createForm.submit()}
        okText="Tạo"
        cancelText="Huỷ"
        confirmLoading={createMutation.isPending}
      >
        <Form form={createForm} layout="vertical" onFinish={(v) => createMutation.mutate(v)}>
          <Form.Item name="sinhVienId" label="ID Sinh viên" rules={[{ required: true }]}>
            <InputNumber style={{ width: "100%" }} placeholder="Nhập ID sinh viên" />
          </Form.Item>
          <Form.Item name="hocKyId" label="Học kỳ" rules={[{ required: true }]}>
            <Select
              placeholder="Chọn học kỳ"
              options={hocKys.map((hk) => ({
                value: hk.id,
                label: `${hk.tenHocKy} — ${hk.tenNamHoc}`,
              }))}
            />
          </Form.Item>
          <Form.Item name="soTien" label="Số tiền (VNĐ)" rules={[{ required: true }]}>
            <InputNumber
              min={0}
              style={{ width: "100%" }}
              formatter={(v) => `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",")}
              parser={(v) => (Number(v?.replace(/,/g, "") ?? 0) as unknown as 0)}
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

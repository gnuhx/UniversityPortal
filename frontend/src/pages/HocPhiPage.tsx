import { useEffect, useRef, useState, type ReactNode } from "react";
import {
  Table, Tag, Card, Collapse, Row, Col, Statistic, Select, Space, Button,
  Modal, Form, InputNumber, message, Alert, Empty, Spin, Typography, Descriptions,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  PlusOutlined, ThunderboltOutlined, CheckCircleOutlined,
  ClockCircleOutlined, GiftOutlined, WalletOutlined, MedicineBoxOutlined,
} from "@ant-design/icons";
import { hocPhiApi, hocKyApi, sinhVienApi, sinhVienMeApi } from "../api/modules";
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

/** Nhóm học phí theo Năm học, giữ nguyên thứ tự xuất hiện (API đã sắp mới nhất trước). */
function groupByNamHoc(items: HocPhi[]) {
  const groups = new Map<number, { namHocId: number; tenNamHoc: string; items: HocPhi[] }>();
  for (const hp of items) {
    const g = groups.get(hp.namHocId);
    if (g) g.items.push(hp);
    else groups.set(hp.namHocId, { namHocId: hp.namHocId, tenNamHoc: hp.tenNamHoc, items: [hp] });
  }
  return Array.from(groups.values());
}

/** Chia học phí của sinh viên theo Năm học (Collapse) — mỗi panel là lưới thẻ học kỳ của năm đó. */
function StudentFeeByYear({ items }: { items: HocPhi[] }) {
  const groups = groupByNamHoc(items);
  // Chỉ mở sẵn năm học mới nhất — groups[0] vì `items` đã sắp mới nhất trước.
  // Controlled activeKey vì defaultActiveKey của Collapse chỉ đọc 1 lần lúc mount,
  // trong khi `groups` chỉ có dữ liệu thật sau khi query tải xong.
  const [activeKeys, setActiveKeys] = useState<string[]>();
  useEffect(() => {
    if (activeKeys === undefined && groups.length > 0) {
      setActiveKeys([String(groups[0].namHocId)]);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [groups.length]);

  return (
    <Collapse
      activeKey={activeKeys}
      onChange={(keys) => setActiveKeys(Array.isArray(keys) ? keys : [keys])}
      items={groups.map((g) => ({
        key: String(g.namHocId),
        label: <Text strong>Năm học {g.tenNamHoc}</Text>,
        children: (
          <Row gutter={[16, 16]}>
            {g.items.map((hp) => (
              <Col key={hp.id} xs={24} sm={12} md={8} lg={6}>
                <SemesterFeeCard hocPhi={hp} />
              </Col>
            ))}
          </Row>
        ),
      }))}
    />
  );
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

  // Lọc Học kỳ theo Năm học trong 2 modal tạo học phí (chỉ để lọc UI, không gửi lên API)
  const [createNamHocId, setCreateNamHocId] = useState<number>();
  const [generateNamHocId, setGenerateNamHocId] = useState<number>();

  // Tìm sinh viên theo tên/MSSV/email (Select showSearch, debounce thủ công)
  const [svKeyword, setSvKeyword] = useState("");
  const svSearchTimeout = useRef<ReturnType<typeof setTimeout> | undefined>(undefined);
  const handleSvSearch = (value: string) => {
    if (svSearchTimeout.current) clearTimeout(svSearchTimeout.current);
    svSearchTimeout.current = setTimeout(() => setSvKeyword(value), 300);
  };
  const { data: svSearchResult, isFetching: svSearching } = useQuery({
    queryKey: ["sinh-vien-search", svKeyword],
    queryFn: () => sinhVienApi.getPaged({ keyword: svKeyword, page: 1, pageSize: 20 }),
    enabled: createOpen && svKeyword.trim().length > 0,
  });
  const svOptions = (svSearchResult?.data ?? []).map((sv) => ({
    value: sv.id,
    label: `${sv.hoTen} — ${sv.mssv}`,
  }));

  // Semester list for dropdowns
  const { data: hocKys = [] } = useQuery({
    queryKey: ["hoc-ky"],
    queryFn: () => hocKyApi.getAll(),
    enabled: isAdmin,
  });

  // Danh sách Năm học suy ra từ chính danh sách Học kỳ đã tải (không cần gọi thêm API)
  const namHocOptions = Array.from(
    new Map(hocKys.map((hk) => [hk.namHocId, hk.tenNamHoc])).entries(),
  ).map(([id, ten]) => ({ value: id, label: ten }));

  const createHocKyOptions = hocKys
    .filter((hk) => !createNamHocId || hk.namHocId === createNamHocId)
    .map((hk) => ({ value: hk.id, label: `${hk.tenHocKy} — ${hk.tenNamHoc}` }));
  const generateHocKyOptions = hocKys
    .filter((hk) => !generateNamHocId || hk.namHocId === generateNamHocId)
    .map((hk) => ({ value: hk.id, label: `${hk.tenHocKy} — ${hk.tenNamHoc}` }));

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

  // Student: hồ sơ cá nhân — dùng cho khối "Thông tin bảo hiểm y tế"
  const { data: hoSo } = useQuery({
    queryKey: ["sinh-vien-me"],
    queryFn: () => sinhVienMeApi.getMe(),
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

      {/* Thông tin bảo hiểm y tế (Sinh viên) */}
      {!isAdmin && hoSo && (
        <Card
          size="small"
          title={
            <Space>
              <MedicineBoxOutlined />
              Thông tin bảo hiểm y tế
            </Space>
          }
          style={{ marginBottom: 16 }}
        >
          <Descriptions column={{ xs: 1, sm: 2 }} size="small">
            <Descriptions.Item label="Mã HSSV">{hoSo.mssv}</Descriptions.Item>
            <Descriptions.Item label="Họ & Tên">{hoSo.hoTen}</Descriptions.Item>
            <Descriptions.Item label="Ngày sinh">Chưa cập nhật</Descriptions.Item>
            <Descriptions.Item label="Lớp">{hoSo.tenLop ?? "Chưa phân lớp"}</Descriptions.Item>
            <Descriptions.Item label="Tình trạng đóng BHYT" span={2}>
              Không có thông tin về việc chưa đóng BHYT.
            </Descriptions.Item>
          </Descriptions>
        </Card>
      )}

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

      {/* Sinh viên: chia theo Năm học, trong mỗi năm chia tiếp theo từng học kỳ (thẻ) */}
      {!isAdmin && !myError && (myLoading || items.length > 0) && (
        <Spin spinning={isLoading}>
          <StudentFeeByYear items={items} />
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
          <Form.Item label="Năm học (lọc danh sách học kỳ bên dưới)">
            <Select
              allowClear
              placeholder="Tất cả năm học"
              options={namHocOptions}
              value={generateNamHocId}
              onChange={(v) => {
                setGenerateNamHocId(v);
                const stillValid = !v || hocKys.some((hk) => hk.id === generateForm.getFieldValue("hocKyId") && hk.namHocId === v);
                if (!stillValid) generateForm.setFieldValue("hocKyId", undefined);
              }}
            />
          </Form.Item>
          <Form.Item name="hocKyId" label="Học kỳ" rules={[{ required: true, message: "Chọn học kỳ" }]}>
            <Select placeholder="Chọn học kỳ" options={generateHocKyOptions} />
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
          <Form.Item name="sinhVienId" label="Sinh viên" rules={[{ required: true, message: "Chọn sinh viên" }]}>
            <Select
              showSearch={{ filterOption: false, onSearch: handleSvSearch }}
              placeholder="Gõ tên, MSSV hoặc email để tìm..."
              notFoundContent={svSearching ? <Spin size="small" /> : "Không tìm thấy sinh viên"}
              options={svOptions}
            />
          </Form.Item>
          <Form.Item label="Năm học (lọc danh sách học kỳ bên dưới)">
            <Select
              allowClear
              placeholder="Tất cả năm học"
              options={namHocOptions}
              value={createNamHocId}
              onChange={(v) => {
                setCreateNamHocId(v);
                const stillValid = !v || hocKys.some((hk) => hk.id === createForm.getFieldValue("hocKyId") && hk.namHocId === v);
                if (!stillValid) createForm.setFieldValue("hocKyId", undefined);
              }}
            />
          </Form.Item>
          <Form.Item name="hocKyId" label="Học kỳ" rules={[{ required: true }]}>
            <Select placeholder="Chọn học kỳ" options={createHocKyOptions} />
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

import { useState } from "react";
import { Table, Tag, Card, Row, Col, Statistic, Select, Space, Button, Modal, Form, Input, InputNumber, message } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { PlusOutlined } from "@ant-design/icons";
import { hocPhiApi } from "../api/modules";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import type { HocPhi, CreateHocPhi } from "../types";

const TRANG_THAI_OPTIONS = ["Chưa đóng", "Đã đóng", "Miễn giảm"];

function TrangThaiTag({ trangThai }: { trangThai: string }) {
  const color = trangThai === "Đã đóng" ? "success" : trangThai === "Miễn giảm" ? "processing" : "warning";
  return <Tag color={color}>{trangThai}</Tag>;
}

export function HocPhiPage() {
  const { user } = useAuthStore();
  const qc = useQueryClient();
  const isAdmin = user?.vaiTro === ROLES.ADMIN || user?.vaiTro === ROLES.GIAO_VU;
  const [hocKyId, setHocKyId] = useState<number | undefined>();
  const [open, setOpen] = useState(false);
  const [form] = Form.useForm<CreateHocPhi>();

  const { data, isLoading } = useQuery({
    queryKey: isAdmin ? ["hoc-phi-admin", hocKyId] : ["hoc-phi-me"],
    queryFn: () =>
      isAdmin
        ? hocKyId ? hocPhiApi.getByHocKy(hocKyId) : Promise.resolve([] as HocPhi[])
        : hocPhiApi.getMe(),
    enabled: !isAdmin || hocKyId !== undefined,
  });

  const createMutation = useMutation({
    mutationFn: (dto: CreateHocPhi) => hocPhiApi.create(dto),
    onSuccess: () => {
      message.success("Tạo học phí thành công.");
      qc.invalidateQueries({ queryKey: ["hoc-phi-admin", hocKyId] });
      setOpen(false);
      form.resetFields();
    },
    onError: () => message.error("Tạo học phí thất bại."),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, trangThai }: { id: number; trangThai: string }) =>
      hocPhiApi.updateTrangThai(id, trangThai),
    onSuccess: () => {
      message.success("Cập nhật trạng thái thành công.");
      qc.invalidateQueries({ queryKey: ["hoc-phi-admin", hocKyId] });
    },
    onError: () => message.error("Cập nhật thất bại."),
  });

  const studentColumns: ColumnsType<HocPhi> = [
    { title: "Học kỳ", dataIndex: "tenHocKy", width: 160 },
    {
      title: "Số tiền", dataIndex: "soTien", width: 150, align: "right",
      render: (v: number) => v.toLocaleString("vi-VN", { style: "currency", currency: "VND" }),
    },
    {
      title: "Trạng thái", dataIndex: "trangThaiDong", width: 130,
      render: (v: string) => <TrangThaiTag trangThai={v} />,
    },
    {
      title: "Ngày tạo", dataIndex: "createdAt", width: 130,
      render: (v: string) => new Date(v).toLocaleDateString("vi-VN"),
    },
  ];

  const adminColumns: ColumnsType<HocPhi> = [
    { title: "Sinh viên", dataIndex: "tenSinhVien", width: 160 },
    { title: "MSSV", dataIndex: "mssv", width: 110 },
    { title: "Học kỳ", dataIndex: "tenHocKy", width: 160 },
    {
      title: "Số tiền", dataIndex: "soTien", width: 150, align: "right",
      render: (v: number) => v.toLocaleString("vi-VN", { style: "currency", currency: "VND" }),
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

  const total = (data ?? []).reduce((s, x) => s + x.soTien, 0);
  const unpaid = (data ?? []).filter((x) => x.trangThaiDong === "Chưa đóng").reduce((s, x) => s + x.soTien, 0);

  return (
    <div>
      <Space style={{ marginBottom: 16 }} align="center">
        <h2 style={{ margin: 0 }}>Học phí</h2>
        {isAdmin && (
          <Button type="primary" icon={<PlusOutlined />} onClick={() => setOpen(true)}>
            Thêm học phí
          </Button>
        )}
      </Space>

      {/* Summary cards */}
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col>
          <Card size="small">
            <Statistic
              title="Tổng học phí"
              value={total}
              suffix="₫"
              formatter={(v) => Number(v).toLocaleString("vi-VN")}
            />
          </Card>
        </Col>
        {!isAdmin && (
          <Col>
            <Card size="small">
              <Statistic
                title="Chưa đóng"
                value={unpaid}
                suffix="₫"
                valueStyle={{ color: unpaid > 0 ? "#cf1322" : "#3f8600" }}
                formatter={(v) => Number(v).toLocaleString("vi-VN")}
              />
            </Card>
          </Col>
        )}
      </Row>

      {isAdmin && (
        <div style={{ marginBottom: 16 }}>
          <Input
            type="number"
            placeholder="Nhập ID học kỳ để lọc"
            style={{ width: 260 }}
            onChange={(e) => setHocKyId(e.target.value ? Number(e.target.value) : undefined)}
          />
        </div>
      )}

      <Table<HocPhi>
        rowKey="id"
        columns={isAdmin ? adminColumns : studentColumns}
        dataSource={data ?? []}
        loading={isLoading}
        pagination={{ pageSize: 20 }}
        scroll={{ x: 700 }}
        size="middle"
      />

      <Modal
        title="Thêm học phí cho sinh viên"
        open={open}
        onCancel={() => { setOpen(false); form.resetFields(); }}
        onOk={() => form.submit()}
        okText="Tạo"
        cancelText="Huỷ"
        confirmLoading={createMutation.isPending}
      >
        <Form form={form} layout="vertical" onFinish={(v) => createMutation.mutate(v)}>
          <Form.Item name="sinhVienId" label="ID Sinh viên" rules={[{ required: true }]}>
            <InputNumber style={{ width: "100%" }} />
          </Form.Item>
          <Form.Item name="hocKyId" label="ID Học kỳ" rules={[{ required: true }]}>
            <InputNumber style={{ width: "100%" }} />
          </Form.Item>
          <Form.Item name="soTien" label="Số tiền (VNĐ)" rules={[{ required: true }]}>
            <InputNumber min={0} style={{ width: "100%" }} formatter={(v) => `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, ",")} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

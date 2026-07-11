import { useState } from "react";
import {
  Table, Button, Modal, Form, Input, Select, Tag, Badge,
  Space, Popconfirm, message, Typography, Card,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { PlusOutlined, DeleteOutlined, CheckOutlined } from "@ant-design/icons";
import { thongBaoApi } from "../api/modules";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import type { ThongBao, CreateThongBao } from "../types";

const { TextArea } = Input;
const { Text } = Typography;

const MUC_DO_OPTIONS = ["Thường", "Quan trọng", "Khẩn cấp"];
const LOAI_OPTIONS = ["Thông báo chung", "Học vụ", "Sự kiện", "Khen thưởng", "Kỷ luật"];

function MucDoTag({ mucDo }: { mucDo: string }) {
  const color = mucDo === "Khẩn cấp" ? "red" : mucDo === "Quan trọng" ? "orange" : "default";
  return <Tag color={color}>{mucDo}</Tag>;
}

export function ThongBaoPage() {
  const { user } = useAuthStore();
  const qc = useQueryClient();
  const isAdmin = user?.vaiTro === ROLES.ADMIN || user?.vaiTro === ROLES.GIAO_VU;
  const [open, setOpen] = useState(false);
  const [form] = Form.useForm<CreateThongBao>();

  const { data, isLoading } = useQuery({
    queryKey: isAdmin ? ["thong-bao-all"] : ["thong-bao-me"],
    queryFn: () => (isAdmin ? thongBaoApi.getAll() : thongBaoApi.getMe()),
  });

  const createMutation = useMutation({
    mutationFn: (dto: CreateThongBao) => thongBaoApi.create(dto),
    onSuccess: () => {
      message.success("Đã tạo thông báo.");
      qc.invalidateQueries({ queryKey: ["thong-bao-all"] });
      setOpen(false);
      form.resetFields();
    },
    onError: () => message.error("Tạo thông báo thất bại."),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => thongBaoApi.remove(id),
    onSuccess: () => {
      message.success("Đã xoá thông báo.");
      qc.invalidateQueries({ queryKey: ["thong-bao-all"] });
    },
    onError: () => message.error("Xoá thông báo thất bại."),
  });

  const readMutation = useMutation({
    mutationFn: (id: number) => thongBaoApi.markAsRead(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["thong-bao-me"] }),
  });

  const adminColumns: ColumnsType<ThongBao> = [
    {
      title: "Tiêu đề", dataIndex: "tieuDe", width: 200,
      render: (v: string, row: ThongBao) => (
        <Space direction="vertical" size={0}>
          <strong>{v}</strong>
          <Text type="secondary" style={{ fontSize: 12 }}>{row.loaiThongBao}</Text>
        </Space>
      ),
    },
    { title: "Nội dung", dataIndex: "noiDung", width: 280, ellipsis: true },
    { title: "Mức độ", dataIndex: "mucDo", width: 110, render: (v) => <MucDoTag mucDo={v} /> },
    {
      title: "Đối tượng", width: 120,
      render: (_: unknown, row: ThongBao) =>
        row.lopNhanId ? <Tag color="blue">{row.tenLopNhan ?? `Lớp #${row.lopNhanId}`}</Tag> : <Tag>Toàn trường</Tag>,
    },
    { title: "Ngày tạo", dataIndex: "ngayTao", width: 130, render: (v: string) => new Date(v).toLocaleDateString("vi-VN") },
    { title: "Người tạo", dataIndex: "tenNguoiTao", width: 130 },
    {
      title: "", width: 80, align: "center",
      render: (_: unknown, row: ThongBao) => (
        <Popconfirm title="Xoá thông báo này?" onConfirm={() => deleteMutation.mutate(row.id)} okText="Xoá" cancelText="Huỷ">
          <Button size="small" danger icon={<DeleteOutlined />} />
        </Popconfirm>
      ),
    },
  ];

  const studentColumns: ColumnsType<ThongBao> = [
    {
      title: "Tiêu đề", dataIndex: "tieuDe", width: 220,
      render: (v: string, row: ThongBao) => (
        <Space>
          {!row.daDoc && <Badge status="processing" />}
          <span style={{ fontWeight: row.daDoc ? "normal" : "bold" }}>{v}</span>
        </Space>
      ),
    },
    { title: "Nội dung", dataIndex: "noiDung", ellipsis: true },
    { title: "Mức độ", dataIndex: "mucDo", width: 110, render: (v) => <MucDoTag mucDo={v} /> },
    {
      title: "Đối tượng", width: 110,
      render: (_: unknown, row: ThongBao) =>
        row.lopNhanId ? <Tag color="blue">Lớp bạn</Tag> : <Tag>Toàn trường</Tag>,
    },
    { title: "Ngày", dataIndex: "ngayTao", width: 120, render: (v: string) => new Date(v).toLocaleDateString("vi-VN") },
    {
      title: "", width: 90, align: "center",
      render: (_: unknown, row: ThongBao) =>
        !row.daDoc ? (
          <Button size="small" icon={<CheckOutlined />} onClick={() => readMutation.mutate(row.id)}>
            Đọc
          </Button>
        ) : <Tag color="success">Đã đọc</Tag>,
    },
  ];

  const unread = (data ?? []).filter((x) => !x.daDoc).length;

  return (
    <div>
      <Space style={{ marginBottom: 16 }} align="center">
        <h2 style={{ margin: 0 }}>Thông báo</h2>
        {!isAdmin && unread > 0 && <Badge count={unread} />}
        {isAdmin && (
          <Button type="primary" icon={<PlusOutlined />} onClick={() => setOpen(true)}>
            Tạo thông báo
          </Button>
        )}
      </Space>

      {!isAdmin && (
        <Card size="small" style={{ marginBottom: 12 }}>
          <Text type="secondary">Hiển thị thông báo toàn trường và thông báo theo lớp của bạn.</Text>
        </Card>
      )}

      <Table<ThongBao>
        rowKey="id"
        columns={isAdmin ? adminColumns : studentColumns}
        dataSource={data ?? []}
        loading={isLoading}
        pagination={{ pageSize: 15 }}
        scroll={{ x: 800 }}
        size="middle"
        rowClassName={(row) => (!row.daDoc && !isAdmin ? "font-bold" : "")}
      />

      <Modal
        title="Tạo thông báo mới"
        open={open}
        onCancel={() => { setOpen(false); form.resetFields(); }}
        onOk={() => form.submit()}
        okText="Tạo"
        cancelText="Huỷ"
        confirmLoading={createMutation.isPending}
      >
        <Form form={form} layout="vertical" onFinish={(v) => createMutation.mutate(v)}>
          <Form.Item name="tieuDe" label="Tiêu đề" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="noiDung" label="Nội dung" rules={[{ required: true }]}>
            <TextArea rows={4} />
          </Form.Item>
          <Form.Item name="loaiThongBao" label="Loại" initialValue="Thông báo chung">
            <Select options={LOAI_OPTIONS.map((v) => ({ value: v, label: v }))} />
          </Form.Item>
          <Form.Item name="mucDo" label="Mức độ" initialValue="Thường">
            <Select options={MUC_DO_OPTIONS.map((v) => ({ value: v, label: v }))} />
          </Form.Item>
          <Form.Item name="lopNhanId" label="Gửi đến lớp (để trống = toàn trường)">
            <Input type="number" placeholder="ID lớp sinh hoạt (tuỳ chọn)" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

import { useState } from "react";
import { Table, Tag, Button, Modal, Form, Input, Select, Space, message, Typography } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { PlusOutlined, CheckOutlined, CloseOutlined } from "@ant-design/icons";
import { yeuCauHanhChinhApi } from "../api/modules";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import type { YeuCauHanhChinh, CreateYeuCauHanhChinh, DuyetYeuCauHanhChinh } from "../types";

const { TextArea } = Input;
const { Text } = Typography;

const LOAI_OPTIONS = [
  "Xác nhận sinh viên",
  "Hoãn học phí",
  "Bảo lưu",
  "Miễn giảm học phí",
  "Khác",
];

function TrangThaiTag({ v }: { v: string }) {
  const color = v === "Đã duyệt" ? "success" : v === "Từ chối" ? "error" : "default";
  return <Tag color={color}>{v}</Tag>;
}

export function YeuCauHanhChinhPage() {
  const { user } = useAuthStore();
  const qc = useQueryClient();
  const isAdmin = user?.vaiTro === ROLES.ADMIN || user?.vaiTro === ROLES.GIAO_VU;

  const [open, setOpen] = useState(false);
  const [duyetOpen, setDuyetOpen] = useState<{ id: number; trangThai: "Đã duyệt" | "Từ chối" } | null>(null);
  const [filterTrangThai, setFilterTrangThai] = useState<string | undefined>();
  const [form] = Form.useForm<CreateYeuCauHanhChinh>();
  const [duyetForm] = Form.useForm<{ ghiChu?: string }>();

  const { data: myData, isLoading: myLoading } = useQuery({
    queryKey: ["yeu-cau-hanh-chinh-me"],
    queryFn: () => yeuCauHanhChinhApi.getMe(),
    enabled: !isAdmin,
  });

  const { data: allData, isLoading: allLoading } = useQuery({
    queryKey: ["yeu-cau-hanh-chinh-all", filterTrangThai],
    queryFn: () => yeuCauHanhChinhApi.getAll(1, 50, filterTrangThai),
    enabled: isAdmin,
  });

  const isLoading = isAdmin ? allLoading : myLoading;
  const items: YeuCauHanhChinh[] = isAdmin
    ? (allData?.data ?? [])
    : (myData ?? []);

  const createMutation = useMutation({
    mutationFn: (dto: CreateYeuCauHanhChinh) => yeuCauHanhChinhApi.create(dto),
    onSuccess: () => {
      message.success("Đã gửi yêu cầu.");
      qc.invalidateQueries({ queryKey: ["yeu-cau-hanh-chinh-me"] });
      setOpen(false);
      form.resetFields();
    },
    onError: () => message.error("Gửi yêu cầu thất bại."),
  });

  const duyetMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: DuyetYeuCauHanhChinh }) =>
      yeuCauHanhChinhApi.duyet(id, dto),
    onSuccess: () => {
      message.success("Đã xử lý yêu cầu.");
      qc.invalidateQueries({ queryKey: ["yeu-cau-hanh-chinh-all", filterTrangThai] });
      setDuyetOpen(null);
      duyetForm.resetFields();
    },
    onError: () => message.error("Xử lý yêu cầu thất bại."),
  });

  const studentColumns: ColumnsType<YeuCauHanhChinh> = [
    { title: "Loại yêu cầu", dataIndex: "loaiYeuCau", width: 170 },
    { title: "Nội dung", dataIndex: "noiDung", ellipsis: true },
    {
      title: "Trạng thái", dataIndex: "trangThai", width: 120,
      render: (v: string) => <TrangThaiTag v={v} />,
    },
    {
      title: "Ngày gửi", dataIndex: "ngayTao", width: 120,
      render: (v: string) => new Date(v).toLocaleDateString("vi-VN"),
    },
    {
      title: "Người duyệt", dataIndex: "tenNguoiDuyet", width: 150,
      render: (v?: string) => v ?? <Text type="secondary">—</Text>,
    },
  ];

  const adminColumns: ColumnsType<YeuCauHanhChinh> = [
    { title: "Sinh viên", dataIndex: "tenSinhVien", width: 150 },
    { title: "MSSV", dataIndex: "mssv", width: 110 },
    { title: "Loại yêu cầu", dataIndex: "loaiYeuCau", width: 170 },
    { title: "Nội dung", dataIndex: "noiDung", ellipsis: true },
    {
      title: "Trạng thái", dataIndex: "trangThai", width: 120,
      render: (v: string) => <TrangThaiTag v={v} />,
    },
    {
      title: "Ngày gửi", dataIndex: "ngayTao", width: 120,
      render: (v: string) => new Date(v).toLocaleDateString("vi-VN"),
    },
    {
      title: "Hành động", width: 160,
      render: (_: unknown, row: YeuCauHanhChinh) =>
        row.trangThai === "Chờ duyệt" ? (
          <Space size={4}>
            <Button
              size="small" type="primary" icon={<CheckOutlined />}
              onClick={() => setDuyetOpen({ id: row.id, trangThai: "Đã duyệt" })}
            >
              Duyệt
            </Button>
            <Button
              size="small" danger icon={<CloseOutlined />}
              onClick={() => setDuyetOpen({ id: row.id, trangThai: "Từ chối" })}
            >
              Từ chối
            </Button>
          </Space>
        ) : <Text type="secondary">—</Text>,
    },
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16 }} align="center">
        <h2 style={{ margin: 0 }}>Yêu cầu hành chính</h2>
        {!isAdmin && (
          <Button type="primary" icon={<PlusOutlined />} onClick={() => setOpen(true)}>
            Tạo yêu cầu
          </Button>
        )}
        {isAdmin && (
          <Select
            allowClear
            placeholder="Lọc trạng thái"
            style={{ width: 160 }}
            options={["Chờ duyệt", "Đã duyệt", "Từ chối"].map((v) => ({ value: v, label: v }))}
            onChange={(v) => setFilterTrangThai(v)}
          />
        )}
      </Space>

      <Table<YeuCauHanhChinh>
        rowKey="id"
        columns={isAdmin ? adminColumns : studentColumns}
        dataSource={items}
        loading={isLoading}
        pagination={{ pageSize: 15 }}
        scroll={{ x: 800 }}
        size="middle"
      />

      {/* Create modal (student) */}
      <Modal
        title="Tạo yêu cầu hành chính"
        open={open}
        onCancel={() => { setOpen(false); form.resetFields(); }}
        onOk={() => form.submit()}
        okText="Gửi"
        cancelText="Huỷ"
        confirmLoading={createMutation.isPending}
      >
        <Form form={form} layout="vertical" onFinish={(v) => createMutation.mutate(v)}>
          <Form.Item name="loaiYeuCau" label="Loại yêu cầu" rules={[{ required: true }]}>
            <Select options={LOAI_OPTIONS.map((v) => ({ value: v, label: v }))} />
          </Form.Item>
          <Form.Item name="noiDung" label="Nội dung chi tiết" rules={[{ required: true }]}>
            <TextArea rows={4} placeholder="Mô tả chi tiết yêu cầu của bạn..." />
          </Form.Item>
        </Form>
      </Modal>

      {/* Approve / reject modal (admin) */}
      <Modal
        title={duyetOpen?.trangThai === "Đã duyệt" ? "Xác nhận duyệt yêu cầu" : "Xác nhận từ chối yêu cầu"}
        open={!!duyetOpen}
        onCancel={() => { setDuyetOpen(null); duyetForm.resetFields(); }}
        onOk={() => duyetForm.submit()}
        okText={duyetOpen?.trangThai === "Đã duyệt" ? "Duyệt" : "Từ chối"}
        okButtonProps={{ danger: duyetOpen?.trangThai === "Từ chối" }}
        cancelText="Huỷ"
        confirmLoading={duyetMutation.isPending}
      >
        <Form
          form={duyetForm}
          layout="vertical"
          onFinish={(v) =>
            duyetMutation.mutate({
              id: duyetOpen!.id,
              dto: { trangThai: duyetOpen!.trangThai, ghiChu: v.ghiChu },
            })
          }
        >
          <Form.Item name="ghiChu" label="Ghi chú (tuỳ chọn)">
            <TextArea rows={3} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

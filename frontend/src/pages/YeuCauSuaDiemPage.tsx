import { useState } from "react";
import {
  Table, Tag, Button, Modal, Form, Select, Input, Space,
  message, Typography, Alert,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { PlusOutlined, CheckOutlined, CloseOutlined } from "@ant-design/icons";
import { yeuCauSuaDiemApi, lopHocPhanApi } from "../api/modules";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import type { YeuCauSuaDiem, CreateYeuCauSuaDiem, LopHocPhan } from "../types";

const { TextArea } = Input;
const { Text } = Typography;

function TrangThaiTag({ v }: { v: string }) {
  const color = v === "Đã duyệt" ? "success" : v === "Từ chối" ? "error" : "processing";
  return <Tag color={color}>{v}</Tag>;
}

export function YeuCauSuaDiemPage() {
  const { user } = useAuthStore();
  const qc = useQueryClient();
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const isGiaoVien = user?.vaiTro === ROLES.GIAO_VIEN;

  const [open, setOpen] = useState(false);
  const [duyetTarget, setDuyetTarget] = useState<{ id: number; trangThai: "Đã duyệt" | "Từ chối" } | null>(null);
  const [filterTrangThai, setFilterTrangThai] = useState<string | undefined>();
  const [form] = Form.useForm<CreateYeuCauSuaDiem>();

  // Teacher: my requests
  const { data: myData, isLoading: myLoading } = useQuery({
    queryKey: ["yeu-cau-sua-diem-me"],
    queryFn: () => yeuCauSuaDiemApi.getMe(),
    enabled: isGiaoVien,
  });

  // Admin: all requests
  const { data: allData, isLoading: allLoading } = useQuery({
    queryKey: ["yeu-cau-sua-diem-all", filterTrangThai],
    queryFn: () => yeuCauSuaDiemApi.getAll(1, 50, filterTrangThai),
    enabled: isAdmin,
  });

  // Teacher: their locked LHPs for the create modal select
  const { data: lopHocPhans } = useQuery({
    queryKey: ["lop-hoc-phan-me"],
    queryFn: () => lopHocPhanApi.getMe(),
    enabled: isGiaoVien,
  });
  const lockedLhps = (lopHocPhans ?? []).filter((x: LopHocPhan) => x.khoaBangDiem);

  const createMutation = useMutation({
    mutationFn: (dto: CreateYeuCauSuaDiem) => yeuCauSuaDiemApi.create(dto),
    onSuccess: () => {
      message.success("Đã gửi yêu cầu mở khoá bảng điểm.");
      qc.invalidateQueries({ queryKey: ["yeu-cau-sua-diem-me"] });
      setOpen(false);
      form.resetFields();
    },
    onError: (e: any) =>
      message.error(e?.response?.data?.message ?? "Gửi yêu cầu thất bại."),
  });

  const duyetMutation = useMutation({
    mutationFn: ({ id, trangThai }: { id: number; trangThai: string }) =>
      yeuCauSuaDiemApi.duyet(id, { trangThai }),
    onSuccess: (_, vars) => {
      const msg = vars.trangThai === "Đã duyệt"
        ? "Đã duyệt — bảng điểm đã được mở khoá."
        : "Đã từ chối yêu cầu.";
      message.success(msg);
      qc.invalidateQueries({ queryKey: ["yeu-cau-sua-diem-all", filterTrangThai] });
      setDuyetTarget(null);
    },
    onError: () => message.error("Xử lý yêu cầu thất bại."),
  });

  const teacherColumns: ColumnsType<YeuCauSuaDiem> = [
    { title: "Lớp HP", dataIndex: "maLopHp", width: 130 },
    { title: "Môn học", dataIndex: "tenMon", width: 200 },
    { title: "Học kỳ", dataIndex: "tenHocKy", width: 130 },
    { title: "Lý do", dataIndex: "lyDo", ellipsis: true },
    {
      title: "Trạng thái", dataIndex: "trangThai", width: 130,
      render: (v: string) => <TrangThaiTag v={v} />,
    },
    {
      title: "Ngày gửi", dataIndex: "createdAt", width: 120,
      render: (v: string) => new Date(v).toLocaleDateString("vi-VN"),
    },
    {
      title: "Người duyệt", dataIndex: "tenNguoiDuyet", width: 140,
      render: (v?: string) => v ?? <Text type="secondary">—</Text>,
    },
  ];

  const adminColumns: ColumnsType<YeuCauSuaDiem> = [
    { title: "Giáo viên", dataIndex: "tenGiaoVien", width: 150 },
    { title: "Lớp HP", dataIndex: "maLopHp", width: 130 },
    { title: "Môn học", dataIndex: "tenMon", width: 200 },
    { title: "Lý do", dataIndex: "lyDo", ellipsis: true },
    {
      title: "Trạng thái", dataIndex: "trangThai", width: 130,
      render: (v: string) => <TrangThaiTag v={v} />,
    },
    {
      title: "Ngày gửi", dataIndex: "createdAt", width: 120,
      render: (v: string) => new Date(v).toLocaleDateString("vi-VN"),
    },
    {
      title: "Hành động", width: 170,
      render: (_: unknown, row: YeuCauSuaDiem) =>
        row.trangThai === "Chờ duyệt" ? (
          <Space size={4}>
            <Button
              size="small" type="primary" icon={<CheckOutlined />}
              onClick={() => setDuyetTarget({ id: row.id, trangThai: "Đã duyệt" })}
            >
              Duyệt
            </Button>
            <Button
              size="small" danger icon={<CloseOutlined />}
              onClick={() => setDuyetTarget({ id: row.id, trangThai: "Từ chối" })}
            >
              Từ chối
            </Button>
          </Space>
        ) : <Text type="secondary">—</Text>,
    },
  ];

  const teacherItems = myData ?? [];
  const adminItems: YeuCauSuaDiem[] = (allData as any)?.data ?? [];

  return (
    <div>
      <Space style={{ marginBottom: 16 }} align="center">
        <h2 style={{ margin: 0 }}>Yêu cầu sửa điểm</h2>
        {isGiaoVien && (
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

      {isAdmin && (
        <Alert
          type="info"
          showIcon
          style={{ marginBottom: 12 }}
          message="Khi duyệt yêu cầu, bảng điểm của lớp học phần đó sẽ được tự động mở khoá."
        />
      )}

      <Table<YeuCauSuaDiem>
        rowKey="id"
        columns={isAdmin ? adminColumns : teacherColumns}
        dataSource={isAdmin ? adminItems : teacherItems}
        loading={isAdmin ? allLoading : myLoading}
        pagination={{ pageSize: 15 }}
        scroll={{ x: 900 }}
        size="middle"
      />

      {/* Create request modal (teacher) */}
      <Modal
        title="Yêu cầu mở khoá bảng điểm"
        open={open}
        onCancel={() => { setOpen(false); form.resetFields(); }}
        onOk={() => form.submit()}
        okText="Gửi yêu cầu"
        cancelText="Huỷ"
        confirmLoading={createMutation.isPending}
      >
        {lockedLhps.length === 0 ? (
          <Alert
            type="warning"
            showIcon
            message="Không có lớp học phần nào đang bị khoá bảng điểm."
          />
        ) : (
          <Form form={form} layout="vertical" onFinish={(v) => createMutation.mutate(v)}>
            <Form.Item name="lopHpId" label="Lớp học phần (đang khoá)" rules={[{ required: true }]}>
              <Select
                options={lockedLhps.map((lhp: LopHocPhan) => ({
                  value: lhp.id,
                  label: `${lhp.maLopHp} — ${lhp.tenMon}`,
                }))}
                placeholder="Chọn lớp học phần"
              />
            </Form.Item>
            <Form.Item name="lyDo" label="Lý do yêu cầu sửa điểm" rules={[{ required: true }]}>
              <TextArea rows={3} placeholder="Mô tả lý do cần mở khoá bảng điểm..." />
            </Form.Item>
          </Form>
        )}
      </Modal>

      {/* Approve / reject confirm */}
      <Modal
        title={
          duyetTarget?.trangThai === "Đã duyệt"
            ? "Duyệt yêu cầu — bảng điểm sẽ được mở khoá"
            : "Từ chối yêu cầu"
        }
        open={!!duyetTarget}
        onCancel={() => setDuyetTarget(null)}
        onOk={() => duyetMutation.mutate({ id: duyetTarget!.id, trangThai: duyetTarget!.trangThai })}
        okText={duyetTarget?.trangThai === "Đã duyệt" ? "Xác nhận duyệt" : "Từ chối"}
        okButtonProps={{ danger: duyetTarget?.trangThai === "Từ chối" }}
        cancelText="Huỷ"
        confirmLoading={duyetMutation.isPending}
      >
        {duyetTarget?.trangThai === "Đã duyệt"
          ? "Bảng điểm của lớp học phần này sẽ được mở khoá để giáo viên chỉnh sửa."
          : "Yêu cầu sẽ bị đánh dấu Từ chối và bảng điểm vẫn bị khoá."}
      </Modal>
    </div>
  );
}

import { useState } from "react";
import { App, Button, Card, Empty, Form, Input, InputNumber, Modal, Space, Spin, Tabs, Typography } from "antd";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { noiDungTinhApi } from "../api/modules";
import type { NoiDungTinh, UpsertNoiDungTinh } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

const { Paragraph } = Typography;

interface Props {
  khuVuc: string;
  tieuDeTrang: string;
}

export function KhuVucNoiDungPage({ khuVuc, tieuDeTrang }: Props) {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const { message } = App.useApp();
  const queryClient = useQueryClient();
  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<NoiDungTinh | null>(null);
  const [form] = Form.useForm();

  const { data, isLoading } = useQuery({
    queryKey: ["noi-dung-tinh", khuVuc],
    queryFn: () => noiDungTinhApi.getByKhuVuc(khuVuc),
  });

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["noi-dung-tinh", khuVuc] });

  const createMutation = useMutation({
    mutationFn: (dto: UpsertNoiDungTinh) => noiDungTinhApi.create(dto),
    onSuccess: () => {
      message.success("Thêm mục thành công");
      setModalOpen(false);
      invalidate();
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Có lỗi xảy ra"),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: UpsertNoiDungTinh }) => noiDungTinhApi.update(id, dto),
    onSuccess: () => {
      message.success("Cập nhật thành công");
      setModalOpen(false);
      invalidate();
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Có lỗi xảy ra"),
  });

  const openCreate = () => {
    setEditingRecord(null);
    form.resetFields();
    setModalOpen(true);
  };

  const openEdit = (record: NoiDungTinh) => {
    setEditingRecord(record);
    form.setFieldsValue({
      maMuc: record.maMuc,
      tieuDe: record.tieuDe,
      thuTu: record.thuTu,
      noiDung: record.noiDung,
    });
    setModalOpen(true);
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    if (editingRecord) {
      updateMutation.mutate({ id: editingRecord.id, dto: { ...values, khuVuc } });
    } else {
      createMutation.mutate({ ...values, khuVuc });
    }
  };

  if (isLoading) return <Spin />;

  return (
    <div>
      <Space style={{ marginBottom: 16, width: "100%", justifyContent: "space-between" }}>
        <h2 style={{ margin: 0 }}>{tieuDeTrang}</h2>
        {isAdmin && (
          <Button type="primary" onClick={openCreate}>
            Thêm mục
          </Button>
        )}
      </Space>

      {!data || data.length === 0 ? (
        <Empty description="Chưa có nội dung." />
      ) : (
        <Tabs
          type="card"
          items={data.map((muc) => ({
            key: muc.maMuc,
            label: muc.tieuDe,
            children: (
              <Card
                size="small"
                extra={
                  isAdmin && (
                    <Button size="small" onClick={() => openEdit(muc)}>
                      Sửa
                    </Button>
                  )
                }
              >
                <Paragraph style={{ whiteSpace: "pre-wrap", marginBottom: 0 }}>
                  {muc.noiDung?.trim() ? muc.noiDung : "Nội dung đang được cập nhật."}
                </Paragraph>
              </Card>
            ),
          }))}
        />
      )}

      {isAdmin && (
        <Modal
          title={editingRecord ? `Sửa mục — ${tieuDeTrang}` : `Thêm mục — ${tieuDeTrang}`}
          open={modalOpen}
          onCancel={() => setModalOpen(false)}
          onOk={handleSubmit}
          confirmLoading={createMutation.isPending || updateMutation.isPending}
          destroyOnHidden
        >
          <Form form={form} layout="vertical">
            <Form.Item
              name="maMuc"
              label="Mã mục (dùng nội bộ, không dấu, không trùng)"
              rules={[{ required: true, message: "Vui lòng nhập mã mục" }]}
            >
              <Input />
            </Form.Item>
            <Form.Item name="tieuDe" label="Tiêu đề hiển thị trên Tab" rules={[{ required: true, message: "Vui lòng nhập tiêu đề" }]}>
              <Input />
            </Form.Item>
            <Form.Item name="thuTu" label="Thứ tự hiển thị" rules={[{ required: true, message: "Vui lòng nhập thứ tự" }]}>
              <InputNumber style={{ width: "100%" }} />
            </Form.Item>
            <Form.Item name="noiDung" label="Nội dung">
              <Input.TextArea rows={6} />
            </Form.Item>
          </Form>
        </Modal>
      )}
    </div>
  );
}

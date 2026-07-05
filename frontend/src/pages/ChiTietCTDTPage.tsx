import { useEffect, useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Button, Tag, Space, Collapse, Table, Modal, Form, Select, InputNumber, Switch, Popconfirm, App } from "antd";
import { ArrowLeftOutlined } from "@ant-design/icons";
import { useNavigate, useParams } from "react-router-dom";
import { chiTietCTDTApi, chuongTrinhDTApi, monHocApi, hocKyApi } from "../api/modules";
import type { ChiTietCTDT, CreateChiTietCTDT } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function ChiTietCTDTPage() {
  const { id } = useParams<{ id: string }>();
  const ctdtId = Number(id);
  const navigate = useNavigate();
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const { message } = App.useApp();
  const queryClient = useQueryClient();

  const { data: ctdt } = useQuery({
    queryKey: ["chuong-trinh-dt", ctdtId],
    queryFn: () => chuongTrinhDTApi.getById(ctdtId),
  });
  const { data: monHocOptions } = useQuery({
    queryKey: ["mon-hoc-all"],
    queryFn: () => monHocApi.getAll(),
  });
  const { data: hocKys = [], isLoading: hocKysLoading } = useQuery({
    queryKey: ["hoc-ky"],
    queryFn: () => hocKyApi.getAll(),
  });
  // Lấy hết (không phân trang) để nhóm theo học kỳ chính xác trên toàn bộ danh sách.
  const { data: items = [], isLoading } = useQuery({
    queryKey: ["chi-tiet-ctdt-all", ctdtId],
    queryFn: () => chiTietCTDTApi.getPaged({ ctdtId, page: 1, pageSize: 500 }),
    select: (res) => res.data,
    enabled: !!ctdtId,
  });

  // Sắp học kỳ tăng dần theo ngày bắt đầu (lộ trình học từ sớm đến muộn), rồi gom môn học của
  // CTĐT này vào từng học kỳ — học kỳ không có môn nào vẫn hiện ra để thấy ngay chỗ còn thiếu.
  const hocKyGroups = useMemo(() => {
    const sorted = [...hocKys].sort((a, b) => a.ngayBatDau.localeCompare(b.ngayBatDau));
    return sorted.map((hk) => ({
      hocKy: hk,
      items: items.filter((it) => it.hocKyId === hk.id),
    }));
  }, [hocKys, items]);

  // Collapse cần activeKey điều khiển (controlled) vì nhóm chỉ có dữ liệu thật sau khi
  // 2 query trên tải xong — defaultActiveKey chỉ đọc 1 lần lúc mount nên sẽ bỏ lỡ mốc đó.
  const [activeKeys, setActiveKeys] = useState<string[]>();
  useEffect(() => {
    if (activeKeys === undefined && !hocKysLoading && !isLoading) {
      setActiveKeys(hocKyGroups.filter((g) => g.items.length > 0).map((g) => String(g.hocKy.id)));
    }
  }, [hocKyGroups, activeKeys, hocKysLoading, isLoading]);

  const [modalOpen, setModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<ChiTietCTDT | null>(null);
  const [form] = Form.useForm();

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ["chi-tiet-ctdt-all", ctdtId] });

  const createMutation = useMutation({
    mutationFn: (dto: Omit<CreateChiTietCTDT, "ctdtId">) => chiTietCTDTApi.create({ ...dto, ctdtId }),
    onSuccess: () => {
      message.success("Thêm môn học thành công");
      setModalOpen(false);
      invalidate();
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Có lỗi xảy ra"),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, soTinChi, tinhDiemTb }: { id: number; soTinChi: number; tinhDiemTb: boolean }) =>
      chiTietCTDTApi.update(id, { soTinChi, tinhDiemTb }),
    onSuccess: () => {
      message.success("Cập nhật thành công");
      setModalOpen(false);
      invalidate();
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Có lỗi xảy ra"),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => chiTietCTDTApi.remove(id),
    onSuccess: () => {
      message.success("Xoá thành công");
      invalidate();
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Có lỗi xảy ra"),
  });

  const openCreate = (hocKyId?: number) => {
    setEditingRecord(null);
    form.resetFields();
    form.setFieldsValue({ tinhDiemTb: true, hocKyId });
    setModalOpen(true);
  };

  const openEdit = (record: ChiTietCTDT) => {
    setEditingRecord(record);
    form.setFieldsValue(record);
    setModalOpen(true);
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    if (editingRecord) {
      updateMutation.mutate({ id: editingRecord.id, soTinChi: values.soTinChi, tinhDiemTb: values.tinhDiemTb });
    } else {
      createMutation.mutate(values);
    }
  };

  const columns: ColumnsType<ChiTietCTDT> = [
    { title: "Mã môn", dataIndex: "maMon" },
    { title: "Tên môn", dataIndex: "tenMon" },
    { title: "Số tín chỉ", dataIndex: "soTinChi" },
    {
      title: "Tính điểm TB",
      dataIndex: "tinhDiemTb",
      render: (v: boolean) => (v ? <Tag color="green">Có</Tag> : <Tag color="default">Không</Tag>),
    },
    ...(isAdmin
      ? [
          {
            title: "Hành động",
            key: "actions",
            render: (_: unknown, record: ChiTietCTDT) => (
              <Space>
                <Button size="small" onClick={() => openEdit(record)}>
                  Sửa
                </Button>
                <Popconfirm title="Xác nhận xoá?" onConfirm={() => deleteMutation.mutate(record.id)}>
                  <Button size="small" danger>
                    Xoá
                  </Button>
                </Popconfirm>
              </Space>
            ),
          },
        ]
      : []),
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16 }}>
        <Button icon={<ArrowLeftOutlined />} onClick={() => navigate("/chuong-trinh-dt")}>
          Quay lại
        </Button>
        <h2 style={{ margin: 0 }}>
          Môn học trong CTĐT: {ctdt?.maCtdt} ({ctdt?.tenNganh} - {ctdt?.khoaHoc})
        </h2>
      </Space>

      {isAdmin && (
        <div style={{ marginBottom: 16 }}>
          <Button type="primary" onClick={() => openCreate()}>
            Thêm môn học
          </Button>
        </div>
      )}

      <Collapse
        activeKey={activeKeys}
        onChange={(keys) => setActiveKeys(Array.isArray(keys) ? keys : [keys])}
        items={hocKyGroups.map((g) => ({
          key: String(g.hocKy.id),
          label: (
            <Space>
              <span>
                {g.hocKy.tenHocKy} ({g.hocKy.tenNamHoc})
              </span>
              {g.items.length > 0 ? (
                <Tag color="blue">{g.items.length} môn</Tag>
              ) : (
                <Tag color="red">Chưa có môn học</Tag>
              )}
            </Space>
          ),
          children: (
            <Table
              rowKey="id"
              size="small"
              loading={isLoading}
              columns={columns}
              dataSource={g.items}
              pagination={false}
              locale={{ emptyText: "Chưa có môn học trong học kỳ này" }}
            />
          ),
        }))}
      />

      <Modal
        title={editingRecord ? "Sửa môn học" : "Thêm môn học vào CTĐT"}
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        onOk={handleSubmit}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
        destroyOnHidden
      >
        <Form form={form} layout="vertical">
          {!editingRecord && (
            <>
              <Form.Item name="monHocId" label="Môn học" rules={[{ required: true, message: "Vui lòng chọn môn học" }]}>
                <Select options={(monHocOptions || []).map((m) => ({ label: `${m.maMon} - ${m.tenMon}`, value: m.id }))} />
              </Form.Item>
              <Form.Item name="hocKyId" label="Học kỳ" rules={[{ required: true, message: "Vui lòng chọn học kỳ" }]}>
                <Select options={hocKys.map((h) => ({ label: `${h.tenHocKy} (${h.tenNamHoc})`, value: h.id }))} />
              </Form.Item>
            </>
          )}
          <Form.Item name="soTinChi" label="Số tín chỉ" rules={[{ required: true, message: "Vui lòng nhập số tín chỉ" }]}>
            <InputNumber min={1} max={10} style={{ width: "100%" }} />
          </Form.Item>
          <Form.Item name="tinhDiemTb" label="Tính điểm trung bình" valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

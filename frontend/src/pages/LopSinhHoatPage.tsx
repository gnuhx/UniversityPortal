import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import {
  Alert, App, Button, Card, DatePicker, Descriptions, Form, Input, List, Modal,
  Select, Space, Spin, Switch, Table, Tag,
} from "antd";
import type { Dayjs } from "dayjs";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { lopSinhHoatApi, giaoVienApi, chuongTrinhDTApi, tuanHocApi, bienBanShcnApi } from "../api/modules";
import type { BienBanSHCN, BienBanSHCNSinhVien, LopSinhHoat, LopSinhHoatThanhVien } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

const TINH_TRANG_LABEL: Record<string, { text: string; color: string }> = {
  CoMat: { text: "Có mặt", color: "green" },
  VangCoPhep: { text: "Vắng có phép", color: "orange" },
  VangKhongPhep: { text: "Vắng không phép", color: "red" },
};

/** Admin/Giáo vụ: CRUD toàn bộ lớp sinh hoạt (hành vi giữ nguyên như trước). */
function QuanLyLopSinhHoat() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const isAdminOrGiaoVu = isAdmin || user?.vaiTro === ROLES.GIAO_VU;

  const { data: giaoVienPage } = useQuery({
    queryKey: ["giao-vien-all"],
    queryFn: () => giaoVienApi.getPaged({ page: 1, pageSize: 100 }),
    enabled: isAdminOrGiaoVu,
  });
  const { data: ctdtOptions } = useQuery({
    queryKey: ["chuong-trinh-dt-all"],
    queryFn: () => chuongTrinhDTApi.getAll(),
  });

  const columns: ColumnsType<LopSinhHoat> = [
    { title: "Mã lớp", dataIndex: "maLop" },
    { title: "GVCN", dataIndex: "tenGvcn" },
    { title: "Thư ký", dataIndex: "tenThuKy" },
    { title: "Chương trình ĐT", dataIndex: "maCtdt" },
    { title: "Số sinh viên", dataIndex: "soSinhVien" },
  ];

  const formFields: CrudFormField[] = [
    { name: "maLop", label: "Mã lớp", required: true },
    {
      name: "gvcnId",
      label: "Giáo viên chủ nhiệm",
      type: "select",
      required: true,
      options: (giaoVienPage?.data || []).map((g) => ({ label: `${g.maGv} - ${g.hoTen}`, value: g.id })),
    },
    {
      name: "chuongTrinhDtId",
      label: "Chương trình đào tạo",
      type: "select",
      required: true,
      options: (ctdtOptions || []).map((c) => ({ label: c.maCtdt, value: c.id })),
    },
    { name: "thuKyId", label: "Mã sinh viên thư ký (Id)", type: "number", hideOnCreate: true },
  ];

  return (
    <CrudTable<LopSinhHoat, any, any>
      title="Lớp sinh hoạt"
      queryKey="lop-sinh-hoat"
      fetchPaged={lopSinhHoatApi.getPaged}
      columns={columns}
      formFields={formFields}
      searchPlaceholder="Tìm theo mã lớp"
      onCreate={isAdminOrGiaoVu ? lopSinhHoatApi.create : undefined}
      onUpdate={isAdminOrGiaoVu ? lopSinhHoatApi.update : undefined}
      onDelete={isAdmin ? lopSinhHoatApi.remove : undefined}
      canCreate={isAdminOrGiaoVu}
      canEdit={isAdminOrGiaoVu}
      canDelete={isAdmin}
    />
  );
}

/** Modal xem chi tiết 1 biên bản — dùng chung cho GVCN (thấy cả danh sách vắng) và Sinh viên (chỉ thấy của mình). */
function ChiTietBienBanModal({
  open, onClose, bienBan, sinhVienView,
}: {
  open: boolean;
  onClose: () => void;
  bienBan: BienBanSHCN | BienBanSHCNSinhVien | null;
  sinhVienView?: BienBanSHCNSinhVien;
}) {
  if (!bienBan) return null;
  const isFull = "danhSachVang" in bienBan;

  return (
    <Modal open={open} onCancel={onClose} onOk={onClose} title="Chi tiết buổi sinh hoạt" footer={null} width={640}>
      <Descriptions column={1} size="small" bordered style={{ marginBottom: 16 }}>
        <Descriptions.Item label="Thời gian">{new Date(bienBan.thoiGian).toLocaleString("vi-VN")}</Descriptions.Item>
        <Descriptions.Item label="Địa điểm">{bienBan.diaDiem}</Descriptions.Item>
        <Descriptions.Item label="Tuần học">{bienBan.maTuan}</Descriptions.Item>
        <Descriptions.Item label="Thư ký">{bienBan.tenThuKy}</Descriptions.Item>
        {!isFull && sinhVienView && (
          <Descriptions.Item label="Tình trạng của bạn">
            <Tag color={TINH_TRANG_LABEL[sinhVienView.tinhTrangCuaToi]?.color}>
              {TINH_TRANG_LABEL[sinhVienView.tinhTrangCuaToi]?.text}
            </Tag>
            {sinhVienView.lyDoVangCuaToi && ` — Lý do: ${sinhVienView.lyDoVangCuaToi}`}
          </Descriptions.Item>
        )}
      </Descriptions>

      <div style={{ whiteSpace: "pre-wrap", marginBottom: 16 }}>{bienBan.noiDung}</div>

      {bienBan.congViecs.length > 0 && (
        <>
          <h4>Công việc</h4>
          <List
            size="small"
            dataSource={bienBan.congViecs}
            renderItem={(c) => (
              <List.Item>
                {c.tenCongViec} <Tag style={{ marginLeft: 8 }}>{c.trangThaiViec}</Tag>
              </List.Item>
            )}
            style={{ marginBottom: 16 }}
          />
        </>
      )}

      {isFull && (bienBan as BienBanSHCN).danhSachVang.length > 0 && (
        <>
          <h4>Danh sách vắng</h4>
          <Table
            size="small"
            rowKey="sinhVienId"
            pagination={false}
            dataSource={(bienBan as BienBanSHCN).danhSachVang}
            columns={[
              { title: "MSSV", dataIndex: "mssv" },
              { title: "Họ tên", dataIndex: "hoTen" },
              { title: "Có phép", dataIndex: "coPhep", render: (v: boolean) => (v ? <Tag color="green">Có phép</Tag> : <Tag color="red">Không phép</Tag>) },
              { title: "Lý do", dataIndex: "lyDo" },
            ]}
            style={{ marginBottom: 16 }}
          />
        </>
      )}

      {bienBan.phanHoiGvcn && (
        <>
          <h4>Phản hồi của GVCN</h4>
          <div style={{ whiteSpace: "pre-wrap" }}>{bienBan.phanHoiGvcn}</div>
        </>
      )}
    </Modal>
  );
}

/** Modal GVCN tạo biên bản sinh hoạt mới cho 1 lớp: nội dung + công việc + điểm danh vắng. */
function TaoBienBanModal({ lop, open, onClose }: { lop: LopSinhHoat; open: boolean; onClose: () => void }) {
  const { message } = App.useApp();
  const queryClient = useQueryClient();
  const [form] = Form.useForm();
  const [congViecs, setCongViecs] = useState<{ tenCongViec: string; trangThaiViec: string }[]>([]);
  const [vangMap, setVangMap] = useState<Record<number, { vang: boolean; coPhep: boolean; lyDo: string }>>({});

  const { data: tuanHocOptions } = useQuery({
    queryKey: ["tuan-hoc-all"],
    queryFn: () => tuanHocApi.getAll(),
    enabled: open,
  });

  useEffect(() => {
    if (open) {
      form.resetFields();
      setCongViecs([]);
      setVangMap({});
    }
  }, [open, form]);

  const createMutation = useMutation({
    mutationFn: (dto: Parameters<typeof bienBanShcnApi.create>[0]) => bienBanShcnApi.create(dto),
    onSuccess: () => {
      message.success("Tạo biên bản sinh hoạt thành công");
      queryClient.invalidateQueries({ queryKey: ["bien-ban-shcn-me-gvcn"] });
      onClose();
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Có lỗi xảy ra"),
  });

  const toggleVang = (svId: number, vang: boolean) => {
    setVangMap((prev) => ({ ...prev, [svId]: { vang, coPhep: prev[svId]?.coPhep ?? false, lyDo: prev[svId]?.lyDo ?? "" } }));
  };
  const updateVang = (svId: number, patch: Partial<{ coPhep: boolean; lyDo: string }>) => {
    setVangMap((prev) => {
      const existing = prev[svId] ?? { vang: true, coPhep: false, lyDo: "" };
      return { ...prev, [svId]: { ...existing, vang: true, ...patch } };
    });
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    const danhSachVang = Object.entries(vangMap)
      .filter(([, v]) => v.vang)
      .map(([sinhVienId, v]) => ({ sinhVienId: Number(sinhVienId), coPhep: v.coPhep, lyDo: v.lyDo || undefined }));

    createMutation.mutate({
      lopId: lop.id,
      tuanHocId: values.tuanHocId,
      thoiGian: (values.thoiGian as Dayjs).toISOString(),
      diaDiem: values.diaDiem,
      thuKyId: values.thuKyId,
      noiDung: values.noiDung,
      congViecs: congViecs.filter((c) => c.tenCongViec.trim() !== ""),
      danhSachVang,
    });
  };

  return (
    <Modal
      title={`Tạo biên bản sinh hoạt — ${lop.maLop}`}
      open={open}
      onCancel={onClose}
      onOk={handleSubmit}
      confirmLoading={createMutation.isPending}
      width={720}
      destroyOnHidden
    >
      <Form form={form} layout="vertical">
        <Form.Item name="tuanHocId" label="Tuần học" rules={[{ required: true, message: "Vui lòng chọn tuần học" }]}>
          <Select options={(tuanHocOptions || []).map((t) => ({ label: `${t.maTuan} (${t.tenNamHoc})`, value: t.id }))} />
        </Form.Item>
        <Form.Item name="thoiGian" label="Thời gian" rules={[{ required: true, message: "Vui lòng chọn thời gian" }]}>
          <DatePicker showTime format="DD/MM/YYYY HH:mm" style={{ width: "100%" }} />
        </Form.Item>
        <Form.Item name="diaDiem" label="Địa điểm" rules={[{ required: true, message: "Vui lòng nhập địa điểm" }]}>
          <Input />
        </Form.Item>
        <Form.Item name="thuKyId" label="Thư ký buổi họp" rules={[{ required: true, message: "Vui lòng chọn thư ký" }]}>
          <Select options={lop.danhSachSinhVien.map((sv) => ({ label: `${sv.mssv} - ${sv.hoTen}`, value: sv.id }))} />
        </Form.Item>
        <Form.Item name="noiDung" label="Nội dung buổi sinh hoạt" rules={[{ required: true, message: "Vui lòng nhập nội dung" }]}>
          <Input.TextArea rows={4} />
        </Form.Item>
      </Form>

      <div style={{ marginBottom: 16 }}>
        <Space style={{ marginBottom: 8, width: "100%", justifyContent: "space-between" }}>
          <strong>Công việc</strong>
          <Button size="small" onClick={() => setCongViecs((prev) => [...prev, { tenCongViec: "", trangThaiViec: "Chưa thực hiện" }])}>
            Thêm công việc
          </Button>
        </Space>
        {congViecs.map((cv, idx) => (
          <Space key={idx} style={{ display: "flex", marginBottom: 8 }}>
            <Input
              placeholder="Tên công việc"
              style={{ width: 300 }}
              value={cv.tenCongViec}
              onChange={(e) => setCongViecs((prev) => prev.map((c, i) => (i === idx ? { ...c, tenCongViec: e.target.value } : c)))}
            />
            <Select
              style={{ width: 160 }}
              value={cv.trangThaiViec}
              onChange={(v) => setCongViecs((prev) => prev.map((c, i) => (i === idx ? { ...c, trangThaiViec: v } : c)))}
              options={[
                { label: "Chưa thực hiện", value: "Chưa thực hiện" },
                { label: "Đang thực hiện", value: "Đang thực hiện" },
                { label: "Đã hoàn thành", value: "Đã hoàn thành" },
              ]}
            />
            <Button danger size="small" onClick={() => setCongViecs((prev) => prev.filter((_, i) => i !== idx))}>
              Xoá
            </Button>
          </Space>
        ))}
      </div>

      <div>
        <strong>Điểm danh</strong>
        <Table
          size="small"
          rowKey="id"
          pagination={false}
          dataSource={lop.danhSachSinhVien}
          style={{ marginTop: 8 }}
          columns={[
            { title: "MSSV", dataIndex: "mssv" },
            { title: "Họ tên", dataIndex: "hoTen" },
            {
              title: "Vắng",
              key: "vang",
              render: (_, sv: LopSinhHoatThanhVien) => (
                <Switch checked={!!vangMap[sv.id]?.vang} onChange={(v) => toggleVang(sv.id, v)} />
              ),
            },
            {
              title: "Có phép",
              key: "coPhep",
              render: (_, sv: LopSinhHoatThanhVien) =>
                vangMap[sv.id]?.vang ? (
                  <Switch checked={!!vangMap[sv.id]?.coPhep} onChange={(v) => updateVang(sv.id, { coPhep: v })} />
                ) : null,
            },
            {
              title: "Lý do",
              key: "lyDo",
              render: (_, sv: LopSinhHoatThanhVien) =>
                vangMap[sv.id]?.vang ? (
                  <Input
                    size="small"
                    value={vangMap[sv.id]?.lyDo}
                    onChange={(e) => updateVang(sv.id, { lyDo: e.target.value })}
                  />
                ) : null,
            },
          ]}
        />
      </div>
    </Modal>
  );
}

/** Bảng danh sách biên bản đã tạo cho 1 lớp — dùng bởi Giáo viên (GVCN). */
function BienBanGvcnTable({ lopId }: { lopId: number }) {
  const [page, setPage] = useState(1);
  const [detail, setDetail] = useState<BienBanSHCN | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ["bien-ban-shcn-me-gvcn", lopId, page],
    queryFn: () => bienBanShcnApi.getPagedForGvcn(lopId, page, 10),
  });

  const columns: ColumnsType<BienBanSHCN> = [
    { title: "Thời gian", dataIndex: "thoiGian", render: (v: string) => new Date(v).toLocaleString("vi-VN") },
    { title: "Địa điểm", dataIndex: "diaDiem" },
    { title: "Tuần học", dataIndex: "maTuan" },
    { title: "Số vắng", key: "soVang", render: (_, r) => r.danhSachVang.length },
    {
      title: "Hành động",
      key: "actions",
      render: (_, r) => (
        <Button size="small" onClick={() => setDetail(r)}>
          Xem chi tiết
        </Button>
      ),
    },
  ];

  return (
    <>
      <Table
        rowKey="id"
        loading={isLoading}
        columns={columns}
        dataSource={data?.data || []}
        pagination={{ current: page, pageSize: 10, total: data?.total || 0, onChange: setPage }}
      />
      <ChiTietBienBanModal open={!!detail} onClose={() => setDetail(null)} bienBan={detail} />
    </>
  );
}

/** Giáo viên (GVCN): xem (các) lớp mình chủ nhiệm và tạo biên bản sinh hoạt. */
function LopSinhHoatGvcn() {
  const { data: lops, isLoading } = useQuery({
    queryKey: ["lop-sinh-hoat-me-gvcn"],
    queryFn: () => lopSinhHoatApi.getMeGvcn(),
  });
  const [selectedLopId, setSelectedLopId] = useState<number | undefined>();
  const [modalOpen, setModalOpen] = useState(false);

  useEffect(() => {
    if (!selectedLopId && lops && lops.length > 0) setSelectedLopId(lops[0].id);
  }, [lops, selectedLopId]);

  if (isLoading) return <Spin />;
  if (!lops || lops.length === 0) {
    return <Alert type="info" showIcon message="Bạn hiện không phải là GVCN của lớp sinh hoạt nào." />;
  }

  const selectedLop = lops.find((l) => l.id === selectedLopId);

  return (
    <div>
      <h2>Lớp sinh hoạt tôi chủ nhiệm</h2>
      <Space style={{ marginBottom: 16 }} wrap>
        <span>Lớp:</span>
        <Select
          style={{ width: 220 }}
          value={selectedLopId}
          onChange={setSelectedLopId}
          options={lops.map((l) => ({ label: l.maLop, value: l.id }))}
        />
        <Button type="primary" disabled={!selectedLop} onClick={() => setModalOpen(true)}>
          Tạo biên bản sinh hoạt
        </Button>
      </Space>

      {selectedLop && (
        <>
          <Card title="Thông tin lớp" style={{ marginBottom: 24 }}>
            <Descriptions column={{ xs: 1, sm: 2 }} bordered size="middle">
              <Descriptions.Item label="Mã lớp">{selectedLop.maLop}</Descriptions.Item>
              <Descriptions.Item label="Thư ký">{selectedLop.tenThuKy ?? "Chưa có"}</Descriptions.Item>
              <Descriptions.Item label="Ngành">{selectedLop.tenNganh}</Descriptions.Item>
              <Descriptions.Item label="Khoá học">{selectedLop.khoaHoc}</Descriptions.Item>
              <Descriptions.Item label="Sĩ số">{selectedLop.soSinhVien}</Descriptions.Item>
            </Descriptions>
          </Card>

          <Card title="Biên bản đã tạo">
            <BienBanGvcnTable lopId={selectedLop.id} />
          </Card>

          <TaoBienBanModal lop={selectedLop} open={modalOpen} onClose={() => setModalOpen(false)} />
        </>
      )}
    </div>
  );
}

/** Sinh viên: xem chi tiết lớp sinh hoạt của mình + lịch sử biên bản (ẩn lý do vắng của bạn khác). */
function LopSinhHoatCuaToi() {
  const { data: lop, isLoading, isError } = useQuery({
    queryKey: ["lop-sinh-hoat-me"],
    queryFn: () => lopSinhHoatApi.getMe(),
    retry: false,
  });
  const { data: bienBans, isLoading: loadingBienBan } = useQuery({
    queryKey: ["bien-ban-shcn-me"],
    queryFn: () => bienBanShcnApi.getMe(),
    enabled: !!lop,
  });
  const [detail, setDetail] = useState<BienBanSHCNSinhVien | null>(null);

  if (isLoading) return <Spin />;
  if (isError || !lop) {
    return (
      <Alert
        type="info"
        showIcon
        message="Chưa xác định được lớp sinh hoạt"
        description="Bạn chưa được phân vào lớp sinh hoạt nào. Vui lòng liên hệ phòng đào tạo để được hỗ trợ."
      />
    );
  }

  return (
    <div>
      <h2>Lớp sinh hoạt của tôi</h2>
      <Card title="Thông tin lớp" style={{ marginBottom: 24 }}>
        <Descriptions column={{ xs: 1, sm: 2 }} bordered size="middle">
          <Descriptions.Item label="Mã lớp">{lop.maLop}</Descriptions.Item>
          <Descriptions.Item label="GVCN">{lop.tenGvcn}</Descriptions.Item>
          <Descriptions.Item label="Thư ký">{lop.tenThuKy ?? "Chưa có"}</Descriptions.Item>
          <Descriptions.Item label="Ngành">{lop.tenNganh}</Descriptions.Item>
          <Descriptions.Item label="Chương trình ĐT">{lop.maCtdt}</Descriptions.Item>
          <Descriptions.Item label="Khoá học">{lop.khoaHoc}</Descriptions.Item>
        </Descriptions>
      </Card>

      <Card title={`Bạn cùng lớp (${lop.danhSachSinhVien.length})`} style={{ marginBottom: 24 }}>
        <Table
          rowKey="id"
          size="small"
          pagination={false}
          dataSource={lop.danhSachSinhVien}
          columns={[
            { title: "MSSV", dataIndex: "mssv" },
            { title: "Họ tên", dataIndex: "hoTen" },
          ]}
        />
      </Card>

      <Card title="Lịch sử sinh hoạt lớp">
        <List
          loading={loadingBienBan}
          dataSource={bienBans || []}
          locale={{ emptyText: "Chưa có buổi sinh hoạt nào được ghi nhận" }}
          renderItem={(bb) => (
            <List.Item
              actions={[
                <Button size="small" key="detail" onClick={() => setDetail(bb)}>
                  Xem chi tiết
                </Button>,
              ]}
            >
              <List.Item.Meta
                title={`${new Date(bb.thoiGian).toLocaleString("vi-VN")} — ${bb.diaDiem}`}
                description={bb.noiDung.length > 120 ? `${bb.noiDung.slice(0, 120)}...` : bb.noiDung}
              />
              <Tag color={TINH_TRANG_LABEL[bb.tinhTrangCuaToi]?.color}>{TINH_TRANG_LABEL[bb.tinhTrangCuaToi]?.text}</Tag>
            </List.Item>
          )}
        />
      </Card>

      <ChiTietBienBanModal open={!!detail} onClose={() => setDetail(null)} bienBan={detail} sinhVienView={detail ?? undefined} />
    </div>
  );
}

export function LopSinhHoatPage() {
  const user = useAuthStore((s) => s.user);

  if (user?.vaiTro === ROLES.SINH_VIEN) return <LopSinhHoatCuaToi />;
  if (user?.vaiTro === ROLES.GIAO_VIEN) return <LopSinhHoatGvcn />;
  return <QuanLyLopSinhHoat />;
}

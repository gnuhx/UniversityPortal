import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Button, Card, Descriptions, Empty, Select, Space, Spin } from "antd";
import { useNavigate } from "react-router-dom";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { NganhMonHocModal } from "../components/NganhMonHocModal";
import { NhanBanCtdtModal } from "../components/NhanBanCtdtModal";
import { nganhHocApi, phongBanApi, sinhVienMeApi } from "../api/modules";
import type { NganhHoc } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import { useKhoaHocOptions } from "../hooks/useKhoaHocOptions";

/** Khu vực A — nổi bật ngành + CTĐT hiện tại của sinh viên đang đăng nhập. */
function NganhCuaToi() {
  const navigate = useNavigate();
  const { data: sv, isLoading } = useQuery({
    queryKey: ["sinh-vien-me"],
    queryFn: () => sinhVienMeApi.getMe(),
  });

  return (
    <Card title="Ngành học của tôi" style={{ marginBottom: 24 }}>
      {isLoading ? (
        <Spin />
      ) : !sv?.nganhId ? (
        <Empty description="Chưa xác định được ngành học. Vui lòng liên hệ phòng đào tạo để được phân lớp." />
      ) : (
        <>
          <Descriptions column={{ xs: 1, sm: 2 }} bordered size="middle">
            <Descriptions.Item label="Ngành">{sv.tenNganh}</Descriptions.Item>
            <Descriptions.Item label="Mã ngành">{sv.maNganh}</Descriptions.Item>
            <Descriptions.Item label="Mã CTĐT">{sv.maCtdt}</Descriptions.Item>
            <Descriptions.Item label="Khoá học">{sv.khoaHoc}</Descriptions.Item>
          </Descriptions>
          <Button type="primary" style={{ marginTop: 16 }} onClick={() => navigate(`/chuong-trinh-dt/${sv.ctdtId}`)}>
            Xem chương trình đào tạo
          </Button>
        </>
      )}
    </Card>
  );
}

interface NganhHocTableProps {
  /** Lọc theo ngành (id) — bỏ trống = mọi ngành. */
  nganhId?: number;
  /** Lọc theo khoá học (chỉ hiện ngành có ít nhất 1 CTĐT thuộc khoá này) — bỏ trống = mọi khoá học. */
  khoaHoc?: string;
}

/** Bảng "Ngành học" thuần — không tự vẽ filter, nhận nganhId/khoaHoc từ nơi gọi. */
function NganhHocTable({ nganhId, khoaHoc }: NganhHocTableProps) {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const [monHocModal, setMonHocModal] = useState<NganhHoc | null>(null);
  const [nhanBanModal, setNhanBanModal] = useState<NganhHoc | null>(null);

  const { data: nganhOptions } = useQuery({
    queryKey: ["nganh-hoc-all"],
    queryFn: () => nganhHocApi.getAll(),
  });
  const { data: phongBanOptions } = useQuery({
    queryKey: ["phong-ban-all"],
    queryFn: () => phongBanApi.getAll(),
  });

  const columns: ColumnsType<NganhHoc> = [
    { title: "Mã ngành", dataIndex: "maNganh" },
    { title: "Tên ngành", dataIndex: "tenNganh" },
    { title: "Ngành cha", dataIndex: "tenNganhCha" },
    { title: "Khoa / Phòng ban", dataIndex: "tenPhongBan" },
    {
      title: "Môn học",
      key: "monHoc",
      render: (_, record) => (
        <Button size="small" onClick={() => setMonHocModal(record)}>
          Xem môn học
        </Button>
      ),
    },
    ...(isAdmin
      ? [
          {
            title: "Nhân bản",
            key: "nhanBan",
            render: (_: unknown, record: NganhHoc) => (
              <Button size="small" onClick={() => setNhanBanModal(record)}>
                Nhân bản
              </Button>
            ),
          },
        ]
      : []),
  ];

  const formFields: CrudFormField[] = [
    { name: "maNganh", label: "Mã ngành", required: true },
    { name: "tenNganh", label: "Tên ngành", required: true },
    {
      name: "nganhChaId",
      label: "Ngành cha",
      type: "select",
      options: (nganhOptions || []).map((n) => ({ label: n.tenNganh, value: n.id })),
    },
    {
      name: "phongBanId",
      label: "Khoa / Phòng ban",
      type: "select",
      options: (phongBanOptions || []).map((p) => ({ label: p.tenPhongBan, value: p.id })),
    },
  ];

  return (
    <>
      <CrudTable<NganhHoc, any, any>
        title="Ngành học"
        queryKey="nganh-hoc"
        fetchPaged={nganhHocApi.getPaged}
        extraParams={{ nganhId, khoaHoc }}
        columns={columns}
        formFields={formFields}
        searchPlaceholder="Tìm theo mã hoặc tên ngành"
        onCreate={isAdmin ? nganhHocApi.create : undefined}
        onUpdate={isAdmin ? nganhHocApi.update : undefined}
        onDelete={isAdmin ? nganhHocApi.remove : undefined}
        canCreate={isAdmin}
        canEdit={isAdmin}
        canDelete={isAdmin}
      />
      {monHocModal && (
        <NganhMonHocModal
          nganhId={monHocModal.id}
          tenNganh={monHocModal.tenNganh}
          khoaHoc={khoaHoc}
          open={!!monHocModal}
          onClose={() => setMonHocModal(null)}
        />
      )}
      {nhanBanModal && (
        <NhanBanCtdtModal
          nganhId={nhanBanModal.id}
          tenNganh={nhanBanModal.tenNganh}
          open={!!nhanBanModal}
          onClose={() => setNhanBanModal(null)}
        />
      )}
    </>
  );
}

/** Khu vực B — dùng cho vai trò không phải Admin: tự quản lý filter Khoá học riêng, lọc thật bảng Ngành học. */
function DanhSachNganhHoc() {
  const { khoaHoc, setKhoaHoc, khoaHocOptions, khoaHocFilter } = useKhoaHocOptions();

  return (
    <>
      <Space style={{ marginBottom: 16 }}>
        <span>Khoá học:</span>
        <Select style={{ width: 160 }} value={khoaHoc} onChange={setKhoaHoc} options={khoaHocOptions} />
      </Space>

      <NganhHocTable khoaHoc={khoaHocFilter} />
    </>
  );
}

/**
 * Admin: bảng "Ngành học" với filter Ngành + Khoá học (đều có "Tất cả").
 * "Chương trình đào tạo" đang ẩn theo yêu cầu — component `ChuongTrinhDTTable`
 * (`./ChuongTrinhDTPage.tsx`) vẫn còn, chỉ không render ở đây nữa.
 */
function QuanLyNganhHoc() {
  const { data: nganhOptions } = useQuery({
    queryKey: ["nganh-hoc-all"],
    queryFn: () => nganhHocApi.getAll(),
  });
  const { khoaHoc, setKhoaHoc, khoaHocOptions, khoaHocFilter } = useKhoaHocOptions();
  const [nganhIdStr, setNganhIdStr] = useState<string>("");
  const nganhIdFilter = nganhIdStr ? Number(nganhIdStr) : undefined;

  const nganhFilterOptions = [
    { label: "Tất cả", value: "" },
    ...(nganhOptions || []).map((n) => ({ label: n.tenNganh, value: String(n.id) })),
  ];

  return (
    <div>
      <Space style={{ marginBottom: 24 }} wrap>
        <span>Ngành:</span>
        <Select style={{ width: 220 }} value={nganhIdStr} onChange={setNganhIdStr} options={nganhFilterOptions} />
        <span>Khoá học:</span>
        <Select style={{ width: 160 }} value={khoaHoc} onChange={setKhoaHoc} options={khoaHocOptions} />
      </Space>

      <Card title="Ngành học">
        <NganhHocTable nganhId={nganhIdFilter} khoaHoc={khoaHocFilter} />
      </Card>
    </div>
  );
}

export function NganhHocPage() {
  const user = useAuthStore((s) => s.user);
  const isSinhVien = user?.vaiTro === ROLES.SINH_VIEN;
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  if (isAdmin) return <QuanLyNganhHoc />;

  return (
    <>
      {isSinhVien && <NganhCuaToi />}
      <DanhSachNganhHoc />
    </>
  );
}

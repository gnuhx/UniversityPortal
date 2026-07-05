import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Button, Card, Descriptions, Empty, Select, Space, Spin, Tabs, Tag } from "antd";
import { useNavigate } from "react-router-dom";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { NganhMonHocModal } from "../components/NganhMonHocModal";
import { nganhHocApi, phongBanApi, sinhVienMeApi } from "../api/modules";
import type { NganhHoc } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import { useKhoaHocOptions } from "../hooks/useKhoaHocOptions";
import { ChuongTrinhDTPage } from "./ChuongTrinhDTPage";

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

/** Khu vực B — danh sách toàn bộ Ngành học, lọc theo Khoá học khi xem môn học của 1 ngành. */
function DanhSachNganhHoc() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const [monHocModal, setMonHocModal] = useState<NganhHoc | null>(null);
  const { khoaHoc, setKhoaHoc, khoaHocOptions } = useKhoaHocOptions();

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
      <Space style={{ marginBottom: 16 }}>
        <span>Khoá học:</span>
        <Select
          style={{ width: 160 }}
          value={khoaHoc}
          onChange={setKhoaHoc}
          options={khoaHocOptions.map((k) => ({ label: k, value: k }))}
        />
        <Tag color="blue">Áp dụng khi xem môn học của 1 ngành</Tag>
      </Space>

      <CrudTable<NganhHoc, any, any>
        title="Ngành học"
        queryKey="nganh-hoc"
        fetchPaged={nganhHocApi.getPaged}
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
    </>
  );
}

export function NganhHocPage() {
  const user = useAuthStore((s) => s.user);
  const isSinhVien = user?.vaiTro === ROLES.SINH_VIEN;
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const tongQuan = (
    <>
      {isSinhVien && <NganhCuaToi />}
      <DanhSachNganhHoc />
    </>
  );

  if (!isAdmin) return tongQuan;

  return (
    <Tabs
      items={[
        { key: "tong-quan", label: "Tổng quan", children: tongQuan },
        { key: "quan-ly-ctdt", label: "Quản lý CTĐT", children: <ChuongTrinhDTPage /> },
      ]}
    />
  );
}

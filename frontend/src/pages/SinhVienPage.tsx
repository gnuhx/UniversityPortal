import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Select, Space, Tag } from "antd";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { sinhVienApi, lopSinhHoatApi } from "../api/modules";
import type { SinhVien } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

const TAT_CA = "";

export function SinhVienPage() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const isAdminOrGiaoVu = isAdmin || user?.vaiTro === ROLES.GIAO_VU;

  const { data: lopOptions } = useQuery({
    queryKey: ["lop-sinh-hoat-all"],
    queryFn: () => lopSinhHoatApi.getAll(),
  });

  // Bộ lọc Khoa → Ngành → Khoá học để thu hẹp dropdown "Lớp sinh hoạt" khi thêm/sửa sinh viên
  // (lọc hoàn toàn ở client dựa trên phongBanId/nganhId/khoaHoc đã có sẵn trong lopOptions).
  const [phongBanIdStr, setPhongBanIdStr] = useState<string>(TAT_CA);
  const [nganhIdStr, setNganhIdStr] = useState<string>(TAT_CA);
  const [khoaHoc, setKhoaHoc] = useState<string>(TAT_CA);

  const phongBanOptions = useMemo(() => {
    const uniq = new Map<number, string>();
    for (const l of lopOptions || []) {
      if (l.phongBanId != null) uniq.set(l.phongBanId, l.tenPhongBan || "");
    }
    return [
      { label: "Tất cả", value: TAT_CA },
      ...Array.from(uniq, ([id, ten]) => ({ label: ten, value: String(id) })),
    ];
  }, [lopOptions]);

  const nganhOptions = useMemo(() => {
    const uniq = new Map<number, string>();
    for (const l of lopOptions || []) {
      if (l.nganhId == null) continue;
      if (phongBanIdStr && String(l.phongBanId) !== phongBanIdStr) continue;
      uniq.set(l.nganhId, l.tenNganh);
    }
    return [
      { label: "Tất cả", value: TAT_CA },
      ...Array.from(uniq, ([id, ten]) => ({ label: ten, value: String(id) })),
    ];
  }, [lopOptions, phongBanIdStr]);

  const khoaHocOptions = useMemo(() => {
    const uniq = new Set<string>();
    for (const l of lopOptions || []) {
      if (!l.khoaHoc) continue;
      if (nganhIdStr && String(l.nganhId) !== nganhIdStr) continue;
      uniq.add(l.khoaHoc);
    }
    return [
      { label: "Tất cả", value: TAT_CA },
      ...Array.from(uniq).sort((a, b) => b.localeCompare(a)).map((k) => ({ label: k, value: k })),
    ];
  }, [lopOptions, nganhIdStr]);

  const filteredLopOptions = useMemo(
    () =>
      (lopOptions || []).filter(
        (l) =>
          (!phongBanIdStr || String(l.phongBanId) === phongBanIdStr) &&
          (!nganhIdStr || String(l.nganhId) === nganhIdStr) &&
          (!khoaHoc || l.khoaHoc === khoaHoc),
      ),
    [lopOptions, phongBanIdStr, nganhIdStr, khoaHoc],
  );

  const columns: ColumnsType<SinhVien> = [
    { title: "MSSV", dataIndex: "mssv" },
    { title: "Họ tên", dataIndex: "hoTen" },
    { title: "Email", dataIndex: "email" },
    { title: "Lớp sinh hoạt", dataIndex: "tenLop" },
    {
      title: "Trạng thái",
      dataIndex: "trangThai",
      render: (v: boolean) => (v ? <Tag color="green">Hoạt động</Tag> : <Tag color="red">Đã khoá</Tag>),
    },
  ];

  const formFields: CrudFormField[] = [
    { name: "tenDangNhap", label: "Tên đăng nhập", required: true, hideOnEdit: true },
    { name: "matKhau", label: "Mật khẩu", type: "password", required: true, hideOnEdit: true },
    { name: "mssv", label: "MSSV", required: true, hideOnEdit: true },
    { name: "hoTen", label: "Họ tên", required: true },
    { name: "email", label: "Email", type: "email", required: true },
    {
      name: "lopId",
      label: "Lớp sinh hoạt",
      type: "select",
      options: filteredLopOptions.map((l) => ({
        label: `${l.maLop} — ${l.tenNganh}${l.khoaHoc ? ` (${l.khoaHoc})` : ""}`,
        value: l.id,
      })),
    },
    { name: "trangThai", label: "Hoạt động", type: "switch", hideOnCreate: true },
  ];

  return (
    <div>
      {isAdminOrGiaoVu && (
        <Space style={{ marginBottom: 16 }} wrap>
          <span>Lọc Lớp sinh hoạt — Khoa:</span>
          <Select
            style={{ width: 200 }}
            value={phongBanIdStr}
            options={phongBanOptions}
            onChange={(v) => {
              setPhongBanIdStr(v);
              setNganhIdStr(TAT_CA);
              setKhoaHoc(TAT_CA);
            }}
          />
          <span>Ngành:</span>
          <Select
            style={{ width: 200 }}
            value={nganhIdStr}
            options={nganhOptions}
            onChange={(v) => {
              setNganhIdStr(v);
              setKhoaHoc(TAT_CA);
            }}
          />
          <span>Khoá học:</span>
          <Select style={{ width: 160 }} value={khoaHoc} options={khoaHocOptions} onChange={setKhoaHoc} />
        </Space>
      )}
      <CrudTable<SinhVien, any, any>
        title="Sinh viên"
        queryKey="sinh-vien"
        fetchPaged={sinhVienApi.getPaged}
        columns={columns}
        formFields={formFields}
        searchPlaceholder="Tìm theo họ tên hoặc MSSV"
        onCreate={isAdmin ? sinhVienApi.create : undefined}
        onUpdate={isAdminOrGiaoVu ? sinhVienApi.update : undefined}
        onDelete={isAdmin ? sinhVienApi.remove : undefined}
        canCreate={isAdmin}
        canEdit={isAdminOrGiaoVu}
        canDelete={isAdmin}
      />
    </div>
  );
}

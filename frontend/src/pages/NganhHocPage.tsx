import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { nganhHocApi } from "../api/modules";
import type { NganhHoc } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function NganhHocPage() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const { data: nganhOptions } = useQuery({
    queryKey: ["nganh-hoc-all"],
    queryFn: () => nganhHocApi.getAll(),
  });

  const columns: ColumnsType<NganhHoc> = [
    { title: "Mã ngành", dataIndex: "maNganh" },
    { title: "Tên ngành", dataIndex: "tenNganh" },
    { title: "Ngành cha", dataIndex: "tenNganhCha" },
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
  ];

  return (
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
  );
}

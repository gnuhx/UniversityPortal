import type { ColumnsType } from "antd/es/table";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { monHocApi } from "../api/modules";
import type { MonHoc } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function MonHocPage() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const columns: ColumnsType<MonHoc> = [
    { title: "Mã môn", dataIndex: "maMon" },
    { title: "Tên môn", dataIndex: "tenMon" },
  ];

  const formFields: CrudFormField[] = [
    { name: "maMon", label: "Mã môn", required: true },
    { name: "tenMon", label: "Tên môn", required: true },
  ];

  return (
    <CrudTable<MonHoc, any, any>
      title="Môn học"
      queryKey="mon-hoc"
      fetchPaged={monHocApi.getPaged}
      columns={columns}
      formFields={formFields}
      searchPlaceholder="Tìm theo mã hoặc tên môn"
      onCreate={isAdmin ? monHocApi.create : undefined}
      onUpdate={isAdmin ? monHocApi.update : undefined}
      onDelete={isAdmin ? monHocApi.remove : undefined}
      canCreate={isAdmin}
      canEdit={isAdmin}
      canDelete={isAdmin}
    />
  );
}

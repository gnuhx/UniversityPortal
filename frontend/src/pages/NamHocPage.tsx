import type { ColumnsType } from "antd/es/table";
import { Button } from "antd";
import { useNavigate } from "react-router-dom";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { namHocApi } from "../api/modules";
import type { NamHoc } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function NamHocPage() {
  const navigate = useNavigate();
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const columns: ColumnsType<NamHoc> = [
    { title: "Năm học", dataIndex: "tenNamHoc" },
    {
      title: "Học kỳ",
      key: "detail",
      render: (_, record) => (
        <Button size="small" onClick={() => navigate(`/hoc-ky?namHocId=${record.id}`)}>
          Xem học kỳ
        </Button>
      ),
    },
  ];

  const formFields: CrudFormField[] = [
    { name: "tenNamHoc", label: "Năm học (định dạng YYYY-YYYY)", required: true },
  ];

  return (
    <CrudTable<NamHoc, any, any>
      title="Năm học"
      queryKey="nam-hoc"
      fetchPaged={namHocApi.getPaged}
      columns={columns}
      formFields={formFields}
      searchPlaceholder="Tìm theo năm học"
      onCreate={isAdmin ? namHocApi.create : undefined}
      onUpdate={isAdmin ? namHocApi.update : undefined}
      onDelete={isAdmin ? namHocApi.remove : undefined}
      canCreate={isAdmin}
      canEdit={isAdmin}
      canDelete={isAdmin}
    />
  );
}

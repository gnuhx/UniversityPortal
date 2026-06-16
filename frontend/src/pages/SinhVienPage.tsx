import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Tag } from "antd";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { sinhVienApi, lopSinhHoatApi } from "../api/modules";
import type { SinhVien } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function SinhVienPage() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const isAdminOrGiaoVu = isAdmin || user?.vaiTro === ROLES.GIAO_VU;

  const { data: lopOptions } = useQuery({
    queryKey: ["lop-sinh-hoat-all"],
    queryFn: () => lopSinhHoatApi.getAll(),
  });

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
      options: (lopOptions || []).map((l) => ({ label: l.maLop, value: l.id })),
    },
    { name: "trangThai", label: "Hoạt động", type: "switch", hideOnCreate: true },
  ];

  return (
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
  );
}

import type { ColumnsType } from "antd/es/table";
import { Tag } from "antd";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { giaoVienApi } from "../api/modules";
import type { GiaoVien } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function GiaoVienPage() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const columns: ColumnsType<GiaoVien> = [
    { title: "Mã GV", dataIndex: "maGv" },
    { title: "Họ tên", dataIndex: "hoTen" },
    { title: "Email", dataIndex: "email" },
    { title: "Phòng ban", dataIndex: "tenPhongBan" },
    {
      title: "Trạng thái",
      dataIndex: "trangThai",
      render: (v: boolean) => (v ? <Tag color="green">Hoạt động</Tag> : <Tag color="red">Đã khoá</Tag>),
    },
  ];

  const formFields: CrudFormField[] = [
    { name: "tenDangNhap", label: "Tên đăng nhập", required: true, hideOnEdit: true },
    { name: "matKhau", label: "Mật khẩu", type: "password", required: true, hideOnEdit: true },
    { name: "maGv", label: "Mã GV", required: true },
    { name: "hoTen", label: "Họ tên", required: true },
    { name: "email", label: "Email", type: "email", required: true },
    { name: "phongBanId", label: "Mã phòng ban", type: "number" },
    { name: "trangThai", label: "Hoạt động", type: "switch", hideOnCreate: true },
  ];

  return (
    <CrudTable<GiaoVien, any, any>
      title="Giáo viên"
      queryKey="giao-vien"
      fetchPaged={giaoVienApi.getPaged}
      columns={columns}
      formFields={formFields}
      searchPlaceholder="Tìm theo họ tên hoặc mã GV"
      onCreate={isAdmin ? giaoVienApi.create : undefined}
      onUpdate={isAdmin ? giaoVienApi.update : undefined}
      onDelete={isAdmin ? giaoVienApi.remove : undefined}
      canCreate={isAdmin}
      canEdit={isAdmin}
      canDelete={isAdmin}
    />
  );
}

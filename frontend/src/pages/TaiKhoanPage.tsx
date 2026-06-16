import type { ColumnsType } from "antd/es/table";
import { Tag, Button, Popconfirm, App } from "antd";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { apiClient } from "../api/client";
import { taiKhoanApi } from "../api/modules";
import type { ApiResponse, TaiKhoan } from "../types";
import { VAI_TRO_OPTIONS } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

async function toggleTrangThai(id: number) {
  const res = await apiClient.patch<ApiResponse<TaiKhoan>>(`/tai-khoan/${id}/trang-thai`);
  return res.data.data;
}

export function TaiKhoanPage() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const { message } = App.useApp();
  const queryClient = useQueryClient();

  const toggleMutation = useMutation({
    mutationFn: toggleTrangThai,
    onSuccess: () => {
      message.success("Cập nhật trạng thái thành công");
      queryClient.invalidateQueries({ queryKey: ["tai-khoan"] });
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Có lỗi xảy ra"),
  });

  const columns: ColumnsType<TaiKhoan> = [
    { title: "Tên đăng nhập", dataIndex: "tenDangNhap" },
    { title: "Họ tên", dataIndex: "hoTen" },
    { title: "Email", dataIndex: "email" },
    { title: "Vai trò", dataIndex: "tenVaiTro" },
    {
      title: "Trạng thái",
      dataIndex: "trangThai",
      render: (v: boolean, record) =>
        isAdmin ? (
          <Popconfirm title="Đổi trạng thái tài khoản?" onConfirm={() => toggleMutation.mutate(record.id)}>
            <Button size="small" type={v ? "default" : "primary"}>
              {v ? <Tag color="green">Hoạt động</Tag> : <Tag color="red">Đã khoá</Tag>}
            </Button>
          </Popconfirm>
        ) : v ? (
          <Tag color="green">Hoạt động</Tag>
        ) : (
          <Tag color="red">Đã khoá</Tag>
        ),
    },
  ];

  const formFields: CrudFormField[] = [
    { name: "tenDangNhap", label: "Tên đăng nhập", required: true, hideOnEdit: true },
    { name: "matKhau", label: "Mật khẩu", type: "password", required: true, hideOnEdit: true },
    { name: "hoTen", label: "Họ tên", required: true },
    { name: "email", label: "Email", type: "email", required: true },
    {
      name: "vaiTroId",
      label: "Vai trò",
      type: "select",
      required: true,
      options: VAI_TRO_OPTIONS.map((v) => ({ label: v.label, value: v.id })),
    },
    { name: "phongBanId", label: "Mã phòng ban", type: "number" },
    { name: "trangThai", label: "Hoạt động", type: "switch", hideOnCreate: true },
  ];

  return (
    <CrudTable<TaiKhoan, any, any>
      title="Tài khoản"
      queryKey="tai-khoan"
      fetchPaged={taiKhoanApi.getPaged}
      columns={columns}
      formFields={formFields}
      searchPlaceholder="Tìm theo tên đăng nhập hoặc họ tên"
      onCreate={isAdmin ? taiKhoanApi.create : undefined}
      onUpdate={isAdmin ? taiKhoanApi.update : undefined}
      canCreate={isAdmin}
      canEdit={isAdmin}
    />
  );
}

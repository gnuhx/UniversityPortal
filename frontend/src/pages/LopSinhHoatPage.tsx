import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { lopSinhHoatApi, giaoVienApi, chuongTrinhDTApi } from "../api/modules";
import type { LopSinhHoat } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function LopSinhHoatPage() {
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

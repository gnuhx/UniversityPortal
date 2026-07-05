import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Button } from "antd";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { NganhMonHocModal } from "../components/NganhMonHocModal";
import { nganhHocApi, phongBanApi } from "../api/modules";
import type { NganhHoc } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function NganhHocPage() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;
  const [monHocModal, setMonHocModal] = useState<NganhHoc | null>(null);

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
          open={!!monHocModal}
          onClose={() => setMonHocModal(null)}
        />
      )}
    </>
  );
}

import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Select, Space } from "antd";
import dayjs from "dayjs";
import { useSearchParams } from "react-router-dom";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { hocKyApi, namHocApi } from "../api/modules";
import type { HocKy } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function HocKyPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const namHocIdParam = searchParams.get("namHocId");
  const namHocId = namHocIdParam ? Number(namHocIdParam) : undefined;
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const { data: namHocOptions } = useQuery({
    queryKey: ["nam-hoc-all"],
    queryFn: () => namHocApi.getAll(),
  });

  const columns: ColumnsType<HocKy> = [
    { title: "Tên học kỳ", dataIndex: "tenHocKy" },
    { title: "Năm học", dataIndex: "tenNamHoc" },
    {
      title: "Ngày bắt đầu",
      dataIndex: "ngayBatDau",
      render: (v: string) => dayjs(v).format("DD/MM/YYYY"),
    },
  ];

  const formFields: CrudFormField[] = [
    { name: "tenHocKy", label: "Tên học kỳ", required: true },
    {
      name: "namHocId",
      label: "Năm học",
      type: "select",
      required: true,
      options: (namHocOptions || []).map((n) => ({ label: n.tenNamHoc, value: n.id })),
    },
    { name: "ngayBatDau", label: "Ngày bắt đầu (thứ Hai)", type: "date", required: true },
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16 }}>
        <span>Năm học:</span>
        <Select
          allowClear
          style={{ width: 160 }}
          placeholder="Tất cả năm học"
          value={namHocId}
          onChange={(v) => setSearchParams(v ? { namHocId: String(v) } : {})}
          options={(namHocOptions || []).map((n) => ({ label: n.tenNamHoc, value: n.id }))}
        />
      </Space>

      <CrudTable<HocKy, any, any>
        title="Học kỳ"
        queryKey="hoc-ky-paged"
        fetchPaged={hocKyApi.getPaged}
        extraParams={{ namHocId }}
        columns={columns}
        formFields={formFields}
        toFormValues={(r) => ({ ...r, ngayBatDau: dayjs(r.ngayBatDau) })}
        onCreate={isAdmin ? hocKyApi.create : undefined}
        onUpdate={isAdmin ? hocKyApi.update : undefined}
        onDelete={isAdmin ? hocKyApi.remove : undefined}
        canCreate={isAdmin}
        canEdit={isAdmin}
        canDelete={isAdmin}
      />
    </div>
  );
}

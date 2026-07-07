import { useState } from "react";
import { Select, Space } from "antd";
import type { ColumnsType } from "antd/es/table";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { noiDungTinhApi } from "../api/modules";
import type { NoiDungTinh, PagedResult, UpsertNoiDungTinh } from "../types";

const KHU_VUC_OPTIONS = [
  { label: "Thư viện", value: "thu-vien" },
  { label: "Học Vụ", value: "hoc-vu" },
];

async function fetchPagedNoiDungTinh(params: Record<string, unknown>): Promise<PagedResult<NoiDungTinh>> {
  const khuVuc = params.khuVuc as string;
  const data = await noiDungTinhApi.getByKhuVuc(khuVuc);
  return { data, total: data.length, page: 1, pageSize: data.length || 1 };
}

export function NoiDungTinhAdminPage() {
  const [khuVuc, setKhuVuc] = useState<string>("thu-vien");

  const columns: ColumnsType<NoiDungTinh> = [
    { title: "Mã mục", dataIndex: "maMuc", width: 180 },
    { title: "Tiêu đề (tên Tab)", dataIndex: "tieuDe", width: 220 },
    { title: "Thứ tự", dataIndex: "thuTu", width: 90 },
  ];

  const formFields: CrudFormField[] = [
    { name: "maMuc", label: "Mã mục (dùng nội bộ, không dấu, không trùng)", required: true },
    { name: "tieuDe", label: "Tiêu đề hiển thị trên Tab", required: true },
    { name: "thuTu", label: "Thứ tự hiển thị", type: "number", required: true },
    { name: "noiDung", label: "Nội dung", type: "textarea" },
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16 }}>
        <span>Khu vực:</span>
        <Select
          style={{ width: 200 }}
          value={khuVuc}
          options={KHU_VUC_OPTIONS}
          onChange={setKhuVuc}
        />
      </Space>

      <CrudTable<NoiDungTinh, UpsertNoiDungTinh, UpsertNoiDungTinh>
        title="Nội dung tĩnh"
        queryKey="noi-dung-tinh-admin"
        fetchPaged={fetchPagedNoiDungTinh}
        extraParams={{ khuVuc }}
        columns={columns}
        formFields={formFields}
        onCreate={(dto) => noiDungTinhApi.create({ ...dto, khuVuc })}
        onUpdate={(id, dto) => noiDungTinhApi.update(id, { ...dto, khuVuc })}
        onDelete={noiDungTinhApi.remove}
        canCreate
        canEdit
        canDelete
      />
    </div>
  );
}

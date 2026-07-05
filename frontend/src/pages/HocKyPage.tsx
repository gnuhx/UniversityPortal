import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Button, Space } from "antd";
import { ArrowLeftOutlined } from "@ant-design/icons";
import dayjs from "dayjs";
import { useNavigate, useParams } from "react-router-dom";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { hocKyApi, namHocApi } from "../api/modules";
import type { HocKy } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function HocKyPage() {
  const { id } = useParams<{ id: string }>();
  const namHocId = Number(id);
  const navigate = useNavigate();
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const { data: namHoc } = useQuery({
    queryKey: ["nam-hoc", namHocId],
    queryFn: () => namHocApi.getById(namHocId),
  });

  const columns: ColumnsType<HocKy> = [
    { title: "Tên học kỳ", dataIndex: "tenHocKy" },
    {
      title: "Ngày bắt đầu",
      dataIndex: "ngayBatDau",
      render: (v: string) => dayjs(v).format("DD/MM/YYYY"),
    },
  ];

  const formFields: CrudFormField[] = [
    { name: "tenHocKy", label: "Tên học kỳ", required: true },
    { name: "ngayBatDau", label: "Ngày bắt đầu (thứ Hai)", type: "date", required: true },
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16 }}>
        <Button icon={<ArrowLeftOutlined />} onClick={() => navigate("/nam-hoc")}>
          Quay lại
        </Button>
        <h2 style={{ margin: 0 }}>Học kỳ của năm học: {namHoc?.tenNamHoc}</h2>
      </Space>

      <CrudTable<HocKy, any, any>
        title="Học kỳ"
        queryKey="hoc-ky-paged"
        fetchPaged={hocKyApi.getPaged}
        extraParams={{ namHocId }}
        columns={columns}
        formFields={formFields}
        toFormValues={(r) => ({ ...r, ngayBatDau: dayjs(r.ngayBatDau) })}
        onCreate={isAdmin ? (dto: any) => hocKyApi.create({ ...dto, namHocId }) : undefined}
        onUpdate={isAdmin ? (id: number, dto: any) => hocKyApi.update(id, { ...dto, namHocId }) : undefined}
        onDelete={isAdmin ? hocKyApi.remove : undefined}
        canCreate={isAdmin}
        canEdit={isAdmin}
        canDelete={isAdmin}
      />
    </div>
  );
}

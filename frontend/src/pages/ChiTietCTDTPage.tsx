import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Button, Tag, Space } from "antd";
import { ArrowLeftOutlined } from "@ant-design/icons";
import { useNavigate, useParams } from "react-router-dom";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { chiTietCTDTApi, chuongTrinhDTApi, monHocApi } from "../api/modules";
import type { ChiTietCTDT } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function ChiTietCTDTPage() {
  const { id } = useParams<{ id: string }>();
  const ctdtId = Number(id);
  const navigate = useNavigate();
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const { data: ctdt } = useQuery({
    queryKey: ["chuong-trinh-dt", ctdtId],
    queryFn: () => chuongTrinhDTApi.getById(ctdtId),
  });
  const { data: monHocOptions } = useQuery({
    queryKey: ["mon-hoc-all"],
    queryFn: () => monHocApi.getAll(),
  });

  const columns: ColumnsType<ChiTietCTDT> = [
    { title: "Mã môn", dataIndex: "maMon" },
    { title: "Tên môn", dataIndex: "tenMon" },
    { title: "Học kỳ", dataIndex: "tenHocKy" },
    { title: "Số tín chỉ", dataIndex: "soTinChi" },
    {
      title: "Tính điểm TB",
      dataIndex: "tinhDiemTb",
      render: (v: boolean) => (v ? <Tag color="green">Có</Tag> : <Tag color="default">Không</Tag>),
    },
  ];

  const formFields: CrudFormField[] = [
    {
      name: "monHocId",
      label: "Môn học",
      type: "select",
      required: true,
      hideOnEdit: true,
      options: (monHocOptions || []).map((m) => ({ label: `${m.maMon} - ${m.tenMon}`, value: m.id })),
    },
    { name: "hocKyId", label: "Mã học kỳ (Id)", type: "number", required: true, hideOnEdit: true },
    { name: "soTinChi", label: "Số tín chỉ", type: "number", required: true },
    { name: "tinhDiemTb", label: "Tính điểm trung bình", type: "switch" },
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16 }}>
        <Button icon={<ArrowLeftOutlined />} onClick={() => navigate("/chuong-trinh-dt")}>
          Quay lại
        </Button>
        <h2 style={{ margin: 0 }}>
          Môn học trong CTĐT: {ctdt?.maCtdt} ({ctdt?.tenNganh} - {ctdt?.khoaHoc})
        </h2>
      </Space>

      <CrudTable<ChiTietCTDT, any, any>
        title="Môn học"
        queryKey="chi-tiet-ctdt"
        fetchPaged={chiTietCTDTApi.getPaged}
        extraParams={{ ctdtId }}
        columns={columns}
        formFields={formFields}
        onCreate={isAdmin ? (dto: any) => chiTietCTDTApi.create({ ...dto, ctdtId }) : undefined}
        onUpdate={isAdmin ? chiTietCTDTApi.update : undefined}
        onDelete={isAdmin ? chiTietCTDTApi.remove : undefined}
        canCreate={isAdmin}
        canEdit={isAdmin}
        canDelete={isAdmin}
      />
    </div>
  );
}

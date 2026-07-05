import { useEffect, useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Button, Select, Space } from "antd";
import { useNavigate } from "react-router-dom";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { chuongTrinhDTApi, nganhHocApi } from "../api/modules";
import type { ChuongTrinhDT } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

export function ChuongTrinhDTPage() {
  const navigate = useNavigate();
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const { data: nganhOptions } = useQuery({
    queryKey: ["nganh-hoc-all"],
    queryFn: () => nganhHocApi.getAll(),
  });

  const { data: ctdtAll } = useQuery({
    queryKey: ["chuong-trinh-dt-all"],
    queryFn: () => chuongTrinhDTApi.getAll(),
  });

  // Khoá học là text tự do, không đồng nhất định dạng -> chỉ distinct + sort giảm dần theo string.
  const khoaHocOptions = useMemo(() => {
    const uniq = Array.from(new Set((ctdtAll || []).map((c) => c.khoaHoc)));
    return uniq.sort((a, b) => b.localeCompare(a));
  }, [ctdtAll]);

  const [khoaHoc, setKhoaHoc] = useState<string>();
  useEffect(() => {
    if (!khoaHoc && khoaHocOptions.length > 0) setKhoaHoc(khoaHocOptions[0]);
  }, [khoaHocOptions, khoaHoc]);

  const columns: ColumnsType<ChuongTrinhDT> = [
    { title: "Mã CTĐT", dataIndex: "maCtdt" },
    { title: "Ngành", dataIndex: "tenNganh" },
    { title: "Khoá học", dataIndex: "khoaHoc" },
    {
      title: "Môn học",
      key: "detail",
      render: (_, record) => (
        <Button size="small" onClick={() => navigate(`/chuong-trinh-dt/${record.id}`)}>
          Xem môn học
        </Button>
      ),
    },
  ];

  const formFields: CrudFormField[] = [
    { name: "maCtdt", label: "Mã CTĐT", required: true },
    {
      name: "nganhId",
      label: "Ngành học",
      type: "select",
      required: true,
      options: (nganhOptions || []).map((n) => ({ label: n.tenNganh, value: n.id })),
    },
    { name: "khoaHoc", label: "Khoá học", required: true },
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16 }}>
        <span>Khoá học:</span>
        <Select
          style={{ width: 160 }}
          value={khoaHoc}
          onChange={setKhoaHoc}
          options={khoaHocOptions.map((k) => ({ label: k, value: k }))}
        />
      </Space>

      <CrudTable<ChuongTrinhDT, any, any>
        title="Chương trình đào tạo"
        queryKey="chuong-trinh-dt"
        fetchPaged={chuongTrinhDTApi.getPaged}
        extraParams={{ khoaHoc }}
        columns={columns}
        formFields={formFields}
        searchPlaceholder="Tìm theo mã CTĐT"
        onCreate={isAdmin ? chuongTrinhDTApi.create : undefined}
        onUpdate={isAdmin ? chuongTrinhDTApi.update : undefined}
        onDelete={isAdmin ? chuongTrinhDTApi.remove : undefined}
        canCreate={isAdmin}
        canEdit={isAdmin}
        canDelete={isAdmin}
      />
    </div>
  );
}

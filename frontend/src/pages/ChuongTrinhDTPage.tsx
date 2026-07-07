import { useQuery } from "@tanstack/react-query";
import type { ColumnsType } from "antd/es/table";
import { Button, Select, Space } from "antd";
import { useNavigate } from "react-router-dom";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { chuongTrinhDTApi, nganhHocApi } from "../api/modules";
import type { ChuongTrinhDT } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import { useKhoaHocOptions } from "../hooks/useKhoaHocOptions";

interface ChuongTrinhDTTableProps {
  /** Lọc theo ngành (id) — bỏ trống = mọi ngành. */
  nganhId?: number;
  /** Lọc theo khoá học (exact match) — bỏ trống = mọi khoá học. */
  khoaHoc?: string;
}

/** Bảng "Chương trình đào tạo" thuần — không tự vẽ filter, nhận nganhId/khoaHoc từ nơi gọi. */
export function ChuongTrinhDTTable({ nganhId, khoaHoc }: ChuongTrinhDTTableProps) {
  const navigate = useNavigate();
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN;

  const { data: nganhOptions } = useQuery({
    queryKey: ["nganh-hoc-all"],
    queryFn: () => nganhHocApi.getAll(),
  });

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
    <CrudTable<ChuongTrinhDT, any, any>
      title="Chương trình đào tạo"
      queryKey="chuong-trinh-dt"
      fetchPaged={chuongTrinhDTApi.getPaged}
      extraParams={{ nganhId, khoaHoc }}
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
  );
}

/**
 * Trang độc lập — tự quản lý filter Khoá học riêng. Không còn được mount qua route nào
 * (route `/chuong-trinh-dt` cũ redirect sang `/nganh-hoc` từ task #11), giữ lại để component
 * vẫn dùng độc lập được nếu cần (vd nhúng ở nơi khác ngoài trang "Ngành học & CTĐT" đã gộp).
 */
export function ChuongTrinhDTPage() {
  const { khoaHoc, setKhoaHoc, khoaHocOptions, khoaHocFilter } = useKhoaHocOptions();

  return (
    <div>
      <Space style={{ marginBottom: 16 }}>
        <span>Khoá học:</span>
        <Select style={{ width: 160 }} value={khoaHoc} onChange={setKhoaHoc} options={khoaHocOptions} />
      </Space>

      <ChuongTrinhDTTable khoaHoc={khoaHocFilter} />
    </div>
  );
}

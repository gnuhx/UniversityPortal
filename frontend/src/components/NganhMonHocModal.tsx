import { useEffect, useState } from "react";
import { Modal, Select, Table, Tag, Empty, Button, Space } from "antd";
import { EditOutlined } from "@ant-design/icons";
import type { ColumnsType } from "antd/es/table";
import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { chuongTrinhDTApi, chiTietCTDTApi } from "../api/modules";
import type { ChiTietCTDT } from "../types";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";

interface NganhMonHocModalProps {
  nganhId: number;
  tenNganh: string;
  open: boolean;
  onClose: () => void;
  /** Khoá học đang được chọn ở ngoài (nếu có) — ưu tiên chọn CTĐT khớp khoá học này khi mở modal. */
  khoaHoc?: string;
}

export function NganhMonHocModal({ nganhId, tenNganh, open, onClose, khoaHoc }: NganhMonHocModalProps) {
  const navigate = useNavigate();
  const isAdmin = useAuthStore((s) => s.user?.vaiTro) === ROLES.ADMIN;
  const [ctdtId, setCtdtId] = useState<number | undefined>();

  const { data: ctdtResult, isLoading: ctdtLoading } = useQuery({
    queryKey: ["chuong-trinh-dt-by-nganh", nganhId],
    queryFn: () => chuongTrinhDTApi.getPaged({ nganhId, page: 1, pageSize: 50 }),
    enabled: open,
  });

  const ctdtOptions = ctdtResult?.data || [];

  useEffect(() => {
    if (!open) {
      setCtdtId(undefined);
      return;
    }
    if (ctdtOptions.length > 0 && !ctdtOptions.some((c) => c.id === ctdtId)) {
      const preferred = ctdtOptions.find((c) => c.khoaHoc === khoaHoc);
      setCtdtId((preferred || ctdtOptions[0]).id);
    }
  }, [open, ctdtOptions, ctdtId, khoaHoc]);

  const { data: monHocResult, isLoading: monHocLoading } = useQuery({
    queryKey: ["chi-tiet-ctdt-by-ctdt", ctdtId],
    queryFn: () => chiTietCTDTApi.getPaged({ ctdtId, page: 1, pageSize: 100 }),
    enabled: open && !!ctdtId,
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

  return (
    <Modal
      title={`Môn học của ngành: ${tenNganh}`}
      open={open}
      onCancel={onClose}
      footer={null}
      width={760}
      destroyOnHidden
    >
      {!ctdtLoading && ctdtOptions.length === 0 ? (
        <Empty description="Ngành này chưa có chương trình đào tạo." />
      ) : (
        <>
          <Space.Compact style={{ width: "100%", marginBottom: 16 }}>
            <Select
              style={{ width: "100%" }}
              placeholder="Chọn chương trình đào tạo (khoá học)"
              loading={ctdtLoading}
              value={ctdtId}
              options={ctdtOptions.map((c) => ({ label: `${c.maCtdt} — ${c.khoaHoc}`, value: c.id }))}
              onChange={(v) => setCtdtId(v)}
            />
            {isAdmin && (
              <Button
                icon={<EditOutlined />}
                disabled={!ctdtId}
                onClick={() => {
                  onClose();
                  navigate(`/chuong-trinh-dt/${ctdtId}`);
                }}
              >
                Sửa
              </Button>
            )}
          </Space.Compact>
          <Table<ChiTietCTDT>
            rowKey="id"
            size="small"
            loading={monHocLoading}
            columns={columns}
            dataSource={monHocResult?.data || []}
            pagination={false}
          />
        </>
      )}
    </Modal>
  );
}

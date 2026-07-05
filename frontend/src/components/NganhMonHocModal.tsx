import { useEffect, useState } from "react";
import { Modal, Select, Table, Tag, Empty } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery } from "@tanstack/react-query";
import { chuongTrinhDTApi, chiTietCTDTApi } from "../api/modules";
import type { ChiTietCTDT } from "../types";

interface NganhMonHocModalProps {
  nganhId: number;
  tenNganh: string;
  open: boolean;
  onClose: () => void;
}

export function NganhMonHocModal({ nganhId, tenNganh, open, onClose }: NganhMonHocModalProps) {
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
      setCtdtId(ctdtOptions[0].id);
    }
  }, [open, ctdtOptions, ctdtId]);

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
          <Select
            style={{ width: "100%", marginBottom: 16 }}
            placeholder="Chọn chương trình đào tạo (khoá học)"
            loading={ctdtLoading}
            value={ctdtId}
            options={ctdtOptions.map((c) => ({ label: `${c.maCtdt} — ${c.khoaHoc}`, value: c.id }))}
            onChange={(v) => setCtdtId(v)}
          />
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

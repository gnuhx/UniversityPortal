import { useState } from "react";
import { Table, Tag, Button, Popconfirm, message, Space } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { lopHocPhanApi } from "../api/modules";
import { NhapDiemDrawer } from "../components/NhapDiemDrawer";
import type { LopHocPhan } from "../types";

export function LopHocPhanPage() {
  const qc = useQueryClient();
  const [drawer, setDrawer] = useState<LopHocPhan | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ["lop-hoc-phan-me"],
    queryFn: () => lopHocPhanApi.getMe(),
  });

  const khoaMutation = useMutation({
    mutationFn: (id: number) => lopHocPhanApi.khoaBangDiem(id),
    onSuccess: () => {
      message.success("Đã khoá bảng điểm.");
      qc.invalidateQueries({ queryKey: ["lop-hoc-phan-me"] });
    },
    onError: () => message.error("Khoá bảng điểm thất bại."),
  });

  const columns: ColumnsType<LopHocPhan> = [
    { title: "Mã lớp HP", dataIndex: "maLopHp", width: 130 },
    { title: "Môn học", dataIndex: "tenMon", width: 200 },
    { title: "Mã môn", dataIndex: "maMon", width: 100 },
    { title: "Học kỳ", dataIndex: "tenHocKy", width: 130 },
    { title: "Số SV", dataIndex: "soSinhVien", width: 70, align: "center" },
    {
      title: "Bảng điểm",
      dataIndex: "khoaBangDiem",
      width: 110,
      align: "center",
      render: (v: boolean) => <Tag color={v ? "red" : "green"}>{v ? "Đã khoá" : "Mở"}</Tag>,
    },
    {
      title: "Hành động",
      width: 220,
      render: (_: unknown, row: LopHocPhan) => (
        <Space>
          <Button size="small" type="primary" onClick={() => setDrawer(row)}>
            Nhập điểm
          </Button>
          {!row.khoaBangDiem && (
            <Popconfirm
              title="Khoá bảng điểm?"
              description="Sau khi khoá, không thể nhập thêm điểm nếu chưa yêu cầu mở lại."
              onConfirm={() => khoaMutation.mutate(row.id)}
              okText="Khoá"
              cancelText="Huỷ"
            >
              <Button size="small" danger loading={khoaMutation.isPending}>
                Khoá bảng điểm
              </Button>
            </Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div>
      <h2 style={{ marginBottom: 16 }}>Lớp học phần của tôi</h2>
      <Table<LopHocPhan>
        rowKey="id"
        columns={columns}
        dataSource={data ?? []}
        loading={isLoading}
        pagination={false}
        scroll={{ x: 900 }}
        size="middle"
      />
      {drawer && (
        <NhapDiemDrawer
          lopHpId={drawer.id}
          maLopHp={drawer.maLopHp}
          tenMon={drawer.tenMon}
          khoaBangDiem={drawer.khoaBangDiem}
          onClose={() => setDrawer(null)}
        />
      )}
    </div>
  );
}

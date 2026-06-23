import { Drawer, Table, InputNumber, Button, Tag, message, Space, Typography } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { danhSachLopHPApi } from "../api/modules";
import type { DanhSachLopHP } from "../types";

const { Text } = Typography;

interface Props {
  lopHpId: number;
  maLopHp: string;
  tenMon: string;
  khoaBangDiem: boolean;
  onClose: () => void;
}

function tinhTongKet(qt1?: number | null, qt2?: number | null, thi?: number | null): string {
  if (qt1 == null || qt2 == null || thi == null) return "—";
  return (qt1 * 0.15 + qt2 * 0.15 + thi * 0.7).toFixed(2);
}

export function NhapDiemDrawer({ lopHpId, maLopHp, tenMon, khoaBangDiem, onClose }: Props) {
  const qc = useQueryClient();
  const [edits, setEdits] = useState<Record<number, { diemQt1?: number; diemQt2?: number; diemThi?: number }>>({});

  const { data, isLoading } = useQuery({
    queryKey: ["danh-sach-lop-hp", lopHpId],
    queryFn: () => danhSachLopHPApi.getByLopHocPhan(lopHpId),
  });

  const saveMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: { diemQt1?: number; diemQt2?: number; diemThi?: number } }) =>
      danhSachLopHPApi.nhapDiem(id, dto),
    onSuccess: () => {
      message.success("Lưu điểm thành công.");
      qc.invalidateQueries({ queryKey: ["danh-sach-lop-hp", lopHpId] });
    },
    onError: () => message.error("Lưu điểm thất bại."),
  });

  const setEdit = (id: number, field: string, val?: number) => {
    setEdits((prev) => ({ ...prev, [id]: { ...prev[id], [field]: val } }));
  };

  const getVal = (row: DanhSachLopHP, field: "diemQt1" | "diemQt2" | "diemThi") =>
    edits[row.id]?.[field] ?? (row[field] as number | null | undefined) ?? undefined;

  const columns: ColumnsType<DanhSachLopHP> = [
    { title: "Họ tên", dataIndex: "tenSinhVien", width: 160,
      render: (_: unknown, row: DanhSachLopHP) => row.tenSinhVien ?? row.mssv ?? String(row.id) },
    { title: "MSSV", dataIndex: "mssv", width: 100,
      render: (_: unknown, row: DanhSachLopHP) => row.mssv ?? "—" },
    {
      title: "ĐQT1 (15%)", width: 110, align: "center",
      render: (_: unknown, row: DanhSachLopHP) =>
        khoaBangDiem ? (row.diemQt1?.toFixed(1) ?? "—") : (
          <InputNumber min={0} max={10} step={0.1} style={{ width: 80 }}
            value={getVal(row, "diemQt1")}
            onChange={(v) => setEdit(row.id, "diemQt1", v ?? undefined)} />
        ),
    },
    {
      title: "ĐQT2 (15%)", width: 110, align: "center",
      render: (_: unknown, row: DanhSachLopHP) =>
        khoaBangDiem ? (row.diemQt2?.toFixed(1) ?? "—") : (
          <InputNumber min={0} max={10} step={0.1} style={{ width: 80 }}
            value={getVal(row, "diemQt2")}
            onChange={(v) => setEdit(row.id, "diemQt2", v ?? undefined)} />
        ),
    },
    {
      title: "ĐThi (70%)", width: 110, align: "center",
      render: (_: unknown, row: DanhSachLopHP) =>
        khoaBangDiem ? (row.diemThi?.toFixed(1) ?? "—") : (
          <InputNumber min={0} max={10} step={0.1} style={{ width: 80 }}
            value={getVal(row, "diemThi")}
            onChange={(v) => setEdit(row.id, "diemThi", v ?? undefined)} />
        ),
    },
    {
      title: "ĐTổng kết", width: 100, align: "center",
      render: (_: unknown, row: DanhSachLopHP) => {
        const qt1 = getVal(row, "diemQt1");
        const qt2 = getVal(row, "diemQt2");
        const thi = getVal(row, "diemThi");
        const tk = tinhTongKet(qt1, qt2, thi);
        if (tk === "—") return <span style={{ color: "#aaa" }}>—</span>;
        const num = parseFloat(tk);
        const color = num >= 8 ? "green" : num >= 5 ? "orange" : "red";
        return <Tag color={color}>{tk}</Tag>;
      },
    },
    {
      title: "Lưu", width: 80, align: "center",
      render: (_: unknown, row: DanhSachLopHP) =>
        khoaBangDiem ? null : (
          <Button size="small" type="primary"
            loading={saveMutation.isPending}
            onClick={() => saveMutation.mutate({ id: row.id, dto: edits[row.id] ?? {} })}>
            Lưu
          </Button>
        ),
    },
  ];

  return (
    <Drawer
      title={
        <Space>
          <span>Nhập điểm — {maLopHp}</span>
          <Text type="secondary" style={{ fontSize: 13 }}>{tenMon}</Text>
          {khoaBangDiem && <Tag color="red">Đã khoá</Tag>}
        </Space>
      }
      open
      onClose={onClose}
      width={900}
    >
      <Table<DanhSachLopHP>
        rowKey="id"
        columns={columns}
        dataSource={data ?? []}
        loading={isLoading}
        pagination={false}
        size="middle"
        scroll={{ x: 700 }}
      />
    </Drawer>
  );
}

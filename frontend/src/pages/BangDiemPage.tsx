import { Badge, Card, Col, Row, Spin, Table, Tabs, Tag } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery } from "@tanstack/react-query";
import { danhSachLopHPApi } from "../api/modules";
import type { DanhSachLopHP } from "../types";

function diemColor(diem: number | null | undefined): string {
  if (diem == null) return "default";
  if (diem >= 8) return "success";
  if (diem >= 5) return "warning";
  return "error";
}

function diemTag(diem: number | null | undefined) {
  if (diem == null) return <span style={{ color: "#aaa" }}>—</span>;
  return <Badge status={diemColor(diem) as any} text={diem.toFixed(1)} />;
}

function tinhGpa(list: DanhSachLopHP[]): string {
  const coGpa = list.filter((x) => x.diemTongKet != null);
  if (coGpa.length === 0) return "—";
  const avg = coGpa.reduce((s, x) => s + (x.diemTongKet ?? 0), 0) / coGpa.length;
  return avg.toFixed(2);
}

const columns: ColumnsType<DanhSachLopHP> = [
  { title: "Môn học", dataIndex: "tenMon", width: 200 },
  { title: "Mã lớp HP", dataIndex: "maLopHp", width: 120 },
  { title: "Giảng viên", dataIndex: "tenGiaoVien", width: 150 },
  { title: "Loại ĐK", dataIndex: "loaiDangKy", width: 100 },
  {
    title: "ĐQT1",
    dataIndex: "diemQt1",
    width: 70,
    align: "center",
    render: (v) => diemTag(v),
  },
  {
    title: "ĐQT2",
    dataIndex: "diemQt2",
    width: 70,
    align: "center",
    render: (v) => diemTag(v),
  },
  {
    title: "ĐThi",
    dataIndex: "diemThi",
    width: 70,
    align: "center",
    render: (v) => diemTag(v),
  },
  {
    title: "ĐTổng kết",
    dataIndex: "diemTongKet",
    width: 90,
    align: "center",
    render: (v) => {
      if (v == null) return <span style={{ color: "#aaa" }}>—</span>;
      const color = v >= 8 ? "green" : v >= 5 ? "orange" : "red";
      return <Tag color={color}>{v.toFixed(1)}</Tag>;
    },
  },
  {
    title: "Trạng thái",
    dataIndex: "trangThaiDuyet",
    width: 110,
    render: (v: string) => {
      const color = v === "Đã duyệt" ? "green" : v === "Từ chối" ? "red" : "default";
      return <Tag color={color}>{v}</Tag>;
    },
  },
];

export function BangDiemPage() {
  const { data, isLoading } = useQuery({
    queryKey: ["danh-sach-lop-hp-me"],
    queryFn: () => danhSachLopHPApi.getMe(),
  });

  if (isLoading) return <Spin />;
  if (!data || data.length === 0)
    return <p style={{ color: "#888" }}>Chưa có dữ liệu bảng điểm.</p>;

  // Group by học kỳ
  const byHocKy = data.reduce<Record<string, DanhSachLopHP[]>>((acc, item) => {
    const key = item.tenHocKy;
    if (!acc[key]) acc[key] = [];
    acc[key].push(item);
    return acc;
  }, {});

  const tabItems = Object.entries(byHocKy).map(([tenHocKy, items]) => ({
    key: tenHocKy,
    label: tenHocKy,
    children: (
      <>
        <Table<DanhSachLopHP>
          rowKey="id"
          columns={columns}
          dataSource={items}
          pagination={false}
          size="middle"
          scroll={{ x: 800 }}
        />
        <Row gutter={16} style={{ marginTop: 16 }}>
          <Col>
            <Card size="small">
              <span style={{ fontWeight: 500 }}>GPA học kỳ: </span>
              <strong>{tinhGpa(items)}</strong>
            </Card>
          </Col>
          <Col>
            <Card size="small">
              <span style={{ fontWeight: 500 }}>Số môn: </span>
              <strong>{items.length}</strong>
            </Card>
          </Col>
        </Row>
      </>
    ),
  }));

  const gpaAll = tinhGpa(data);

  return (
    <div>
      <Row justify="space-between" align="middle" style={{ marginBottom: 16 }}>
        <Col>
          <h2 style={{ margin: 0 }}>Bảng điểm</h2>
        </Col>
        <Col>
          <Card size="small">
            GPA tích lũy: <strong>{gpaAll}</strong>
          </Card>
        </Col>
      </Row>
      <Tabs items={tabItems} type="card" />
    </div>
  );
}

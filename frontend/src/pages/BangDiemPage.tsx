import { useEffect, useState } from "react";
import { Badge, Card, Col, Collapse, Row, Spin, Table, Tabs, Tag, Typography } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useQuery } from "@tanstack/react-query";
import { danhSachLopHPApi } from "../api/modules";
import type { DanhSachLopHP } from "../types";

const { Text } = Typography;

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

/** Nhóm danh sách theo Năm học → Học kỳ. Khoá nhóm dùng id (không dùng tên) để tránh gộp nhầm khi trùng tên hiển thị. */
function groupByNamHocRoiHocKy(data: DanhSachLopHP[]) {
  const namHocMap = new Map<number, { namHocId: number; tenNamHoc: string; hocKyMap: Map<number, { hocKyId: number; tenHocKy: string; items: DanhSachLopHP[] }> }>();

  for (const item of data) {
    let nh = namHocMap.get(item.namHocId);
    if (!nh) {
      nh = { namHocId: item.namHocId, tenNamHoc: item.tenNamHoc, hocKyMap: new Map() };
      namHocMap.set(item.namHocId, nh);
    }
    let hk = nh.hocKyMap.get(item.hocKyId);
    if (!hk) {
      hk = { hocKyId: item.hocKyId, tenHocKy: item.tenHocKy, items: [] };
      nh.hocKyMap.set(item.hocKyId, hk);
    }
    hk.items.push(item);
  }

  return Array.from(namHocMap.values())
    .sort((a, b) => b.namHocId - a.namHocId)
    .map((nh) => ({
      namHocId: nh.namHocId,
      tenNamHoc: nh.tenNamHoc,
      hocKyList: Array.from(nh.hocKyMap.values()).sort((a, b) => a.hocKyId - b.hocKyId),
    }));
}

export function BangDiemPage() {
  const { data, isLoading } = useQuery({
    queryKey: ["danh-sach-lop-hp-me"],
    queryFn: () => danhSachLopHPApi.getMe(),
  });

  const namHocGroups = data ? groupByNamHocRoiHocKy(data) : [];

  // Chỉ mở sẵn năm học mới nhất — controlled activeKey vì defaultActiveKey chỉ đọc 1 lần lúc mount,
  // trong khi `namHocGroups` chỉ có dữ liệu thật sau khi query tải xong.
  const [activeKeys, setActiveKeys] = useState<string[]>();
  useEffect(() => {
    if (activeKeys === undefined && namHocGroups.length > 0) {
      setActiveKeys([String(namHocGroups[0].namHocId)]);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [namHocGroups.length]);

  if (isLoading) return <Spin />;
  if (!data || data.length === 0)
    return <p style={{ color: "#888" }}>Chưa có dữ liệu bảng điểm.</p>;

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
      <Collapse
        activeKey={activeKeys}
        onChange={(keys) => setActiveKeys(Array.isArray(keys) ? keys : [keys])}
        items={namHocGroups.map((nh) => ({
          key: String(nh.namHocId),
          label: <Text strong>Năm học {nh.tenNamHoc}</Text>,
          children: (
            <Tabs
              type="card"
              items={nh.hocKyList.map(({ hocKyId, tenHocKy, items }) => ({
                key: String(hocKyId),
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
              }))}
            />
          ),
        }))}
      />
    </div>
  );
}

import { useEffect, useMemo, useState } from "react";
import { Select, Calendar, Badge, Table, Space, Spin } from "antd";
import type { ColumnsType } from "antd/es/table";
import dayjs, { type Dayjs } from "dayjs";
import { useQuery } from "@tanstack/react-query";
import { CrudTable, type CrudFormField } from "../components/CrudTable";
import { thoiKhoaBieuApi, tuanHocApi, hocKyApi, lopHocPhanApi } from "../api/modules";
import { useAuthStore } from "../store/authStore";
import { ROLES } from "../constants/roles";
import type { ThoiKhoaBieu, LopHocPhan } from "../types";

// Quy ước: Thu = 2..8 (2 = Thứ Hai ... 7 = Thứ Bảy, 8 = Chủ nhật)
const THU_OPTIONS = [
  { value: 2, label: "Thứ Hai" },
  { value: 3, label: "Thứ Ba" },
  { value: 4, label: "Thứ Tư" },
  { value: 5, label: "Thứ Năm" },
  { value: 6, label: "Thứ Sáu" },
  { value: 7, label: "Thứ Bảy" },
  { value: 8, label: "Chủ nhật" },
];

function thuLabel(thu: number) {
  return THU_OPTIONS.find((t) => t.value === thu)?.label ?? `Thứ ${thu}`;
}

export function ThoiKhoaBieuPage() {
  const user = useAuthStore((s) => s.user);
  const isAdmin = user?.vaiTro === ROLES.ADMIN || user?.vaiTro === ROLES.GIAO_VU;

  return isAdmin ? <AdminThoiKhoaBieu /> : <MyLichHoc />;
}

function MyLichHoc() {
  const [hocKyId, setHocKyId] = useState<number | undefined>();

  const { data: hocKys = [] } = useQuery({ queryKey: ["hoc-ky"], queryFn: () => hocKyApi.getAll() });

  // HocKyController trả về danh sách sắp xếp giảm dần theo ngày bắt đầu -> phần tử đầu là học kỳ mới nhất.
  const effectiveHocKyId = hocKyId ?? hocKys[0]?.id;
  const selectedHocKy = hocKys.find((hk) => hk.id === effectiveHocKyId);

  const { data: items = [], isLoading } = useQuery({
    queryKey: ["thoi-khoa-bieu-me", effectiveHocKyId],
    queryFn: () => thoiKhoaBieuApi.getMe(effectiveHocKyId),
    enabled: !!effectiveHocKyId,
  });

  // Nhảy lịch tới đúng tháng bắt đầu học kỳ đang chọn, tránh hiện lịch trống ở tháng hiện tại
  // khi học kỳ đã qua/ở tương lai. Người dùng vẫn điều hướng tự do sau đó qua onPanelChange.
  const [calendarValue, setCalendarValue] = useState<Dayjs>(dayjs());
  useEffect(() => {
    if (selectedHocKy) setCalendarValue(dayjs(selectedHocKy.ngayBatDau));
  }, [selectedHocKy?.id]);

  const byDate = useMemo(() => {
    const map = new Map<string, ThoiKhoaBieu[]>();
    for (const item of items) {
      const key = item.ngayHoc.slice(0, 10);
      const list = map.get(key) ?? [];
      list.push(item);
      map.set(key, list);
    }
    return map;
  }, [items]);

  return (
    <div>
      <Space style={{ marginBottom: 16 }} align="center" wrap>
        <h2 style={{ margin: 0 }}>Thời khoá biểu</h2>
        <Select
          style={{ width: 260 }}
          placeholder="Chọn học kỳ"
          value={effectiveHocKyId}
          options={hocKys.map((hk) => ({ value: hk.id, label: `${hk.tenHocKy} — ${hk.tenNamHoc}` }))}
          onChange={(v) => setHocKyId(v)}
        />
      </Space>
      <Spin spinning={isLoading}>
        <Calendar
          value={calendarValue}
          onPanelChange={(date) => setCalendarValue(date)}
          onSelect={(date) => setCalendarValue(date)}
          cellRender={(date: Dayjs, info) => {
            if (info.type !== "date") return info.originNode;
            const events = byDate.get(date.format("YYYY-MM-DD")) ?? [];
            if (events.length === 0) return null;
            return (
              <ul style={{ margin: 0, padding: 0, listStyle: "none" }}>
                {events.map((e) => (
                  <li key={e.id} style={{ marginBottom: 2 }}>
                    <Badge
                      status="processing"
                      text={`Tiết ${e.tietBatDau}-${e.tietKetThuc}: ${e.tenMon} (${e.phongHoc})`}
                    />
                  </li>
                ))}
              </ul>
            );
          }}
        />
      </Spin>
    </div>
  );
}

function AdminThoiKhoaBieu() {
  const [hocKyId, setHocKyId] = useState<number | undefined>();

  const { data: hocKys = [] } = useQuery({ queryKey: ["hoc-ky"], queryFn: () => hocKyApi.getAll() });
  const { data: tuanHocs = [] } = useQuery({ queryKey: ["tuan-hoc-all"], queryFn: () => tuanHocApi.getAll() });

  const { data: lopResult, isLoading } = useQuery({
    queryKey: ["lop-hoc-phan-admin", hocKyId],
    queryFn: () => lopHocPhanApi.getPaged({ hocKyId, page: 1, pageSize: 100 }),
  });

  const columns: ColumnsType<LopHocPhan> = [
    { title: "Mã lớp HP", dataIndex: "maLopHp" },
    { title: "Môn học", dataIndex: "tenMon" },
    { title: "Học kỳ", dataIndex: "tenHocKy" },
    { title: "Giáo viên", dataIndex: "tenGiaoVien" },
    { title: "Số SV", dataIndex: "soSinhVien", align: "center" },
  ];

  const tkbFormFields: CrudFormField[] = [
    {
      name: "tuanHocId",
      label: "Tuần học",
      type: "select",
      required: true,
      options: tuanHocs.map((t) => ({
        label: `${t.maTuan} (${t.ngayBatDau} → ${t.ngayKetThuc}) — ${t.tenNamHoc}`,
        value: t.id,
      })),
    },
    { name: "thu", label: "Thứ", type: "select", required: true, options: THU_OPTIONS },
    { name: "tietBatDau", label: "Tiết bắt đầu", type: "number", required: true },
    { name: "tietKetThuc", label: "Tiết kết thúc", type: "number", required: true },
    { name: "phongHoc", label: "Phòng học", required: true },
  ];

  const tkbColumns: ColumnsType<ThoiKhoaBieu> = [
    { title: "Tuần", dataIndex: "maTuan" },
    { title: "Thứ", dataIndex: "thu", render: (v: number) => thuLabel(v) },
    { title: "Tiết", key: "tiet", render: (_, r) => `${r.tietBatDau} - ${r.tietKetThuc}` },
    { title: "Phòng", dataIndex: "phongHoc" },
    { title: "Ngày học", dataIndex: "ngayHoc" },
  ];

  return (
    <div>
      <Space style={{ marginBottom: 16 }} align="center" wrap>
        <h2 style={{ margin: 0 }}>Quản lý thời khoá biểu</h2>
        <Select
          allowClear
          style={{ width: 260 }}
          placeholder="Lọc theo học kỳ"
          options={hocKys.map((hk) => ({ value: hk.id, label: `${hk.tenHocKy} — ${hk.tenNamHoc}` }))}
          onChange={(v) => setHocKyId(v)}
        />
      </Space>
      <Table<LopHocPhan>
        rowKey="id"
        loading={isLoading}
        columns={columns}
        dataSource={lopResult?.data || []}
        pagination={{ pageSize: 20 }}
        expandable={{
          expandedRowRender: (record) => (
            <CrudTable<ThoiKhoaBieu, any, any>
              title={`Buổi học — ${record.maLopHp}`}
              queryKey={`thoi-khoa-bieu-${record.id}`}
              fetchPaged={thoiKhoaBieuApi.getPaged}
              extraParams={{ lopHpId: record.id }}
              columns={tkbColumns}
              formFields={tkbFormFields}
              onCreate={(dto: any) => thoiKhoaBieuApi.create({ ...dto, lopHpId: record.id })}
              onUpdate={thoiKhoaBieuApi.update}
              onDelete={thoiKhoaBieuApi.remove}
              canCreate
              canEdit
              canDelete
            />
          ),
        }}
      />
    </div>
  );
}

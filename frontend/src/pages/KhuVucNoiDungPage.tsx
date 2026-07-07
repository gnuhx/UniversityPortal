import { Card, Empty, Spin, Tabs, Typography } from "antd";
import { useQuery } from "@tanstack/react-query";
import { noiDungTinhApi } from "../api/modules";

const { Paragraph } = Typography;

interface Props {
  khuVuc: string;
  tieuDeTrang: string;
}

export function KhuVucNoiDungPage({ khuVuc, tieuDeTrang }: Props) {
  const { data, isLoading } = useQuery({
    queryKey: ["noi-dung-tinh", khuVuc],
    queryFn: () => noiDungTinhApi.getByKhuVuc(khuVuc),
  });

  if (isLoading) return <Spin />;
  if (!data || data.length === 0)
    return <Empty description="Chưa có nội dung." />;

  return (
    <div>
      <h2 style={{ marginBottom: 16 }}>{tieuDeTrang}</h2>
      <Tabs
        type="card"
        items={data.map((muc) => ({
          key: muc.maMuc,
          label: muc.tieuDe,
          children: (
            <Card size="small">
              <Paragraph style={{ whiteSpace: "pre-wrap", marginBottom: 0 }}>
                {muc.noiDung?.trim() ? muc.noiDung : "Nội dung đang được cập nhật."}
              </Paragraph>
            </Card>
          ),
        }))}
      />
    </div>
  );
}

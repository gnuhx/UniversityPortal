import { useEffect, useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { chuongTrinhDTApi } from "../api/modules";

/**
 * Danh sách "Khoá học" distinct từ toàn bộ CTĐT, mặc định chọn khoá mới nhất.
 * `khoaHoc` là text tự do (không đồng nhất định dạng) nên chỉ distinct + sort giảm dần theo string.
 */
export function useKhoaHocOptions() {
  const { data: ctdtAll, isLoading } = useQuery({
    queryKey: ["chuong-trinh-dt-all"],
    queryFn: () => chuongTrinhDTApi.getAll(),
  });

  const khoaHocOptions = useMemo(() => {
    const uniq = Array.from(new Set((ctdtAll || []).map((c) => c.khoaHoc)));
    return uniq.sort((a, b) => b.localeCompare(a));
  }, [ctdtAll]);

  const [khoaHoc, setKhoaHoc] = useState<string>();
  useEffect(() => {
    if (!khoaHoc && khoaHocOptions.length > 0) setKhoaHoc(khoaHocOptions[0]);
  }, [khoaHocOptions, khoaHoc]);

  return { khoaHoc, setKhoaHoc, khoaHocOptions, isLoading };
}

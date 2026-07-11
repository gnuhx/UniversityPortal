import { useEffect, useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { chuongTrinhDTApi } from "../api/modules";

const TAT_CA = "";

/**
 * Danh sách "Khoá học" distinct từ toàn bộ CTĐT, mặc định chọn khoá mới nhất.
 * `khoaHoc` là text tự do (không đồng nhất định dạng) nên chỉ distinct + sort giảm dần theo string.
 * Có thêm lựa chọn "Tất cả" (sentinel chuỗi rỗng) — `khoaHocFilter` là giá trị đã quy đổi
 * (`undefined` khi đang ở "Tất cả") dùng để truyền thẳng vào query param lọc API.
 */
export function useKhoaHocOptions() {
  const { data: ctdtAll, isLoading } = useQuery({
    queryKey: ["chuong-trinh-dt-all"],
    queryFn: () => chuongTrinhDTApi.getAll(),
  });

  const khoaHocValues = useMemo(() => {
    const uniq = Array.from(new Set((ctdtAll || []).map((c) => c.khoaHoc)));
    return uniq.sort((a, b) => b.localeCompare(a));
  }, [ctdtAll]);

  const khoaHocOptions = useMemo(
    () => [{ label: "Tất cả", value: TAT_CA }, ...khoaHocValues.map((k) => ({ label: k, value: k }))],
    [khoaHocValues],
  );

  const [khoaHoc, setKhoaHocState] = useState<string>(TAT_CA);
  const [initialized, setInitialized] = useState(false);

  // Mặc định tự chọn khoá mới nhất khi tải xong lần đầu — chỉ chạy 1 lần,
  // không ghi đè lại nếu người dùng đã chủ động chọn "Tất cả" sau đó.
  useEffect(() => {
    if (!initialized && khoaHocValues.length > 0) {
      setKhoaHocState(khoaHocValues[0]);
      setInitialized(true);
    }
  }, [khoaHocValues, initialized]);

  const setKhoaHoc = (value: string) => {
    setInitialized(true);
    setKhoaHocState(value);
  };

  return {
    khoaHoc,
    setKhoaHoc,
    khoaHocOptions,
    /** Giá trị dùng để lọc API — `undefined` khi đang ở "Tất cả". */
    khoaHocFilter: khoaHoc || undefined,
    isLoading,
  };
}

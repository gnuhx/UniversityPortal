import { useEffect, useMemo, useState } from "react";
import { Modal, Form, Input, App, Alert } from "antd";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { chuongTrinhDTApi } from "../api/modules";
import type { CloneChuongTrinhDT } from "../types";

interface NhanBanCtdtModalProps {
  nganhId: number;
  tenNganh: string;
  open: boolean;
  onClose: () => void;
}

/** Gợi ý mã CTĐT mới bằng cách thay hậu tố "-Kxx" của mã nguồn bằng năm bắt đầu của khoá học mới. */
function goiYMaCtdt(maNguon: string, khoaHocMoi: string): string {
  const namMoi = khoaHocMoi.match(/^(\d{4})-\d{4}$/)?.[1];
  if (!namMoi) return "";
  if (/-K\d{2}$/i.test(maNguon)) {
    return maNguon.replace(/-K\d{2}$/i, `-K${namMoi.slice(2)}`);
  }
  return "";
}

export function NhanBanCtdtModal({ nganhId, tenNganh, open, onClose }: NhanBanCtdtModalProps) {
  const { message, modal } = App.useApp();
  const queryClient = useQueryClient();
  const [form] = Form.useForm<{ khoaHocMoi: string; maCtdtMoi: string }>();
  const [maCtdtDaSua, setMaCtdtDaSua] = useState(false);

  const { data: ctdtResult, isLoading: ctdtLoading } = useQuery({
    queryKey: ["chuong-trinh-dt-by-nganh", nganhId],
    queryFn: () => chuongTrinhDTApi.getPaged({ nganhId, page: 1, pageSize: 50 }),
    enabled: open,
  });

  const nguon = useMemo(() => {
    const items = ctdtResult?.data || [];
    return [...items].sort((a, b) => b.khoaHoc.localeCompare(a.khoaHoc))[0];
  }, [ctdtResult]);

  useEffect(() => {
    if (!open) {
      form.resetFields();
      setMaCtdtDaSua(false);
    }
  }, [open, form]);

  // Phản ứng theo cả `nguon` (có thể tải xong SAU khi admin đã gõ Khoá học mới) lẫn giá trị
  // đang gõ — dùng onChange đơn lẻ sẽ bỏ lỡ gợi ý nếu CTĐT nguồn chưa tải xong lúc gõ.
  const khoaHocMoiValue = Form.useWatch("khoaHocMoi", form);
  useEffect(() => {
    if (!maCtdtDaSua && nguon && khoaHocMoiValue) {
      form.setFieldValue("maCtdtMoi", goiYMaCtdt(nguon.maCtdt, khoaHocMoiValue));
    }
  }, [nguon, khoaHocMoiValue, maCtdtDaSua, form]);

  const cloneMutation = useMutation({
    mutationFn: (dto: CloneChuongTrinhDT) => chuongTrinhDTApi.clone(dto),
    onSuccess: (result) => {
      queryClient.invalidateQueries({ queryKey: ["chuong-trinh-dt"] });
      queryClient.invalidateQueries({ queryKey: ["chuong-trinh-dt-all"] });
      if (result.monBoQua.length === 0) {
        message.success(`Đã tạo CTĐT '${result.maCtdtMoi}' và sao chép ${result.soMonDaSaoChep} môn học.`);
      } else {
        modal.warning({
          title: `Đã tạo CTĐT '${result.maCtdtMoi}', sao chép ${result.soMonDaSaoChep} môn học.`,
          content: (
            <>
              <p>{result.monBoQua.length} môn không sao chép được:</p>
              <ul style={{ paddingLeft: 20, margin: 0 }}>
                {result.monBoQua.map((ly, i) => (
                  <li key={i}>{ly}</li>
                ))}
              </ul>
            </>
          ),
        });
      }
      onClose();
    },
    onError: (err: any) => message.error(err?.response?.data?.message || "Nhân bản thất bại"),
  });

  const handleSubmit = async () => {
    const values = await form.validateFields();
    cloneMutation.mutate({ nganhId, ...values });
  };

  return (
    <Modal
      title={`Nhân bản chương trình đào tạo: ${tenNganh}`}
      open={open}
      onCancel={onClose}
      onOk={handleSubmit}
      okText="Nhân bản"
      cancelText="Huỷ"
      confirmLoading={cloneMutation.isPending}
      destroyOnHidden
    >
      {!ctdtLoading && !nguon ? (
        <Alert type="warning" showIcon message="Ngành này chưa có chương trình đào tạo nào để nhân bản." />
      ) : (
        <>
          {nguon && (
            <Alert
              type="info"
              showIcon
              style={{ marginBottom: 16 }}
              message={`Sẽ sao chép môn học từ CTĐT mới nhất hiện có: '${nguon.maCtdt}' (${nguon.khoaHoc}).`}
            />
          )}
          <Form form={form} layout="vertical">
            <Form.Item
              name="khoaHocMoi"
              label="Khoá học mới"
              rules={[
                { required: true, message: "Vui lòng nhập khoá học mới" },
                { pattern: /^\d{4}-\d{4}$/, message: "Định dạng phải là YYYY-YYYY, vd: 2026-2030" },
              ]}
            >
              <Input placeholder="vd: 2026-2030" />
            </Form.Item>
            <Form.Item
              name="maCtdtMoi"
              label="Mã CTĐT mới"
              rules={[{ required: true, message: "Vui lòng nhập mã CTĐT mới" }]}
            >
              <Input onChange={() => setMaCtdtDaSua(true)} />
            </Form.Item>
          </Form>
        </>
      )}
    </Modal>
  );
}

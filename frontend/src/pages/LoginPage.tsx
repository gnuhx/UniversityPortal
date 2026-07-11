import { useEffect } from "react";
import { Button, Card, Checkbox, Form, Input, Typography, App } from "antd";
import { useNavigate } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { login } from "../api/auth";
import { useAuthStore } from "../store/authStore";

const GHI_NHO_TAI_KHOAN_KEY = "university-portal-remembered-username";

interface LoginFormValues {
  tenDangNhap: string;
  matKhau: string;
  ghiNhoTaiKhoan?: boolean;
}

export function LoginPage() {
  const navigate = useNavigate();
  const setAuth = useAuthStore((s) => s.login);
  const { message } = App.useApp();
  const [form] = Form.useForm<LoginFormValues>();

  // Điền sẵn tên đăng nhập đã lưu (nếu có) từ lần đăng nhập trước có tích "Ghi nhớ tài khoản".
  useEffect(() => {
    const saved = localStorage.getItem(GHI_NHO_TAI_KHOAN_KEY);
    if (saved) form.setFieldsValue({ tenDangNhap: saved, ghiNhoTaiKhoan: true });
  }, [form]);

  const loginMutation = useMutation({
    mutationFn: ({ tenDangNhap, matKhau }: LoginFormValues) => login(tenDangNhap, matKhau),
    onSuccess: (result, variables) => {
      if (variables.ghiNhoTaiKhoan) {
        localStorage.setItem(GHI_NHO_TAI_KHOAN_KEY, variables.tenDangNhap);
      } else {
        localStorage.removeItem(GHI_NHO_TAI_KHOAN_KEY);
      }
      setAuth(result.accessToken, result.refreshToken, result.userInfo);
      navigate("/");
    },
    onError: (err: any) => {
      message.error(err?.response?.data?.message || "Đăng nhập thất bại");
    },
  });

  return (
    <div style={{ display: "flex", alignItems: "center", justifyContent: "center", minHeight: "100vh", background: "#f0f2f5" }}>
      <Card style={{ width: 360 }}>
        <Typography.Title level={3} style={{ textAlign: "center" }}>
          University Portal
        </Typography.Title>
        <Form
          form={form}
          layout="vertical"
          onFinish={(v) => loginMutation.mutate(v)}
        >
          <Form.Item name="tenDangNhap" label="Tên đăng nhập" rules={[{ required: true, message: "Vui lòng nhập tên đăng nhập" }]}>
            <Input autoFocus />
          </Form.Item>
          <Form.Item name="matKhau" label="Mật khẩu" rules={[{ required: true, message: "Vui lòng nhập mật khẩu" }]}>
            <Input.Password />
          </Form.Item>
          <Form.Item name="ghiNhoTaiKhoan" valuePropName="checked" initialValue={false} style={{ marginBottom: 12 }}>
            <Checkbox>Ghi nhớ tài khoản</Checkbox>
          </Form.Item>
          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              block
              loading={loginMutation.isPending}
            >
              {loginMutation.isPending ? "Đang đăng nhập..." : "Đăng nhập"}
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
}

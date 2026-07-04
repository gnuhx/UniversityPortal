import { Button, Card, Form, Input, Typography, App } from "antd";
import { useNavigate } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { login } from "../api/auth";
import { useAuthStore } from "../store/authStore";

export function LoginPage() {
  const navigate = useNavigate();
  const setAuth = useAuthStore((s) => s.login);
  const { message } = App.useApp();
  const [form] = Form.useForm();

  const loginMutation = useMutation({
    mutationFn: ({ tenDangNhap, matKhau }: { tenDangNhap: string; matKhau: string }) =>
      login(tenDangNhap, matKhau),
    onSuccess: (result) => {
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

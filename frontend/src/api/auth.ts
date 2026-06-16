import { apiClient } from "./client";
import type { ApiResponse, LoginResponse } from "../types";

export async function login(tenDangNhap: string, matKhau: string) {
  const res = await apiClient.post<ApiResponse<LoginResponse>>("/auth/login", {
    tenDangNhap,
    matKhau,
  });
  return res.data.data;
}

export async function logout() {
  await apiClient.post("/auth/logout");
}

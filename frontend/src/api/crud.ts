import { apiClient } from "./client";
import type { ApiResponse, PagedResult } from "../types";

export function createCrudApi<TDto, TCreate, TUpdate = TCreate>(basePath: string) {
  return {
    async getPaged(params: Record<string, unknown> = {}) {
      const res = await apiClient.get<ApiResponse<PagedResult<TDto>>>(basePath, { params });
      return res.data.data;
    },
    async getAll() {
      const res = await apiClient.get<ApiResponse<TDto[]>>(`${basePath}/all`);
      return res.data.data;
    },
    async getById(id: number) {
      const res = await apiClient.get<ApiResponse<TDto>>(`${basePath}/${id}`);
      return res.data.data;
    },
    async create(dto: TCreate) {
      const res = await apiClient.post<ApiResponse<TDto>>(basePath, dto);
      return res.data.data;
    },
    async update(id: number, dto: TUpdate) {
      const res = await apiClient.put<ApiResponse<TDto>>(`${basePath}/${id}`, dto);
      return res.data.data;
    },
    async remove(id: number) {
      const res = await apiClient.delete<ApiResponse<object>>(`${basePath}/${id}`);
      return res.data;
    },
  };
}

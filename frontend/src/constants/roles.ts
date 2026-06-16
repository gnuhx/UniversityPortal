export const ROLES = {
  ADMIN: "Admin",
  GIAO_VIEN: "Giáo viên",
  SINH_VIEN: "Sinh viên",
  GIAO_VU: "Giáo vụ",
} as const;

export type Role = (typeof ROLES)[keyof typeof ROLES];

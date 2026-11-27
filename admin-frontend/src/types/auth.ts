export type Role = "Admin" | "Manager";

export interface User {
  username: string;
  role: Role;
}

export interface LoginCredentials {
  username: string;
  password: string;
}

export interface CsrfResponse {
  token: string;
}
import type {
  User,
  LoginCredentials,
  CsrfResponse,
} from "../types/auth";

export async function getCsrfToken(): Promise<string> {
  const response = await fetch("/api/auth/csrf", {
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Failed to obtain CSRF token");
  }

  const data: CsrfResponse = await response.json();

  return data.token;
}

export async function login(
  credentials: LoginCredentials
): Promise<User> {
  const token = await getCsrfToken();

  const response = await fetch("/api/auth/login", {
    method: "POST",
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      "X-CSRF-TOKEN": token,
    },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    throw new Error("Login failed");
  }

  return response.json();
}

export async function logout(): Promise<void> {
  const token = await getCsrfToken();

  const response = await fetch("/api/auth/logout", {
    method: "POST",
    credentials: "include",
    headers: {
      "X-CSRF-TOKEN": token,
    },
  });

  if (!response.ok) {
    throw new Error("Logout failed");
  }
}

export async function getCurrentUser(): Promise<User | null> {
  const response = await fetch("/api/auth/me", {
    credentials: "include",
  });

  if (response.status === 401) {
    return null;
  }

  if (!response.ok) {
    throw new Error("Failed to retrieve current user");
  }

  return response.json();
}
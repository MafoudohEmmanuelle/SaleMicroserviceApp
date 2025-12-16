// src/services/auth.js

const USERS_BASE = import.meta.env.VITE_USERS_API_URL || "http://localhost:5001/api";

export const login = async (username, password) => {
  const res = await fetch(`${USERS_BASE}/users/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });

  if (!res.ok) throw new Error("Login failed");

  const data = await res.json();
  const token = data.token || data.Token;
  if (!token) throw new Error("No token returned from backend");

  localStorage.setItem("token", token);
  localStorage.setItem("username", data.username);
  localStorage.setItem("userId", data.userId);

  // decode token (safe)
  const payload = JSON.parse(atob(token.split(".")[1]));

  let role =
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
    payload["role"] ||
    payload["roles"];

  if (!role) throw new Error("Role claim not found in token");
  if (Array.isArray(role)) role = role[0];

  localStorage.setItem("role", role);
  return role;
};

export const getToken = () => localStorage.getItem("token");

// ✅ Add logout
export const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("username");
  localStorage.removeItem("userId");
  localStorage.removeItem("role");

  // optional: redirect to login page
  window.location.href = "/";
};
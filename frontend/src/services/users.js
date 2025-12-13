// src/services/users.js
import { getToken } from "./auth";

const BASE_URL = import.meta.env.VITE_USERS_API_URL;

// Register a new user
export async function registerUser(username, password, role) {
  const roleMap = { Admin: 0, Employee: 1 };

  const res = await fetch(`${BASE_URL}/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password, role: roleMap[role] }),
  });

  const data = await res.json().catch(() => ({}));
  if (!res.ok) throw new Error(data?.message || "Failed to register user");
  return data;
}

// Get all users
export async function getAllUsers() {
  const res = await fetch(BASE_URL, {
    headers: { Authorization: `Bearer ${getToken()}` },
  });
  return res.json();
}

// Delete a user
export async function deleteUser(id) {
  const res = await fetch(`${BASE_URL}/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${getToken()}` },
  });
  if (!res.ok) throw new Error("Failed to delete user");
  return true;
}

// Update a user
export async function updateUser(id, user) {
  const res = await fetch(`${BASE_URL}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${getToken()}`,
    },
    body: JSON.stringify(user),
  });
  return res.json();
}
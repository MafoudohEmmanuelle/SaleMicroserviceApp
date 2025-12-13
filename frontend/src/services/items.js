import { getToken } from "./auth";

const BASE_URL = import.meta.env.VITE_ITEMS_API_URL;

// Get all products
export const getAllProducts = async () => {
  const res = await fetch(BASE_URL, {
    headers: { Authorization: `Bearer ${getToken()}` }
  });
  return res.json();
};

// Add a product
export const addProduct = async (product) => {
  const res = await fetch(BASE_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${getToken()}`
    },
    body: JSON.stringify(product)
  });
  return res.json();
};

// Delete a product
export const deleteProduct = async (id) => {
  const res = await fetch(`${BASE_URL}/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${getToken()}` }
  });
  if (!res.ok) {
    throw new Error("Failed to delete product");
  }
  return true;
};

// Update a product
export const updateProduct = async (id, product) => {
  const res = await fetch(`${BASE_URL}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${getToken()}`
    },
    body: JSON.stringify(product)
  });
  return res.json();
};
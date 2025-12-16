import { getToken } from "./auth";

const BASE_URL = import.meta.env.VITE_SALES_API_URL || "http://localhost:5003/api";

export const createSale = async (sale) => {
  const token = getToken();
  if (!token) throw new Error("You must be logged in to perform a sale.");

  const payload = {
    customerName: sale.customerName,
    items: sale.items.map(i => ({
      productId: i.productId,
      quantity: i.quantity
    }))
  };

  console.log("Payload sent to backend:", payload);

  const res = await fetch(`${BASE_URL}/sales`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(payload)
  });

  if (!res.ok) {
    const errorText = await res.text();
    if (res.status === 401) throw new Error("Unauthorized. Please login again.");
    throw new Error(errorText || `Error ${res.status}`);
  }

  return res.json();
};

// ✅ Get all sales
export const getSales = async () => {
  const token = getToken();
  if (!token) throw new Error("You must be logged in to view sales.");

  const res = await fetch(`${BASE_URL}/sales`, {
    headers: { Authorization: `Bearer ${token}` }
  });

  if (!res.ok) {
    const errorText = await res.text();
    throw new Error(errorText || `Error ${res.status}`);
  }

  return res.json();
};

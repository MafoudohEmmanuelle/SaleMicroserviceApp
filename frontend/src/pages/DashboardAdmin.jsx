import React, { useState, useEffect } from "react";
import { logout } from "../services/auth";
import { useNavigate } from "react-router-dom"; 
import {
  registerUser,
  getAllUsers,
  deleteUser,
  updateUser,
} from "../services/users";
import {
  getAllProducts,
  addProduct,
  deleteProduct,
  updateProduct,
} from "../services/items";
import { getSales } from "../services/sales";
import "../styles/dashboard.css";

export default function DashboardAdmin() {
  const [users, setUsers] = useState([]);
  const [products, setProducts] = useState([]);
  const [sales, setSales] = useState([]);

  // Search states
  const [userSearch, setUserSearch] = useState("");
  const [productSearch, setProductSearch] = useState("");
  const [salesSearch, setSalesSearch] = useState("");

  // User form state
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("Employee");

  // Product form state
  const [name, setName] = useState("");
  const [desc, setDesc] = useState("");
  const [price, setPrice] = useState("");
  const [quantity, setQuantity] = useState("");

  // Edit modal states
  const [editingUser, setEditingUser] = useState(null);
  const [editingProduct, setEditingProduct] = useState(null);

  const navigate = useNavigate();

  // Fetch initial data
  const fetchData = async () => {
    try {
      const usersData = await getAllUsers();
      setUsers(Array.isArray(usersData) ? usersData : []);

      const productsData = await getAllProducts();
      setProducts(Array.isArray(productsData) ? productsData : []);

      const salesData = await getSales();
      setSales(Array.isArray(salesData) ? salesData : []);
    } catch (err) {
      console.error("Failed to load data", err);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  // Add user
  const handleAddUser = async (e) => {
    e.preventDefault();
    try {
      const newUser = await registerUser(username, password, role);
      setUsers((prev) => [...prev, newUser]);
      setUsername("");
      setPassword("");
      setRole("Employee");
    } catch (err) {
      console.error("Failed to add user", err);
    }
  };

  // Delete user
  const handleDeleteUser = async (id) => {
    if (!window.confirm("Are you sure you want to delete this user?")) return;
    try {
      await deleteUser(id);
      setUsers((prev) => prev.filter((u) => u.id !== id));
    } catch (err) {
      console.error("Failed to delete user", err);
    }
  };

  // Add product
  const handleAddProduct = async (e) => {
    e.preventDefault();
    try {
      const newProduct = await addProduct({
        name,
        description: desc,
        price: parseFloat(price),
        quantity: parseInt(quantity, 10),
      });
      setProducts((prev) => [...prev, newProduct]);
      setName("");
      setDesc("");
      setPrice("");
      setQuantity("");
    } catch (err) {
      console.error("Failed to add product", err);
    }
  };

  // Delete product
  const handleDeleteProduct = async (id) => {
    if (!window.confirm("Are you sure you want to delete this product?")) return;
    try {
      await deleteProduct(id);
      setProducts((prev) => prev.filter((p) => p.id !== id));
    } catch (err) {
      console.error("Failed to delete product", err);
    }
  };

  // Save edited user
  const handleSaveUser = async (e) => {
    e.preventDefault();
    try {
      await updateUser(editingUser.id, {
        username: editingUser.username,
        role: editingUser.role,
      });
      setUsers((prev) =>
        prev.map((u) => (u.id === editingUser.id ? editingUser : u))
      );
      setEditingUser(null);
    } catch (err) {
      console.error("Failed to update user", err);
    }
  };

  // Save edited product
  const handleSaveProduct = async (e) => {
    e.preventDefault();
    try {
      await updateProduct(editingProduct.id, {
        name: editingProduct.name,
        description: editingProduct.description,
        price: parseFloat(editingProduct.price),
        quantity: parseInt(editingProduct.quantity, 10),
      });
      setProducts((prev) =>
        prev.map((p) => (p.id === editingProduct.id ? editingProduct : p))
      );
      setEditingProduct(null);
    } catch (err) {
      console.error("Failed to update product", err);
    }
  };

  // Logout
  const handleLogout = () => {
    logout();
    navigate("/");
  };

  // Filtered lists
  const filteredUsers = users.filter((u) =>
    u.username?.toLowerCase().includes(userSearch.toLowerCase())
  );
  const filteredProducts = products.filter((p) =>
    p.name?.toLowerCase().includes(productSearch.toLowerCase())
  );
  const filteredSales = sales.filter((s) =>
    s.customerName?.toLowerCase().includes(salesSearch.toLowerCase())
  );

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <div className="header-left">
          <h2>Admin Dashboard</h2>
          <p className="subtitle">Manage users, products & sales</p>
        </div>
        <button className="logout-btn" onClick={handleLogout}>
          Logout
        </button>
      </header>

      {/* Users Section */}
      <section className="dashboard-section">
        <div className="section-top">
          <h3>Users</h3>
          <input
            type="text"
            placeholder="Search users..."
            value={userSearch}
            onChange={(e) => setUserSearch(e.target.value)}
            className="search-bar"
          />
        </div>
        <div className="dashboard-table-container">
          <table className="dashboard-table">
            <thead>
              <tr>
                <th>Username</th>
                <th>Role</th>
                <th className="actions-col">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredUsers.map((u) => (
                <tr key={u.id}>
                  <td>{u.username}</td>
                  <td>{u.role}</td>
                  <td className="actions-col">
                    <button
                      className="action-btn edit"
                      onClick={() => setEditingUser({ ...u })}
                    >
                      Edit
                    </button>
                    <button
                      className="action-btn delete"
                      onClick={() => handleDeleteUser(u.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
              {filteredUsers.length === 0 && (
                <tr>
                  <td colSpan={3} className="empty-row">
                    No users found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
        <form className="dashboard-form" onSubmit={handleAddUser}>
          <input
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
          <input
            placeholder="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <select value={role} onChange={(e) => setRole(e.target.value)}>
            <option value="Admin">Admin</option>
            <option value="Employee">Employee</option>
          </select>
          <button type="submit">Add User</button>
        </form>
      </section>

      {/* Products Section */}
      <section className="dashboard-section">
        <div className="section-top">
          <h3>Products</h3>
          <input
            type="text"
            placeholder="Search products..."
            value={productSearch}
            onChange={(e) => setProductSearch(e.target.value)}
            className="search-bar"
          />
        </div>
        <div className="dashboard-table-container">
          <table className="dashboard-table">
            <thead>
              <tr>
                <th>Name</th>
                <th className="desc-col">Description</th>
                <th>Price</th>
                <th>Quantity in stock</th>
                <th className="actions-col">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredProducts.map((p) => (
                <tr key={p.id}>
                  <td>{p.name}</td>
                  <td className="desc-col">{p.description}</td>
                  <td>{p.price}</td>
                  <td>{p.quantity}</td>
                  <td className="actions-col">
                    <button
                      className="action-btn edit"
                      onClick={() => setEditingProduct({ ...p })}
                    >
                      Edit
                    </button>
                    <button
                      className="action-btn delete"
                      onClick={() => handleDeleteProduct(p.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
              {filteredProducts.length === 0 && (
                <tr>
                  <td colSpan={5} className="empty-row">
                    No products found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
        <form className="dashboard-form" onSubmit={handleAddProduct}>
          <input
            placeholder="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
          <input
            placeholder="Description"
            value={desc}
            onChange={(e) => setDesc(e.target.value)}
            required
          />
          <input
            placeholder="Price"
            type="number"
            step="0.01"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            required
          />
          <input
            placeholder="Quantity"
            type="number"
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
            required
          />
          <button type="submit">Add Product</button>
        </form>
      </section>

      {/* Sales Section */}
      <section className="dashboard-section">
        <div className="section-top">
          <h3>Sales</h3>
          <input
            type="text"
            placeholder="Search by customer..."
            value={salesSearch}
            onChange={(e) => setSalesSearch(e.target.value)}
            className="search-bar"
          />
        </div>
        <div className="dashboard-table-container">
          <table className="dashboard-table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Customer</th>
                <th>User</th>
                <th>Total Amount</th>
                <th>Items</th>
              </tr>
            </thead>
            <tbody>
              {filteredSales.map((s) => (
                <tr key={s.id}>
                  <td>{new Date(s.date).toLocaleString()}</td>
                  <td>{s.customerName}</td>
                  <td>{s.userName}</td>
                  <td>{s.totalAmount.toLocaleString()} FCFA</td>
                  <td>
                    {s.items.map((i) => (
                      <div key={i.id}>
                        {i.productName} (x{i.quantity})
                      </div>
                    ))}
                  </td>
                </tr>
              ))}
              {filteredSales.length === 0 && (
                <tr>
                  <td colSpan={5} className="empty-row">
                    No sales found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}

import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { logout } from "../services/auth";
import { getAllProducts } from "../services/items";
import { createSale } from "../services/sales";
import "../styles/employee.css";

export default function DashboardEmployee() {
  const navigate = useNavigate();
  const [products, setProducts] = useState([]);
  const [customer, setCustomer] = useState("");
  const [saleItems, setSaleItems] = useState([]);
  const [msg, setMsg] = useState("");

  // Load products
  useEffect(() => {
    getAllProducts()
      .then(setProducts)
      .catch((err) => setMsg("Failed to load products: " + err.message));
  }, []);

  // Add a new empty sale item
  const handleAddSaleItem = () => {
    setSaleItems([...saleItems, { productName: "", quantity: 0 }]);
  };

  // Handle changes in product name or quantity
  const handleChangeItem = (index, field, value) => {
    const updated = [...saleItems];

    if (field === "quantity") {
      const product = products.find(
        (p) =>
          p.name.toLowerCase() === updated[index].productName.toLowerCase()
      );
      const maxQty = product?.quantity || 0;
      if (value > maxQty) value = maxQty; // cap quantity to stock
      if (value < 0) value = 0;
    }

    updated[index][field] = value;
    setSaleItems(updated);
  };

  // Calculate subtotal for a single item
  const calculateSubtotal = (item) => {
    const product = products.find(
      (p) => p.name.toLowerCase() === item.productName.toLowerCase()
    );
    if (!product) return 0;
    return product.price * (item.quantity || 0);
  };

  // Calculate total sale amount
  const calculateTotal = () => {
    return saleItems.reduce((sum, item) => sum + calculateSubtotal(item), 0);
  };

  // Check if sale can be completed (all quantities > 0 and <= stock)
  const canCompleteSale = saleItems.length > 0 && saleItems.every((item) => {
    const product = products.find(
      (p) => p.name.toLowerCase() === item.productName.toLowerCase()
    );
    return product && item.quantity > 0 && item.quantity <= product.quantity;
  });

  // Handle sale submission
  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!customer.trim()) {
      setMsg("Customer name is required.");
      return;
    }

    if (!saleItems.length || saleItems.every((i) => i.quantity <= 0)) {
      setMsg("Please add at least one product with quantity > 0.");
      return;
    }

    try {
      const items = saleItems
        .map((i) => {
          const product = products.find(
            (p) => p.name.toLowerCase() === i.productName.toLowerCase()
          );
          if (!product) return null;
          return { productId: product.id, quantity: i.quantity };
        })
        .filter(Boolean);

      await createSale({ customerName: customer, items });

      setMsg("Sale completed successfully!");
      setCustomer("");
      setSaleItems([]);

      // Refresh products to get updated stock
      const updatedProducts = await getAllProducts();
      setProducts(Array.isArray(updatedProducts) ? updatedProducts : []);

    } catch (err) {
      setMsg("Failed to complete sale: " + err.message);
    }
  };

  // Logout
  const handleLogout = () => {
    logout();
    navigate("/"); // back to login
  };

  return (
    <div className="employee-dashboard">
      <header className="employee-header">
        <h2>Employee Dashboard</h2>
        <button className="logout-btn" onClick={handleLogout}>
          Logout
        </button>
      </header>

      {/* Product List */}
      <section className="product-section">
        <h3>Available Products</h3>
        <div className="product-table-container">
          <table className="product-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Description</th>
                <th>Price (FCFA)</th>
                <th>Quantity in stock</th>
              </tr>
            </thead>
            <tbody>
              {products.map((p) => (
                <tr key={p.id}>
                  <td>{p.name}</td>
                  <td>{p.description}</td>
                  <td>{p.price.toLocaleString()} FCFA</td>
                  <td>{p.quantity}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      {/* Sale Form */}
      <section className="sale-section">
        <h3>Perform Sale</h3>
        {msg && <p className="info-msg">{msg}</p>}

        <form onSubmit={handleSubmit} className="sale-form">
          <label>
            Customer Name
            <input
              type="text"
              placeholder="Enter customer name"
              value={customer}
              onChange={(e) => setCustomer(e.target.value)}
              required
            />
          </label>

          <div className="sale-items">
            {saleItems.map((item, index) => {
              const product = products.find(
                (p) => p.name.toLowerCase() === item.productName.toLowerCase()
              );
              const maxQty = product?.quantity || 0;

              return (
                <div key={index} className="sale-row">
                  <input
                    type="text"
                    placeholder="Product Name"
                    value={item.productName}
                    onChange={(e) =>
                      handleChangeItem(index, "productName", e.target.value)
                    }
                    list="product-options"
                  />
                  <datalist id="product-options">
                    {products.map((p) => (
                      <option key={p.id} value={p.name} />
                    ))}
                  </datalist>

                  <input
                    type="number"
                    min="0"
                    max={maxQty}
                    placeholder="Qty"
                    value={item.quantity}
                    onChange={(e) =>
                      handleChangeItem(
                        index,
                        "quantity",
                        parseInt(e.target.value) || 0
                      )
                    }
                  />
                  <span className="subtotal">
                    {calculateSubtotal(item).toLocaleString()} FCFA
                  </span>
                  <span className="stock-info">{maxQty} in stock</span>
                </div>
              );
            })}
          </div>

          <button
            type="button"
            className="add-item-btn"
            onClick={handleAddSaleItem}
          >
            + Add Product
          </button>

          <div className="total-row">
            <strong>Total: {calculateTotal().toLocaleString()} FCFA</strong>
          </div>

          <button
            type="submit"
            className="complete-btn"
            disabled={!canCompleteSale}
          >
            Complete Sale
          </button>
        </form>
      </section>
    </div>
  );
}

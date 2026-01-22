"use client";

import { useEffect, useState } from "react";
import { getCart, updateCartItem, deleteCartItem, Cart, CartItem } from "@/lib/api";

export default function CartPage() {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadCart();
  }, []);

  const loadCart = async () => {
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        setError("Please log in to view your cart");
        setLoading(false);
        return;
      }
      const cartData = await getCart(token);
      setCart(cartData);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load cart");
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateQuantity = async (itemId: string, newQuantity: number) => {
    if (newQuantity < 1) return;
    try {
      const token = localStorage.getItem("authToken");
      if (!token) return;
      await updateCartItem(token, itemId, { quantity: newQuantity });
      await loadCart();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update quantity");
    }
  };

  const handleToggleSelect = async (itemId: string, selected: boolean) => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) return;
      await updateCartItem(token, itemId, { selected });
      await loadCart();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update selection");
    }
  };

  const handleDelete = async (itemId: string) => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) return;
      await deleteCartItem(token, itemId);
      await loadCart();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to delete item");
    }
  };

  if (loading) {
    return (
      <div style={{ padding: 24 }} data-testid="cart-loading">
        Loading cart...
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: 24, color: "red" }} data-testid="cart-error">
        Error: {error}
      </div>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div style={{ padding: 24 }} data-testid="cart-empty">
        <h1>Your Cart</h1>
        <p>Your cart is empty</p>
        <a
          href="/"
          style={{ color: "#1976d2", textDecoration: "underline", cursor: "pointer" }}
        >
          Back to Home
        </a>
      </div>
    );
  }

  return (
    <main style={{ padding: 24, fontFamily: "system-ui, sans-serif" }} data-testid="cart-page">
      <h1 data-testid="cart-title">Your Cart</h1>
      <div style={{ marginTop: 24 }}>
        <table
          data-testid="cart-table"
          style={{
            width: "100%",
            borderCollapse: "collapse",
            border: "1px solid #ddd",
          }}
        >
          <thead>
            <tr style={{ backgroundColor: "#f5f5f5" }}>
              <th style={{ padding: 12, textAlign: "left", border: "1px solid #ddd" }}>Select</th>
              <th style={{ padding: 12, textAlign: "left", border: "1px solid #ddd" }}>Product</th>
              <th style={{ padding: 12, textAlign: "left", border: "1px solid #ddd" }}>Quantity</th>
              <th style={{ padding: 12, textAlign: "right", border: "1px solid #ddd" }}>Unit Price</th>
              <th style={{ padding: 12, textAlign: "right", border: "1px solid #ddd" }}>Line Total</th>
              <th style={{ padding: 12, textAlign: "center", border: "1px solid #ddd" }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {cart.items.map((item: CartItem) => (
              <tr key={item.id} data-testid={`cart-item-${item.id}`}>
                <td style={{ padding: 12, border: "1px solid #ddd" }}>
                  <input
                    type="checkbox"
                    checked={item.selected}
                    onChange={(e) => handleToggleSelect(item.id, e.target.checked)}
                    data-testid={`cart-item-select-${item.id}`}
                  />
                </td>
                <td style={{ padding: 12, border: "1px solid #ddd" }}>
                  <div data-testid={`cart-item-name-${item.id}`}>{item.productName}</div>
                  {item.variantName && (
                    <div style={{ fontSize: 12, color: "#666" }}>{item.variantName}</div>
                  )}
                </td>
                <td style={{ padding: 12, border: "1px solid #ddd" }}>
                  <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                    <button
                      onClick={() => handleUpdateQuantity(item.id, item.quantity - 1)}
                      disabled={item.quantity <= 1}
                      data-testid={`cart-item-decrease-${item.id}`}
                      style={{
                        padding: "4px 12px",
                        cursor: item.quantity <= 1 ? "not-allowed" : "pointer",
                      }}
                    >
                      -
                    </button>
                    <span data-testid={`cart-item-quantity-${item.id}`}>{item.quantity}</span>
                    <button
                      onClick={() => handleUpdateQuantity(item.id, item.quantity + 1)}
                      data-testid={`cart-item-increase-${item.id}`}
                      style={{ padding: "4px 12px", cursor: "pointer" }}
                    >
                      +
                    </button>
                  </div>
                </td>
                <td
                  style={{ padding: 12, textAlign: "right", border: "1px solid #ddd" }}
                  data-testid={`cart-item-price-${item.id}`}
                >
                  ${item.price.toFixed(2)}
                </td>
                <td
                  style={{ padding: 12, textAlign: "right", border: "1px solid #ddd" }}
                  data-testid={`cart-item-linetotal-${item.id}`}
                >
                  ${item.lineTotal.toFixed(2)}
                </td>
                <td style={{ padding: 12, textAlign: "center", border: "1px solid #ddd" }}>
                  <button
                    onClick={() => handleDelete(item.id)}
                    data-testid={`cart-item-delete-${item.id}`}
                    style={{
                      padding: "4px 12px",
                      backgroundColor: "#dc3545",
                      color: "white",
                      border: "none",
                      borderRadius: 4,
                      cursor: "pointer",
                    }}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        <div
          style={{
            marginTop: 24,
            padding: 16,
            backgroundColor: "#f5f5f5",
            border: "1px solid #ddd",
            borderRadius: 4,
          }}
        >
          <div style={{ display: "flex", justifyContent: "space-between", fontSize: 18 }}>
            <strong data-testid="cart-subtotal-label">Subtotal (Selected Items):</strong>
            <strong data-testid="cart-subtotal">${cart.subtotal.toFixed(2)}</strong>
          </div>
        </div>

        <div style={{ marginTop: 24 }}>
          <a
            href="/"
            style={{
              color: "#1976d2",
              textDecoration: "underline",
              cursor: "pointer",
              marginRight: 16,
            }}
          >
            Continue Shopping
          </a>
        </div>
      </div>
    </main>
  );
}

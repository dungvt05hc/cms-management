"use client";

import { useEffect, useState } from "react";
import { getCart, updateCartItem, deleteCartItem, applyVoucher, Cart, CartItem, CheckoutTotals } from "@/lib/api";

export default function CartPage() {
  const [cart, setCart] = useState<Cart | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [discountCode, setDiscountCode] = useState("");
  const [shippingCode, setShippingCode] = useState("");
  const [totals, setTotals] = useState<CheckoutTotals | null>(null);
  const [voucherError, setVoucherError] = useState<string | null>(null);
  const [applyingVoucher, setApplyingVoucher] = useState(false);

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
      // Reset totals when cart changes
      setTotals(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to delete item");
    }
  };

  const handleApplyVoucher = async () => {
    setApplyingVoucher(true);
    setVoucherError(null);
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        setVoucherError("Please log in to apply vouchers");
        return;
      }
      const result = await applyVoucher(
        token,
        discountCode || undefined,
        shippingCode || undefined
      );
      setTotals(result);
    } catch (err) {
      setVoucherError(err instanceof Error ? err.message : "Failed to apply voucher");
    } finally {
      setApplyingVoucher(false);
    }
  };

  const handleClearVouchers = () => {
    setDiscountCode("");
    setShippingCode("");
    setTotals(null);
    setVoucherError(null);
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

        {/* Voucher Application Section */}
        <div
          style={{
            marginTop: 24,
            padding: 16,
            backgroundColor: "#fff",
            border: "1px solid #ddd",
            borderRadius: 4,
          }}
          data-testid="voucher-section"
        >
          <h3 style={{ marginTop: 0, marginBottom: 16 }}>Apply Vouchers</h3>
          
          <div style={{ marginBottom: 12 }}>
            <label style={{ display: "block", marginBottom: 4, fontSize: 14 }}>
              Discount Code:
            </label>
            <input
              type="text"
              value={discountCode}
              onChange={(e) => setDiscountCode(e.target.value)}
              placeholder="Enter discount code"
              data-testid="discount-code-input"
              style={{
                padding: "8px 12px",
                border: "1px solid #ddd",
                borderRadius: 4,
                width: "100%",
                maxWidth: 300,
              }}
            />
          </div>

          <div style={{ marginBottom: 12 }}>
            <label style={{ display: "block", marginBottom: 4, fontSize: 14 }}>
              Shipping Code:
            </label>
            <input
              type="text"
              value={shippingCode}
              onChange={(e) => setShippingCode(e.target.value)}
              placeholder="Enter shipping code"
              data-testid="shipping-code-input"
              style={{
                padding: "8px 12px",
                border: "1px solid #ddd",
                borderRadius: 4,
                width: "100%",
                maxWidth: 300,
              }}
            />
          </div>

          <div style={{ display: "flex", gap: 12, marginBottom: 12 }}>
            <button
              onClick={handleApplyVoucher}
              disabled={applyingVoucher || (!discountCode && !shippingCode)}
              data-testid="apply-voucher-button"
              style={{
                padding: "8px 16px",
                backgroundColor: "#28a745",
                color: "white",
                border: "none",
                borderRadius: 4,
                cursor: applyingVoucher || (!discountCode && !shippingCode) ? "not-allowed" : "pointer",
                opacity: applyingVoucher || (!discountCode && !shippingCode) ? 0.6 : 1,
              }}
            >
              {applyingVoucher ? "Applying..." : "Apply Vouchers"}
            </button>

            {totals && (
              <button
                onClick={handleClearVouchers}
                data-testid="clear-voucher-button"
                style={{
                  padding: "8px 16px",
                  backgroundColor: "#6c757d",
                  color: "white",
                  border: "none",
                  borderRadius: 4,
                  cursor: "pointer",
                }}
              >
                Clear Vouchers
              </button>
            )}
          </div>

          {voucherError && (
            <div
              style={{ color: "red", fontSize: 14, marginTop: 8 }}
              data-testid="voucher-error"
            >
              {voucherError}
            </div>
          )}
        </div>

        {/* Totals Section */}
        <div
          style={{
            marginTop: 24,
            padding: 16,
            backgroundColor: "#f5f5f5",
            border: "1px solid #ddd",
            borderRadius: 4,
          }}
          data-testid="totals-section"
        >
          {totals ? (
            <>
              <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 8 }}>
                <span data-testid="totals-subtotal-label">Subtotal (Selected):</span>
                <span data-testid="totals-subtotal">${totals.subtotal.toFixed(2)}</span>
              </div>
              
              {totals.discountAmount > 0 && (
                <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 8, color: "#28a745" }}>
                  <span data-testid="totals-discount-label">
                    Discount ({totals.discountVoucherCode}):
                  </span>
                  <span data-testid="totals-discount">-${totals.discountAmount.toFixed(2)}</span>
                </div>
              )}

              <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 8 }}>
                <span data-testid="totals-shipping-label">Shipping Fee:</span>
                <span data-testid="totals-shipping">${totals.shippingFee.toFixed(2)}</span>
              </div>

              {totals.shippingDiscount > 0 && (
                <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 8, color: "#28a745" }}>
                  <span data-testid="totals-shipping-discount-label">
                    Shipping Discount ({totals.shippingVoucherCode}):
                  </span>
                  <span data-testid="totals-shipping-discount">-${totals.shippingDiscount.toFixed(2)}</span>
                </div>
              )}

              <div style={{ borderTop: "2px solid #ddd", marginTop: 12, paddingTop: 12 }}>
                <div style={{ display: "flex", justifyContent: "space-between", fontSize: 18 }}>
                  <strong data-testid="totals-total-label">Total:</strong>
                  <strong data-testid="totals-total">${totals.total.toFixed(2)}</strong>
                </div>
              </div>
            </>
          ) : (
            <div style={{ display: "flex", justifyContent: "space-between", fontSize: 18 }}>
              <strong data-testid="cart-subtotal-label">Subtotal (Selected Items):</strong>
              <strong data-testid="cart-subtotal">${cart.subtotal.toFixed(2)}</strong>
            </div>
          )}
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

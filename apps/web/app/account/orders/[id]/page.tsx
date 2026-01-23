"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import { getOrderById, cancelOrder, confirmOrderReceived, reorderOrder, Order, OrderStatus } from "@/lib/api";
import Link from "next/link";

export default function OrderDetailPage() {
  const params = useParams();
  const router = useRouter();
  const orderId = params.id as string;

  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [actionLoading, setActionLoading] = useState(false);

  useEffect(() => {
    fetchOrder();
  }, [orderId]);

  const fetchOrder = async () => {
    try {
      setLoading(true);
      setError(null);
      const token = localStorage.getItem("authToken");
      if (!token) {
        setError("Not authenticated");
        setLoading(false);
        return;
      }

      const result = await getOrderById(token, orderId);
      setOrder(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load order");
    } finally {
      setLoading(false);
    }
  };

  const handleCancelOrder = async () => {
    if (!order || actionLoading) return;
    if (!confirm("Are you sure you want to cancel this order?")) return;

    try {
      setActionLoading(true);
      const token = localStorage.getItem("authToken");
      if (!token) {
        alert("Not authenticated");
        return;
      }

      await cancelOrder(token, orderId, "CUSTOMER_REQUEST", "Cancelled by customer");
      alert("Order cancelled successfully");
      router.push("/account/orders");
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to cancel order");
    } finally {
      setActionLoading(false);
    }
  };

  const handleConfirmReceived = async () => {
    if (!order || actionLoading) return;
    if (!confirm("Confirm that you have received this order?")) return;

    try {
      setActionLoading(true);
      const token = localStorage.getItem("authToken");
      if (!token) {
        alert("Not authenticated");
        return;
      }

      await confirmOrderReceived(token, orderId);
      alert("Order confirmed successfully");
      await fetchOrder();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to confirm order");
    } finally {
      setActionLoading(false);
    }
  };

  const handleReorder = async () => {
    if (!order || actionLoading) return;

    try {
      setActionLoading(true);
      const token = localStorage.getItem("authToken");
      if (!token) {
        alert("Not authenticated");
        return;
      }

      await reorderOrder(token, orderId);
      alert("Items added to cart successfully");
      router.push("/cart");
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to reorder");
    } finally {
      setActionLoading(false);
    }
  };

  const getStatusLabel = (status: OrderStatus): string => {
    switch (status) {
      case OrderStatus.Processing:
        return "Processing";
      case OrderStatus.Shipping:
        return "Shipping";
      case OrderStatus.Delivered:
        return "Delivered";
      case OrderStatus.Cancelled:
        return "Cancelled";
      default:
        return "Unknown";
    }
  };

  const getStatusColor = (status: OrderStatus): string => {
    switch (status) {
      case OrderStatus.Processing:
        return "bg-blue-100 text-blue-800";
      case OrderStatus.Shipping:
        return "bg-yellow-100 text-yellow-800";
      case OrderStatus.Delivered:
        return "bg-green-100 text-green-800";
      case OrderStatus.Cancelled:
        return "bg-red-100 text-red-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  };

  const formatDate = (dateString: string): string => {
    return new Date(dateString).toLocaleString("vi-VN", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div data-testid="order-loading" className="text-center">
          Loading order details...
        </div>
      </div>
    );
  }

  if (error || !order) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div data-testid="order-error" className="text-center text-red-600">
          {error || "Order not found"}
        </div>
        <div className="text-center mt-4">
          <Link href="/account/orders" className="text-blue-500 hover:underline">
            Back to Orders
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8" data-testid="order-detail-page">
      <div className="mb-4">
        <Link href="/account/orders" className="text-blue-500 hover:underline">
          ← Back to Orders
        </Link>
      </div>

      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="flex justify-between items-start mb-6">
          <div>
            <h1 className="text-2xl font-bold">Order #{order.id.substring(0, 8)}</h1>
            <div className="text-sm text-gray-600 mt-1">{formatDate(order.createdAt)}</div>
          </div>
          <span className={`px-4 py-2 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
            {getStatusLabel(order.status)}
          </span>
        </div>

        {/* Order Items */}
        <div className="mb-6">
          <h2 className="text-lg font-semibold mb-3">Order Items</h2>
          <div className="space-y-3" data-testid="order-items">
            {order.items.map((item) => (
              <div key={item.id} className="flex justify-between items-center border-b pb-3" data-testid={`item-${item.id}`}>
                <div>
                  <div className="font-medium">{item.productName}</div>
                  {item.variantName && <div className="text-sm text-gray-600">{item.variantName}</div>}
                  <div className="text-sm text-gray-500">SKU: {item.sku}</div>
                  <div className="text-sm">
                    {formatCurrency(item.unitPrice)} x {item.quantity}
                  </div>
                </div>
                <div className="font-semibold">{formatCurrency(item.totalPrice)}</div>
              </div>
            ))}
          </div>
        </div>

        {/* Shipping Address */}
        <div className="mb-6">
          <h2 className="text-lg font-semibold mb-3">Shipping Address</h2>
          <div className="text-sm" data-testid="shipping-address">
            <div className="font-medium">{order.shippingFullName}</div>
            <div>{order.shippingPhone}</div>
            <div>{order.shippingAddressLine}</div>
            <div>{order.shippingWard}, {order.shippingDistrict}</div>
            <div>{order.shippingCity}</div>
          </div>
        </div>

        {/* Order Summary */}
        <div className="mb-6">
          <h2 className="text-lg font-semibold mb-3">Order Summary</h2>
          <div className="space-y-2" data-testid="order-summary">
            <div className="flex justify-between">
              <span>Subtotal:</span>
              <span>{formatCurrency(order.subtotal)}</span>
            </div>
            {order.discountAmount > 0 && (
              <div className="flex justify-between text-green-600">
                <span>Discount:</span>
                <span>-{formatCurrency(order.discountAmount)}</span>
              </div>
            )}
            <div className="flex justify-between">
              <span>Shipping Fee:</span>
              <span>{formatCurrency(order.shippingFee)}</span>
            </div>
            {order.shippingDiscount > 0 && (
              <div className="flex justify-between text-green-600">
                <span>Shipping Discount:</span>
                <span>-{formatCurrency(order.shippingDiscount)}</span>
              </div>
            )}
            <div className="flex justify-between text-xl font-bold border-t pt-2">
              <span>Total:</span>
              <span data-testid="order-total">{formatCurrency(order.total)}</span>
            </div>
          </div>
        </div>

        {/* Action Buttons */}
        <div className="flex gap-3">
          {order.status === OrderStatus.Processing && (
            <button
              onClick={handleCancelOrder}
              disabled={actionLoading}
              className="px-6 py-2 bg-red-500 text-white rounded hover:bg-red-600 disabled:bg-gray-400"
              data-testid="cancel-order-btn"
            >
              {actionLoading ? "Processing..." : "Cancel Order"}
            </button>
          )}
          {order.status === OrderStatus.Delivered && (
            <button
              onClick={handleConfirmReceived}
              disabled={actionLoading}
              className="px-6 py-2 bg-green-500 text-white rounded hover:bg-green-600 disabled:bg-gray-400"
              data-testid="confirm-received-btn"
            >
              {actionLoading ? "Processing..." : "Confirm Received"}
            </button>
          )}
          {(order.status === OrderStatus.Delivered || order.status === OrderStatus.Cancelled) && (
            <button
              onClick={handleReorder}
              disabled={actionLoading}
              className="px-6 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 disabled:bg-gray-400"
              data-testid="reorder-btn"
            >
              {actionLoading ? "Processing..." : "Reorder"}
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

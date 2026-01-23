"use client";

import { useState, useEffect } from "react";
import { getOrders, Order, OrderStatus } from "@/lib/api";
import Link from "next/link";

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedStatus, setSelectedStatus] = useState<OrderStatus | undefined>(undefined);

  useEffect(() => {
    fetchOrders();
  }, [selectedStatus]);

  const fetchOrders = async () => {
    try {
      setLoading(true);
      setError(null);
      const token = localStorage.getItem("authToken");
      if (!token) {
        setError("Not authenticated");
        setLoading(false);
        return;
      }

      const result = await getOrders(token, selectedStatus);
      setOrders(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load orders");
    } finally {
      setLoading(false);
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
        <div data-testid="orders-loading" className="text-center">
          Loading orders...
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div data-testid="orders-error" className="text-center text-red-600">
          {error}
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8" data-testid="orders-page">
      <h1 className="text-3xl font-bold mb-6">My Orders</h1>

      {/* Status tabs */}
      <div className="flex gap-2 mb-6 border-b" data-testid="status-tabs">
        <button
          onClick={() => setSelectedStatus(undefined)}
          className={`px-4 py-2 ${
            selectedStatus === undefined
              ? "border-b-2 border-blue-500 font-semibold"
              : "text-gray-600"
          }`}
          data-testid="tab-all"
        >
          All
        </button>
        <button
          onClick={() => setSelectedStatus(OrderStatus.Processing)}
          className={`px-4 py-2 ${
            selectedStatus === OrderStatus.Processing
              ? "border-b-2 border-blue-500 font-semibold"
              : "text-gray-600"
          }`}
          data-testid="tab-processing"
        >
          Processing
        </button>
        <button
          onClick={() => setSelectedStatus(OrderStatus.Shipping)}
          className={`px-4 py-2 ${
            selectedStatus === OrderStatus.Shipping
              ? "border-b-2 border-blue-500 font-semibold"
              : "text-gray-600"
          }`}
          data-testid="tab-shipping"
        >
          Shipping
        </button>
        <button
          onClick={() => setSelectedStatus(OrderStatus.Delivered)}
          className={`px-4 py-2 ${
            selectedStatus === OrderStatus.Delivered
              ? "border-b-2 border-blue-500 font-semibold"
              : "text-gray-600"
          }`}
          data-testid="tab-delivered"
        >
          Delivered
        </button>
        <button
          onClick={() => setSelectedStatus(OrderStatus.Cancelled)}
          className={`px-4 py-2 ${
            selectedStatus === OrderStatus.Cancelled
              ? "border-b-2 border-blue-500 font-semibold"
              : "text-gray-600"
          }`}
          data-testid="tab-cancelled"
        >
          Cancelled
        </button>
      </div>

      {/* Orders list */}
      {orders.length === 0 ? (
        <div data-testid="orders-empty" className="text-center py-8 text-gray-500">
          No orders found
        </div>
      ) : (
        <div className="space-y-4" data-testid="orders-list">
          {orders.map((order) => (
            <div
              key={order.id}
              className="border rounded-lg p-4 hover:shadow-md transition-shadow"
              data-testid={`order-${order.id}`}
            >
              <div className="flex justify-between items-start mb-3">
                <div>
                  <div className="font-semibold">Order #{order.id.substring(0, 8)}</div>
                  <div className="text-sm text-gray-600">{formatDate(order.createdAt)}</div>
                </div>
                <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
                  {getStatusLabel(order.status)}
                </span>
              </div>

              <div className="mb-3">
                <div className="text-sm text-gray-600">
                  {order.items.length} item{order.items.length > 1 ? "s" : ""}
                </div>
                <div className="font-semibold text-lg">{formatCurrency(order.total)}</div>
              </div>

              <div className="flex gap-2">
                <Link
                  href={`/account/orders/${order.id}`}
                  className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
                  data-testid={`view-order-${order.id}`}
                >
                  View Details
                </Link>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

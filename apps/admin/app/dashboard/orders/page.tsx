"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";

export default function OrdersPage() {
  const { token } = useAuth();
  const router = useRouter();
  const [orders, setOrders] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    // For now, we'll show a placeholder since orders require customer activity
    setLoading(false);
  }, [token, router]);

  if (!token) return null;

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar active="orders" />

      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <h1 style={{ marginBottom: 20 }}>Orders</h1>

        <div style={{ backgroundColor: "white", borderRadius: 8, padding: 40, textAlign: "center", boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
          <div style={{ fontSize: 48, marginBottom: 16 }}>🛒</div>
          <h2 style={{ color: "#333", marginBottom: 8 }}>No Orders Yet</h2>
          <p style={{ color: "#666", marginBottom: 20 }}>
            Orders will appear here once customers start placing orders on your store.
          </p>
          <p style={{ color: "#888", fontSize: 14 }}>
            Customer frontend: <a href="http://localhost:3000" target="_blank" style={{ color: "#007bff" }}>http://localhost:3000</a>
          </p>
        </div>

        <div style={{ marginTop: 30, backgroundColor: "white", borderRadius: 8, padding: 20, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
          <h3 style={{ marginBottom: 16 }}>Order Status Guide</h3>
          <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))", gap: 16 }}>
            <StatusCard status="Processing" color="#ffc107" description="Order received, preparing for shipment" />
            <StatusCard status="Shipping" color="#17a2b8" description="Order shipped, in transit" />
            <StatusCard status="Delivered" color="#28a745" description="Order successfully delivered" />
            <StatusCard status="Cancelled" color="#dc3545" description="Order was cancelled" />
          </div>
        </div>
      </main>
    </div>
  );
}

function StatusCard({ status, color, description }: { status: string; color: string; description: string }) {
  return (
    <div style={{ padding: 16, borderLeft: `4px solid ${color}`, backgroundColor: "#f8f9fa", borderRadius: 4 }}>
      <div style={{ fontWeight: 600, marginBottom: 4 }}>{status}</div>
      <div style={{ fontSize: 13, color: "#666" }}>{description}</div>
    </div>
  );
}

function Sidebar({ active }: { active: string }) {
  const { user, logout } = useAuth();
  const router = useRouter();

  const menuItems = [
    { href: "/dashboard", label: "📊 Dashboard", id: "dashboard" },
    { href: "/dashboard/products", label: "📦 Products", id: "products" },
    { href: "/dashboard/categories", label: "📂 Categories", id: "categories" },
    { href: "/dashboard/orders", label: "🛒 Orders", id: "orders" },
    { href: "/dashboard/import", label: "📥 Import Data", id: "import" },
  ];

  return (
    <aside style={{ width: 250, backgroundColor: "#1a1a2e", color: "white", padding: 20 }}>
      <h2 style={{ marginBottom: 30, fontSize: 20 }}>🛠️ Admin Portal</h2>
      <nav>
        {menuItems.map((item) => (
          <Link
            key={item.href}
            href={item.href}
            style={{
              display: "block",
              padding: "12px 16px",
              color: item.id === active ? "#fff" : "#aaa",
              backgroundColor: item.id === active ? "#007bff" : "transparent",
              borderRadius: 4,
              textDecoration: "none",
              marginBottom: 4,
            }}
          >
            {item.label}
          </Link>
        ))}
      </nav>
      <div style={{ marginTop: "auto", paddingTop: 40 }}>
        <div style={{ color: "#888", fontSize: 12, marginBottom: 8 }}>Logged in as:</div>
        <div style={{ color: "#fff", marginBottom: 4 }}>{user?.fullName}</div>
        <div style={{ color: "#888", fontSize: 12, marginBottom: 16 }}>{user?.role}</div>
        <button
          onClick={() => { logout(); router.push("/"); }}
          style={{ width: "100%", padding: 10, backgroundColor: "#dc3545", color: "white", border: "none", borderRadius: 4, cursor: "pointer" }}
        >
          Logout
        </button>
      </div>
    </aside>
  );
}

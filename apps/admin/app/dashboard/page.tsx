"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";
import { getDashboardStats, DashboardStats } from "@/lib/api";

export default function DashboardPage() {
  const { token, user, logout } = useAuth();
  const router = useRouter();
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }

    getDashboardStats(token)
      .then(setStats)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [token, router]);

  if (!token) return null;

  const menuItems = [
    { href: "/dashboard", label: "📊 Dashboard", active: true },
    { href: "/dashboard/products", label: "📦 Products" },
    { href: "/dashboard/categories", label: "📂 Categories" },
    { href: "/dashboard/banners", label: "🖼️ Banners" },
    { href: "/dashboard/orders", label: "🛒 Orders" },
    { href: "/dashboard/customers", label: "👥 Customers" },
    { href: "/dashboard/staff", label: "👤 Staff" },
    { href: "/dashboard/import", label: "📥 Import Data" },
  ];

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      {/* Sidebar */}
      <aside style={{
        width: 250,
        backgroundColor: "#1a1a2e",
        color: "white",
        padding: 20,
      }}>
        <h2 style={{ marginBottom: 30, fontSize: 20 }}>🛠️ Admin Portal</h2>
        <nav>
          {menuItems.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              style={{
                display: "block",
                padding: "12px 16px",
                color: item.active ? "#fff" : "#aaa",
                backgroundColor: item.active ? "#007bff" : "transparent",
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
          <div style={{ color: "#888", fontSize: 12, marginBottom: 8 }}>
            Logged in as:
          </div>
          <div style={{ color: "#fff", marginBottom: 4 }}>{user?.fullName}</div>
          <div style={{ color: "#888", fontSize: 12, marginBottom: 16 }}>{user?.role}</div>
          <button
            onClick={() => { logout(); router.push("/"); }}
            style={{
              width: "100%",
              padding: 10,
              backgroundColor: "#dc3545",
              color: "white",
              border: "none",
              borderRadius: 4,
              cursor: "pointer",
            }}
          >
            Logout
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <h1 style={{ marginBottom: 30 }}>Dashboard</h1>

        {loading ? (
          <p>Loading statistics...</p>
        ) : (
          <div style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))",
            gap: 20,
            marginBottom: 30,
          }}>
            <StatCard title="Total Products" value={stats?.totalProducts || 0} icon="📦" color="#007bff" />
            <StatCard title="Categories" value={stats?.totalCategories || 0} icon="📂" color="#28a745" />
            <StatCard title="Total Orders" value={stats?.totalOrders || 0} icon="🛒" color="#ffc107" />
            <StatCard title="Pending Orders" value={stats?.pendingOrders || 0} icon="⏳" color="#dc3545" />
          </div>
        )}

        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(300px, 1fr))", gap: 20 }}>
          <QuickActionCard
            title="Add New Product"
            description="Create a new product with variants"
            href="/dashboard/products/new"
            icon="➕"
          />
          <QuickActionCard
            title="Manage Banners"
            description="Update homepage hero slider banners"
            href="/dashboard/banners"
            icon="🖼️"
          />
          <QuickActionCard
            title="Manage Categories"
            description="Organize your product categories"
            href="/dashboard/categories"
            icon="📂"
          />
          <QuickActionCard
            title="View Orders"
            description="Process and manage orders"
            href="/dashboard/orders"
            icon="🛒"
          />
        </div>
      </main>
    </div>
  );
}

function StatCard({ title, value, icon, color }: { title: string; value: number; icon: string; color: string }) {
  return (
    <div style={{
      backgroundColor: "white",
      padding: 20,
      borderRadius: 8,
      boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
      borderLeft: `4px solid ${color}`,
    }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <div>
          <div style={{ fontSize: 14, color: "#666" }}>{title}</div>
          <div style={{ fontSize: 28, fontWeight: "bold", color: "#333" }}>{value}</div>
        </div>
        <div style={{ fontSize: 32 }}>{icon}</div>
      </div>
    </div>
  );
}

function QuickActionCard({ title, description, href, icon }: { title: string; description: string; href: string; icon: string }) {
  return (
    <Link href={href} style={{ textDecoration: "none" }}>
      <div style={{
        backgroundColor: "white",
        padding: 20,
        borderRadius: 8,
        boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
        cursor: "pointer",
        transition: "transform 0.2s",
      }}>
        <div style={{ fontSize: 24, marginBottom: 8 }}>{icon}</div>
        <div style={{ fontSize: 16, fontWeight: 600, color: "#333", marginBottom: 4 }}>{title}</div>
        <div style={{ fontSize: 14, color: "#666" }}>{description}</div>
      </div>
    </Link>
  );
}

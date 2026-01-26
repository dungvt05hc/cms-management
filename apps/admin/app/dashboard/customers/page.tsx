"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5027";

interface Customer {
  id: string;
  email: string;
  fullName: string;
  phone: string | null;
  createdAt: string;
  ordersCount?: number;
}

export default function CustomersPage() {
  const { token } = useAuth();
  const router = useRouter();
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    loadCustomers();
  }, [token, router]);

  const loadCustomers = async () => {
    if (!token) return;
    setLoading(true);
    try {
      const response = await fetch(`${API_BASE_URL}/admin/customers`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (response.ok) {
        const data = await response.json();
        setCustomers(data);
      }
    } catch (error) {
      console.error("Failed to load customers:", error);
    } finally {
      setLoading(false);
    }
  };

  if (!token) return null;

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar active="customers" />

      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <h1 style={{ marginBottom: 20 }}>Customers</h1>

        {loading ? (
          <p>Loading customers...</p>
        ) : customers.length === 0 ? (
          <div style={{ backgroundColor: "white", borderRadius: 8, padding: 40, textAlign: "center", boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <div style={{ fontSize: 48, marginBottom: 16 }}>👥</div>
            <h2 style={{ color: "#333", marginBottom: 8 }}>No Customers Yet</h2>
            <p style={{ color: "#666", marginBottom: 20 }}>
              Customers will appear here once they register on your store.
            </p>
            <p style={{ color: "#888", fontSize: 14 }}>
              Use the <Link href="/dashboard/import" style={{ color: "#007bff" }}>Import Data</Link> page to seed sample customers.
            </p>
          </div>
        ) : (
          <div style={{ backgroundColor: "white", borderRadius: 8, overflow: "hidden", boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <table style={{ width: "100%", borderCollapse: "collapse" }}>
              <thead>
                <tr style={{ backgroundColor: "#f8f9fa" }}>
                  <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Name</th>
                  <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Email</th>
                  <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Phone</th>
                  <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Joined</th>
                  <th style={{ padding: 12, textAlign: "center", borderBottom: "1px solid #dee2e6" }}>Orders</th>
                </tr>
              </thead>
              <tbody>
                {customers.map((customer) => (
                  <tr key={customer.id} style={{ borderBottom: "1px solid #dee2e6" }}>
                    <td style={{ padding: 12 }}>
                      <strong>{customer.fullName}</strong>
                    </td>
                    <td style={{ padding: 12, color: "#666" }}>{customer.email}</td>
                    <td style={{ padding: 12, color: "#666" }}>{customer.phone || "-"}</td>
                    <td style={{ padding: 12, color: "#666" }}>
                      {new Date(customer.createdAt).toLocaleDateString()}
                    </td>
                    <td style={{ padding: 12, textAlign: "center" }}>
                      <span style={{
                        padding: "4px 12px",
                        borderRadius: 12,
                        fontSize: 12,
                        backgroundColor: "#e3f2fd",
                        color: "#1976d2",
                      }}>
                        {customer.ordersCount || 0}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </main>
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
    { href: "/dashboard/customers", label: "👥 Customers", id: "customers" },
    { href: "/dashboard/import", label: "📥 Import Data", id: "import" },
  ];

  return (
    <aside style={{ width: 250, backgroundColor: "#1a1a2e", color: "white", padding: 20 }}>
      <h2 style={{ marginBottom: 30, fontSize: 20 }}>🛠️ Admin Portal</h2>
      <nav>
        {menuItems.map((item) => (
          <Link key={item.href} href={item.href} style={{
            display: "block", padding: "12px 16px",
            color: item.id === active ? "#fff" : "#aaa",
            backgroundColor: item.id === active ? "#007bff" : "transparent",
            borderRadius: 4, textDecoration: "none", marginBottom: 4,
          }}>
            {item.label}
          </Link>
        ))}
      </nav>
      <div style={{ marginTop: "auto", paddingTop: 40 }}>
        <div style={{ color: "#888", fontSize: 12, marginBottom: 8 }}>Logged in as:</div>
        <div style={{ color: "#fff", marginBottom: 4 }}>{user?.fullName}</div>
        <div style={{ color: "#888", fontSize: 12, marginBottom: 16 }}>{user?.role}</div>
        <button onClick={() => { logout(); router.push("/"); }} style={{ width: "100%", padding: 10, backgroundColor: "#dc3545", color: "white", border: "none", borderRadius: 4, cursor: "pointer" }}>
          Logout
        </button>
      </div>
    </aside>
  );
}

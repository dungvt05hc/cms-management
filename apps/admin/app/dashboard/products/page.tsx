"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";
import { getProducts, deleteProduct, Product, PagedResult } from "@/lib/api";

export default function ProductsPage() {
  const { token } = useAuth();
  const router = useRouter();
  const [products, setProducts] = useState<PagedResult<Product> | null>(null);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);

  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    loadProducts();
  }, [token, router, page]);

  const loadProducts = async () => {
    if (!token) return;
    setLoading(true);
    try {
      const data = await getProducts(token, page, 20);
      setProducts(data);
    } catch (error) {
      console.error("Failed to load products:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string, name: string) => {
    if (!token) return;
    if (!confirm(`Are you sure you want to delete "${name}"?`)) return;
    
    try {
      await deleteProduct(token, id);
      loadProducts();
    } catch (error) {
      alert("Failed to delete product");
    }
  };

  if (!token) return null;

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar active="products" />
      
      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
          <h1>Products</h1>
          <Link href="/dashboard/products/new">
            <button style={{
              padding: "10px 20px",
              backgroundColor: "#007bff",
              color: "white",
              border: "none",
              borderRadius: 4,
              cursor: "pointer",
              fontSize: 14,
            }}>
              ➕ Add Product
            </button>
          </Link>
        </div>

        {loading ? (
          <p>Loading products...</p>
        ) : (
          <>
            <div style={{ backgroundColor: "white", borderRadius: 8, overflow: "hidden", boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
              <table style={{ width: "100%", borderCollapse: "collapse" }}>
                <thead>
                  <tr style={{ backgroundColor: "#f8f9fa" }}>
                    <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Name</th>
                    <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Slug</th>
                    <th style={{ padding: 12, textAlign: "center", borderBottom: "1px solid #dee2e6" }}>Variants</th>
                    <th style={{ padding: 12, textAlign: "center", borderBottom: "1px solid #dee2e6" }}>Status</th>
                    <th style={{ padding: 12, textAlign: "center", borderBottom: "1px solid #dee2e6" }}>Featured</th>
                    <th style={{ padding: 12, textAlign: "center", borderBottom: "1px solid #dee2e6" }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {products?.items.map((product) => (
                    <tr key={product.id} style={{ borderBottom: "1px solid #dee2e6" }}>
                      <td style={{ padding: 12 }}>
                        <strong>{product.name}</strong>
                        {product.description && (
                          <div style={{ fontSize: 12, color: "#666", marginTop: 4 }}>
                            {product.description.substring(0, 50)}...
                          </div>
                        )}
                      </td>
                      <td style={{ padding: 12, color: "#666" }}>{product.slug}</td>
                      <td style={{ padding: 12, textAlign: "center" }}>
                        {product.variants.length} variant{product.variants.length !== 1 ? "s" : ""}
                      </td>
                      <td style={{ padding: 12, textAlign: "center" }}>
                        <span style={{
                          padding: "4px 8px",
                          borderRadius: 4,
                          fontSize: 12,
                          backgroundColor: product.isActive ? "#d4edda" : "#f8d7da",
                          color: product.isActive ? "#155724" : "#721c24",
                        }}>
                          {product.isActive ? "Active" : "Inactive"}
                        </span>
                      </td>
                      <td style={{ padding: 12, textAlign: "center" }}>
                        {product.isFeatured && "⭐"}
                      </td>
                      <td style={{ padding: 12, textAlign: "center" }}>
                        <button
                          onClick={() => router.push(`/dashboard/products/${product.id}`)}
                          style={{ marginRight: 8, padding: "4px 8px", cursor: "pointer" }}
                        >
                          ✏️ Edit
                        </button>
                        <button
                          onClick={() => handleDelete(product.id, product.name)}
                          style={{ padding: "4px 8px", cursor: "pointer", color: "#dc3545" }}
                        >
                          🗑️ Delete
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {products && products.totalCount > 20 && (
              <div style={{ display: "flex", justifyContent: "center", marginTop: 20, gap: 10 }}>
                <button
                  onClick={() => setPage(p => Math.max(1, p - 1))}
                  disabled={page === 1}
                  style={{ padding: "8px 16px", cursor: page === 1 ? "not-allowed" : "pointer" }}
                >
                  Previous
                </button>
                <span style={{ padding: "8px 16px" }}>
                  Page {page} of {Math.ceil(products.totalCount / 20)}
                </span>
                <button
                  onClick={() => setPage(p => p + 1)}
                  disabled={page >= Math.ceil(products.totalCount / 20)}
                  style={{ padding: "8px 16px", cursor: "pointer" }}
                >
                  Next
                </button>
              </div>
            )}
          </>
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

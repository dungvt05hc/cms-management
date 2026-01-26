"use client";

import { useEffect, useState } from "react";
import { useRouter, useParams } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";
import { getCategories, updateProduct, Category, Product } from "@/lib/api";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5027";

export default function EditProductPage() {
  const { token } = useAuth();
  const router = useRouter();
  const params = useParams();
  const productId = params.id as string;

  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [product, setProduct] = useState<Product | null>(null);

  const [name, setName] = useState("");
  const [slug, setSlug] = useState("");
  const [description, setDescription] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [isActive, setIsActive] = useState(true);
  const [isFeatured, setIsFeatured] = useState(false);
  const [specifications, setSpecifications] = useState("");
  const [images, setImages] = useState<string[]>([]);
  const [newImageUrl, setNewImageUrl] = useState("");

  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    loadData();
  }, [token, router, productId]);

  const loadData = async () => {
    if (!token) return;
    setLoading(true);
    try {
      const [cats, productRes] = await Promise.all([
        getCategories(token),
        fetch(`${API_BASE_URL}/products/${productId}`, {
          headers: { Authorization: `Bearer ${token}` },
        }),
      ]);

      setCategories(cats);

      if (productRes.ok) {
        const prod = await productRes.json();
        setProduct(prod);
        setName(prod.name);
        setSlug(prod.slug);
        setDescription(prod.description || "");
        setCategoryId(prod.categoryId || "");
        setIsActive(prod.isActive);
        setIsFeatured(prod.isFeatured);
        setSpecifications(prod.specifications || "");
        setImages(prod.images ? JSON.parse(prod.images) : []);
      } else {
        setError("Product not found");
      }
    } catch (err) {
      setError("Failed to load product");
    } finally {
      setLoading(false);
    }
  };

  const flattenCategories = (cats: Category[], prefix = ""): { id: string; name: string }[] => {
    return cats.flatMap((cat) => [
      { id: cat.id, name: prefix + cat.name },
      ...(cat.children ? flattenCategories(cat.children, prefix + "— ") : []),
    ]);
  };

  const addImage = () => {
    if (newImageUrl.trim()) {
      setImages([...images, newImageUrl.trim()]);
      setNewImageUrl("");
    }
  };

  const removeImage = (index: number) => {
    setImages(images.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token || !product) return;

    setSaving(true);
    setError("");

    try {
      const response = await fetch(`${API_BASE_URL}/admin/products/${productId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          name,
          slug,
          description: description || null,
          categoryId: categoryId || null,
          isActive,
          isFeatured,
          specifications: specifications || null,
          images: images.length > 0 ? JSON.stringify(images) : null,
        }),
      });

      if (!response.ok) {
        throw new Error("Failed to update product");
      }

      router.push("/dashboard/products");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update product");
    } finally {
      setSaving(false);
    }
  };

  if (!token) return null;

  if (loading) {
    return (
      <div style={{ display: "flex", minHeight: "100vh" }}>
        <Sidebar active="products" />
        <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
          <p>Loading product...</p>
        </main>
      </div>
    );
  }

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar active="products" />

      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <div style={{ display: "flex", alignItems: "center", marginBottom: 20, gap: 16 }}>
          <Link href="/dashboard/products" style={{ color: "#007bff", textDecoration: "none" }}>
            ← Back to Products
          </Link>
          <h1 style={{ margin: 0 }}>Edit Product</h1>
        </div>

        {error && (
          <div style={{ backgroundColor: "#fee", color: "#c00", padding: 12, borderRadius: 4, marginBottom: 20 }}>
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div style={{ backgroundColor: "white", padding: 24, borderRadius: 8, marginBottom: 20, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <h3 style={{ marginTop: 0, marginBottom: 20 }}>Basic Information</h3>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20 }}>
              <div>
                <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Product Name *</label>
                <input
                  type="text"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  required
                  style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
                />
              </div>
              <div>
                <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Slug *</label>
                <input
                  type="text"
                  value={slug}
                  onChange={(e) => setSlug(e.target.value)}
                  required
                  style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
                />
              </div>
            </div>

            <div style={{ marginTop: 16 }}>
              <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Description</label>
              <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                rows={4}
                style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box", resize: "vertical" }}
              />
            </div>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: 20, marginTop: 16 }}>
              <div>
                <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Category</label>
                <select
                  value={categoryId}
                  onChange={(e) => setCategoryId(e.target.value)}
                  style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4 }}
                >
                  <option value="">Select Category</option>
                  {flattenCategories(categories).map((cat) => (
                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                  ))}
                </select>
              </div>
              <div style={{ display: "flex", alignItems: "center", gap: 20, paddingTop: 24 }}>
                <label style={{ display: "flex", alignItems: "center", gap: 8, cursor: "pointer" }}>
                  <input type="checkbox" checked={isActive} onChange={(e) => setIsActive(e.target.checked)} />
                  Active
                </label>
                <label style={{ display: "flex", alignItems: "center", gap: 8, cursor: "pointer" }}>
                  <input type="checkbox" checked={isFeatured} onChange={(e) => setIsFeatured(e.target.checked)} />
                  Featured
                </label>
              </div>
            </div>

            <div style={{ marginTop: 16 }}>
              <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Specifications (JSON)</label>
              <textarea
                value={specifications}
                onChange={(e) => setSpecifications(e.target.value)}
                rows={3}
                placeholder='{"key": "value"}'
                style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box", fontFamily: "monospace" }}
              />
            </div>
          </div>

          {/* Images Section */}
          <div style={{ backgroundColor: "white", padding: 24, borderRadius: 8, marginBottom: 20, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <h3 style={{ marginTop: 0, marginBottom: 20 }}>Product Images</h3>

            <div style={{ display: "flex", gap: 12, marginBottom: 16 }}>
              <input
                type="url"
                value={newImageUrl}
                onChange={(e) => setNewImageUrl(e.target.value)}
                placeholder="Enter image URL"
                style={{ flex: 1, padding: 10, border: "1px solid #ddd", borderRadius: 4 }}
              />
              <button
                type="button"
                onClick={addImage}
                style={{ padding: "10px 20px", backgroundColor: "#28a745", color: "white", border: "none", borderRadius: 4, cursor: "pointer" }}
              >
                Add Image
              </button>
            </div>

            {images.length > 0 ? (
              <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(150px, 1fr))", gap: 16 }}>
                {images.map((url, index) => (
                  <div key={index} style={{ position: "relative", border: "1px solid #ddd", borderRadius: 4, overflow: "hidden" }}>
                    <img src={url} alt={`Product ${index + 1}`} style={{ width: "100%", height: 120, objectFit: "cover" }} />
                    <button
                      type="button"
                      onClick={() => removeImage(index)}
                      style={{
                        position: "absolute", top: 4, right: 4,
                        backgroundColor: "#dc3545", color: "white",
                        border: "none", borderRadius: "50%",
                        width: 24, height: 24, cursor: "pointer",
                      }}
                    >
                      ×
                    </button>
                  </div>
                ))}
              </div>
            ) : (
              <p style={{ color: "#666", fontStyle: "italic" }}>No images added yet</p>
            )}
          </div>

          {/* Variants Display */}
          {product && product.variants.length > 0 && (
            <div style={{ backgroundColor: "white", padding: 24, borderRadius: 8, marginBottom: 20, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
              <h3 style={{ marginTop: 0, marginBottom: 20 }}>Variants (Read-only)</h3>
              <table style={{ width: "100%", borderCollapse: "collapse" }}>
                <thead>
                  <tr style={{ backgroundColor: "#f8f9fa" }}>
                    <th style={{ padding: 10, textAlign: "left", borderBottom: "1px solid #ddd" }}>SKU</th>
                    <th style={{ padding: 10, textAlign: "left", borderBottom: "1px solid #ddd" }}>Name</th>
                    <th style={{ padding: 10, textAlign: "right", borderBottom: "1px solid #ddd" }}>Price</th>
                    <th style={{ padding: 10, textAlign: "right", borderBottom: "1px solid #ddd" }}>Stock</th>
                  </tr>
                </thead>
                <tbody>
                  {product.variants.map((v) => (
                    <tr key={v.id}>
                      <td style={{ padding: 10, borderBottom: "1px solid #eee" }}>{v.sku}</td>
                      <td style={{ padding: 10, borderBottom: "1px solid #eee" }}>{v.variantName || "-"}</td>
                      <td style={{ padding: 10, borderBottom: "1px solid #eee", textAlign: "right" }}>${v.price.toFixed(2)}</td>
                      <td style={{ padding: 10, borderBottom: "1px solid #eee", textAlign: "right" }}>{v.stockQuantity}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          <div style={{ display: "flex", gap: 12 }}>
            <button
              type="submit"
              disabled={saving}
              style={{
                padding: "12px 24px",
                backgroundColor: saving ? "#ccc" : "#007bff",
                color: "white",
                border: "none",
                borderRadius: 4,
                cursor: saving ? "not-allowed" : "pointer",
                fontSize: 16,
              }}
            >
              {saving ? "Saving..." : "Save Changes"}
            </button>
            <Link href="/dashboard/products">
              <button type="button" style={{ padding: "12px 24px", backgroundColor: "#6c757d", color: "white", border: "none", borderRadius: 4, cursor: "pointer", fontSize: 16 }}>
                Cancel
              </button>
            </Link>
          </div>
        </form>
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

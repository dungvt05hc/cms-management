"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";
import { createProduct, getCategories, Category } from "@/lib/api";

export default function NewProductPage() {
  const { token } = useAuth();
  const router = useRouter();
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [name, setName] = useState("");
  const [slug, setSlug] = useState("");
  const [description, setDescription] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [isActive, setIsActive] = useState(true);
  const [isFeatured, setIsFeatured] = useState(false);
  const [specifications, setSpecifications] = useState("");
  const [variants, setVariants] = useState([
    { sku: "", variantName: "", price: 0, stockQuantity: 0 },
  ]);

  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    getCategories(token).then(setCategories).catch(console.error);
  }, [token, router]);

  const flattenCategories = (cats: Category[], prefix = ""): { id: string; name: string }[] => {
    return cats.flatMap((cat) => [
      { id: cat.id, name: prefix + cat.name },
      ...(cat.children ? flattenCategories(cat.children, prefix + "— ") : []),
    ]);
  };

  const generateSlug = (text: string) => {
    return text
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, "-")
      .replace(/(^-|-$)/g, "");
  };

  const handleNameChange = (value: string) => {
    setName(value);
    if (!slug || slug === generateSlug(name)) {
      setSlug(generateSlug(value));
    }
  };

  const addVariant = () => {
    setVariants([...variants, { sku: "", variantName: "", price: 0, stockQuantity: 0 }]);
  };

  const removeVariant = (index: number) => {
    if (variants.length > 1) {
      setVariants(variants.filter((_, i) => i !== index));
    }
  };

  const updateVariant = (index: number, field: string, value: string | number) => {
    const updated = [...variants];
    updated[index] = { ...updated[index], [field]: value };
    setVariants(updated);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token) return;

    setLoading(true);
    setError("");

    try {
      await createProduct(token, {
        name,
        slug,
        description: description || undefined,
        categoryId: categoryId || undefined,
        isActive,
        isFeatured,
        specifications: specifications || undefined,
        variants: variants.filter(v => v.sku && v.price > 0),
      });
      router.push("/dashboard/products");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create product");
    } finally {
      setLoading(false);
    }
  };

  if (!token) return null;

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar active="products" />

      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <div style={{ display: "flex", alignItems: "center", marginBottom: 20, gap: 16 }}>
          <Link href="/dashboard/products" style={{ color: "#007bff", textDecoration: "none" }}>
            ← Back to Products
          </Link>
          <h1 style={{ margin: 0 }}>Add New Product</h1>
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
                  onChange={(e) => handleNameChange(e.target.value)}
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
                  <input
                    type="checkbox"
                    checked={isActive}
                    onChange={(e) => setIsActive(e.target.checked)}
                  />
                  Active
                </label>
                <label style={{ display: "flex", alignItems: "center", gap: 8, cursor: "pointer" }}>
                  <input
                    type="checkbox"
                    checked={isFeatured}
                    onChange={(e) => setIsFeatured(e.target.checked)}
                  />
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

          <div style={{ backgroundColor: "white", padding: 24, borderRadius: 8, marginBottom: 20, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
              <h3 style={{ margin: 0 }}>Variants</h3>
              <button
                type="button"
                onClick={addVariant}
                style={{ padding: "8px 16px", backgroundColor: "#28a745", color: "white", border: "none", borderRadius: 4, cursor: "pointer" }}
              >
                ➕ Add Variant
              </button>
            </div>

            {variants.map((variant, index) => (
              <div key={index} style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr 1fr auto", gap: 12, marginBottom: 12, padding: 16, backgroundColor: "#f8f9fa", borderRadius: 4 }}>
                <div>
                  <label style={{ display: "block", marginBottom: 4, fontSize: 12, color: "#666" }}>SKU *</label>
                  <input
                    type="text"
                    value={variant.sku}
                    onChange={(e) => updateVariant(index, "sku", e.target.value)}
                    required
                    style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
                  />
                </div>
                <div>
                  <label style={{ display: "block", marginBottom: 4, fontSize: 12, color: "#666" }}>Variant Name</label>
                  <input
                    type="text"
                    value={variant.variantName}
                    onChange={(e) => updateVariant(index, "variantName", e.target.value)}
                    placeholder="e.g., 128GB Black"
                    style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
                  />
                </div>
                <div>
                  <label style={{ display: "block", marginBottom: 4, fontSize: 12, color: "#666" }}>Price *</label>
                  <input
                    type="number"
                    value={variant.price}
                    onChange={(e) => updateVariant(index, "price", parseFloat(e.target.value) || 0)}
                    min="0"
                    step="0.01"
                    required
                    style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
                  />
                </div>
                <div>
                  <label style={{ display: "block", marginBottom: 4, fontSize: 12, color: "#666" }}>Stock</label>
                  <input
                    type="number"
                    value={variant.stockQuantity}
                    onChange={(e) => updateVariant(index, "stockQuantity", parseInt(e.target.value) || 0)}
                    min="0"
                    style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
                  />
                </div>
                <div style={{ display: "flex", alignItems: "flex-end" }}>
                  <button
                    type="button"
                    onClick={() => removeVariant(index)}
                    disabled={variants.length === 1}
                    style={{ padding: 8, color: variants.length === 1 ? "#ccc" : "#dc3545", background: "none", border: "none", cursor: variants.length === 1 ? "not-allowed" : "pointer", fontSize: 16 }}
                  >
                    🗑️
                  </button>
                </div>
              </div>
            ))}
          </div>

          <div style={{ display: "flex", gap: 12 }}>
            <button
              type="submit"
              disabled={loading}
              style={{
                padding: "12px 24px",
                backgroundColor: loading ? "#ccc" : "#007bff",
                color: "white",
                border: "none",
                borderRadius: 4,
                cursor: loading ? "not-allowed" : "pointer",
                fontSize: 16,
              }}
            >
              {loading ? "Creating..." : "Create Product"}
            </button>
            <Link href="/dashboard/products">
              <button
                type="button"
                style={{ padding: "12px 24px", backgroundColor: "#6c757d", color: "white", border: "none", borderRadius: 4, cursor: "pointer", fontSize: 16 }}
              >
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

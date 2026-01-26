"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";
import { getCategories, createCategory, deleteCategory, Category } from "@/lib/api";

export default function CategoriesPage() {
  const { token } = useAuth();
  const router = useRouter();
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [newCategoryName, setNewCategoryName] = useState("");
  const [parentId, setParentId] = useState<string>("");

  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    loadCategories();
  }, [token, router]);

  const loadCategories = async () => {
    if (!token) return;
    setLoading(true);
    try {
      const data = await getCategories(token);
      setCategories(data);
    } catch (error) {
      console.error("Failed to load categories:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!token || !newCategoryName.trim()) return;

    try {
      await createCategory(token, {
        name: newCategoryName.trim(),
        parentId: parentId || undefined,
      });
      setNewCategoryName("");
      setParentId("");
      setShowForm(false);
      loadCategories();
    } catch (error) {
      alert("Failed to create category");
    }
  };

  const handleDelete = async (id: string, name: string) => {
    if (!token) return;
    if (!confirm(`Are you sure you want to delete "${name}"? This will also delete all subcategories.`)) return;

    try {
      await deleteCategory(token, id);
      loadCategories();
    } catch (error) {
      alert("Failed to delete category");
    }
  };

  const flattenCategories = (cats: Category[], prefix = ""): { id: string; name: string }[] => {
    return cats.flatMap((cat) => [
      { id: cat.id, name: prefix + cat.name },
      ...(cat.children ? flattenCategories(cat.children, prefix + "— ") : []),
    ]);
  };

  if (!token) return null;

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar active="categories" />

      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
          <h1>Categories</h1>
          <button
            onClick={() => setShowForm(!showForm)}
            style={{
              padding: "10px 20px",
              backgroundColor: "#007bff",
              color: "white",
              border: "none",
              borderRadius: 4,
              cursor: "pointer",
            }}
          >
            {showForm ? "Cancel" : "➕ Add Category"}
          </button>
        </div>

        {showForm && (
          <div style={{ backgroundColor: "white", padding: 20, borderRadius: 8, marginBottom: 20, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <form onSubmit={handleCreate}>
              <div style={{ display: "flex", gap: 16, alignItems: "flex-end" }}>
                <div style={{ flex: 1 }}>
                  <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Category Name</label>
                  <input
                    type="text"
                    value={newCategoryName}
                    onChange={(e) => setNewCategoryName(e.target.value)}
                    placeholder="Enter category name"
                    required
                    style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
                  />
                </div>
                <div style={{ flex: 1 }}>
                  <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Parent Category (optional)</label>
                  <select
                    value={parentId}
                    onChange={(e) => setParentId(e.target.value)}
                    style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4 }}
                  >
                    <option value="">None (Root Category)</option>
                    {flattenCategories(categories).map((cat) => (
                      <option key={cat.id} value={cat.id}>{cat.name}</option>
                    ))}
                  </select>
                </div>
                <button
                  type="submit"
                  style={{ padding: "10px 20px", backgroundColor: "#28a745", color: "white", border: "none", borderRadius: 4, cursor: "pointer" }}
                >
                  Create
                </button>
              </div>
            </form>
          </div>
        )}

        {loading ? (
          <p>Loading categories...</p>
        ) : (
          <div style={{ backgroundColor: "white", borderRadius: 8, padding: 20, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            {categories.length === 0 ? (
              <p style={{ color: "#666" }}>No categories yet. Create your first category above.</p>
            ) : (
              <CategoryTree categories={categories} onDelete={handleDelete} level={0} />
            )}
          </div>
        )}
      </main>
    </div>
  );
}

function CategoryTree({ categories, onDelete, level }: { categories: Category[]; onDelete: (id: string, name: string) => void; level: number }) {
  return (
    <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
      {categories.map((category) => (
        <li key={category.id} style={{ marginLeft: level * 24 }}>
          <div style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            padding: "12px 16px",
            borderBottom: "1px solid #eee",
            backgroundColor: level === 0 ? "#f8f9fa" : "transparent",
          }}>
            <span>
              {level > 0 && "└─ "}
              <strong>{category.name}</strong>
              {category.children && category.children.length > 0 && (
                <span style={{ color: "#666", marginLeft: 8 }}>({category.children.length} subcategories)</span>
              )}
            </span>
            <button
              onClick={() => onDelete(category.id, category.name)}
              style={{ padding: "4px 8px", cursor: "pointer", color: "#dc3545", background: "none", border: "1px solid #dc3545", borderRadius: 4 }}
            >
              🗑️ Delete
            </button>
          </div>
          {category.children && category.children.length > 0 && (
            <CategoryTree categories={category.children} onDelete={onDelete} level={level + 1} />
          )}
        </li>
      ))}
    </ul>
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

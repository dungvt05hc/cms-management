"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";

interface Banner {
  id: string;
  title: string;
  subtitle: string;
  description: string;
  buttonText: string;
  buttonLink: string;
  imageUrl: string;
  bgColor: string;
  isActive: boolean;
  displayOrder: number;
}

// Local storage key for banners (until backend API is ready)
const BANNERS_STORAGE_KEY = "cms_banners";

const defaultBanners: Banner[] = [
  {
    id: "1",
    title: "New Collection 2026",
    subtitle: "Spring Sale",
    description: "Discover amazing deals up to 50% off on our latest collection. Free shipping on orders over $50.",
    buttonText: "Shop Now",
    buttonLink: "/products",
    imageUrl: "",
    bgColor: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
    isActive: true,
    displayOrder: 1,
  },
  {
    id: "2",
    title: "Premium Quality",
    subtitle: "Best Sellers",
    description: "Explore our most popular products loved by thousands of customers worldwide.",
    buttonText: "Explore",
    buttonLink: "/products?featured=true",
    imageUrl: "",
    bgColor: "linear-gradient(135deg, #f093fb 0%, #f5576c 100%)",
    isActive: true,
    displayOrder: 2,
  },
  {
    id: "3",
    title: "Exclusive Deals",
    subtitle: "Limited Time",
    description: "Don't miss out on exclusive offers. Get up to 70% off on selected items.",
    buttonText: "View Deals",
    buttonLink: "/deals",
    imageUrl: "",
    bgColor: "linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)",
    isActive: true,
    displayOrder: 3,
  },
];

export default function BannersPage() {
  const { token } = useAuth();
  const router = useRouter();
  const [banners, setBanners] = useState<Banner[]>([]);
  const [loading, setLoading] = useState(true);
  const [editingBanner, setEditingBanner] = useState<Banner | null>(null);
  const [showForm, setShowForm] = useState(false);

  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    loadBanners();
  }, [token, router]);

  const loadBanners = () => {
    setLoading(true);
    try {
      const stored = localStorage.getItem(BANNERS_STORAGE_KEY);
      if (stored) {
        setBanners(JSON.parse(stored));
      } else {
        setBanners(defaultBanners);
        localStorage.setItem(BANNERS_STORAGE_KEY, JSON.stringify(defaultBanners));
      }
    } catch (error) {
      console.error("Failed to load banners:", error);
      setBanners(defaultBanners);
    } finally {
      setLoading(false);
    }
  };

  const saveBanners = (newBanners: Banner[]) => {
    setBanners(newBanners);
    localStorage.setItem(BANNERS_STORAGE_KEY, JSON.stringify(newBanners));
  };

  const handleCreate = (banner: Omit<Banner, "id">) => {
    const newBanner: Banner = {
      ...banner,
      id: Date.now().toString(),
    };
    saveBanners([...banners, newBanner]);
    setShowForm(false);
  };

  const handleUpdate = (updatedBanner: Banner) => {
    const newBanners = banners.map((b) => 
      b.id === updatedBanner.id ? updatedBanner : b
    );
    saveBanners(newBanners);
    setEditingBanner(null);
  };

  const handleDelete = (id: string) => {
    if (!confirm("Are you sure you want to delete this banner?")) return;
    saveBanners(banners.filter((b) => b.id !== id));
  };

  const handleToggleActive = (id: string) => {
    const newBanners = banners.map((b) =>
      b.id === id ? { ...b, isActive: !b.isActive } : b
    );
    saveBanners(newBanners);
  };

  const handleMoveUp = (id: string) => {
    const index = banners.findIndex((b) => b.id === id);
    if (index <= 0) return;
    const newBanners = [...banners];
    [newBanners[index - 1], newBanners[index]] = [newBanners[index], newBanners[index - 1]];
    newBanners.forEach((b, i) => (b.displayOrder = i + 1));
    saveBanners(newBanners);
  };

  const handleMoveDown = (id: string) => {
    const index = banners.findIndex((b) => b.id === id);
    if (index >= banners.length - 1) return;
    const newBanners = [...banners];
    [newBanners[index], newBanners[index + 1]] = [newBanners[index + 1], newBanners[index]];
    newBanners.forEach((b, i) => (b.displayOrder = i + 1));
    saveBanners(newBanners);
  };

  if (!token) return null;

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar active="banners" />

      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
          <div>
            <h1>Banner Management</h1>
            <p style={{ color: "#666", marginTop: 4 }}>Manage homepage hero slider banners</p>
          </div>
          <button
            onClick={() => { setShowForm(true); setEditingBanner(null); }}
            style={{
              padding: "10px 20px",
              backgroundColor: "#007bff",
              color: "white",
              border: "none",
              borderRadius: 4,
              cursor: "pointer",
              fontSize: 14,
            }}
          >
            ➕ Add Banner
          </button>
        </div>

        {(showForm || editingBanner) && (
          <BannerForm
            banner={editingBanner}
            onSave={editingBanner ? handleUpdate : handleCreate}
            onCancel={() => { setShowForm(false); setEditingBanner(null); }}
          />
        )}

        {loading ? (
          <p>Loading banners...</p>
        ) : (
          <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
            {banners.length === 0 ? (
              <div style={{ backgroundColor: "white", padding: 40, borderRadius: 8, textAlign: "center" }}>
                <p style={{ color: "#666" }}>No banners yet. Create your first banner above.</p>
              </div>
            ) : (
              banners
                .sort((a, b) => a.displayOrder - b.displayOrder)
                .map((banner, index) => (
                  <div
                    key={banner.id}
                    style={{
                      backgroundColor: "white",
                      borderRadius: 8,
                      overflow: "hidden",
                      boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
                      opacity: banner.isActive ? 1 : 0.6,
                    }}
                  >
                    <div style={{ display: "flex" }}>
                      {/* Preview */}
                      <div
                        style={{
                          width: 300,
                          height: 150,
                          background: banner.bgColor,
                          display: "flex",
                          alignItems: "center",
                          justifyContent: "center",
                          color: "white",
                          padding: 20,
                          flexShrink: 0,
                        }}
                      >
                        <div>
                          <div style={{ fontSize: 10, opacity: 0.8, marginBottom: 4 }}>{banner.subtitle}</div>
                          <div style={{ fontSize: 16, fontWeight: "bold" }}>{banner.title}</div>
                        </div>
                      </div>

                      {/* Details */}
                      <div style={{ flex: 1, padding: 16 }}>
                        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                          <div>
                            <h3 style={{ margin: 0, marginBottom: 4 }}>{banner.title}</h3>
                            <p style={{ margin: 0, color: "#666", fontSize: 14 }}>{banner.subtitle}</p>
                            <p style={{ margin: "8px 0", color: "#888", fontSize: 13 }}>
                              {banner.description.substring(0, 100)}...
                            </p>
                            <div style={{ display: "flex", gap: 8, marginTop: 8 }}>
                              <span style={{
                                padding: "2px 8px",
                                borderRadius: 4,
                                fontSize: 11,
                                backgroundColor: banner.isActive ? "#d4edda" : "#f8d7da",
                                color: banner.isActive ? "#155724" : "#721c24",
                              }}>
                                {banner.isActive ? "Active" : "Inactive"}
                              </span>
                              <span style={{
                                padding: "2px 8px",
                                borderRadius: 4,
                                fontSize: 11,
                                backgroundColor: "#e2e8f0",
                                color: "#475569",
                              }}>
                                Order: {banner.displayOrder}
                              </span>
                              <span style={{
                                padding: "2px 8px",
                                borderRadius: 4,
                                fontSize: 11,
                                backgroundColor: "#dbeafe",
                                color: "#1e40af",
                              }}>
                                Link: {banner.buttonLink}
                              </span>
                            </div>
                          </div>

                          {/* Actions */}
                          <div style={{ display: "flex", gap: 8, flexWrap: "wrap" }}>
                            <button
                              onClick={() => handleMoveUp(banner.id)}
                              disabled={index === 0}
                              style={{ padding: "6px 10px", cursor: index === 0 ? "not-allowed" : "pointer", opacity: index === 0 ? 0.5 : 1 }}
                              title="Move Up"
                            >
                              ⬆️
                            </button>
                            <button
                              onClick={() => handleMoveDown(banner.id)}
                              disabled={index === banners.length - 1}
                              style={{ padding: "6px 10px", cursor: index === banners.length - 1 ? "not-allowed" : "pointer", opacity: index === banners.length - 1 ? 0.5 : 1 }}
                              title="Move Down"
                            >
                              ⬇️
                            </button>
                            <button
                              onClick={() => handleToggleActive(banner.id)}
                              style={{ padding: "6px 10px", cursor: "pointer" }}
                              title={banner.isActive ? "Deactivate" : "Activate"}
                            >
                              {banner.isActive ? "🔴" : "🟢"}
                            </button>
                            <button
                              onClick={() => setEditingBanner(banner)}
                              style={{ padding: "6px 10px", cursor: "pointer" }}
                              title="Edit"
                            >
                              ✏️
                            </button>
                            <button
                              onClick={() => handleDelete(banner.id)}
                              style={{ padding: "6px 10px", cursor: "pointer", color: "#dc3545" }}
                              title="Delete"
                            >
                              🗑️
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                ))
            )}
          </div>
        )}

        <div style={{ marginTop: 30, padding: 20, backgroundColor: "#fff3cd", borderRadius: 8, border: "1px solid #ffc107" }}>
          <h4 style={{ margin: "0 0 8px 0", color: "#856404" }}>💡 How Banners Work</h4>
          <ul style={{ margin: 0, paddingLeft: 20, color: "#856404", fontSize: 14 }}>
            <li>Banners appear in the homepage hero slider</li>
            <li>Use the ⬆️⬇️ buttons to change the display order</li>
            <li>Toggle 🔴/🟢 to activate or deactivate a banner</li>
            <li>Background colors support CSS gradients (e.g., <code>linear-gradient(135deg, #667eea 0%, #764ba2 100%)</code>)</li>
            <li>Changes are saved to localStorage (will persist in browser)</li>
          </ul>
        </div>
      </main>
    </div>
  );
}

interface BannerFormProps {
  banner: Banner | null;
  onSave: (banner: any) => void;
  onCancel: () => void;
}

function BannerForm({ banner, onSave, onCancel }: BannerFormProps) {
  const [form, setForm] = useState({
    title: banner?.title || "",
    subtitle: banner?.subtitle || "",
    description: banner?.description || "",
    buttonText: banner?.buttonText || "Shop Now",
    buttonLink: banner?.buttonLink || "/products",
    imageUrl: banner?.imageUrl || "",
    bgColor: banner?.bgColor || "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
    isActive: banner?.isActive ?? true,
    displayOrder: banner?.displayOrder || 1,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (banner) {
      onSave({ ...banner, ...form });
    } else {
      onSave(form);
    }
  };

  const presetColors = [
    { name: "Purple", value: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)" },
    { name: "Pink", value: "linear-gradient(135deg, #f093fb 0%, #f5576c 100%)" },
    { name: "Blue", value: "linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)" },
    { name: "Green", value: "linear-gradient(135deg, #11998e 0%, #38ef7d 100%)" },
    { name: "Orange", value: "linear-gradient(135deg, #ff6b35 0%, #f7931e 100%)" },
    { name: "Dark", value: "linear-gradient(135deg, #1a1a2e 0%, #16213e 100%)" },
  ];

  return (
    <div style={{ backgroundColor: "white", padding: 24, borderRadius: 8, marginBottom: 20, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
      <h3 style={{ marginTop: 0 }}>{banner ? "Edit Banner" : "Create New Banner"}</h3>
      <form onSubmit={handleSubmit}>
        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16 }}>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Title *</label>
            <input
              type="text"
              value={form.title}
              onChange={(e) => setForm({ ...form, title: e.target.value })}
              required
              placeholder="e.g., Summer Sale 2026"
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Subtitle *</label>
            <input
              type="text"
              value={form.subtitle}
              onChange={(e) => setForm({ ...form, subtitle: e.target.value })}
              required
              placeholder="e.g., Limited Time Offer"
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
        </div>

        <div style={{ marginTop: 16 }}>
          <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Description *</label>
          <textarea
            value={form.description}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
            required
            rows={3}
            placeholder="Banner description text..."
            style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box", resize: "vertical" }}
          />
        </div>

        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16, marginTop: 16 }}>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Button Text</label>
            <input
              type="text"
              value={form.buttonText}
              onChange={(e) => setForm({ ...form, buttonText: e.target.value })}
              placeholder="e.g., Shop Now"
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Button Link</label>
            <input
              type="text"
              value={form.buttonLink}
              onChange={(e) => setForm({ ...form, buttonLink: e.target.value })}
              placeholder="e.g., /products or /deals"
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
        </div>

        <div style={{ marginTop: 16 }}>
          <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Image URL (optional)</label>
          <input
            type="text"
            value={form.imageUrl}
            onChange={(e) => setForm({ ...form, imageUrl: e.target.value })}
            placeholder="https://example.com/banner-image.jpg"
            style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
          />
        </div>

        <div style={{ marginTop: 16 }}>
          <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Background Color / Gradient</label>
          <div style={{ display: "flex", gap: 8, marginBottom: 8, flexWrap: "wrap" }}>
            {presetColors.map((color) => (
              <button
                key={color.name}
                type="button"
                onClick={() => setForm({ ...form, bgColor: color.value })}
                style={{
                  width: 60,
                  height: 30,
                  background: color.value,
                  border: form.bgColor === color.value ? "3px solid #333" : "1px solid #ddd",
                  borderRadius: 4,
                  cursor: "pointer",
                }}
                title={color.name}
              />
            ))}
          </div>
          <input
            type="text"
            value={form.bgColor}
            onChange={(e) => setForm({ ...form, bgColor: e.target.value })}
            placeholder="CSS gradient or color"
            style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
          />
        </div>

        {/* Preview */}
        <div style={{ marginTop: 16 }}>
          <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Preview</label>
          <div
            style={{
              background: form.bgColor,
              padding: 24,
              borderRadius: 8,
              color: "white",
            }}
          >
            <div style={{ fontSize: 12, opacity: 0.8, marginBottom: 4 }}>{form.subtitle || "Subtitle"}</div>
            <div style={{ fontSize: 24, fontWeight: "bold", marginBottom: 8 }}>{form.title || "Title"}</div>
            <div style={{ fontSize: 14, opacity: 0.9, marginBottom: 16 }}>{form.description || "Description"}</div>
            <span style={{ padding: "8px 16px", backgroundColor: "white", color: "#333", borderRadius: 20, fontWeight: 600 }}>
              {form.buttonText || "Button"}
            </span>
          </div>
        </div>

        <div style={{ display: "flex", gap: 12, marginTop: 20 }}>
          <button
            type="submit"
            style={{ padding: "10px 24px", backgroundColor: "#28a745", color: "white", border: "none", borderRadius: 4, cursor: "pointer", fontWeight: 600 }}
          >
            {banner ? "Update Banner" : "Create Banner"}
          </button>
          <button
            type="button"
            onClick={onCancel}
            style={{ padding: "10px 24px", backgroundColor: "#6c757d", color: "white", border: "none", borderRadius: 4, cursor: "pointer" }}
          >
            Cancel
          </button>
        </div>
      </form>
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
    { href: "/dashboard/banners", label: "🖼️ Banners", id: "banners" },
    { href: "/dashboard/orders", label: "🛒 Orders", id: "orders" },
    { href: "/dashboard/customers", label: "👥 Customers", id: "customers" },
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

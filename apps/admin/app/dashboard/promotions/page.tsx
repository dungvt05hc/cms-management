"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";
import {
  Promotion,
  CreatePromotionRequest,
  getPromotions,
  createPromotion,
  updatePromotion,
  deletePromotion,
  togglePromotion,
} from "@/lib/api";

export default function PromotionsPage() {
  const { token, user, logout } = useAuth();
  const router = useRouter();
  const [promotions, setPromotions] = useState<Promotion[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editingPromotion, setEditingPromotion] = useState<Promotion | null>(null);

  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    loadPromotions();
  }, [token, router]);

  const loadPromotions = async () => {
    if (!token) return;
    setLoading(true);
    setError(null);
    try {
      const result = await getPromotions(token);
      setPromotions(result.items);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load promotions");
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: CreatePromotionRequest) => {
    if (!token) return;
    try {
      await createPromotion(token, data);
      setShowForm(false);
      loadPromotions();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create promotion");
    }
  };

  const handleUpdate = async (id: string, data: Partial<CreatePromotionRequest>) => {
    if (!token) return;
    try {
      await updatePromotion(token, id, data);
      setEditingPromotion(null);
      loadPromotions();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update promotion");
    }
  };

  const handleDelete = async (id: string) => {
    if (!token) return;
    if (!confirm("Are you sure you want to delete this promotion?")) return;
    try {
      await deletePromotion(token, id);
      loadPromotions();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to delete promotion");
    }
  };

  const handleToggle = async (id: string) => {
    if (!token) return;
    try {
      await togglePromotion(token, id);
      loadPromotions();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to toggle promotion");
    }
  };

  const isPromotionValid = (promo: Promotion) => {
    const now = new Date();
    const start = new Date(promo.startDate);
    const end = new Date(promo.endDate);
    return promo.isActive && now >= start && now <= end && 
      (promo.usageLimit === null || promo.usedCount < promo.usageLimit);
  };

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  };

  const formatDiscount = (promo: Promotion) => {
    if (promo.discountType === "percentage") {
      return `${promo.discountValue}%`;
    }
    return `$${promo.discountValue.toFixed(2)}`;
  };

  if (!token) return null;

  const menuItems = [
    { href: "/dashboard", label: "📊 Dashboard", id: "dashboard" },
    { href: "/dashboard/products", label: "📦 Products", id: "products" },
    { href: "/dashboard/categories", label: "📂 Categories", id: "categories" },
    { href: "/dashboard/promotions", label: "🎟️ Promotions", id: "promotions" },
    { href: "/dashboard/banners", label: "🖼️ Banners", id: "banners" },
    { href: "/dashboard/orders", label: "🛒 Orders", id: "orders" },
    { href: "/dashboard/customers", label: "👥 Customers", id: "customers" },
    { href: "/dashboard/staff", label: "👤 Staff", id: "staff" },
    { href: "/dashboard/import", label: "📥 Import Data", id: "import" },
  ];

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      {/* Sidebar */}
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
                color: item.id === "promotions" ? "#fff" : "#aaa",
                backgroundColor: item.id === "promotions" ? "#007bff" : "transparent",
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

      {/* Main Content */}
      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
          <div>
            <h1 style={{ margin: 0 }}>Promotions & Coupons</h1>
            <p style={{ color: "#666", marginTop: 4 }}>Manage discount codes and promotional offers</p>
          </div>
          <button
            onClick={() => { setShowForm(true); setEditingPromotion(null); }}
            style={{
              padding: "10px 20px",
              backgroundColor: "#28a745",
              color: "white",
              border: "none",
              borderRadius: 4,
              cursor: "pointer",
              fontSize: 14,
              fontWeight: 600,
            }}
          >
            ➕ Create Promotion
          </button>
        </div>

        {error && (
          <div style={{ backgroundColor: "#f8d7da", color: "#721c24", padding: 12, borderRadius: 4, marginBottom: 20 }}>
            {error}
            <button onClick={() => setError(null)} style={{ float: "right", background: "none", border: "none", cursor: "pointer" }}>✕</button>
          </div>
        )}

        {(showForm || editingPromotion) && (
          <PromotionForm
            promotion={editingPromotion}
            onSave={(data) => {
              if (editingPromotion) {
                handleUpdate(editingPromotion.id, data);
              } else {
                handleCreate(data as CreatePromotionRequest);
              }
            }}
            onCancel={() => { setShowForm(false); setEditingPromotion(null); }}
          />
        )}

        {loading ? (
          <p>Loading promotions...</p>
        ) : promotions.length === 0 ? (
          <div style={{ backgroundColor: "white", padding: 40, borderRadius: 8, textAlign: "center" }}>
            <div style={{ fontSize: 48, marginBottom: 16 }}>🎟️</div>
            <h3>No promotions yet</h3>
            <p style={{ color: "#666" }}>Create your first promotion to offer discounts to customers.</p>
          </div>
        ) : (
          <div style={{ backgroundColor: "white", borderRadius: 8, overflow: "hidden", boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <table style={{ width: "100%", borderCollapse: "collapse" }}>
              <thead>
                <tr style={{ backgroundColor: "#f8f9fa" }}>
                  <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Code</th>
                  <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Name</th>
                  <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Discount</th>
                  <th style={{ padding: 12, textAlign: "left", borderBottom: "1px solid #dee2e6" }}>Validity</th>
                  <th style={{ padding: 12, textAlign: "center", borderBottom: "1px solid #dee2e6" }}>Usage</th>
                  <th style={{ padding: 12, textAlign: "center", borderBottom: "1px solid #dee2e6" }}>Status</th>
                  <th style={{ padding: 12, textAlign: "center", borderBottom: "1px solid #dee2e6" }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {promotions.map((promo) => (
                  <tr key={promo.id} style={{ opacity: promo.isActive ? 1 : 0.6 }}>
                    <td style={{ padding: 12, borderBottom: "1px solid #dee2e6" }}>
                      <code style={{ backgroundColor: "#e9ecef", padding: "2px 8px", borderRadius: 4, fontWeight: 600 }}>
                        {promo.code}
                      </code>
                    </td>
                    <td style={{ padding: 12, borderBottom: "1px solid #dee2e6" }}>
                      <div style={{ fontWeight: 500 }}>{promo.name}</div>
                      {promo.description && (
                        <div style={{ fontSize: 12, color: "#666", marginTop: 2 }}>
                          {promo.description.substring(0, 50)}...
                        </div>
                      )}
                    </td>
                    <td style={{ padding: 12, borderBottom: "1px solid #dee2e6" }}>
                      <span style={{
                        display: "inline-block",
                        padding: "4px 8px",
                        borderRadius: 4,
                        backgroundColor: promo.discountType === "percentage" ? "#e3f2fd" : "#fff3e0",
                        color: promo.discountType === "percentage" ? "#1565c0" : "#e65100",
                        fontWeight: 600,
                      }}>
                        {formatDiscount(promo)}
                      </span>
                      {promo.minOrderAmount && (
                        <div style={{ fontSize: 11, color: "#888", marginTop: 2 }}>
                          Min: ${promo.minOrderAmount}
                        </div>
                      )}
                    </td>
                    <td style={{ padding: 12, borderBottom: "1px solid #dee2e6", fontSize: 13 }}>
                      <div>{formatDate(promo.startDate)}</div>
                      <div style={{ color: "#888" }}>to {formatDate(promo.endDate)}</div>
                    </td>
                    <td style={{ padding: 12, borderBottom: "1px solid #dee2e6", textAlign: "center" }}>
                      <span style={{ fontWeight: 600 }}>{promo.usedCount}</span>
                      {promo.usageLimit && <span style={{ color: "#888" }}> / {promo.usageLimit}</span>}
                    </td>
                    <td style={{ padding: 12, borderBottom: "1px solid #dee2e6", textAlign: "center" }}>
                      {isPromotionValid(promo) ? (
                        <span style={{ color: "#28a745", fontWeight: 600 }}>✓ Active</span>
                      ) : promo.isActive ? (
                        <span style={{ color: "#ffc107", fontWeight: 600 }}>⏰ Expired</span>
                      ) : (
                        <span style={{ color: "#dc3545", fontWeight: 600 }}>✗ Inactive</span>
                      )}
                    </td>
                    <td style={{ padding: 12, borderBottom: "1px solid #dee2e6", textAlign: "center" }}>
                      <button
                        onClick={() => handleToggle(promo.id)}
                        style={{ padding: "4px 8px", marginRight: 4, cursor: "pointer", border: "1px solid #ddd", borderRadius: 4, backgroundColor: "white" }}
                        title={promo.isActive ? "Deactivate" : "Activate"}
                      >
                        {promo.isActive ? "🔴" : "🟢"}
                      </button>
                      <button
                        onClick={() => setEditingPromotion(promo)}
                        style={{ padding: "4px 8px", marginRight: 4, cursor: "pointer", border: "1px solid #ddd", borderRadius: 4, backgroundColor: "white" }}
                        title="Edit"
                      >
                        ✏️
                      </button>
                      <button
                        onClick={() => handleDelete(promo.id)}
                        style={{ padding: "4px 8px", cursor: "pointer", border: "1px solid #ddd", borderRadius: 4, backgroundColor: "white", color: "#dc3545" }}
                        title="Delete"
                      >
                        🗑️
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Help Section */}
        <div style={{ marginTop: 30, padding: 20, backgroundColor: "#e7f3ff", borderRadius: 8, border: "1px solid #b6d4fe" }}>
          <h4 style={{ margin: "0 0 8px 0", color: "#084298" }}>💡 How Promotions Work</h4>
          <ul style={{ margin: 0, paddingLeft: 20, color: "#084298", fontSize: 14 }}>
            <li><strong>Percentage:</strong> Discounts a percentage off the order (e.g., 20% off)</li>
            <li><strong>Fixed:</strong> Discounts a fixed amount (e.g., $10 off)</li>
            <li><strong>Min Order:</strong> Requires minimum order amount to apply</li>
            <li><strong>Usage Limit:</strong> Maximum times the code can be used globally</li>
            <li>Customers enter the code at checkout to apply the discount</li>
          </ul>
        </div>
      </main>
    </div>
  );
}

interface PromotionFormProps {
  promotion: Promotion | null;
  onSave: (data: CreatePromotionRequest | Partial<CreatePromotionRequest>) => void;
  onCancel: () => void;
}

function PromotionForm({ promotion, onSave, onCancel }: PromotionFormProps) {
  const [form, setForm] = useState<{
    code: string;
    name: string;
    description: string;
    discountType: "percentage" | "fixed";
    discountValue: number;
    minOrderAmount: string | number;
    maxDiscountAmount: string | number;
    startDate: string;
    endDate: string;
    usageLimit: string | number;
    usageLimitPerCustomer: string | number;
    isActive: boolean;
  }>({
    code: promotion?.code || "",
    name: promotion?.name || "",
    description: promotion?.description || "",
    discountType: promotion?.discountType || "percentage",
    discountValue: promotion?.discountValue || 10,
    minOrderAmount: promotion?.minOrderAmount || "",
    maxDiscountAmount: promotion?.maxDiscountAmount || "",
    startDate: promotion?.startDate ? new Date(promotion.startDate).toISOString().split("T")[0] : new Date().toISOString().split("T")[0],
    endDate: promotion?.endDate ? new Date(promotion.endDate).toISOString().split("T")[0] : new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split("T")[0],
    usageLimit: promotion?.usageLimit || "",
    usageLimitPerCustomer: promotion?.usageLimitPerCustomer || "",
    isActive: promotion?.isActive ?? true,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    const data: CreatePromotionRequest = {
      code: form.code.toUpperCase(),
      name: form.name,
      description: form.description || undefined,
      discountType: form.discountType as "percentage" | "fixed",
      discountValue: Number(form.discountValue),
      minOrderAmount: form.minOrderAmount ? Number(form.minOrderAmount) : undefined,
      maxDiscountAmount: form.maxDiscountAmount ? Number(form.maxDiscountAmount) : undefined,
      startDate: new Date(form.startDate).toISOString(),
      endDate: new Date(form.endDate).toISOString(),
      usageLimit: form.usageLimit ? Number(form.usageLimit) : undefined,
      usageLimitPerCustomer: form.usageLimitPerCustomer ? Number(form.usageLimitPerCustomer) : undefined,
      isActive: form.isActive,
    };

    onSave(data);
  };

  return (
    <div style={{ backgroundColor: "white", padding: 24, borderRadius: 8, marginBottom: 20, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
      <h3 style={{ marginTop: 0 }}>{promotion ? "Edit Promotion" : "Create New Promotion"}</h3>
      <form onSubmit={handleSubmit}>
        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16 }}>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Code *</label>
            <input
              type="text"
              value={form.code}
              onChange={(e) => setForm({ ...form, code: e.target.value.toUpperCase() })}
              required
              disabled={!!promotion}
              placeholder="e.g., SUMMER20"
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box", textTransform: "uppercase" }}
            />
            {promotion && <small style={{ color: "#888" }}>Code cannot be changed after creation</small>}
          </div>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Name *</label>
            <input
              type="text"
              value={form.name}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
              required
              placeholder="e.g., Summer Sale 20% Off"
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
        </div>

        <div style={{ marginTop: 16 }}>
          <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Description</label>
          <textarea
            value={form.description}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
            rows={2}
            placeholder="Optional description for internal reference"
            style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box", resize: "vertical" }}
          />
        </div>

        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: 16, marginTop: 16 }}>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Discount Type *</label>
            <select
              value={form.discountType}
              onChange={(e) => setForm({ ...form, discountType: e.target.value as "percentage" | "fixed" })}
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            >
              <option value="percentage">Percentage (%)</option>
              <option value="fixed">Fixed Amount ($)</option>
            </select>
          </div>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>
              Discount Value * {form.discountType === "percentage" ? "(%)" : "($)"}
            </label>
            <input
              type="number"
              value={form.discountValue}
              onChange={(e) => setForm({ ...form, discountValue: Number(e.target.value) })}
              required
              min="0"
              max={form.discountType === "percentage" ? "100" : undefined}
              step="0.01"
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Min Order Amount ($)</label>
            <input
              type="number"
              value={form.minOrderAmount}
              onChange={(e) => setForm({ ...form, minOrderAmount: e.target.value })}
              min="0"
              step="0.01"
              placeholder="No minimum"
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
        </div>

        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: 16, marginTop: 16 }}>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Start Date *</label>
            <input
              type="date"
              value={form.startDate}
              onChange={(e) => setForm({ ...form, startDate: e.target.value })}
              required
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>End Date *</label>
            <input
              type="date"
              value={form.endDate}
              onChange={(e) => setForm({ ...form, endDate: e.target.value })}
              required
              min={form.startDate}
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
          <div>
            <label style={{ display: "block", marginBottom: 4, fontWeight: 500 }}>Usage Limit</label>
            <input
              type="number"
              value={form.usageLimit}
              onChange={(e) => setForm({ ...form, usageLimit: e.target.value })}
              min="1"
              placeholder="Unlimited"
              style={{ width: "100%", padding: 10, border: "1px solid #ddd", borderRadius: 4, boxSizing: "border-box" }}
            />
          </div>
        </div>

        <div style={{ marginTop: 16 }}>
          <label style={{ display: "flex", alignItems: "center", cursor: "pointer" }}>
            <input
              type="checkbox"
              checked={form.isActive}
              onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
              style={{ marginRight: 8 }}
            />
            <span style={{ fontWeight: 500 }}>Active</span>
            <span style={{ color: "#888", marginLeft: 8 }}>- Promotion can be used by customers</span>
          </label>
        </div>

        <div style={{ display: "flex", gap: 12, marginTop: 24 }}>
          <button
            type="submit"
            style={{ padding: "10px 24px", backgroundColor: "#28a745", color: "white", border: "none", borderRadius: 4, cursor: "pointer", fontWeight: 600 }}
          >
            {promotion ? "Update Promotion" : "Create Promotion"}
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

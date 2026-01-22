"use client";

import { useEffect, useState } from "react";
import {
  getAddresses,
  createAddress,
  updateAddress,
  deleteAddress,
  setDefaultAddress,
  Address,
} from "@/lib/api";

export default function AddressesPage() {
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    fullName: "",
    phone: "",
    addressLine: "",
    ward: "",
    district: "",
    city: "",
    isDefault: false,
  });

  useEffect(() => {
    loadAddresses();
  }, []);

  const loadAddresses = async () => {
    setLoading(true);
    setError(null);
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        setError("Please log in to view your addresses");
        setLoading(false);
        return;
      }
      const data = await getAddresses(token);
      setAddresses(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load addresses");
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      const token = localStorage.getItem("authToken");
      if (!token) return;

      if (editingId) {
        await updateAddress(token, editingId, {
          fullName: formData.fullName,
          phone: formData.phone,
          addressLine: formData.addressLine,
          ward: formData.ward,
          district: formData.district,
          city: formData.city,
        });
      } else {
        await createAddress(token, formData);
      }

      setShowForm(false);
      setEditingId(null);
      resetForm();
      await loadAddresses();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to save address");
    }
  };

  const handleEdit = (address: Address) => {
    setFormData({
      fullName: address.fullName,
      phone: address.phone,
      addressLine: address.addressLine,
      ward: address.ward,
      district: address.district,
      city: address.city,
      isDefault: address.isDefault,
    });
    setEditingId(address.id);
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this address?")) return;
    try {
      const token = localStorage.getItem("authToken");
      if (!token) return;
      await deleteAddress(token, id);
      await loadAddresses();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to delete address");
    }
  };

  const handleSetDefault = async (id: string) => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) return;
      await setDefaultAddress(token, id);
      await loadAddresses();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to set default address");
    }
  };

  const resetForm = () => {
    setFormData({
      fullName: "",
      phone: "",
      addressLine: "",
      ward: "",
      district: "",
      city: "",
      isDefault: false,
    });
    setEditingId(null);
  };

  const handleCancel = () => {
    setShowForm(false);
    setEditingId(null);
    resetForm();
  };

  if (loading) {
    return (
      <div style={{ padding: 24 }} data-testid="addresses-loading">
        Loading addresses...
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: 24, color: "red" }} data-testid="addresses-error">
        Error: {error}
      </div>
    );
  }

  return (
    <main style={{ padding: 24, fontFamily: "system-ui, sans-serif" }} data-testid="addresses-page">
      <h1 data-testid="addresses-title">My Addresses</h1>

      {!showForm && (
        <div style={{ marginTop: 16 }}>
          <button
            onClick={() => setShowForm(true)}
            disabled={addresses.length >= 5}
            data-testid="add-address-button"
            style={{
              padding: "8px 16px",
              backgroundColor: addresses.length >= 5 ? "#ccc" : "#1976d2",
              color: "white",
              border: "none",
              borderRadius: 4,
              cursor: addresses.length >= 5 ? "not-allowed" : "pointer",
            }}
          >
            Add New Address {addresses.length >= 5 && "(Max 5 reached)"}
          </button>
        </div>
      )}

      {showForm && (
        <div
          style={{
            marginTop: 16,
            padding: 16,
            border: "1px solid #ddd",
            borderRadius: 4,
          }}
          data-testid="address-form"
        >
          <h2>{editingId ? "Edit Address" : "Add New Address"}</h2>
          <form onSubmit={handleSubmit}>
            <div style={{ marginBottom: 12 }}>
              <label style={{ display: "block", marginBottom: 4 }}>Full Name *</label>
              <input
                type="text"
                value={formData.fullName}
                onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                required
                data-testid="address-fullname-input"
                style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4 }}
              />
            </div>
            <div style={{ marginBottom: 12 }}>
              <label style={{ display: "block", marginBottom: 4 }}>Phone *</label>
              <input
                type="text"
                value={formData.phone}
                onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                required
                data-testid="address-phone-input"
                style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4 }}
              />
            </div>
            <div style={{ marginBottom: 12 }}>
              <label style={{ display: "block", marginBottom: 4 }}>Address Line *</label>
              <input
                type="text"
                value={formData.addressLine}
                onChange={(e) => setFormData({ ...formData, addressLine: e.target.value })}
                required
                data-testid="address-line-input"
                style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4 }}
              />
            </div>
            <div style={{ marginBottom: 12 }}>
              <label style={{ display: "block", marginBottom: 4 }}>Ward *</label>
              <input
                type="text"
                value={formData.ward}
                onChange={(e) => setFormData({ ...formData, ward: e.target.value })}
                required
                data-testid="address-ward-input"
                style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4 }}
              />
            </div>
            <div style={{ marginBottom: 12 }}>
              <label style={{ display: "block", marginBottom: 4 }}>District *</label>
              <input
                type="text"
                value={formData.district}
                onChange={(e) => setFormData({ ...formData, district: e.target.value })}
                required
                data-testid="address-district-input"
                style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4 }}
              />
            </div>
            <div style={{ marginBottom: 12 }}>
              <label style={{ display: "block", marginBottom: 4 }}>City *</label>
              <input
                type="text"
                value={formData.city}
                onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                required
                data-testid="address-city-input"
                style={{ width: "100%", padding: 8, border: "1px solid #ddd", borderRadius: 4 }}
              />
            </div>
            {!editingId && (
              <div style={{ marginBottom: 12 }}>
                <label style={{ display: "flex", alignItems: "center" }}>
                  <input
                    type="checkbox"
                    checked={formData.isDefault}
                    onChange={(e) => setFormData({ ...formData, isDefault: e.target.checked })}
                    data-testid="address-default-input"
                    style={{ marginRight: 8 }}
                  />
                  Set as default address
                </label>
              </div>
            )}
            <div style={{ display: "flex", gap: 8 }}>
              <button
                type="submit"
                data-testid="address-submit-button"
                style={{
                  padding: "8px 16px",
                  backgroundColor: "#1976d2",
                  color: "white",
                  border: "none",
                  borderRadius: 4,
                  cursor: "pointer",
                }}
              >
                {editingId ? "Update" : "Add"}
              </button>
              <button
                type="button"
                onClick={handleCancel}
                data-testid="address-cancel-button"
                style={{
                  padding: "8px 16px",
                  backgroundColor: "#666",
                  color: "white",
                  border: "none",
                  borderRadius: 4,
                  cursor: "pointer",
                }}
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {addresses.length === 0 && !showForm ? (
        <div style={{ marginTop: 24 }} data-testid="addresses-empty">
          <p>You don&apos;t have any addresses yet.</p>
        </div>
      ) : (
        <div style={{ marginTop: 24 }} data-testid="addresses-list">
          {addresses.map((address) => (
            <div
              key={address.id}
              data-testid={`address-item-${address.id}`}
              style={{
                marginBottom: 16,
                padding: 16,
                border: "1px solid #ddd",
                borderRadius: 4,
                backgroundColor: address.isDefault ? "#f0f8ff" : "white",
              }}
            >
              <div style={{ display: "flex", justifyContent: "space-between", alignItems: "start" }}>
                <div>
                  {address.isDefault && (
                    <span
                      data-testid={`address-default-badge-${address.id}`}
                      style={{
                        display: "inline-block",
                        padding: "2px 8px",
                        backgroundColor: "#1976d2",
                        color: "white",
                        borderRadius: 4,
                        fontSize: 12,
                        marginBottom: 8,
                      }}
                    >
                      Default
                    </span>
                  )}
                  <div data-testid={`address-fullname-${address.id}`} style={{ fontWeight: "bold" }}>
                    {address.fullName}
                  </div>
                  <div data-testid={`address-phone-${address.id}`}>{address.phone}</div>
                  <div data-testid={`address-line-${address.id}`}>{address.addressLine}</div>
                  <div data-testid={`address-location-${address.id}`}>
                    {address.ward}, {address.district}, {address.city}
                  </div>
                </div>
                <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
                  {!address.isDefault && (
                    <button
                      onClick={() => handleSetDefault(address.id)}
                      data-testid={`address-set-default-${address.id}`}
                      style={{
                        padding: "4px 12px",
                        backgroundColor: "#28a745",
                        color: "white",
                        border: "none",
                        borderRadius: 4,
                        cursor: "pointer",
                        fontSize: 14,
                      }}
                    >
                      Set Default
                    </button>
                  )}
                  <button
                    onClick={() => handleEdit(address)}
                    data-testid={`address-edit-${address.id}`}
                    style={{
                      padding: "4px 12px",
                      backgroundColor: "#ffc107",
                      color: "black",
                      border: "none",
                      borderRadius: 4,
                      cursor: "pointer",
                      fontSize: 14,
                    }}
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(address.id)}
                    data-testid={`address-delete-${address.id}`}
                    style={{
                      padding: "4px 12px",
                      backgroundColor: "#dc3545",
                      color: "white",
                      border: "none",
                      borderRadius: 4,
                      cursor: "pointer",
                      fontSize: 14,
                    }}
                  >
                    Delete
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </main>
  );
}

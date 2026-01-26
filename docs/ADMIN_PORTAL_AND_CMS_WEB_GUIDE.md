# Admin Portal & CMS Web Documentation

This comprehensive guide explains how to use and develop features for the Admin Portal and CMS Web applications in this project.

---

## Table of Contents

1. [Overview](#overview)
2. [Getting Started](#getting-started)
3. [Admin Portal Features](#admin-portal-features)
   - [Authentication](#authentication)
   - [Dashboard](#dashboard)
   - [Product Management (CRUD)](#product-management-crud)
   - [Category Management](#category-management)
   - [Banner Management](#banner-management)
   - [Order Management](#order-management)
   - [Promotion Management](#promotion-management)
4. [CMS Web Features](#cms-web-features)
5. [Development Guide](#development-guide)
   - [Project Structure](#project-structure)
   - [API Integration](#api-integration)
   - [Creating New CRUD Features](#creating-new-crud-features)
   - [State Management](#state-management)
   - [Authentication Flow](#authentication-flow)
6. [Code Examples](#code-examples)
7. [Best Practices](#best-practices)

---

## Overview

This CMS (Content Management System) consists of two main applications:

| Application | Location | Purpose |
|------------|----------|---------|
| **Admin Portal** | `apps/admin/` | Backend management interface for administrators |
| **CMS Web** | `apps/web/` | Public-facing website for customers |

### Technology Stack

- **Framework**: Next.js 14+ with App Router
- **Language**: TypeScript
- **Styling**: CSS Modules
- **State Management**: React Context + useState/useEffect hooks
- **API**: RESTful API (backend at `src/Api/`)

---

## Getting Started

### Prerequisites

```bash
# Install Node.js (v18+)
node --version

# Install dependencies
cd apps/admin && npm install
cd apps/web && npm install
```

### Running the Applications

```bash
# Start Admin Portal (development)
cd apps/admin
npm run dev
# Access at: http://localhost:3001

# Start CMS Web (development)
cd apps/web
npm run dev
# Access at: http://localhost:3000

# Start Backend API
cd src/Api
dotnet run
# API runs at: http://localhost:5027
```

### Environment Configuration

Create `.env.local` in each app folder:

```env
# apps/admin/.env.local
NEXT_PUBLIC_API_URL=http://localhost:5027

# apps/web/.env.local
NEXT_PUBLIC_API_URL=http://localhost:5027
```

---

## Admin Portal Features

### Authentication

The Admin Portal uses JWT-based authentication.

#### How to Login

1. Navigate to the Admin Portal URL (e.g., `http://localhost:3001`)
2. Enter your admin credentials (email/phone and password)
3. Click "Login"

#### Authentication Flow (Code Explanation)

```typescript
// File: apps/admin/lib/auth.tsx

// The AuthProvider wraps the entire application
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<User | null>(null);

  // On mount, check for saved session
  useEffect(() => {
    const savedToken = localStorage.getItem("adminToken");
    const savedUser = localStorage.getItem("adminUser");
    if (savedToken && savedUser) {
      setToken(savedToken);
      setUser(JSON.parse(savedUser));
    }
  }, []);

  // Login function saves credentials to localStorage
  const login = (newToken: string, newUser: User) => {
    localStorage.setItem("adminToken", newToken);
    localStorage.setItem("adminUser", JSON.stringify(newUser));
    setToken(newToken);
    setUser(newUser);
  };

  // Logout clears the session
  const logout = () => {
    localStorage.removeItem("adminToken");
    localStorage.removeItem("adminUser");
    setToken(null);
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ token, user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
```

#### Using Authentication in Components

```typescript
// In any component that needs auth
import { useAuth } from "@/lib/auth";

export default function ProtectedPage() {
  const { token, user, logout } = useAuth();
  const router = useRouter();

  // Redirect if not authenticated
  useEffect(() => {
    if (!token) {
      router.push("/");
    }
  }, [token, router]);

  if (!token) return null;

  return <div>Welcome, {user?.fullName}!</div>;
}
```

---

### Dashboard

**Location**: `apps/admin/app/dashboard/page.tsx`

The dashboard displays summary statistics:
- Total Products
- Total Categories
- Total Orders
- Pending Orders
- Total Revenue

---

### Product Management (CRUD)

**Location**: `apps/admin/app/dashboard/products/page.tsx`

#### Feature Overview

| Action | Description |
|--------|-------------|
| **Create** | Add new products with variants, pricing, and images |
| **Read** | View product list with pagination |
| **Update** | Edit product details, status, and variants |
| **Delete** | Remove products from the system |

#### API Functions (apps/admin/lib/api.ts)

```typescript
// 1. GET Products (with pagination)
export async function getProducts(
  token: string, 
  page = 1, 
  pageSize = 20
): Promise<PagedResult<Product>> {
  const response = await fetch(
    `${API_BASE_URL}/products?page=${page}&pageSize=${pageSize}`,
    {
      headers: { Authorization: `Bearer ${token}` },
    }
  );
  if (!response.ok) throw new Error("Failed to fetch products");
  return response.json();
}

// 2. CREATE Product
export async function createProduct(
  token: string, 
  data: CreateProductRequest
): Promise<Product> {
  const response = await fetch(`${API_BASE_URL}/admin/products`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed" }));
    throw new Error(error.message);
  }
  return response.json();
}

// 3. UPDATE Product
export async function updateProduct(
  token: string, 
  id: string, 
  data: Partial<CreateProductRequest>
): Promise<Product> {
  const response = await fetch(`${API_BASE_URL}/admin/products/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error("Failed to update product");
  return response.json();
}

// 4. DELETE Product
export async function deleteProduct(token: string, id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/admin/products/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to delete product");
}
```

#### Product Data Structure

```typescript
interface Product {
  id: string;
  name: string;
  slug: string;
  description: string | null;
  categoryId: string | null;
  images: string | null;
  videos: string | null;
  specifications: string | null;
  isActive: boolean;
  isFeatured: boolean;
  variants: ProductVariant[];
  createdAt: string;
  updatedAt: string;
}

interface ProductVariant {
  id: string;
  sku: string;
  variantName: string | null;
  price: number;
  stockQuantity: number;
}

interface CreateProductRequest {
  name: string;
  slug: string;
  description?: string;
  categoryId?: string;
  isActive: boolean;
  isFeatured: boolean;
  specifications?: string;
  variants: {
    sku: string;
    variantName?: string;
    price: number;
    stockQuantity: number;
  }[];
}
```

#### How to Create a Product (UI Steps)

1. Navigate to **Dashboard → Products**
2. Click the **"➕ Add Product"** button
3. Fill in the product form:
   - **Name**: Product name (required)
   - **Slug**: URL-friendly identifier (auto-generated or custom)
   - **Description**: Product description
   - **Category**: Select from dropdown
   - **Status**: Active/Inactive toggle
   - **Featured**: Mark as featured product
   - **Variants**: Add at least one variant with SKU, price, and stock
4. Click **"Create"** to save

#### Implementation Example (Product CRUD Page)

```typescript
"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth";
import { getProducts, createProduct, deleteProduct, Product } from "@/lib/api";

export default function ProductsPage() {
  const { token } = useAuth();
  const router = useRouter();
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  // Protect the route
  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    loadProducts();
  }, [token, router, page]);

  // Load products from API
  const loadProducts = async () => {
    if (!token) return;
    setLoading(true);
    try {
      const data = await getProducts(token, page, 20);
      setProducts(data.items);
      setTotalCount(data.totalCount);
    } catch (error) {
      console.error("Failed to load products:", error);
    } finally {
      setLoading(false);
    }
  };

  // Handle delete
  const handleDelete = async (id: string, name: string) => {
    if (!token) return;
    if (!confirm(`Delete "${name}"?`)) return;

    try {
      await deleteProduct(token, id);
      loadProducts(); // Refresh the list
    } catch (error) {
      alert("Failed to delete product");
    }
  };

  if (!token) return null;

  return (
    <div>
      <h1>Products</h1>
      {loading ? (
        <p>Loading...</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Price</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {products.map((product) => (
              <tr key={product.id}>
                <td>{product.name}</td>
                <td>${product.variants[0]?.price || 0}</td>
                <td>{product.isActive ? "Active" : "Inactive"}</td>
                <td>
                  <button onClick={() => router.push(`/dashboard/products/${product.id}`)}>
                    Edit
                  </button>
                  <button onClick={() => handleDelete(product.id, product.name)}>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
```

---

### Category Management

**Location**: `apps/admin/app/dashboard/categories/page.tsx`

Categories support hierarchical structure (parent-child relationships).

#### API Functions

```typescript
// Get category tree (hierarchical)
export async function getCategories(token: string): Promise<Category[]> {
  const response = await fetch(`${API_BASE_URL}/categories/tree`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch categories");
  return response.json();
}

// Create category
export async function createCategory(
  token: string, 
  data: { name: string; parentId?: string }
): Promise<Category> {
  const response = await fetch(`${API_BASE_URL}/admin/categories`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error("Failed to create category");
  return response.json();
}

// Delete category (cascades to children)
export async function deleteCategory(token: string, id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/admin/categories/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to delete category");
}
```

#### Category Data Structure

```typescript
interface Category {
  id: string;
  name: string;
  parentId: string | null;
  children?: Category[];  // Nested subcategories
  createdAt: string;
  updatedAt: string;
}
```

#### How to Manage Categories

1. Navigate to **Dashboard → Categories**
2. To **create**: Click "➕ Add Category", enter name, optionally select parent
3. To **delete**: Click "🗑️ Delete" on any category (deletes subcategories too)

---

### Banner Management

**Location**: `apps/admin/app/dashboard/banners/page.tsx`

Banners are displayed in the homepage hero slider on the CMS Web.

#### Banner Data Structure

```typescript
interface Banner {
  id: string;
  title: string;
  subtitle: string;
  description: string;
  buttonText: string;
  buttonLink: string;
  imageUrl: string;
  bgColor: string;  // CSS gradient or color
  isActive: boolean;
  displayOrder: number;
}
```

#### How to Manage Banners

1. Navigate to **Dashboard → Banners**
2. **Create**: Click "➕ Add Banner" and fill in:
   - Title, Subtitle, Description
   - Button Text and Link
   - Background Color (gradient supported)
   - Image URL (optional)
3. **Edit**: Click on any banner to modify
4. **Reorder**: Use ↑↓ buttons to change display order
5. **Toggle**: Enable/disable banners with the toggle switch
6. **Delete**: Remove unwanted banners

#### Banner Implementation (Current: localStorage, Future: API)

```typescript
// Current implementation uses localStorage
const BANNERS_STORAGE_KEY = "cms_banners";

// Load banners
const loadBanners = () => {
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
  }
};

// Save banners
const saveBanners = (newBanners: Banner[]) => {
  setBanners(newBanners);
  localStorage.setItem(BANNERS_STORAGE_KEY, JSON.stringify(newBanners));
};

// Create banner
const handleCreate = (banner: Omit<Banner, "id">) => {
  const newBanner: Banner = {
    ...banner,
    id: Date.now().toString(),
  };
  saveBanners([...banners, newBanner]);
};

// Update banner
const handleUpdate = (updatedBanner: Banner) => {
  const newBanners = banners.map((b) => 
    b.id === updatedBanner.id ? updatedBanner : b
  );
  saveBanners(newBanners);
};

// Delete banner
const handleDelete = (id: string) => {
  saveBanners(banners.filter((b) => b.id !== id));
};

// Toggle active status
const handleToggleActive = (id: string) => {
  const newBanners = banners.map((b) =>
    b.id === id ? { ...b, isActive: !b.isActive } : b
  );
  saveBanners(newBanners);
};
```

---

### Order Management

**Location**: `apps/admin/app/dashboard/orders/page.tsx`

#### Order Status Flow

```
Processing → Shipping → Delivered
     ↓
  Cancelled
```

#### Order Data Structure

```typescript
enum OrderStatus {
  Processing = 0,
  Shipping = 1,
  Delivered = 2,
  Cancelled = 3,
}

interface Order {
  id: string;
  userId: string;
  status: OrderStatus;
  subtotal: number;
  discountAmount: number;
  shippingFee: number;
  total: number;
  shippingFullName: string;
  shippingPhone: string;
  shippingAddressLine: string;
  shippingCity: string;
  items: OrderItem[];
  createdAt: string;
  updatedAt: string;
}

interface OrderItem {
  id: string;
  productId: string;
  variantId: string | null;
  productName: string;
  variantName: string | null;
  sku: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}
```

#### API Functions

```typescript
// Get orders (optionally filter by status)
export async function getOrders(
  token: string, 
  status?: OrderStatus
): Promise<Order[]> {
  const params = status !== undefined ? `?status=${status}` : "";
  const response = await fetch(`${API_BASE_URL}/admin/orders${params}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch orders");
  return response.json();
}

// Update order status
export async function updateOrderStatus(
  token: string, 
  orderId: string, 
  status: OrderStatus
): Promise<void> {
  const response = await fetch(
    `${API_BASE_URL}/admin/orders/${orderId}/status`,
    {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ status }),
    }
  );
  if (!response.ok) throw new Error("Failed to update order status");
}
```

---

### Promotion Management

**Location**: `apps/admin/app/dashboard/promotions/page.tsx`

#### Promotion Data Structure

```typescript
interface Promotion {
  id: string;
  code: string;               // Unique promo code
  name: string;
  description: string | null;
  discountType: "percentage" | "fixed";
  discountValue: number;
  minOrderAmount: number | null;
  maxDiscountAmount: number | null;
  startDate: string;
  endDate: string;
  usageLimit: number | null;
  usageLimitPerCustomer: number | null;
  usedCount: number;
  isActive: boolean;
}
```

#### API Functions

```typescript
// CRUD operations for promotions
export async function getPromotions(
  token: string,
  page = 1,
  pageSize = 20,
  isActive?: boolean
): Promise<PagedResult<Promotion>>;

export async function createPromotion(
  token: string, 
  data: CreatePromotionRequest
): Promise<Promotion>;

export async function updatePromotion(
  token: string, 
  id: string, 
  data: UpdatePromotionRequest
): Promise<Promotion>;

export async function deletePromotion(token: string, id: string): Promise<void>;

export async function togglePromotion(token: string, id: string): Promise<Promotion>;
```

---

## CMS Web Features

The CMS Web (`apps/web/`) is the public-facing website that displays content managed in the Admin Portal.

### Hero Slider Component

**Location**: `apps/web/components/home/HeroSlider.tsx`

This component displays the banners configured in the Admin Portal.

```typescript
'use client';

import React, { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';

interface Slide {
  id: string;
  title: string;
  subtitle: string;
  description: string;
  buttonText: string;
  buttonLink: string;
  image: string;
  bgColor: string;
}

interface HeroSliderProps {
  slides?: Slide[];
  autoPlayInterval?: number;  // Default: 5000ms
}

export default function HeroSlider({ 
  slides = defaultSlides, 
  autoPlayInterval = 5000 
}: HeroSliderProps) {
  const [currentSlide, setCurrentSlide] = useState(0);
  const [isAutoPlaying, setIsAutoPlaying] = useState(true);

  // Navigate to next slide
  const nextSlide = useCallback(() => {
    setCurrentSlide((prev) => (prev + 1) % slides.length);
  }, [slides.length]);

  // Navigate to previous slide
  const prevSlide = useCallback(() => {
    setCurrentSlide((prev) => (prev - 1 + slides.length) % slides.length);
  }, [slides.length]);

  // Auto-play functionality
  useEffect(() => {
    if (!isAutoPlaying) return;
    const interval = setInterval(nextSlide, autoPlayInterval);
    return () => clearInterval(interval);
  }, [isAutoPlaying, nextSlide, autoPlayInterval]);

  return (
    <section className={styles.heroSlider}>
      {/* Slides */}
      {slides.map((slide, index) => (
        <div
          key={slide.id}
          className={`${styles.slide} ${index === currentSlide ? styles.active : ''}`}
          style={{ background: slide.bgColor }}
        >
          <h1>{slide.title}</h1>
          <p>{slide.description}</p>
          <Link href={slide.buttonLink}>{slide.buttonText}</Link>
        </div>
      ))}

      {/* Navigation */}
      <button onClick={prevSlide}>Previous</button>
      <button onClick={nextSlide}>Next</button>

      {/* Dots Indicator */}
      <div className={styles.dotsContainer}>
        {slides.map((_, index) => (
          <button
            key={index}
            className={index === currentSlide ? styles.activeDot : ''}
            onClick={() => setCurrentSlide(index)}
          />
        ))}
      </div>
    </section>
  );
}
```

---

## Development Guide

### Project Structure

```
apps/
├── admin/                    # Admin Portal
│   ├── app/
│   │   ├── layout.tsx        # Root layout with AuthProvider
│   │   ├── page.tsx          # Login page
│   │   └── dashboard/        # Protected admin pages
│   │       ├── page.tsx      # Dashboard home
│   │       ├── products/     # Product management
│   │       ├── categories/   # Category management
│   │       ├── banners/      # Banner management
│   │       ├── orders/       # Order management
│   │       └── promotions/   # Promotion management
│   └── lib/
│       ├── api.ts            # API client functions
│       └── auth.tsx          # Authentication context
│
├── web/                      # Public Website
│   ├── app/
│   │   ├── layout.tsx        # Root layout
│   │   ├── page.tsx          # Homepage
│   │   └── products/         # Product pages
│   └── components/
│       ├── home/
│       │   └── HeroSlider.tsx
│       └── ui/               # Reusable UI components
```

### API Integration

#### Step 1: Define Types

```typescript
// In apps/admin/lib/api.ts

export interface MyEntity {
  id: string;
  name: string;
  // ... other fields
}

export interface CreateMyEntityRequest {
  name: string;
  // ... required fields for creation
}
```

#### Step 2: Create API Functions

```typescript
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5027";

// GET all
export async function getMyEntities(token: string): Promise<MyEntity[]> {
  const response = await fetch(`${API_BASE_URL}/admin/my-entities`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch");
  return response.json();
}

// GET one
export async function getMyEntity(token: string, id: string): Promise<MyEntity> {
  const response = await fetch(`${API_BASE_URL}/admin/my-entities/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch");
  return response.json();
}

// CREATE
export async function createMyEntity(
  token: string, 
  data: CreateMyEntityRequest
): Promise<MyEntity> {
  const response = await fetch(`${API_BASE_URL}/admin/my-entities`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({}));
    throw new Error(error.message || "Failed to create");
  }
  return response.json();
}

// UPDATE
export async function updateMyEntity(
  token: string, 
  id: string, 
  data: Partial<CreateMyEntityRequest>
): Promise<MyEntity> {
  const response = await fetch(`${API_BASE_URL}/admin/my-entities/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error("Failed to update");
  return response.json();
}

// DELETE
export async function deleteMyEntity(token: string, id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/admin/my-entities/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to delete");
}
```

### Creating New CRUD Features

Follow this template to create a new management page:

#### Step 1: Create the Page File

```typescript
// apps/admin/app/dashboard/my-feature/page.tsx
"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth";
import { getMyEntities, createMyEntity, deleteMyEntity, MyEntity } from "@/lib/api";

export default function MyFeaturePage() {
  // 1. Auth and routing
  const { token } = useAuth();
  const router = useRouter();

  // 2. State management
  const [items, setItems] = useState<MyEntity[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingItem, setEditingItem] = useState<MyEntity | null>(null);

  // 3. Load data on mount
  useEffect(() => {
    if (!token) {
      router.push("/");
      return;
    }
    loadItems();
  }, [token, router]);

  // 4. Data loading function
  const loadItems = async () => {
    if (!token) return;
    setLoading(true);
    try {
      const data = await getMyEntities(token);
      setItems(data);
    } catch (error) {
      console.error("Failed to load:", error);
    } finally {
      setLoading(false);
    }
  };

  // 5. CRUD handlers
  const handleCreate = async (data: CreateMyEntityRequest) => {
    if (!token) return;
    try {
      await createMyEntity(token, data);
      setShowForm(false);
      loadItems();
    } catch (error) {
      alert("Failed to create");
    }
  };

  const handleDelete = async (id: string) => {
    if (!token) return;
    if (!confirm("Are you sure?")) return;
    try {
      await deleteMyEntity(token, id);
      loadItems();
    } catch (error) {
      alert("Failed to delete");
    }
  };

  // 6. Guard clause
  if (!token) return null;

  // 7. Render
  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar active="my-feature" />
      <main style={{ flex: 1, padding: 30, backgroundColor: "#f5f5f5" }}>
        <div style={{ 
          display: "flex", 
          justifyContent: "space-between", 
          marginBottom: 20 
        }}>
          <h1>My Feature</h1>
          <button onClick={() => setShowForm(true)}>
            ➕ Add New
          </button>
        </div>

        {showForm && (
          <MyFeatureForm
            onSubmit={handleCreate}
            onCancel={() => setShowForm(false)}
          />
        )}

        {loading ? (
          <p>Loading...</p>
        ) : (
          <ItemList 
            items={items} 
            onEdit={(item) => setEditingItem(item)}
            onDelete={handleDelete}
          />
        )}
      </main>
    </div>
  );
}
```

#### Step 2: Create Form Component

```typescript
interface MyFeatureFormProps {
  initialData?: MyEntity;
  onSubmit: (data: CreateMyEntityRequest) => void;
  onCancel: () => void;
}

function MyFeatureForm({ initialData, onSubmit, onCancel }: MyFeatureFormProps) {
  const [formData, setFormData] = useState({
    name: initialData?.name || "",
    // ... other fields
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <div style={{ 
      backgroundColor: "white", 
      padding: 20, 
      borderRadius: 8,
      marginBottom: 20 
    }}>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: 16 }}>
          <label style={{ display: "block", marginBottom: 4 }}>Name</label>
          <input
            type="text"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            required
            style={{ 
              width: "100%", 
              padding: 10, 
              border: "1px solid #ddd", 
              borderRadius: 4 
            }}
          />
        </div>

        <div style={{ display: "flex", gap: 10 }}>
          <button type="submit" style={{ 
            padding: "10px 20px", 
            backgroundColor: "#28a745", 
            color: "white",
            border: "none",
            borderRadius: 4 
          }}>
            {initialData ? "Update" : "Create"}
          </button>
          <button type="button" onClick={onCancel} style={{
            padding: "10px 20px",
            backgroundColor: "#6c757d",
            color: "white",
            border: "none",
            borderRadius: 4
          }}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
```

#### Step 3: Add to Sidebar

```typescript
// Update the Sidebar component menuItems
const menuItems = [
  { href: "/dashboard", label: "📊 Dashboard", id: "dashboard" },
  { href: "/dashboard/products", label: "📦 Products", id: "products" },
  { href: "/dashboard/categories", label: "📂 Categories", id: "categories" },
  { href: "/dashboard/my-feature", label: "✨ My Feature", id: "my-feature" }, // Add new item
  // ... other items
];
```

---

## Code Examples

### Example 1: Reusable Table Component

```typescript
interface Column<T> {
  key: keyof T;
  header: string;
  render?: (value: T[keyof T], item: T) => React.ReactNode;
}

interface DataTableProps<T> {
  data: T[];
  columns: Column<T>[];
  onEdit?: (item: T) => void;
  onDelete?: (item: T) => void;
}

function DataTable<T extends { id: string }>({ 
  data, 
  columns, 
  onEdit, 
  onDelete 
}: DataTableProps<T>) {
  return (
    <table style={{ width: "100%", borderCollapse: "collapse" }}>
      <thead>
        <tr>
          {columns.map((col) => (
            <th key={String(col.key)} style={{ 
              padding: 12, 
              textAlign: "left", 
              borderBottom: "2px solid #dee2e6" 
            }}>
              {col.header}
            </th>
          ))}
          {(onEdit || onDelete) && <th>Actions</th>}
        </tr>
      </thead>
      <tbody>
        {data.map((item) => (
          <tr key={item.id}>
            {columns.map((col) => (
              <td key={String(col.key)} style={{ 
                padding: 12, 
                borderBottom: "1px solid #dee2e6" 
              }}>
                {col.render 
                  ? col.render(item[col.key], item)
                  : String(item[col.key])
                }
              </td>
            ))}
            {(onEdit || onDelete) && (
              <td>
                {onEdit && (
                  <button onClick={() => onEdit(item)}>Edit</button>
                )}
                {onDelete && (
                  <button onClick={() => onDelete(item)}>Delete</button>
                )}
              </td>
            )}
          </tr>
        ))}
      </tbody>
    </table>
  );
}

// Usage
<DataTable
  data={products}
  columns={[
    { key: "name", header: "Name" },
    { key: "isActive", header: "Status", render: (v) => v ? "Active" : "Inactive" },
    { key: "createdAt", header: "Created", render: (v) => new Date(v as string).toLocaleDateString() },
  ]}
  onEdit={(product) => setEditingProduct(product)}
  onDelete={(product) => handleDelete(product.id)}
/>
```

### Example 2: Pagination Component

```typescript
interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

function Pagination({ currentPage, totalPages, onPageChange }: PaginationProps) {
  const pages = Array.from({ length: totalPages }, (_, i) => i + 1);

  return (
    <div style={{ display: "flex", gap: 8, justifyContent: "center", marginTop: 20 }}>
      <button
        disabled={currentPage === 1}
        onClick={() => onPageChange(currentPage - 1)}
      >
        Previous
      </button>

      {pages.map((page) => (
        <button
          key={page}
          onClick={() => onPageChange(page)}
          style={{
            backgroundColor: page === currentPage ? "#007bff" : "white",
            color: page === currentPage ? "white" : "black",
          }}
        >
          {page}
        </button>
      ))}

      <button
        disabled={currentPage === totalPages}
        onClick={() => onPageChange(currentPage + 1)}
      >
        Next
      </button>
    </div>
  );
}

// Usage
const [page, setPage] = useState(1);
const pageSize = 20;
const totalPages = Math.ceil(totalCount / pageSize);

<Pagination
  currentPage={page}
  totalPages={totalPages}
  onPageChange={setPage}
/>
```

### Example 3: Loading and Error States

```typescript
function useAsyncData<T>(fetchFn: () => Promise<T>, deps: any[] = []) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const refetch = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await fetchFn();
      setData(result);
    } catch (err) {
      setError(err instanceof Error ? err : new Error("Unknown error"));
    } finally {
      setLoading(false);
    }
  }, [fetchFn]);

  useEffect(() => {
    refetch();
  }, deps);

  return { data, loading, error, refetch };
}

// Usage
const { data: products, loading, error, refetch } = useAsyncData(
  () => getProducts(token!, page),
  [token, page]
);

if (loading) return <LoadingSpinner />;
if (error) return <ErrorMessage message={error.message} onRetry={refetch} />;
if (!products) return null;
```

---

## Best Practices

### 1. Authentication

- ✅ Always check for token before API calls
- ✅ Redirect to login page if not authenticated
- ✅ Store tokens securely (consider httpOnly cookies for production)
- ✅ Implement token refresh mechanism

### 2. Error Handling

```typescript
// Always wrap API calls in try-catch
try {
  const result = await apiFunction(token, data);
  // Handle success
} catch (error) {
  // Show user-friendly error message
  const message = error instanceof Error ? error.message : "An error occurred";
  alert(message);
  // Log for debugging
  console.error("API Error:", error);
}
```

### 3. Form Validation

```typescript
const validateForm = (data: FormData): string[] => {
  const errors: string[] = [];
  
  if (!data.name?.trim()) {
    errors.push("Name is required");
  }
  
  if (data.price < 0) {
    errors.push("Price must be positive");
  }
  
  return errors;
};

// In form submit handler
const handleSubmit = (e: React.FormEvent) => {
  e.preventDefault();
  const errors = validateForm(formData);
  
  if (errors.length > 0) {
    alert(errors.join("\n"));
    return;
  }
  
  onSubmit(formData);
};
```

### 4. Confirmation Dialogs

```typescript
// Always confirm destructive actions
const handleDelete = async (id: string, name: string) => {
  const confirmed = confirm(
    `Are you sure you want to delete "${name}"?\n\nThis action cannot be undone.`
  );
  
  if (!confirmed) return;
  
  try {
    await deleteItem(token, id);
    loadItems();
  } catch (error) {
    alert("Failed to delete item");
  }
};
```

### 5. Loading States

```typescript
// Show loading indicators during async operations
const [loading, setLoading] = useState(false);

const handleSubmit = async (data: FormData) => {
  setLoading(true);
  try {
    await createItem(token, data);
    // Success handling
  } catch (error) {
    // Error handling
  } finally {
    setLoading(false);
  }
};

<button type="submit" disabled={loading}>
  {loading ? "Saving..." : "Save"}
</button>
```

### 6. Optimistic Updates

```typescript
// Update UI immediately, revert on error
const handleToggleActive = async (id: string) => {
  // Optimistic update
  const previousItems = [...items];
  setItems(items.map(item => 
    item.id === id ? { ...item, isActive: !item.isActive } : item
  ));

  try {
    await toggleItem(token, id);
  } catch (error) {
    // Revert on error
    setItems(previousItems);
    alert("Failed to update status");
  }
};
```

---

## Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| "Unauthorized" error | Token expired - log out and log in again |
| API connection failed | Check if backend is running at correct URL |
| Changes not appearing | Clear browser cache or hard refresh (Ctrl+Shift+R) |
| Form not submitting | Check browser console for JavaScript errors |

### Debug Tips

```typescript
// Add console logs for debugging
console.log("API Response:", data);
console.log("Current State:", { token, user, items });

// Check network requests in browser DevTools
// Network tab → Filter by XHR/Fetch
```

---

## Contributing

1. Create a feature branch from `main`
2. Follow the coding standards in `reactjs.instructions.md`
3. Test your changes thoroughly
4. Submit a pull request with clear description

---

## Additional Resources

- [React Documentation](https://react.dev)
- [Next.js Documentation](https://nextjs.org/docs)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- Project-specific: See `reactjs.instructions.md` for coding standards

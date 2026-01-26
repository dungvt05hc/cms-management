// Admin Portal API Client
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5027";

// Auth types and functions
export interface AdminLoginRequest {
  emailOrPhone: string;
  password: string;
}

export interface AdminLoginResult {
  accessToken: string;
  expiresIn: number;
  role: string;
}

export async function adminLogin(credentials: AdminLoginRequest): Promise<AdminLoginResult> {
  const response = await fetch(`${API_BASE_URL}/admin/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Login failed" }));
    throw new Error(error.message || "Invalid credentials");
  }

  return response.json();
}

// Category types and functions
export interface Category {
  id: string;
  name: string;
  parentId: string | null;
  children?: Category[];
  createdAt: string;
  updatedAt: string;
}

export async function getCategories(token: string): Promise<Category[]> {
  const response = await fetch(`${API_BASE_URL}/categories/tree`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch categories");
  return response.json();
}

export async function createCategory(token: string, data: { name: string; parentId?: string }): Promise<Category> {
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

export async function updateCategory(token: string, id: string, data: { name: string }): Promise<Category> {
  const response = await fetch(`${API_BASE_URL}/admin/categories/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error("Failed to update category");
  return response.json();
}

export async function deleteCategory(token: string, id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/admin/categories/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to delete category");
}

// Product types and functions
export interface ProductVariant {
  id: string;
  sku: string;
  variantName: string | null;
  price: number;
  stockQuantity: number;
}

export interface Product {
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

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export async function getProducts(token: string, page = 1, pageSize = 20): Promise<PagedResult<Product>> {
  const response = await fetch(`${API_BASE_URL}/products?page=${page}&pageSize=${pageSize}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch products");
  return response.json();
}

export interface CreateProductRequest {
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

export async function createProduct(token: string, data: CreateProductRequest): Promise<Product> {
  const response = await fetch(`${API_BASE_URL}/admin/products`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to create product" }));
    throw new Error(error.message || "Failed to create product");
  }
  return response.json();
}

export async function updateProduct(token: string, id: string, data: Partial<CreateProductRequest>): Promise<Product> {
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

export async function deleteProduct(token: string, id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/admin/products/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to delete product");
}

// Order types and functions
export enum OrderStatus {
  Processing = 0,
  Shipping = 1,
  Delivered = 2,
  Cancelled = 3,
}

export interface OrderItem {
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

export interface Order {
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

export async function getOrders(token: string, status?: OrderStatus): Promise<Order[]> {
  const params = status !== undefined ? `?status=${status}` : "";
  const response = await fetch(`${API_BASE_URL}/admin/orders${params}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch orders");
  return response.json();
}

export async function updateOrderStatus(token: string, orderId: string, status: OrderStatus): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/admin/orders/${orderId}/status`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ status }),
  });
  if (!response.ok) throw new Error("Failed to update order status");
}

// Staff types and functions
export interface StaffUser {
  id: string;
  email: string;
  fullName: string;
  phone: string | null;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export async function getStaffUsers(token: string): Promise<StaffUser[]> {
  const response = await fetch(`${API_BASE_URL}/admin/staff`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch staff users");
  return response.json();
}

export async function createStaffUser(
  token: string,
  data: { email: string; password: string; fullName: string; role: string }
): Promise<StaffUser> {
  const response = await fetch(`${API_BASE_URL}/admin/staff`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error("Failed to create staff user");
  return response.json();
}

// Dashboard stats
export interface DashboardStats {
  totalProducts: number;
  totalCategories: number;
  totalOrders: number;
  pendingOrders: number;
  totalRevenue: number;
}

export async function getDashboardStats(token: string): Promise<DashboardStats> {
  // This would ideally be a dedicated endpoint, but we'll aggregate from existing data
  const [products, categories] = await Promise.all([
    getProducts(token, 1, 1),
    getCategories(token),
  ]);

  const flattenCategories = (cats: Category[]): number => {
    return cats.reduce((acc, cat) => acc + 1 + (cat.children ? flattenCategories(cat.children) : 0), 0);
  };

  return {
    totalProducts: products.totalCount,
    totalCategories: flattenCategories(categories),
    totalOrders: 0,
    pendingOrders: 0,
    totalRevenue: 0,
  };
}

// Promotion types and functions
export interface Promotion {
  id: string;
  code: string;
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
  createdAt: string;
  updatedAt: string;
}

export interface CreatePromotionRequest {
  code: string;
  name: string;
  description?: string;
  discountType: "percentage" | "fixed";
  discountValue: number;
  minOrderAmount?: number;
  maxDiscountAmount?: number;
  startDate: string;
  endDate: string;
  usageLimit?: number;
  usageLimitPerCustomer?: number;
  isActive?: boolean;
}

export interface UpdatePromotionRequest {
  name?: string;
  description?: string;
  discountType?: "percentage" | "fixed";
  discountValue?: number;
  minOrderAmount?: number;
  maxDiscountAmount?: number;
  startDate?: string;
  endDate?: string;
  usageLimit?: number;
  usageLimitPerCustomer?: number;
  isActive?: boolean;
}

export async function getPromotions(
  token: string,
  page = 1,
  pageSize = 20,
  isActive?: boolean
): Promise<PagedResult<Promotion>> {
  const params = new URLSearchParams({
    page: page.toString(),
    pageSize: pageSize.toString(),
  });
  if (isActive !== undefined) {
    params.append("isActive", isActive.toString());
  }

  const response = await fetch(`${API_BASE_URL}/admin/promotions?${params}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch promotions");
  return response.json();
}

export async function getPromotion(token: string, id: string): Promise<Promotion> {
  const response = await fetch(`${API_BASE_URL}/admin/promotions/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to fetch promotion");
  return response.json();
}

export async function createPromotion(token: string, data: CreatePromotionRequest): Promise<Promotion> {
  const response = await fetch(`${API_BASE_URL}/admin/promotions`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to create promotion" }));
    throw new Error(error.message || "Failed to create promotion");
  }
  return response.json();
}

export async function updatePromotion(
  token: string,
  id: string,
  data: UpdatePromotionRequest
): Promise<Promotion> {
  const response = await fetch(`${API_BASE_URL}/admin/promotions/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error("Failed to update promotion");
  return response.json();
}

export async function deletePromotion(token: string, id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/admin/promotions/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to delete promotion");
}

export async function togglePromotion(token: string, id: string): Promise<Promotion> {
  const response = await fetch(`${API_BASE_URL}/admin/promotions/${id}/toggle`, {
    method: "POST",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to toggle promotion");
  return response.json();
}

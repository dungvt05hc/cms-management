// API client for categories
export interface CategoryTreeNode {
  id: string;
  name: string;
  parentId: string | null;
  children: CategoryTreeNode[];
}

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

export interface CursorPagedResult<T> {
  items: T[];
  nextCursor: string | null;
  hasMore: boolean;
}

export interface GetProductsOptions {
  category?: string;
  categoryId?: string;
  q?: string;
  featured?: boolean;
  sort?: "priceAsc" | "priceDesc";
  cursor?: string;
  limit?: number;
}

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

export async function getCategoryTree(): Promise<CategoryTreeNode[]> {
  const response = await fetch(`${API_BASE_URL}/categories/tree`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch category tree");
  }

  return response.json();
}

export async function getFeaturedProducts(limit: number = 10): Promise<Product[]> {
  const response = await fetch(`${API_BASE_URL}/products?featured=true&pageSize=${limit}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch featured products");
  }

  const result: PagedResult<Product> = await response.json();
  return result.items;
}

export async function getProducts(options: GetProductsOptions): Promise<CursorPagedResult<Product>> {
  const params = new URLSearchParams();

  if (options.category) params.append("category", options.category);
  if (options.categoryId) params.append("categoryId", options.categoryId);
  if (options.q) params.append("q", options.q);
  if (options.featured !== undefined) params.append("featured", options.featured.toString());
  if (options.sort) params.append("sort", options.sort);
  if (options.cursor) params.append("cursor", options.cursor);
  if (options.limit) params.append("limit", options.limit.toString());

  const response = await fetch(`${API_BASE_URL}/products?${params.toString()}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch products");
  }

  return response.json();
}

export async function getProductBySlug(slug: string): Promise<Product> {
  const response = await fetch(`${API_BASE_URL}/products/${slug}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    if (response.status === 404) {
      throw new Error("Product not found");
    }
    throw new Error("Failed to fetch product");
  }

  return response.json();
}

export async function getProductSuggestions(slug: string, limit: number = 4): Promise<Product[]> {
  const response = await fetch(`${API_BASE_URL}/products/${slug}/suggestions?limit=${limit}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch product suggestions");
  }

  return response.json();
}

export interface SearchSuggestion {
  id: string;
  name: string;
  slug: string;
  images: string | null;
}

export async function searchSuggestions(q: string, limit: number = 10): Promise<SearchSuggestion[]> {
  const params = new URLSearchParams();
  if (q) params.append("q", q);
  if (limit) params.append("limit", limit.toString());

  const response = await fetch(`${API_BASE_URL}/search/suggest?${params.toString()}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch search suggestions");
  }

  return response.json();
}

export interface CartItem {
  id: string;
  productId: string;
  productName: string;
  productSlug: string;
  variantId: string | null;
  variantName: string | null;
  price: number;
  quantity: number;
  selected: boolean;
  lineTotal: number;
}

export interface Cart {
  id: string;
  userId: string;
  items: CartItem[];
  subtotal: number;
}

export async function getCart(token: string): Promise<Cart> {
  const response = await fetch(`${API_BASE_URL}/cart`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch cart");
  }

  return response.json();
}

export async function addCartItem(
  token: string,
  productId: string,
  variantId: string | null,
  quantity: number
): Promise<CartItem> {
  const response = await fetch(`${API_BASE_URL}/cart/items`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ productId, variantId, quantity }),
  });

  if (!response.ok) {
    throw new Error("Failed to add item to cart");
  }

  return response.json();
}

export async function updateCartItem(
  token: string,
  itemId: string,
  updates: { quantity?: number; variantId?: string | null; selected?: boolean }
): Promise<CartItem> {
  const response = await fetch(`${API_BASE_URL}/cart/items/${itemId}`, {
    method: "PATCH",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(updates),
  });

  if (!response.ok) {
    throw new Error("Failed to update cart item");
  }

  return response.json();
}

export async function deleteCartItem(token: string, itemId: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/cart/items/${itemId}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error("Failed to delete cart item");
  }
}

// Address API types and functions

export interface Address {
  id: string;
  userId: string;
  fullName: string;
  phone: string;
  addressLine: string;
  ward: string;
  district: string;
  city: string;
  isDefault: boolean;
  createdAt: string;
  updatedAt: string;
}

export async function getAddresses(token: string): Promise<Address[]> {
  const response = await fetch(`${API_BASE_URL}/me/addresses`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch addresses");
  }

  return response.json();
}

export async function createAddress(
  token: string,
  address: {
    fullName: string;
    phone: string;
    addressLine: string;
    ward: string;
    district: string;
    city: string;
    isDefault: boolean;
  }
): Promise<Address> {
  const response = await fetch(`${API_BASE_URL}/me/addresses`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(address),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to create address" }));
    throw new Error(error.message || "Failed to create address");
  }

  return response.json();
}

export async function updateAddress(
  token: string,
  id: string,
  address: {
    fullName: string;
    phone: string;
    addressLine: string;
    ward: string;
    district: string;
    city: string;
  }
): Promise<Address> {
  const response = await fetch(`${API_BASE_URL}/me/addresses/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(address),
  });

  if (!response.ok) {
    throw new Error("Failed to update address");
  }

  return response.json();
}

export async function deleteAddress(token: string, id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/me/addresses/${id}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error("Failed to delete address");
  }
}

export async function setDefaultAddress(token: string, id: string): Promise<Address> {
  const response = await fetch(`${API_BASE_URL}/me/addresses/${id}/default`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error("Failed to set default address");
  }

  return response.json();
}


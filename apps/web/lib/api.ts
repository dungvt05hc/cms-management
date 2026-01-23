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

// Checkout API types and functions

export interface CheckoutTotals {
  subtotal: number;
  discountAmount: number;
  shippingFee: number;
  shippingDiscount: number;
  total: number;
  discountVoucherCode: string | null;
  shippingVoucherCode: string | null;
}

export async function applyVoucher(
  token: string,
  discountCode?: string,
  shippingCode?: string
): Promise<CheckoutTotals> {
  const response = await fetch(`${API_BASE_URL}/checkout/apply-voucher`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ discountCode, shippingCode }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to apply voucher" }));
    throw new Error(error.message || "Failed to apply voucher");
  }

  return response.json();
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

// Shipping API types and functions

export interface ShippingMethod {
  id: string;
  code: string;
  name: string;
  description: string | null;
  carriers: ShippingCarrier[];
}

export interface ShippingCarrier {
  id: string;
  code: string;
  name: string;
  description: string | null;
  supportsCOD: boolean;
}

export async function getShippingMethods(): Promise<ShippingMethod[]> {
  const response = await fetch(`${API_BASE_URL}/shipping/methods`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch shipping methods");
  }

  return response.json();
}

// Checkout Submit API types and functions

export enum PaymentMethod {
  COD = 0,
  Payoo = 1,
}

export interface CheckoutSubmitResult {
  orderId?: string;
  paymentUrl?: string;
  paymentReference?: string;
}

export async function checkoutPreview(
  token: string,
  addressId: string,
  shippingMethodCode: string,
  shippingCarrierCode: string,
  discountCode?: string,
  shippingCode?: string
): Promise<CheckoutTotals> {
  const response = await fetch(`${API_BASE_URL}/checkout/preview`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      addressId,
      shippingMethodCode,
      shippingCarrierCode,
      discountCode,
      shippingCode,
    }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to preview checkout" }));
    throw new Error(error.message || "Failed to preview checkout");
  }

  return response.json();
}

export async function checkoutSubmit(
  token: string,
  addressId: string,
  shippingMethodCode: string,
  shippingCarrierCode: string,
  paymentMethod: PaymentMethod,
  discountCode?: string,
  shippingCode?: string,
  notes?: string
): Promise<CheckoutSubmitResult> {
  const response = await fetch(`${API_BASE_URL}/checkout/submit`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      addressId,
      shippingMethodCode,
      shippingCarrierCode,
      paymentMethod,
      discountCode,
      shippingCode,
      notes,
    }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to submit checkout" }));
    throw new Error(error.message || "Failed to submit checkout");
  }

  return response.json();
}

// Orders API types and functions

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
  status: OrderStatus;
  paymentMethod: PaymentMethod;
  subtotal: number;
  discountAmount: number;
  shippingFee: number;
  shippingDiscount: number;
  total: number;
  shippingFullName: string;
  shippingPhone: string;
  shippingAddressLine: string;
  shippingWard: string;
  shippingDistrict: string;
  shippingCity: string;
  shippingMethodCode: string;
  shippingCarrierCode: string;
  discountVoucherCode: string | null;
  shippingVoucherCode: string | null;
  notes: string | null;
  items: OrderItem[];
  createdAt: string;
  updatedAt: string;
}

export async function getOrders(token: string, status?: OrderStatus): Promise<Order[]> {
  const params = new URLSearchParams();
  if (status !== undefined) {
    params.append("status", status.toString());
  }

  const response = await fetch(`${API_BASE_URL}/me/orders?${params.toString()}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch orders");
  }

  return response.json();
}

export async function getOrderById(token: string, orderId: string): Promise<Order> {
  const response = await fetch(`${API_BASE_URL}/me/orders/${orderId}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    if (response.status === 404) {
      throw new Error("Order not found");
    }
    throw new Error("Failed to fetch order");
  }

  return response.json();
}

export async function cancelOrder(
  token: string,
  orderId: string,
  reasonCode: string,
  note?: string
): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/me/orders/${orderId}/cancel`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ reasonCode, note }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to cancel order" }));
    throw new Error(error.message || "Failed to cancel order");
  }
}

export async function confirmOrderReceived(token: string, orderId: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/me/orders/${orderId}/confirm-received`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({}),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to confirm order" }));
    throw new Error(error.message || "Failed to confirm order");
  }
}

export async function reorderOrder(token: string, orderId: string): Promise<{ cartId: string }> {
  const response = await fetch(`${API_BASE_URL}/me/orders/${orderId}/reorder`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({}),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to reorder" }));
    throw new Error(error.message || "Failed to reorder");
  }

  return response.json();
}

// Notifications API types and functions

export interface Notification {
  id: string;
  type: string;
  title: string;
  message: string;
  data: string | null;
  isRead: boolean;
  createdAt: string;
}

export async function getNotifications(
  token: string,
  type: string = "all"
): Promise<Notification[]> {
  const response = await fetch(`${API_BASE_URL}/me/notifications?type=${type}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch notifications");
  }

  return response.json();
}

// Device Tokens API types and functions

export interface DeviceToken {
  id: string;
  token: string;
  deviceType: string | null;
  createdAt: string;
}

export async function getDevices(token: string): Promise<DeviceToken[]> {
  const response = await fetch(`${API_BASE_URL}/me/devices`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch device tokens");
  }

  return response.json();
}

export async function registerDevice(
  token: string,
  deviceToken: string,
  deviceType?: string
): Promise<{ id: string }> {
  const response = await fetch(`${API_BASE_URL}/me/devices`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ token: deviceToken, deviceType }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Failed to register device" }));
    throw new Error(error.message || "Failed to register device");
  }

  return response.json();
}

export async function deleteDevice(token: string, deviceId: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/me/devices/${deviceId}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error("Failed to delete device token");
  }
}

// Articles (CMS) API types and functions

export interface ArticleCategory {
  id: string;
  name: string;
  slug: string;
  description: string | null;
  displayOrder: number;
}

export interface Article {
  id: string;
  title: string;
  slug: string;
  summary: string | null;
  content: string;
  categoryId: string | null;
  categoryName: string | null;
  categorySlug: string | null;
  thumbnailUrl: string | null;
  isPublished: boolean;
  publishedAt: string | null;
  viewCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface ArticleListItem {
  id: string;
  title: string;
  slug: string;
  summary: string | null;
  categoryName: string | null;
  categorySlug: string | null;
  thumbnailUrl: string | null;
  publishedAt: string | null;
  viewCount: number;
}

// TODO: Implement when backend articles API is ready
export async function getArticleCategories(): Promise<ArticleCategory[]> {
  // Stub for future API integration
  return Promise.resolve([
    {
      id: "1",
      name: "News",
      slug: "news",
      description: "Latest news and updates",
      displayOrder: 1,
    },
    {
      id: "2",
      name: "Guides",
      slug: "guides",
      description: "How-to guides and tutorials",
      displayOrder: 2,
    },
    {
      id: "3",
      name: "Help",
      slug: "help",
      description: "Help and support articles",
      displayOrder: 3,
    },
  ]);
}

// TODO: Implement when backend articles API is ready
export async function getArticles(
  categorySlug?: string,
  page: number = 1,
  pageSize: number = 12
): Promise<PagedResult<ArticleListItem>> {
  // Stub for future API integration
  const items: ArticleListItem[] = Array.from({ length: 6 }, (_, i) => ({
    id: `article-${i + 1}`,
    title: `Sample Article ${i + 1}${categorySlug ? ` in ${categorySlug}` : ""}`,
    slug: `sample-article-${i + 1}`,
    summary: `This is a summary of article ${i + 1}. Lorem ipsum dolor sit amet, consectetur adipiscing elit.`,
    categoryName: categorySlug || "News",
    categorySlug: categorySlug || "news",
    thumbnailUrl: null,
    publishedAt: new Date(Date.now() - i * 86400000).toISOString(),
    viewCount: Math.floor(Math.random() * 1000),
  }));

  return Promise.resolve({
    items,
    totalCount: items.length,
    page,
    pageSize,
  });
}

// TODO: Implement when backend articles API is ready
export async function getArticleBySlug(slug: string): Promise<Article> {
  // Stub for future API integration
  return Promise.resolve({
    id: "1",
    title: `Article: ${slug}`,
    slug,
    summary: "This is a sample article summary.",
    content: `<h2>Article Content</h2>
<p>This is the full content of the article with slug: <strong>${slug}</strong>.</p>
<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</p>
<p>Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</p>`,
    categoryId: "1",
    categoryName: "News",
    categorySlug: "news",
    thumbnailUrl: null,
    isPublished: true,
    publishedAt: new Date().toISOString(),
    viewCount: 42,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  });
}

// TODO: Implement when backend articles API is ready
export async function getFeaturedArticles(limit: number = 3): Promise<ArticleListItem[]> {
  // Stub for future API integration
  const items: ArticleListItem[] = Array.from({ length: Math.min(limit, 3) }, (_, i) => ({
    id: `featured-${i + 1}`,
    title: `Featured Article ${i + 1}`,
    slug: `featured-article-${i + 1}`,
    summary: `This is a featured article summary ${i + 1}.`,
    categoryName: "News",
    categorySlug: "news",
    thumbnailUrl: null,
    publishedAt: new Date(Date.now() - i * 86400000).toISOString(),
    viewCount: Math.floor(Math.random() * 1000),
  }));

  return Promise.resolve(items);
}


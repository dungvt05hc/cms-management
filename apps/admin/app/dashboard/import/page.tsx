"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/lib/auth";
import { createProduct, createCategory, getCategories, Category } from "@/lib/api";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5027";

// Sample product data for import
const SAMPLE_PRODUCTS = [
  {
    name: "Google Pixel 8 Pro",
    slug: "google-pixel-8-pro",
    description: "Google's flagship phone with advanced AI features, Tensor G3 chip, and exceptional camera system.",
    category: "Smartphones",
    isFeatured: true,
    specifications: '{"screen": "6.7-inch LTPO OLED", "chip": "Google Tensor G3", "camera": "50MP + 48MP + 48MP", "battery": "5050mAh"}',
    variants: [
      { sku: "PIX8P-128-OBS", variantName: "128GB Obsidian", price: 999, stockQuantity: 40 },
      { sku: "PIX8P-256-BAY", variantName: "256GB Bay", price: 1059, stockQuantity: 35 },
    ],
  },
  {
    name: "OnePlus 12",
    slug: "oneplus-12",
    description: "Flagship killer with Snapdragon 8 Gen 3, 100W fast charging, and Hasselblad cameras.",
    category: "Smartphones",
    isFeatured: false,
    specifications: '{"screen": "6.82-inch LTPO AMOLED", "processor": "Snapdragon 8 Gen 3", "camera": "50MP + 64MP + 48MP", "battery": "5400mAh"}',
    variants: [
      { sku: "OP12-256-GRN", variantName: "256GB Flowy Emerald", price: 799, stockQuantity: 50 },
      { sku: "OP12-512-BLK", variantName: "512GB Silky Black", price: 899, stockQuantity: 30 },
    ],
  },
  {
    name: "ASUS ROG Zephyrus G14",
    slug: "asus-rog-zephyrus-g14",
    description: "Powerful gaming laptop with AMD Ryzen 9 and RTX 4090 in a portable 14-inch form factor.",
    category: "Laptops",
    isFeatured: true,
    specifications: '{"screen": "14-inch QHD+ 165Hz", "processor": "AMD Ryzen 9 8945HS", "graphics": "RTX 4090", "memory": "32GB DDR5"}',
    variants: [
      { sku: "ROG-G14-32-1TB", variantName: "32GB / 1TB SSD", price: 2499, stockQuantity: 15 },
    ],
  },
  {
    name: "Lenovo ThinkPad X1 Carbon Gen 11",
    slug: "lenovo-thinkpad-x1-carbon-gen11",
    description: "Premium business ultrabook with Intel Core Ultra processors and legendary ThinkPad reliability.",
    category: "Laptops",
    isFeatured: false,
    specifications: '{"screen": "14-inch 2.8K OLED", "processor": "Intel Core Ultra 7", "memory": "32GB LPDDR5", "battery": "Up to 15 hours"}',
    variants: [
      { sku: "X1C-32-512", variantName: "32GB / 512GB", price: 1849, stockQuantity: 25 },
      { sku: "X1C-32-1TB", variantName: "32GB / 1TB", price: 2049, stockQuantity: 20 },
    ],
  },
  {
    name: "Bose QuietComfort Ultra",
    slug: "bose-quietcomfort-ultra",
    description: "Premium noise-canceling headphones with immersive spatial audio and world-class comfort.",
    category: "Headphones",
    isFeatured: true,
    specifications: '{"type": "Over-ear", "anc": "CustomTune ANC", "battery": "Up to 24 hours", "connectivity": "Bluetooth 5.3"}',
    variants: [
      { sku: "BOSE-QCU-BLK", variantName: "Black", price: 429, stockQuantity: 45 },
      { sku: "BOSE-QCU-WHT", variantName: "White Smoke", price: 429, stockQuantity: 40 },
    ],
  },
  {
    name: "Sennheiser Momentum 4",
    slug: "sennheiser-momentum-4",
    description: "Audiophile-grade wireless headphones with exceptional sound quality and 60-hour battery life.",
    category: "Headphones",
    isFeatured: false,
    specifications: '{"type": "Over-ear", "drivers": "42mm", "battery": "Up to 60 hours", "codec": "aptX, AAC, SBC"}',
    variants: [
      { sku: "SENN-M4-BLK", variantName: "Black", price: 349, stockQuantity: 35 },
      { sku: "SENN-M4-WHT", variantName: "White", price: 349, stockQuantity: 30 },
    ],
  },
  {
    name: "Nike Air Max 270",
    slug: "nike-air-max-270",
    description: "Iconic lifestyle sneakers with the tallest Air unit yet for unmatched comfort.",
    category: "Men's Clothing",
    isFeatured: true,
    specifications: '{"type": "Sneakers", "material": "Mesh upper", "sole": "Air Max unit", "style": "Lifestyle"}',
    variants: [
      { sku: "AM270-BLK-9", variantName: "Black / Size 9", price: 150, stockQuantity: 100 },
      { sku: "AM270-BLK-10", variantName: "Black / Size 10", price: 150, stockQuantity: 100 },
      { sku: "AM270-WHT-9", variantName: "White / Size 9", price: 150, stockQuantity: 80 },
      { sku: "AM270-WHT-10", variantName: "White / Size 10", price: 150, stockQuantity: 80 },
    ],
  },
  {
    name: "Levi's 501 Original Jeans",
    slug: "levis-501-original-jeans",
    description: "The original blue jean since 1873. Straight leg, button fly, iconic style.",
    category: "Men's Clothing",
    isFeatured: false,
    specifications: '{"fit": "Straight", "rise": "Regular", "material": "100% Cotton", "closure": "Button fly"}',
    variants: [
      { sku: "501-INDIGO-32", variantName: "Indigo / 32x32", price: 69.50, stockQuantity: 150 },
      { sku: "501-INDIGO-34", variantName: "Indigo / 34x32", price: 69.50, stockQuantity: 150 },
      { sku: "501-BLACK-32", variantName: "Black / 32x32", price: 69.50, stockQuantity: 120 },
    ],
  },
  {
    name: "Dyson V15 Detect",
    slug: "dyson-v15-detect",
    description: "Most powerful cordless vacuum with laser dust detection and intelligent suction optimization.",
    category: "Home & Living",
    isFeatured: true,
    specifications: '{"power": "240AW suction", "runtime": "Up to 60 mins", "bin": "0.76L", "weight": "3.1kg"}',
    variants: [
      { sku: "V15-DETECT-ABS", variantName: "Absolute", price: 749, stockQuantity: 30 },
    ],
  },
  {
    name: "Philips Hue Starter Kit",
    slug: "philips-hue-starter-kit",
    description: "Smart lighting starter kit with 3 color bulbs and Hue Bridge for whole-home automation.",
    category: "Home & Living",
    isFeatured: false,
    specifications: '{"bulbs": "3x A19 Color", "lumens": "800 per bulb", "connectivity": "Zigbee + Bridge", "voice": "Alexa, Google, Siri"}',
    variants: [
      { sku: "HUE-STARTER-3", variantName: "3-Bulb Starter Kit", price: 199, stockQuantity: 60 },
    ],
  },
];

interface ScrapedProduct {
  name: string;
  description?: string;
  price: number;
  images?: string[];
  selected?: boolean;
}

export default function ImportPage() {
  const { token } = useAuth();
  const router = useRouter();
  const [importing, setImporting] = useState(false);
  const [progress, setProgress] = useState<string[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);

  // Web Scraper state
  const [scrapeUrl, setScrapeUrl] = useState("");
  const [scraping, setScraping] = useState(false);
  const [scrapedProducts, setScrapedProducts] = useState<ScrapedProduct[]>([]);
  const [scrapeError, setScrapeError] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("");

  // Customer/Order seeding state
  const [seedingCustomers, setSeedingCustomers] = useState(false);
  const [seedingOrders, setSeedingOrders] = useState(false);

  useEffect(() => {
    if (token) {
      loadCategories();
    }
  }, [token]);

  const loadCategories = async () => {
    if (!token) return;
    const cats = await getCategories(token);
    setCategories(cats);
    return cats;
  };

  const findCategoryId = (cats: Category[], name: string): string | undefined => {
    for (const cat of cats) {
      if (cat.name === name) return cat.id;
      if (cat.children) {
        const found = findCategoryId(cat.children, name);
        if (found) return found;
      }
    }
    return undefined;
  };

  const flattenCategories = (cats: Category[], prefix = ""): { id: string; name: string }[] => {
    return cats.flatMap((cat) => [
      { id: cat.id, name: prefix + cat.name },
      ...(cat.children ? flattenCategories(cat.children, prefix + "— ") : []),
    ]);
  };

  const handleImportSample = async () => {
    if (!token) return;
    setImporting(true);
    setProgress([]);

    try {
      const cats = await loadCategories();
      if (!cats) throw new Error("Failed to load categories");

      for (const product of SAMPLE_PRODUCTS) {
        try {
          const categoryId = findCategoryId(cats, product.category);
          
          await createProduct(token, {
            name: product.name,
            slug: product.slug,
            description: product.description,
            categoryId,
            isActive: true,
            isFeatured: product.isFeatured,
            specifications: product.specifications,
            variants: product.variants,
          });
          
          setProgress(prev => [...prev, `✅ Imported: ${product.name}`]);
        } catch (error) {
          setProgress(prev => [...prev, `❌ Failed: ${product.name} - ${error instanceof Error ? error.message : 'Unknown error'}`]);
        }
      }

      setProgress(prev => [...prev, "", "🎉 Import completed!"]);
    } catch (error) {
      setProgress(prev => [...prev, `❌ Import failed: ${error instanceof Error ? error.message : 'Unknown error'}`]);
    } finally {
      setImporting(false);
    }
  };

  // Web Scraper handlers
  const handleScrape = async () => {
    if (!token || !scrapeUrl) return;
    setScraping(true);
    setScrapeError("");
    setScrapedProducts([]);

    try {
      const response = await fetch(`${API_BASE_URL}/admin/scraper/scrape`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ url: scrapeUrl }),
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.message || "Failed to scrape URL");
      }

      if (data.products && data.products.length > 0) {
        setScrapedProducts(data.products.map((p: ScrapedProduct) => ({ ...p, selected: true })));
      } else {
        setScrapeError("No products found on this page. Try a product listing or detail page.");
      }
    } catch (error) {
      setScrapeError(error instanceof Error ? error.message : "Failed to scrape URL");
    } finally {
      setScraping(false);
    }
  };

  const handleImportScraped = async () => {
    if (!token) return;
    const selectedProducts = scrapedProducts.filter(p => p.selected);
    if (selectedProducts.length === 0) return;

    setImporting(true);
    setProgress([]);

    try {
      const response = await fetch(`${API_BASE_URL}/admin/scraper/import`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          categoryId: selectedCategory || null,
          products: selectedProducts,
        }),
      });

      const data = await response.json();

      if (response.ok) {
        setProgress([`✅ ${data.message}`]);
        if (data.errors && data.errors.length > 0) {
          data.errors.forEach((err: string) => setProgress(prev => [...prev, `⚠️ ${err}`]));
        }
        setScrapedProducts([]);
        setScrapeUrl("");
      } else {
        throw new Error(data.message || "Import failed");
      }
    } catch (error) {
      setProgress([`❌ ${error instanceof Error ? error.message : "Import failed"}`]);
    } finally {
      setImporting(false);
    }
  };

  const toggleProductSelection = (index: number) => {
    setScrapedProducts(prev => prev.map((p, i) => 
      i === index ? { ...p, selected: !p.selected } : p
    ));
  };

  // Customer & Order seeding
  const handleSeedCustomers = async () => {
    if (!token) return;
    setSeedingCustomers(true);
    setProgress([]);

    try {
      const response = await fetch(`${API_BASE_URL}/admin/customers/seed`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` },
      });

      const data = await response.json();
      setProgress([response.ok ? `✅ ${data.message}` : `❌ ${data.message}`]);
    } catch (error) {
      setProgress([`❌ ${error instanceof Error ? error.message : "Failed to seed customers"}`]);
    } finally {
      setSeedingCustomers(false);
    }
  };

  const handleSeedOrders = async () => {
    if (!token) return;
    setSeedingOrders(true);
    setProgress([]);

    try {
      const response = await fetch(`${API_BASE_URL}/admin/orders/seed`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` },
      });

      const data = await response.json();
      setProgress([response.ok ? `✅ ${data.message}` : `❌ ${data.message}`]);
    } catch (error) {
      setProgress([`❌ ${error instanceof Error ? error.message : "Failed to seed orders"}`]);
    } finally {
      setSeedingOrders(false);
    }
  };

  if (!token) {
    router.push("/");
    return null;
  }

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar active="import" />

      <main style={{ flex: 1, backgroundColor: "#f5f5f5", padding: 30 }}>
        <h1 style={{ marginBottom: 20 }}>Import Data</h1>

        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(300px, 1fr))", gap: 20, marginBottom: 30 }}>
          {/* Sample Products Import */}
          <div style={{ backgroundColor: "white", padding: 20, borderRadius: 8, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <h3 style={{ marginBottom: 12 }}>📦 Import Sample Products</h3>
            <p style={{ color: "#666", marginBottom: 16, fontSize: 14 }}>
              Import {SAMPLE_PRODUCTS.length} sample products including smartphones, laptops, headphones, clothing, and home items.
            </p>
            <button
              onClick={handleImportSample}
              disabled={importing}
              style={{
                padding: "10px 20px",
                backgroundColor: importing ? "#ccc" : "#28a745",
                color: "white",
                border: "none",
                borderRadius: 4,
                cursor: importing ? "not-allowed" : "pointer",
              }}
            >
              {importing ? "Importing..." : "Import Sample Data"}
            </button>
          </div>

          {/* Customer Seeding */}
          <div style={{ backgroundColor: "white", padding: 20, borderRadius: 8, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <h3 style={{ marginBottom: 12 }}>👥 Seed Sample Customers</h3>
            <p style={{ color: "#666", marginBottom: 16, fontSize: 14 }}>
              Create 5 sample customer accounts with addresses for testing. Password: Customer123!
            </p>
            <button
              onClick={handleSeedCustomers}
              disabled={seedingCustomers}
              style={{
                padding: "10px 20px",
                backgroundColor: seedingCustomers ? "#ccc" : "#17a2b8",
                color: "white",
                border: "none",
                borderRadius: 4,
                cursor: seedingCustomers ? "not-allowed" : "pointer",
              }}
            >
              {seedingCustomers ? "Creating..." : "Create Sample Customers"}
            </button>
          </div>

          {/* Order Seeding */}
          <div style={{ backgroundColor: "white", padding: 20, borderRadius: 8, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <h3 style={{ marginBottom: 12 }}>🛒 Seed Sample Orders</h3>
            <p style={{ color: "#666", marginBottom: 16, fontSize: 14 }}>
              Create sample orders for existing customers. Requires customers and products to exist first.
            </p>
            <button
              onClick={handleSeedOrders}
              disabled={seedingOrders}
              style={{
                padding: "10px 20px",
                backgroundColor: seedingOrders ? "#ccc" : "#6f42c1",
                color: "white",
                border: "none",
                borderRadius: 4,
                cursor: seedingOrders ? "not-allowed" : "pointer",
              }}
            >
              {seedingOrders ? "Creating..." : "Create Sample Orders"}
            </button>
          </div>
        </div>

        {/* Web Scraper Section */}
        <div style={{ backgroundColor: "white", padding: 24, borderRadius: 8, boxShadow: "0 2px 4px rgba(0,0,0,0.1)", marginBottom: 20 }}>
          <h3 style={{ marginBottom: 16 }}>🌐 Web Scraper</h3>
          <p style={{ color: "#666", marginBottom: 16, fontSize: 14 }}>
            Import products from external e-commerce websites. Enter a product page URL to extract product data.
          </p>

          <div style={{ display: "flex", gap: 12, marginBottom: 16 }}>
            <input
              type="url"
              value={scrapeUrl}
              onChange={(e) => setScrapeUrl(e.target.value)}
              placeholder="https://example.com/product-page"
              style={{ flex: 1, padding: 10, border: "1px solid #ddd", borderRadius: 4 }}
            />
            <button
              onClick={handleScrape}
              disabled={scraping || !scrapeUrl}
              style={{
                padding: "10px 20px",
                backgroundColor: scraping || !scrapeUrl ? "#ccc" : "#007bff",
                color: "white",
                border: "none",
                borderRadius: 4,
                cursor: scraping || !scrapeUrl ? "not-allowed" : "pointer",
              }}
            >
              {scraping ? "Scraping..." : "Scrape Products"}
            </button>
          </div>

          {scrapeError && (
            <div style={{ backgroundColor: "#fee", color: "#c00", padding: 12, borderRadius: 4, marginBottom: 16 }}>
              {scrapeError}
            </div>
          )}

          {scrapedProducts.length > 0 && (
            <div>
              <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 12 }}>
                <h4 style={{ margin: 0 }}>Found {scrapedProducts.length} Products</h4>
                <div style={{ display: "flex", gap: 12, alignItems: "center" }}>
                  <select
                    value={selectedCategory}
                    onChange={(e) => setSelectedCategory(e.target.value)}
                    style={{ padding: 8, border: "1px solid #ddd", borderRadius: 4 }}
                  >
                    <option value="">No Category</option>
                    {flattenCategories(categories).map((cat) => (
                      <option key={cat.id} value={cat.id}>{cat.name}</option>
                    ))}
                  </select>
                  <button
                    onClick={handleImportScraped}
                    disabled={importing || !scrapedProducts.some(p => p.selected)}
                    style={{
                      padding: "8px 16px",
                      backgroundColor: importing ? "#ccc" : "#28a745",
                      color: "white",
                      border: "none",
                      borderRadius: 4,
                      cursor: importing ? "not-allowed" : "pointer",
                    }}
                  >
                    Import Selected ({scrapedProducts.filter(p => p.selected).length})
                  </button>
                </div>
              </div>

              <div style={{ border: "1px solid #ddd", borderRadius: 4, maxHeight: 300, overflowY: "auto" }}>
                {scrapedProducts.map((product, index) => (
                  <div
                    key={index}
                    style={{
                      display: "flex",
                      alignItems: "center",
                      padding: 12,
                      borderBottom: index < scrapedProducts.length - 1 ? "1px solid #eee" : "none",
                      backgroundColor: product.selected ? "#f0fff0" : "#fff",
                    }}
                  >
                    <input
                      type="checkbox"
                      checked={product.selected}
                      onChange={() => toggleProductSelection(index)}
                      style={{ marginRight: 12 }}
                    />
                    {product.images && product.images[0] && (
                      <img
                        src={product.images[0]}
                        alt={product.name}
                        style={{ width: 50, height: 50, objectFit: "cover", marginRight: 12, borderRadius: 4 }}
                      />
                    )}
                    <div style={{ flex: 1 }}>
                      <div style={{ fontWeight: 500 }}>{product.name}</div>
                      {product.description && (
                        <div style={{ fontSize: 12, color: "#666", marginTop: 4 }}>
                          {product.description.substring(0, 100)}...
                        </div>
                      )}
                    </div>
                    <div style={{ fontWeight: 600, color: "#28a745" }}>
                      ${product.price.toFixed(2)}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        {/* Progress Log */}
        {progress.length > 0 && (
          <div style={{ backgroundColor: "white", padding: 20, borderRadius: 8, boxShadow: "0 2px 4px rgba(0,0,0,0.1)" }}>
            <h3 style={{ marginBottom: 12 }}>Progress Log</h3>
            <div style={{ fontFamily: "monospace", fontSize: 13, maxHeight: 400, overflowY: "auto" }}>
              {progress.map((line, i) => (
                <div key={i} style={{ padding: "4px 0" }}>{line}</div>
              ))}
            </div>
          </div>
        )}
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

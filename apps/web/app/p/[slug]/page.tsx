"use client";

import { useState, useEffect } from "react";
import { getProductBySlug, getProductSuggestions, Product } from "@/lib/api";

export default function ProductDetailPage({ params }: { params: { slug: string } }) {
  const [product, setProduct] = useState<Product | null>(null);
  const [suggestions, setSuggestions] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedVariantId, setSelectedVariantId] = useState<string | null>(null);
  const [quantity, setQuantity] = useState(1);

  useEffect(() => {
    const loadProductData = async () => {
      try {
        setLoading(true);
        
        // Load product details
        const productData = await getProductBySlug(params.slug);
        setProduct(productData);
        
        // Select first variant by default
        if (productData.variants.length > 0) {
          setSelectedVariantId(productData.variants[0].id);
        }
        
        // Load suggestions
        const suggestionsData = await getProductSuggestions(params.slug, 4);
        setSuggestions(suggestionsData);
        
        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load product");
      } finally {
        setLoading(false);
      }
    };

    loadProductData();
  }, [params.slug]);

  const selectedVariant = product?.variants.find(v => v.id === selectedVariantId);

  const handleQuantityChange = (delta: number) => {
    setQuantity(prev => Math.max(1, prev + delta));
  };

  if (loading) {
    return (
      <div style={{ padding: 24 }}>
        <div data-testid="product-detail-loading">Loading product...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: 24 }}>
        <div data-testid="product-detail-error" style={{ color: "red" }}>
          {error}
        </div>
        <a
          href="/products"
          data-testid="back-to-products"
          style={{ color: "#1976d2", textDecoration: "underline", cursor: "pointer", marginTop: 16, display: "inline-block" }}
        >
          Back to Products
        </a>
      </div>
    );
  }

  if (!product) {
    return (
      <div style={{ padding: 24 }}>
        <div data-testid="product-detail-not-found">Product not found</div>
      </div>
    );
  }

  return (
    <main style={{ minHeight: "100vh", fontFamily: "system-ui, sans-serif" }}>
      {/* Header */}
      <div style={{ padding: 24, borderBottom: "1px solid #ddd" }}>
        <a
          href="/products"
          data-testid="back-to-products"
          style={{ color: "#1976d2", textDecoration: "underline", cursor: "pointer" }}
        >
          ← Back to Products
        </a>
      </div>

      {/* Product Details */}
      <div style={{ padding: 24, maxWidth: 1200, margin: "0 auto" }} data-testid="product-detail-container">
        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 48 }}>
          {/* Left: Image Placeholder */}
          <div>
            <div
              data-testid="product-image-placeholder"
              style={{
                width: "100%",
                aspectRatio: "1",
                backgroundColor: "#f0f0f0",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                border: "1px solid #ddd",
                borderRadius: 8,
              }}
            >
              {product.images ? (
                <span style={{ color: "#666" }}>[Image: {product.images}]</span>
              ) : (
                <span style={{ color: "#999" }}>No image</span>
              )}
            </div>
          </div>

          {/* Right: Product Info */}
          <div>
            <h1 data-testid="product-detail-title" style={{ fontSize: 32, fontWeight: "bold", marginBottom: 16 }}>
              {product.name}
            </h1>

            {product.description && (
              <p data-testid="product-detail-description" style={{ color: "#666", marginBottom: 24 }}>
                {product.description}
              </p>
            )}

            {/* Variant Selector */}
            {product.variants.length > 0 && (
              <div style={{ marginBottom: 24 }} data-testid="variant-selector-container">
                <label htmlFor="variant-select" style={{ display: "block", marginBottom: 8, fontWeight: "500" }}>
                  Select Variant:
                </label>
                <select
                  id="variant-select"
                  data-testid="variant-select"
                  value={selectedVariantId || ""}
                  onChange={(e) => setSelectedVariantId(e.target.value)}
                  style={{
                    width: "100%",
                    padding: "12px",
                    fontSize: 16,
                    border: "1px solid #ddd",
                    borderRadius: 4,
                    backgroundColor: "white",
                    cursor: "pointer",
                  }}
                >
                  {product.variants.map((variant) => (
                    <option key={variant.id} value={variant.id}>
                      {variant.variantName || variant.sku} - ${variant.price.toFixed(2)}
                      {variant.stockQuantity <= 0 ? " (Out of stock)" : ` (${variant.stockQuantity} in stock)`}
                    </option>
                  ))}
                </select>
              </div>
            )}

            {/* Price */}
            {selectedVariant && (
              <div data-testid="product-detail-price" style={{ fontSize: 28, fontWeight: "bold", color: "#1976d2", marginBottom: 24 }}>
                ${selectedVariant.price.toFixed(2)}
              </div>
            )}

            {/* Quantity Controls */}
            <div style={{ marginBottom: 24 }} data-testid="quantity-controls">
              <label style={{ display: "block", marginBottom: 8, fontWeight: "500" }}>Quantity:</label>
              <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
                <button
                  data-testid="quantity-decrease"
                  onClick={() => handleQuantityChange(-1)}
                  disabled={quantity <= 1}
                  style={{
                    width: 40,
                    height: 40,
                    fontSize: 20,
                    border: "1px solid #ddd",
                    borderRadius: 4,
                    backgroundColor: "white",
                    cursor: quantity <= 1 ? "not-allowed" : "pointer",
                    opacity: quantity <= 1 ? 0.5 : 1,
                  }}
                >
                  -
                </button>
                <span data-testid="quantity-display" style={{ fontSize: 18, fontWeight: "500", minWidth: 40, textAlign: "center" }}>
                  {quantity}
                </span>
                <button
                  data-testid="quantity-increase"
                  onClick={() => handleQuantityChange(1)}
                  style={{
                    width: 40,
                    height: 40,
                    fontSize: 20,
                    border: "1px solid #ddd",
                    borderRadius: 4,
                    backgroundColor: "white",
                    cursor: "pointer",
                  }}
                >
                  +
                </button>
              </div>
            </div>

            {/* Stock Info */}
            {selectedVariant && (
              <div data-testid="stock-info" style={{ marginBottom: 24, color: selectedVariant.stockQuantity > 0 ? "#4caf50" : "#f44336" }}>
                {selectedVariant.stockQuantity > 0 ? `${selectedVariant.stockQuantity} in stock` : "Out of stock"}
              </div>
            )}

            {/* Add to Cart (Placeholder) */}
            <button
              data-testid="add-to-cart-button"
              disabled
              style={{
                width: "100%",
                padding: "16px",
                fontSize: 18,
                fontWeight: "bold",
                color: "white",
                backgroundColor: "#1976d2",
                border: "none",
                borderRadius: 4,
                cursor: "not-allowed",
                opacity: 0.6,
              }}
            >
              Add to Cart (Coming Soon)
            </button>

            {/* Specifications */}
            {product.specifications && (
              <div style={{ marginTop: 32, padding: 16, backgroundColor: "#f9f9f9", borderRadius: 4 }}>
                <h3 style={{ marginBottom: 12, fontWeight: "500" }}>Specifications:</h3>
                <pre data-testid="product-specifications" style={{ fontSize: 14, color: "#666", whiteSpace: "pre-wrap" }}>
                  {product.specifications}
                </pre>
              </div>
            )}
          </div>
        </div>

        {/* Suggested Products */}
        {suggestions.length > 0 && (
          <div style={{ marginTop: 48 }} data-testid="suggestions-container">
            <h2 style={{ fontSize: 24, fontWeight: "bold", marginBottom: 24 }}>You May Also Like</h2>
            <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(250px, 1fr))", gap: 24 }}>
              {suggestions.map((suggestedProduct, index) => (
                <div
                  key={suggestedProduct.id}
                  data-testid={`suggestion-card-${index}`}
                  style={{
                    border: "1px solid #ddd",
                    borderRadius: 8,
                    padding: 16,
                    backgroundColor: "white",
                  }}
                >
                  <div
                    style={{
                      width: "100%",
                      aspectRatio: "1",
                      backgroundColor: "#f0f0f0",
                      marginBottom: 12,
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      borderRadius: 4,
                    }}
                  >
                    <span style={{ color: "#999", fontSize: 12 }}>No image</span>
                  </div>
                  <h3
                    data-testid={`suggestion-name-${index}`}
                    style={{ fontSize: 16, fontWeight: "500", marginBottom: 8 }}
                  >
                    {suggestedProduct.name}
                  </h3>
                  {suggestedProduct.variants.length > 0 && (
                    <p data-testid={`suggestion-price-${index}`} style={{ color: "#1976d2", fontWeight: "bold" }}>
                      ${suggestedProduct.variants[0].price.toFixed(2)}
                    </p>
                  )}
                  <a
                    href={`/p/${suggestedProduct.slug}`}
                    data-testid={`suggestion-link-${index}`}
                    style={{
                      display: "inline-block",
                      marginTop: 12,
                      color: "#1976d2",
                      textDecoration: "underline",
                      cursor: "pointer",
                    }}
                  >
                    View Details
                  </a>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </main>
  );
}

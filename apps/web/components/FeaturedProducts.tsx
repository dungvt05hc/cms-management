"use client";

import type { Product } from "@/lib/api";

interface FeaturedProductsProps {
  products: Product[];
}

function ProductCard({ product }: { product: Product }) {
  const firstVariant = product.variants[0];
  const price = firstVariant ? `$${firstVariant.price.toFixed(2)}` : "N/A";

  return (
    <div
      data-testid={`featured-product-${product.id}`}
      style={{
        border: "1px solid #e0e0e0",
        borderRadius: 8,
        padding: 16,
        marginBottom: 16,
      }}
    >
      <h3
        data-testid={`product-name-${product.id}`}
        style={{ margin: "0 0 8px 0", fontSize: "1.1rem" }}
      >
        {product.name}
      </h3>
      {product.description && (
        <p
          data-testid={`product-description-${product.id}`}
          style={{ margin: "0 0 8px 0", color: "#666", fontSize: "0.9rem" }}
        >
          {product.description}
        </p>
      )}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <span
          data-testid={`product-price-${product.id}`}
          style={{ fontWeight: "bold", fontSize: "1.2rem", color: "#2e7d32" }}
        >
          {price}
        </span>
        {firstVariant && firstVariant.stockQuantity > 0 ? (
          <span style={{ color: "#2e7d32", fontSize: "0.9rem" }}>In Stock</span>
        ) : (
          <span style={{ color: "#d32f2f", fontSize: "0.9rem" }}>Out of Stock</span>
        )}
      </div>
    </div>
  );
}

export function FeaturedProducts({ products }: FeaturedProductsProps) {
  if (!products || products.length === 0) {
    return (
      <div data-testid="featured-products-empty">
        No featured products available.
      </div>
    );
  }

  return (
    <section data-testid="featured-products">
      <h2 style={{ marginBottom: 16, fontSize: "1.5rem", fontWeight: "bold" }}>
        Featured Products
      </h2>
      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(250px, 1fr))", gap: 16 }}>
        {products.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </section>
  );
}

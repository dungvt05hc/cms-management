"use client";

import { useState, useEffect, useCallback, useRef } from "react";
import { getProducts, Product } from "@/lib/api";

interface ProductListProps {
  categorySlug?: string;
  categoryId?: string;
  initialSort?: "priceAsc" | "priceDesc";
}

export default function ProductList({ categorySlug, categoryId, initialSort }: ProductListProps) {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState(false);
  const [sort, setSort] = useState<"priceAsc" | "priceDesc" | undefined>(initialSort);
  
  const observerRef = useRef<IntersectionObserver | null>(null);
  const loadMoreTriggerRef = useRef<HTMLDivElement | null>(null);

  const loadProducts = useCallback(async (cursor?: string) => {
    try {
      if (cursor) {
        setLoadingMore(true);
      } else {
        setLoading(true);
        setProducts([]);
      }

      const result = await getProducts({
        category: categorySlug,
        categoryId,
        sort,
        cursor,
        limit: 20,
      });

      if (cursor) {
        setProducts((prev) => [...prev, ...result.items]);
      } else {
        setProducts(result.items);
      }

      setNextCursor(result.nextCursor);
      setHasMore(result.hasMore);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load products");
    } finally {
      setLoading(false);
      setLoadingMore(false);
    }
  }, [categorySlug, categoryId, sort]);

  // Initial load and reload on sort change
  useEffect(() => {
    loadProducts();
  }, [loadProducts]);

  // Infinite scroll observer
  useEffect(() => {
    if (!hasMore || loadingMore || loading) return;

    if (observerRef.current) {
      observerRef.current.disconnect();
    }

    observerRef.current = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && nextCursor) {
          loadProducts(nextCursor);
        }
      },
      { threshold: 0.1 }
    );

    if (loadMoreTriggerRef.current) {
      observerRef.current.observe(loadMoreTriggerRef.current);
    }

    return () => {
      if (observerRef.current) {
        observerRef.current.disconnect();
      }
    };
  }, [hasMore, loadingMore, loading, nextCursor, loadProducts]);

  const handleSortChange = (newSort: "priceAsc" | "priceDesc") => {
    setSort(newSort);
    setNextCursor(null);
    setHasMore(false);
  };

  if (loading && products.length === 0) {
    return (
      <div data-testid="product-list-loading" style={{ padding: 24 }}>
        <p>Loading products...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div data-testid="product-list-error" style={{ padding: 24, color: "red" }}>
        <p>Error: {error}</p>
      </div>
    );
  }

  if (products.length === 0) {
    return (
      <div data-testid="product-list-empty" style={{ padding: 24 }}>
        <p>No products found.</p>
      </div>
    );
  }

  return (
    <div data-testid="product-list-container">
      {/* Sort controls */}
      <div data-testid="sort-control" style={{ padding: "16px 24px", borderBottom: "1px solid #ddd" }}>
        <label htmlFor="sort-select" style={{ marginRight: 8, fontWeight: 500 }}>
          Sort by:
        </label>
        <select
          id="sort-select"
          data-testid="sort-select"
          value={sort || ""}
          onChange={(e) => handleSortChange(e.target.value as "priceAsc" | "priceDesc")}
          style={{
            padding: "4px 8px",
            borderRadius: 4,
            border: "1px solid #ccc",
            cursor: "pointer",
          }}
        >
          <option value="">Newest First</option>
          <option value="priceAsc">Price: Low to High</option>
          <option value="priceDesc">Price: High to Low</option>
        </select>
      </div>

      {/* Product grid */}
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fill, minmax(250px, 1fr))",
          gap: 16,
          padding: 24,
        }}
      >
        {products.map((product, index) => (
          <div
            key={product.id}
            data-testid={`product-card-${index}`}
            className="product-card"
            style={{
              border: "1px solid #ddd",
              borderRadius: 8,
              padding: 16,
              display: "flex",
              flexDirection: "column",
              gap: 8,
            }}
          >
            {/* Product image placeholder */}
            <div
              style={{
                width: "100%",
                height: 150,
                backgroundColor: "#f0f0f0",
                borderRadius: 4,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                color: "#999",
              }}
            >
              {product.images ? "📷" : "No Image"}
            </div>

            {/* Product name */}
            <h3
              data-testid={`product-name-${index}`}
              style={{
                margin: 0,
                fontSize: "16px",
                fontWeight: 600,
                lineHeight: 1.4,
              }}
            >
              {product.name}
            </h3>

            {/* Product price */}
            {product.variants.length > 0 && (
              <p
                data-testid={`product-price-${index}`}
                style={{
                  margin: 0,
                  fontSize: "18px",
                  fontWeight: 700,
                  color: "#1976d2",
                }}
              >
                ${product.variants[0].price.toFixed(2)}
              </p>
            )}

            {/* Actions */}
            <div style={{ display: "flex", gap: 8, marginTop: "auto" }}>
              <button
                data-testid={`product-view-${index}`}
                style={{
                  flex: 1,
                  padding: "8px 12px",
                  backgroundColor: "#1976d2",
                  color: "white",
                  border: "none",
                  borderRadius: 4,
                  cursor: "pointer",
                  fontWeight: 500,
                }}
              >
                View
              </button>
              <button
                data-testid={`product-buy-${index}`}
                style={{
                  flex: 1,
                  padding: "8px 12px",
                  backgroundColor: "#e0e0e0",
                  color: "#666",
                  border: "none",
                  borderRadius: 4,
                  cursor: "not-allowed",
                  fontWeight: 500,
                }}
                disabled
              >
                Buy
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Load more trigger */}
      {hasMore && (
        <div
          ref={loadMoreTriggerRef}
          data-testid="load-more-trigger"
          style={{
            padding: 24,
            textAlign: "center",
          }}
        >
          {loadingMore ? (
            <p data-testid="loading-more">Loading more products...</p>
          ) : (
            <p>Scroll to load more</p>
          )}
        </div>
      )}

      {/* End of list */}
      {!hasMore && products.length > 0 && (
        <div data-testid="end-of-list" style={{ padding: 24, textAlign: "center", color: "#999" }}>
          <p>No more products to load.</p>
        </div>
      )}
    </div>
  );
}

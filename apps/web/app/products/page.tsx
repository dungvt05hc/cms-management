import ProductList from "@/components/ProductList";

export default function ProductsPage() {
  return (
    <main style={{ minHeight: "100vh", fontFamily: "system-ui, sans-serif" }}>
      <div style={{ padding: 24, borderBottom: "1px solid #ddd" }}>
        <h1 data-testid="products-page-title">All Products</h1>
        <a
          href="/"
          data-testid="back-to-home"
          style={{ color: "#1976d2", textDecoration: "underline", cursor: "pointer" }}
        >
          Back to Home
        </a>
      </div>
      
      <ProductList />
    </main>
  );
}

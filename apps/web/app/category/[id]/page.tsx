export default function CategoryPage({ params }: { params: { id: string } }) {
  return (
    <main style={{ padding: 24, fontFamily: "system-ui, sans-serif" }}>
      <h1 data-testid="category-page-title">Category Products</h1>
      <p data-testid="category-id">Category ID: {params.id}</p>
      <p style={{ color: "#666" }}>
        Product list for this category will be displayed here.
      </p>
      <a
        href="/"
        data-testid="back-to-home"
        style={{ color: "#1976d2", textDecoration: "underline", cursor: "pointer" }}
      >
        Back to Home
      </a>
    </main>
  );
}

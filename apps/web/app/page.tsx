import { CategoryNavigation } from "@/components/CategoryNavigation";
import { FeaturedProducts } from "@/components/FeaturedProducts";
import { getCategoryTree, getFeaturedProducts, type CategoryTreeNode, type Product } from "@/lib/api";

export default async function HomePage() {
  let categories: CategoryTreeNode[] = [];
  let featuredProducts: Product[] = [];
  let categoriesError: string | null = null;
  let productsError: string | null = null;

  try {
    categories = await getCategoryTree();
  } catch (err) {
    categoriesError = (err as Error).message;
    console.error("Failed to fetch categories:", err);
  }

  try {
    featuredProducts = await getFeaturedProducts(8);
  } catch (err) {
    productsError = (err as Error).message;
    console.error("Failed to fetch featured products:", err);
  }

  return (
    <main style={{ padding: 24, fontFamily: "system-ui, sans-serif", maxWidth: 1200, margin: "0 auto" }}>
      <h1 data-testid="home-title">CMS Management</h1>
      <p>Bootstrap web app for e-commerce + CMS.</p>
      
      {/* Banner Placeholder */}
      <div
        data-testid="banner-placeholder"
        style={{
          marginTop: 32,
          marginBottom: 32,
          padding: 48,
          backgroundColor: "#f5f5f5",
          borderRadius: 8,
          textAlign: "center",
          border: "2px dashed #ccc",
        }}
      >
        <h2 style={{ margin: 0, color: "#999" }}>Banner Placeholder</h2>
        <p style={{ margin: "8px 0 0 0", color: "#999" }}>
          Future home of promotional banners
        </p>
      </div>

      <div style={{ display: "grid", gridTemplateColumns: "250px 1fr", gap: 32, marginTop: 32 }}>
        {/* Categories Navigation */}
        <div>
          {categoriesError && (
            <div data-testid="category-error" style={{ color: "red", marginBottom: 16 }}>
              Error loading categories: {categoriesError}
            </div>
          )}
          <CategoryNavigation categories={categories} />
        </div>

        {/* Featured Products */}
        <div>
          {productsError && (
            <div data-testid="products-error" style={{ color: "red", marginBottom: 16 }}>
              Error loading products: {productsError}
            </div>
          )}
          <FeaturedProducts products={featuredProducts} />
        </div>
      </div>
    </main>
  );
}

  
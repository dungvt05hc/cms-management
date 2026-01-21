import { CategoryNavigation } from "@/components/CategoryNavigation";
import { getCategoryTree, type CategoryTreeNode } from "@/lib/api";

export default async function HomePage() {
  let categories: CategoryTreeNode[] = [];
  let error: string | null = null;

  try {
    categories = await getCategoryTree();
  } catch (err) {
    error = (err as Error).message;
    console.error("Failed to fetch categories:", err);
  }

  return (
    <main style={{ padding: 24, fontFamily: "system-ui, sans-serif" }}>
      <h1 data-testid="home-title">CMS Management</h1>
      <p>Bootstrap web app for e-commerce + CMS.</p>
      
      <div style={{ marginTop: 32 }}>
        {error && (
          <div data-testid="category-error" style={{ color: "red", marginBottom: 16 }}>
            Error loading categories: {error}
          </div>
        )}
        <CategoryNavigation categories={categories} />
      </div>
    </main>
  );
}

  
import {
  getArticleCategories,
  getFeaturedArticles,
  type ArticleCategory,
  type ArticleListItem,
} from "@/lib/api";

export default async function ArticlesHomePage() {
  let categories: ArticleCategory[] = [];
  let featuredArticles: ArticleListItem[] = [];
  let categoriesError: string | null = null;
  let articlesError: string | null = null;

  try {
    categories = await getArticleCategories();
  } catch (err) {
    categoriesError = (err as Error).message;
    console.error("Failed to fetch article categories:", err);
  }

  try {
    featuredArticles = await getFeaturedArticles(6);
  } catch (err) {
    articlesError = (err as Error).message;
    console.error("Failed to fetch featured articles:", err);
  }

  return (
    <main
      style={{
        padding: 24,
        fontFamily: "system-ui, sans-serif",
        maxWidth: 1200,
        margin: "0 auto",
      }}
    >
      {/* Header */}
      <div style={{ marginBottom: 32 }}>
        <h1 data-testid="articles-home-title" style={{ fontSize: 32, marginBottom: 8 }}>
          Articles & Resources
        </h1>
        <p style={{ color: "#666", fontSize: 16 }}>
          Explore our latest articles, guides, and helpful resources
        </p>
      </div>

      {/* Categories Navigation */}
      <div style={{ marginBottom: 48 }}>
        <h2 style={{ fontSize: 24, marginBottom: 16 }}>Browse by Category</h2>
        {categoriesError && (
          <div
            data-testid="categories-error"
            style={{ color: "red", marginBottom: 16 }}
          >
            Error loading categories: {categoriesError}
          </div>
        )}
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(250px, 1fr))",
            gap: 16,
          }}
        >
          {categories.map((category) => (
            <a
              key={category.id}
              href={`/articles/category/${category.slug}`}
              data-testid={`category-card-${category.slug}`}
              style={{
                display: "block",
                padding: 24,
                border: "1px solid #ddd",
                borderRadius: 8,
                textDecoration: "none",
                color: "inherit",
                transition: "all 0.2s",
                backgroundColor: "white",
              }}
            >
              <h3 style={{ fontSize: 20, marginBottom: 8, color: "#1976d2" }}>
                {category.name}
              </h3>
              {category.description && (
                <p style={{ color: "#666", fontSize: 14, margin: 0 }}>
                  {category.description}
                </p>
              )}
            </a>
          ))}
        </div>
      </div>

      {/* Featured Articles */}
      <div>
        <h2 style={{ fontSize: 24, marginBottom: 16 }}>Featured Articles</h2>
        {articlesError && (
          <div data-testid="articles-error" style={{ color: "red", marginBottom: 16 }}>
            Error loading articles: {articlesError}
          </div>
        )}
        {featuredArticles.length === 0 && !articlesError && (
          <div
            data-testid="no-articles"
            style={{
              padding: 48,
              textAlign: "center",
              color: "#999",
              border: "2px dashed #ddd",
              borderRadius: 8,
            }}
          >
            No featured articles available
          </div>
        )}
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(300px, 1fr))",
            gap: 24,
          }}
        >
          {featuredArticles.map((article, index) => (
            <article
              key={article.id}
              data-testid={`article-card-${index}`}
              style={{
                border: "1px solid #ddd",
                borderRadius: 8,
                overflow: "hidden",
                backgroundColor: "white",
              }}
            >
              {/* Thumbnail placeholder */}
              <div
                data-testid={`article-thumbnail-${index}`}
                style={{
                  width: "100%",
                  aspectRatio: "16/9",
                  backgroundColor: "#f0f0f0",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  color: "#999",
                }}
              >
                {article.thumbnailUrl ? (
                  <span>[Image: {article.thumbnailUrl}]</span>
                ) : (
                  <span>No thumbnail</span>
                )}
              </div>

              {/* Article info */}
              <div style={{ padding: 16 }}>
                {article.categoryName && (
                  <div style={{ marginBottom: 8 }}>
                    <a
                      href={`/articles/category/${article.categorySlug}`}
                      style={{
                        fontSize: 12,
                        color: "#1976d2",
                        textDecoration: "none",
                        textTransform: "uppercase",
                        fontWeight: "500",
                      }}
                    >
                      {article.categoryName}
                    </a>
                  </div>
                )}
                <h3
                  data-testid={`article-title-${index}`}
                  style={{
                    fontSize: 18,
                    marginBottom: 8,
                    fontWeight: "600",
                  }}
                >
                  <a
                    href={`/articles/${article.slug}`}
                    style={{ color: "inherit", textDecoration: "none" }}
                  >
                    {article.title}
                  </a>
                </h3>
                {article.summary && (
                  <p
                    data-testid={`article-summary-${index}`}
                    style={{
                      color: "#666",
                      fontSize: 14,
                      marginBottom: 12,
                      lineHeight: 1.5,
                    }}
                  >
                    {article.summary}
                  </p>
                )}
                <div
                  style={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    fontSize: 12,
                    color: "#999",
                  }}
                >
                  <span>
                    {article.publishedAt
                      ? new Date(article.publishedAt).toLocaleDateString()
                      : "Draft"}
                  </span>
                  <span>{article.viewCount} views</span>
                </div>
              </div>
            </article>
          ))}
        </div>
      </div>
    </main>
  );
}

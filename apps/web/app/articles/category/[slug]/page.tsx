"use client";

import { useState, useEffect } from "react";
import {
  getArticles,
  getArticleCategories,
  type ArticleListItem,
  type ArticleCategory,
  type PagedResult,
} from "@/lib/api";

export default function ArticleCategoryPage({
  params,
}: {
  params: { slug: string };
}) {
  const [articles, setArticles] = useState<ArticleListItem[]>([]);
  const [category, setCategory] = useState<ArticleCategory | null>(null);
  const [pagination, setPagination] = useState<{
    totalCount: number;
    page: number;
    pageSize: number;
  } | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const currentPage = 1; // TODO: Add pagination controls in future iteration

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);

        // Load category info
        const categories = await getArticleCategories();
        const foundCategory = categories.find((c) => c.slug === params.slug);
        setCategory(foundCategory || null);

        // Load articles for this category
        const result: PagedResult<ArticleListItem> = await getArticles(
          params.slug,
          currentPage,
          12
        );
        setArticles(result.items);
        setPagination({
          totalCount: result.totalCount,
          page: result.page,
          pageSize: result.pageSize,
        });

        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load articles");
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [params.slug, currentPage]);

  if (loading) {
    return (
      <div style={{ padding: 24 }}>
        <div data-testid="category-loading">Loading articles...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: 24 }}>
        <div data-testid="category-error" style={{ color: "red" }}>
          {error}
        </div>
        <a
          href="/articles"
          style={{
            color: "#1976d2",
            textDecoration: "underline",
            cursor: "pointer",
            marginTop: 16,
            display: "inline-block",
          }}
        >
          ← Back to Articles Home
        </a>
      </div>
    );
  }

  return (
    <main
      style={{
        padding: 24,
        fontFamily: "system-ui, sans-serif",
        maxWidth: 1200,
        margin: "0 auto",
      }}
      data-testid="category-page-container"
    >
      {/* Breadcrumb */}
      <div style={{ marginBottom: 24 }}>
        <a
          href="/articles"
          style={{ color: "#1976d2", textDecoration: "underline" }}
        >
          Articles
        </a>
        <span style={{ margin: "0 8px", color: "#999" }}>/</span>
        <span data-testid="category-name">{category?.name || params.slug}</span>
      </div>

      {/* Header */}
      <div style={{ marginBottom: 32 }}>
        <h1 data-testid="category-title" style={{ fontSize: 32, marginBottom: 8 }}>
          {category?.name || params.slug}
        </h1>
        {category?.description && (
          <p style={{ color: "#666", fontSize: 16 }}>{category.description}</p>
        )}
      </div>

      {/* Articles List */}
      {articles.length === 0 ? (
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
          No articles found in this category
        </div>
      ) : (
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(300px, 1fr))",
            gap: 24,
          }}
        >
          {articles.map((article, index) => (
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
      )}

      {/* Pagination Info */}
      {pagination && pagination.totalCount > 0 && (
        <div
          data-testid="pagination-info"
          style={{
            marginTop: 32,
            padding: 16,
            textAlign: "center",
            color: "#666",
            border: "1px solid #ddd",
            borderRadius: 8,
            backgroundColor: "#f9f9f9",
          }}
        >
          <p style={{ margin: 0 }}>
            Showing {articles.length} of {pagination.totalCount} articles
          </p>
          <p style={{ margin: "8px 0 0 0", fontSize: 14, color: "#999" }}>
            (Pagination controls will be added in future iteration)
          </p>
        </div>
      )}
    </main>
  );
}

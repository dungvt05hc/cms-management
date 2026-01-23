"use client";

import { useState, useEffect } from "react";
import { getArticleBySlug, type Article } from "@/lib/api";

export default function ArticleDetailPage({
  params,
}: {
  params: { slug: string };
}) {
  const [article, setArticle] = useState<Article | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadArticle = async () => {
      try {
        setLoading(true);
        const data = await getArticleBySlug(params.slug);
        setArticle(data);
        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load article");
      } finally {
        setLoading(false);
      }
    };

    loadArticle();
  }, [params.slug]);

  if (loading) {
    return (
      <div style={{ padding: 24 }}>
        <div data-testid="article-detail-loading">Loading article...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: 24 }}>
        <div data-testid="article-detail-error" style={{ color: "red" }}>
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
          ← Back to Articles
        </a>
      </div>
    );
  }

  if (!article) {
    return (
      <div style={{ padding: 24 }}>
        <div data-testid="article-not-found">Article not found</div>
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
          ← Back to Articles
        </a>
      </div>
    );
  }

  return (
    <main
      style={{
        fontFamily: "system-ui, sans-serif",
        minHeight: "100vh",
      }}
      data-testid="article-detail-container"
    >
      {/* Header */}
      <div style={{ padding: 24, borderBottom: "1px solid #ddd" }}>
        <a
          href="/articles"
          data-testid="back-to-articles"
          style={{ color: "#1976d2", textDecoration: "underline", cursor: "pointer" }}
        >
          ← Back to Articles
        </a>
      </div>

      {/* Article Content */}
      <article
        style={{
          maxWidth: 800,
          margin: "0 auto",
          padding: "48px 24px",
        }}
      >
        {/* Category */}
        {article.categoryName && article.categorySlug && (
          <div style={{ marginBottom: 16 }}>
            <a
              href={`/articles/category/${article.categorySlug}`}
              data-testid="article-category-link"
              style={{
                fontSize: 14,
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

        {/* Title */}
        <h1
          data-testid="article-detail-title"
          style={{
            fontSize: 40,
            fontWeight: "bold",
            marginBottom: 16,
            lineHeight: 1.2,
          }}
        >
          {article.title}
        </h1>

        {/* Meta info */}
        <div
          data-testid="article-meta"
          style={{
            display: "flex",
            gap: 24,
            marginBottom: 32,
            paddingBottom: 24,
            borderBottom: "1px solid #eee",
            fontSize: 14,
            color: "#666",
          }}
        >
          {article.publishedAt && (
            <span>
              Published: {new Date(article.publishedAt).toLocaleDateString()}
            </span>
          )}
          <span>{article.viewCount} views</span>
        </div>

        {/* Thumbnail */}
        {article.thumbnailUrl ? (
          <div
            data-testid="article-thumbnail"
            style={{
              width: "100%",
              aspectRatio: "16/9",
              backgroundColor: "#f0f0f0",
              marginBottom: 32,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              border: "1px solid #ddd",
              borderRadius: 8,
              color: "#999",
            }}
          >
            [Image: {article.thumbnailUrl}]
          </div>
        ) : (
          <div
            data-testid="article-thumbnail-placeholder"
            style={{
              width: "100%",
              aspectRatio: "16/9",
              backgroundColor: "#f0f0f0",
              marginBottom: 32,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              border: "1px solid #ddd",
              borderRadius: 8,
              color: "#999",
            }}
          >
            No thumbnail
          </div>
        )}

        {/* Summary */}
        {article.summary && (
          <div
            data-testid="article-summary"
            style={{
              fontSize: 18,
              lineHeight: 1.6,
              color: "#666",
              marginBottom: 32,
              fontStyle: "italic",
            }}
          >
            {article.summary}
          </div>
        )}

        {/* Content */}
        <div
          data-testid="article-content"
          style={{
            fontSize: 16,
            lineHeight: 1.8,
            color: "#333",
          }}
          dangerouslySetInnerHTML={{ __html: article.content }}
        />

        {/* Related Articles Placeholder */}
        <div
          data-testid="related-articles-placeholder"
          style={{
            marginTop: 64,
            padding: 32,
            backgroundColor: "#f9f9f9",
            border: "2px dashed #ddd",
            borderRadius: 8,
            textAlign: "center",
          }}
        >
          <h3 style={{ color: "#999", marginBottom: 8 }}>Related Articles</h3>
          <p style={{ color: "#999", margin: 0, fontSize: 14 }}>
            (Related articles will be shown here in future iteration)
          </p>
        </div>
      </article>
    </main>
  );
}

"use client";

import { useState } from "react";
import Link from "next/link";
import type { CategoryTreeNode } from "@/lib/api";

interface CategoryNavigationProps {
  categories: CategoryTreeNode[];
}

interface CategoryItemProps {
  category: CategoryTreeNode;
  level: number;
}

function CategoryItem({ category, level }: CategoryItemProps) {
  const [isExpanded, setIsExpanded] = useState(true);
  const hasChildren = category.children && category.children.length > 0;

  return (
    <div style={{ marginLeft: level * 20 }}>
      <div
        style={{
          display: "flex",
          alignItems: "center",
          padding: "8px 0",
        }}
        data-testid={`category-${category.id}`}
      >
        {hasChildren && (
          <span
            style={{
              marginRight: 8,
              width: 16,
              display: "inline-block",
              fontWeight: "bold",
              cursor: "pointer",
            }}
            onClick={() => setIsExpanded(!isExpanded)}
            data-testid={`category-toggle-${category.id}`}
          >
            {isExpanded ? "▼" : "▶"}
          </span>
        )}
        {!hasChildren && (
          <span style={{ marginRight: 8, width: 16, display: "inline-block" }}>
            •
          </span>
        )}
        <Link
          href={`/category/${category.id}`}
          data-testid={`category-link-${category.id}`}
          style={{ textDecoration: "none", color: "#1976d2", cursor: "pointer" }}
        >
          <span data-testid={`category-name-${category.id}`}>{category.name}</span>
        </Link>
      </div>

      {hasChildren && isExpanded && (
        <div data-testid={`category-children-${category.id}`}>
          {category.children.map((child) => (
            <CategoryItem key={child.id} category={child} level={level + 1} />
          ))}
        </div>
      )}
    </div>
  );
}

export function CategoryNavigation({ categories }: CategoryNavigationProps) {
  if (!categories || categories.length === 0) {
    return (
      <div data-testid="category-navigation-empty">
        No categories available.
      </div>
    );
  }

  return (
    <nav data-testid="category-navigation">
      <h2 style={{ marginBottom: 16, fontSize: "1.25rem", fontWeight: "bold" }}>
        Categories
      </h2>
      {categories.map((category) => (
        <CategoryItem key={category.id} category={category} level={0} />
      ))}
    </nav>
  );
}

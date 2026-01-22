"use client";

import { useState, useEffect, useRef } from "react";
import { searchSuggestions, SearchSuggestion } from "../lib/api";

export default function SearchInput() {
  const [query, setQuery] = useState("");
  const [suggestions, setSuggestions] = useState<SearchSuggestion[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const wrapperRef = useRef<HTMLDivElement>(null);

  // Debounced search
  useEffect(() => {
    const timer = setTimeout(async () => {
      if (query.length >= 2) {
        setIsLoading(true);
        try {
          const results = await searchSuggestions(query, 10);
          setSuggestions(results);
          setIsOpen(true);
        } catch (error) {
          console.error("Failed to fetch suggestions:", error);
          setSuggestions([]);
        } finally {
          setIsLoading(false);
        }
      } else {
        setSuggestions([]);
        setIsOpen(false);
      }
    }, 300); // 300ms debounce

    return () => clearTimeout(timer);
  }, [query]);

  // Close dropdown when clicking outside
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleSuggestionClick = (slug: string) => {
    setIsOpen(false);
    setQuery("");
    window.location.href = `/p/${slug}`;
  };

  return (
    <div ref={wrapperRef} className="search-input-wrapper" style={{ position: "relative", width: "100%" }}>
      <input
        type="text"
        data-testid="search-input"
        placeholder="Search products..."
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        style={{
          width: "100%",
          padding: "8px 12px",
          border: "1px solid #ddd",
          borderRadius: "4px",
          fontSize: "14px",
        }}
      />

      {isLoading && (
        <div
          data-testid="search-loading"
          style={{
            position: "absolute",
            right: "12px",
            top: "50%",
            transform: "translateY(-50%)",
            fontSize: "12px",
            color: "#666",
          }}
        >
          Loading...
        </div>
      )}

      {isOpen && suggestions.length > 0 && (
        <div
          data-testid="search-suggestions-dropdown"
          style={{
            position: "absolute",
            top: "100%",
            left: 0,
            right: 0,
            backgroundColor: "white",
            border: "1px solid #ddd",
            borderTop: "none",
            borderRadius: "0 0 4px 4px",
            maxHeight: "300px",
            overflowY: "auto",
            zIndex: 1000,
            boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
          }}
        >
          {suggestions.map((suggestion, index) => (
            <div
              key={suggestion.id}
              data-testid={`search-suggestion-${index}`}
              onClick={() => handleSuggestionClick(suggestion.slug)}
              style={{
                padding: "10px 12px",
                cursor: "pointer",
                borderBottom: index < suggestions.length - 1 ? "1px solid #eee" : "none",
              }}
              onMouseEnter={(e) => {
                e.currentTarget.style.backgroundColor = "#f5f5f5";
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.backgroundColor = "white";
              }}
            >
              <div style={{ fontWeight: "500", fontSize: "14px" }}>{suggestion.name}</div>
            </div>
          ))}
        </div>
      )}

      {isOpen && suggestions.length === 0 && query.length >= 2 && !isLoading && (
        <div
          data-testid="search-no-results"
          style={{
            position: "absolute",
            top: "100%",
            left: 0,
            right: 0,
            backgroundColor: "white",
            border: "1px solid #ddd",
            borderTop: "none",
            borderRadius: "0 0 4px 4px",
            padding: "12px",
            zIndex: 1000,
            color: "#666",
            fontSize: "14px",
          }}
        >
          No results found
        </div>
      )}
    </div>
  );
}

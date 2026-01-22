"use client";

import dynamic from "next/dynamic";

const SearchInput = dynamic(() => import("@/components/SearchInput"), {
  ssr: false,
  loading: () => (
    <div style={{ 
      width: "100%", 
      padding: "8px 12px", 
      border: "1px solid #ddd", 
      borderRadius: "4px" 
    }}>
      Loading...
    </div>
  ),
});

export default function SearchWrapper() {
  return <SearchInput />;
}

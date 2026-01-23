import SearchWrapper from "@/components/SearchWrapper";
import GoogleAnalytics from "@/components/GoogleAnalytics";
import GoogleTagManager from "@/components/GoogleTagManager";
import AnalyticsTracker from "@/components/AnalyticsTracker";

export const metadata = {
    title: "CMS Management",
    description: "E-commerce + CMS"
  };
  
  export default function RootLayout({
    children
  }: {
    children: React.ReactNode;
  }) {
    return (
      <html lang="en">
        <body>
          <GoogleAnalytics />
          <GoogleTagManager />
          <AnalyticsTracker />
          <header style={{ 
            backgroundColor: "#f8f9fa", 
            padding: "16px 24px", 
            borderBottom: "1px solid #dee2e6",
            position: "sticky",
            top: 0,
            zIndex: 100
          }}>
            <div style={{ 
              maxWidth: 1200, 
              margin: "0 auto", 
              display: "flex", 
              alignItems: "center", 
              gap: 24 
            }}>
              <a href="/" style={{ 
                fontSize: 20, 
                fontWeight: "bold", 
                textDecoration: "none", 
                color: "#000",
                whiteSpace: "nowrap" 
              }}>
                CMS Management
              </a>
              <div style={{ flex: 1, maxWidth: 500 }} data-testid="search-container">
                <SearchWrapper />
              </div>
            </div>
          </header>
          {children}
        </body>
      </html>
    );
  }
  
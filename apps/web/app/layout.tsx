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
          <footer style={{
            backgroundColor: "#f8f9fa",
            borderTop: "1px solid #dee2e6",
            marginTop: 64,
            padding: "32px 24px",
          }}>
            <div style={{
              maxWidth: 1200,
              margin: "0 auto",
              display: "grid",
              gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))",
              gap: 32
            }}>
              <div>
                <h3 style={{ fontSize: 16, marginBottom: 12, fontWeight: "bold" }}>Help & Support</h3>
                <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
                  <a href="/help" data-testid="footer-link-help" style={{ color: "#666", textDecoration: "none", fontSize: 14 }}>Help Center</a>
                  <a href="/contact" data-testid="footer-link-contact" style={{ color: "#666", textDecoration: "none", fontSize: 14 }}>Contact Us</a>
                </div>
              </div>
              <div>
                <h3 style={{ fontSize: 16, marginBottom: 12, fontWeight: "bold" }}>Policies</h3>
                <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
                  <a href="/terms" data-testid="footer-link-terms" style={{ color: "#666", textDecoration: "none", fontSize: 14 }}>Terms of Service</a>
                  <a href="/sales-policy" data-testid="footer-link-sales-policy" style={{ color: "#666", textDecoration: "none", fontSize: 14 }}>Sales Policy</a>
                </div>
              </div>
              <div>
                <h3 style={{ fontSize: 16, marginBottom: 12, fontWeight: "bold" }}>About</h3>
                <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
                  <a href="/articles" data-testid="footer-link-articles" style={{ color: "#666", textDecoration: "none", fontSize: 14 }}>Articles</a>
                  <a href="/products" data-testid="footer-link-products" style={{ color: "#666", textDecoration: "none", fontSize: 14 }}>Products</a>
                </div>
              </div>
            </div>
            <div style={{
              maxWidth: 1200,
              margin: "24px auto 0",
              paddingTop: 24,
              borderTop: "1px solid #dee2e6",
              textAlign: "center",
              color: "#999",
              fontSize: 14
            }}>
              © {new Date().getFullYear()} CMS Management. All rights reserved.
            </div>
          </footer>
        </body>
      </html>
    );
  }
  
import './globals.css';
import Header from "@/components/layout/Header";
import Footer from "@/components/layout/Footer";
import GoogleAnalytics from "@/components/GoogleAnalytics";
import GoogleTagManager from "@/components/GoogleTagManager";
import AnalyticsTracker from "@/components/AnalyticsTracker";
import { getCategoryTree, type CategoryTreeNode } from "@/lib/api";

export const metadata = {
  title: "CMS Shop - Your One-Stop E-commerce Destination",
  description: "Discover amazing products at great prices. Free shipping on orders over $50.",
};

export default async function RootLayout({
  children
}: {
  children: React.ReactNode;
}) {
  let categories: CategoryTreeNode[] = [];

  try {
    categories = await getCategoryTree();
  } catch (err) {
    console.error("Failed to fetch categories for header:", err);
  }

  return (
    <html lang="en">
      <head>
        <link rel="preconnect" href="https://fonts.googleapis.com" />
        <link rel="preconnect" href="https://fonts.gstatic.com" crossOrigin="anonymous" />
        <link 
          href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=Poppins:wght@500;600;700&display=swap" 
          rel="stylesheet" 
        />
      </head>
      <body>
        <GoogleAnalytics />
        <GoogleTagManager />
        <AnalyticsTracker />
        <Header categories={categories} />
        <main>{children}</main>
        <Footer />
      </body>
    </html>
  );
}

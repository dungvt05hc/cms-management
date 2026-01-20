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
        <body>{children}</body>
      </html>
    );
  }
  
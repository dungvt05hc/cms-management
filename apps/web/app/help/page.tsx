import { supportConfig } from "@/lib/config";

export default function HelpPage() {
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
        <h1 data-testid="help-title" style={{ fontSize: 32, marginBottom: 8 }}>
          Help Center
        </h1>
        <p style={{ color: "#666", fontSize: 16 }}>
          Find answers to common questions and get support
        </p>
      </div>

      {/* Support Channels */}
      <section style={{ marginBottom: 48 }}>
        <h2 style={{ fontSize: 24, marginBottom: 16 }}>Contact Support</h2>
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))",
            gap: 16,
          }}
        >
          {/* Hotline */}
          <div
            data-testid="support-hotline"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              backgroundColor: "white",
            }}
          >
            <div style={{ fontSize: 40, marginBottom: 8 }}>📞</div>
            <h3 style={{ fontSize: 18, marginBottom: 8 }}>Hotline</h3>
            <p style={{ color: "#1976d2", fontSize: 20, fontWeight: "bold", margin: 0 }}>
              {supportConfig.hotline}
            </p>
            <p style={{ color: "#666", fontSize: 14, marginTop: 8 }}>
              {supportConfig.businessHours}
            </p>
          </div>

          {/* Email */}
          <div
            data-testid="support-email"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              backgroundColor: "white",
            }}
          >
            <div style={{ fontSize: 40, marginBottom: 8 }}>✉️</div>
            <h3 style={{ fontSize: 18, marginBottom: 8 }}>Email</h3>
            <a
              href={`mailto:${supportConfig.email}`}
              style={{ color: "#1976d2", fontSize: 16 }}
            >
              {supportConfig.email}
            </a>
            <p style={{ color: "#666", fontSize: 14, marginTop: 8 }}>
              We&apos;ll respond within 24 hours
            </p>
          </div>

          {/* Viber */}
          <div
            data-testid="support-viber"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              backgroundColor: "white",
            }}
          >
            <div style={{ fontSize: 40, marginBottom: 8 }}>💬</div>
            <h3 style={{ fontSize: 18, marginBottom: 8 }}>Viber</h3>
            <a
              href={supportConfig.viber}
              target="_blank"
              rel="noopener noreferrer"
              style={{
                display: "inline-block",
                padding: "8px 16px",
                backgroundColor: "#7360f2",
                color: "white",
                textDecoration: "none",
                borderRadius: 4,
                fontSize: 14,
              }}
            >
              Chat on Viber
            </a>
          </div>

          {/* Zalo */}
          <div
            data-testid="support-zalo"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              backgroundColor: "white",
            }}
          >
            <div style={{ fontSize: 40, marginBottom: 8 }}>💬</div>
            <h3 style={{ fontSize: 18, marginBottom: 8 }}>Zalo</h3>
            <a
              href={supportConfig.zalo}
              target="_blank"
              rel="noopener noreferrer"
              style={{
                display: "inline-block",
                padding: "8px 16px",
                backgroundColor: "#0068ff",
                color: "white",
                textDecoration: "none",
                borderRadius: 4,
                fontSize: 14,
              }}
            >
              Chat on Zalo
            </a>
          </div>

          {/* Messenger */}
          <div
            data-testid="support-messenger"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              backgroundColor: "white",
            }}
          >
            <div style={{ fontSize: 40, marginBottom: 8 }}>💬</div>
            <h3 style={{ fontSize: 18, marginBottom: 8 }}>Messenger</h3>
            <a
              href={supportConfig.messenger}
              target="_blank"
              rel="noopener noreferrer"
              style={{
                display: "inline-block",
                padding: "8px 16px",
                backgroundColor: "#0084ff",
                color: "white",
                textDecoration: "none",
                borderRadius: 4,
                fontSize: 14,
              }}
            >
              Chat on Messenger
            </a>
          </div>
        </div>
      </section>

      {/* FAQ Categories */}
      <section style={{ marginBottom: 48 }}>
        <h2 style={{ fontSize: 24, marginBottom: 16 }}>Frequently Asked Questions</h2>
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))",
            gap: 16,
          }}
        >
          {[
            { title: "Orders & Payment", icon: "🛒" },
            { title: "Shipping & Delivery", icon: "🚚" },
            { title: "Returns & Refunds", icon: "↩️" },
            { title: "Account & Privacy", icon: "🔒" },
            { title: "Products & Warranty", icon: "📦" },
            { title: "Promotions & Vouchers", icon: "🎟️" },
          ].map((category, index) => (
            <div
              key={index}
              data-testid={`faq-category-${index}`}
              style={{
                padding: 24,
                border: "1px solid #ddd",
                borderRadius: 8,
                backgroundColor: "white",
                cursor: "pointer",
                transition: "all 0.2s",
              }}
            >
              <div style={{ fontSize: 32, marginBottom: 8 }}>{category.icon}</div>
              <h3 style={{ fontSize: 16, margin: 0 }}>{category.title}</h3>
            </div>
          ))}
        </div>
      </section>

      {/* Quick Links */}
      <section>
        <h2 style={{ fontSize: 24, marginBottom: 16 }}>Helpful Resources</h2>
        <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
          <a
            href="/terms"
            data-testid="link-terms"
            style={{
              padding: 16,
              border: "1px solid #ddd",
              borderRadius: 8,
              textDecoration: "none",
              color: "#1976d2",
              backgroundColor: "white",
            }}
          >
            Terms of Service →
          </a>
          <a
            href="/sales-policy"
            data-testid="link-sales-policy"
            style={{
              padding: 16,
              border: "1px solid #ddd",
              borderRadius: 8,
              textDecoration: "none",
              color: "#1976d2",
              backgroundColor: "white",
            }}
          >
            Sales Policy →
          </a>
          <a
            href="/contact"
            data-testid="link-contact"
            style={{
              padding: 16,
              border: "1px solid #ddd",
              borderRadius: 8,
              textDecoration: "none",
              color: "#1976d2",
              backgroundColor: "white",
            }}
          >
            Partner Contact →
          </a>
        </div>
      </section>
    </main>
  );
}

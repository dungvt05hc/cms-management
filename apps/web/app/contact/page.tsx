import { partnerContactConfig, supportConfig } from "@/lib/config";

export default function ContactPage() {
  return (
    <main
      style={{
        padding: 24,
        fontFamily: "system-ui, sans-serif",
        maxWidth: 1200,
        margin: "0 auto",
      }}
    >
      <div style={{ marginBottom: 32 }}>
        <h1 data-testid="contact-title" style={{ fontSize: 32, marginBottom: 8 }}>
          Contact Us
        </h1>
        <p style={{ color: "#666", fontSize: 16 }}>
          Get in touch with our team for partnership opportunities or support
        </p>
      </div>

      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fit, minmax(400px, 1fr))",
          gap: 32,
          marginBottom: 48,
        }}
      >
        {/* Partner Contact Information */}
        <section>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>Partner Contact</h2>
          <div
            data-testid="partner-contact"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              backgroundColor: "white",
            }}
          >
            <div style={{ marginBottom: 16 }}>
              <h3 style={{ fontSize: 18, marginBottom: 8, color: "#1976d2" }}>
                {partnerContactConfig.companyName}
              </h3>
            </div>

            <div style={{ marginBottom: 16 }}>
              <div style={{ fontWeight: "bold", marginBottom: 4 }}>📍 Address</div>
              <p style={{ margin: 0, color: "#666" }}>
                {partnerContactConfig.address}
              </p>
            </div>

            <div style={{ marginBottom: 16 }}>
              <div style={{ fontWeight: "bold", marginBottom: 4 }}>📧 Email</div>
              <a
                href={`mailto:${partnerContactConfig.email}`}
                style={{ color: "#1976d2", textDecoration: "none" }}
              >
                {partnerContactConfig.email}
              </a>
            </div>

            <div>
              <div style={{ fontWeight: "bold", marginBottom: 4 }}>📞 Phone</div>
              <a
                href={`tel:${partnerContactConfig.phone}`}
                style={{ color: "#1976d2", textDecoration: "none" }}
              >
                {partnerContactConfig.phone}
              </a>
            </div>
          </div>
        </section>

        {/* Customer Support */}
        <section>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>Customer Support</h2>
          <div
            data-testid="customer-support"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              backgroundColor: "white",
            }}
          >
            <div style={{ marginBottom: 16 }}>
              <div style={{ fontWeight: "bold", marginBottom: 4 }}>📞 Hotline</div>
              <p style={{ margin: 0, fontSize: 18, color: "#1976d2", fontWeight: "bold" }}>
                {supportConfig.hotline}
              </p>
              <p style={{ margin: "4px 0 0 0", fontSize: 14, color: "#666" }}>
                {supportConfig.businessHours}
              </p>
            </div>

            <div style={{ marginBottom: 16 }}>
              <div style={{ fontWeight: "bold", marginBottom: 4 }}>📧 Email</div>
              <a
                href={`mailto:${supportConfig.email}`}
                style={{ color: "#1976d2", textDecoration: "none" }}
              >
                {supportConfig.email}
              </a>
            </div>

            <div>
              <div style={{ fontWeight: "bold", marginBottom: 8 }}>💬 Social & Messaging</div>
              <div style={{ display: "flex", gap: 8, flexWrap: "wrap" }}>
                <a
                  href={supportConfig.viber}
                  target="_blank"
                  rel="noopener noreferrer"
                  data-testid="contact-viber-link"
                  style={{
                    padding: "8px 16px",
                    backgroundColor: "#7360f2",
                    color: "white",
                    textDecoration: "none",
                    borderRadius: 4,
                    fontSize: 14,
                  }}
                >
                  Viber
                </a>
                <a
                  href={supportConfig.zalo}
                  target="_blank"
                  rel="noopener noreferrer"
                  data-testid="contact-zalo-link"
                  style={{
                    padding: "8px 16px",
                    backgroundColor: "#0068ff",
                    color: "white",
                    textDecoration: "none",
                    borderRadius: 4,
                    fontSize: 14,
                  }}
                >
                  Zalo
                </a>
                <a
                  href={supportConfig.messenger}
                  target="_blank"
                  rel="noopener noreferrer"
                  data-testid="contact-messenger-link"
                  style={{
                    padding: "8px 16px",
                    backgroundColor: "#0084ff",
                    color: "white",
                    textDecoration: "none",
                    borderRadius: 4,
                    fontSize: 14,
                  }}
                >
                  Messenger
                </a>
              </div>
            </div>
          </div>
        </section>
      </div>

      {/* Quick Links */}
      <section>
        <h2 style={{ fontSize: 24, marginBottom: 16 }}>Helpful Resources</h2>
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fit, minmax(250px, 1fr))",
            gap: 16,
          }}
        >
          <a
            href="/help"
            data-testid="contact-link-help"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              textDecoration: "none",
              color: "inherit",
              backgroundColor: "white",
              transition: "all 0.2s",
            }}
          >
            <div style={{ fontSize: 32, marginBottom: 8 }}>❓</div>
            <h3 style={{ fontSize: 18, margin: 0, color: "#1976d2" }}>Help Center</h3>
            <p style={{ margin: "8px 0 0 0", color: "#666", fontSize: 14 }}>
              Find answers to common questions
            </p>
          </a>

          <a
            href="/terms"
            data-testid="contact-link-terms"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              textDecoration: "none",
              color: "inherit",
              backgroundColor: "white",
              transition: "all 0.2s",
            }}
          >
            <div style={{ fontSize: 32, marginBottom: 8 }}>📄</div>
            <h3 style={{ fontSize: 18, margin: 0, color: "#1976d2" }}>Terms of Service</h3>
            <p style={{ margin: "8px 0 0 0", color: "#666", fontSize: 14 }}>
              Read our terms and conditions
            </p>
          </a>

          <a
            href="/sales-policy"
            data-testid="contact-link-sales-policy"
            style={{
              padding: 24,
              border: "1px solid #ddd",
              borderRadius: 8,
              textDecoration: "none",
              color: "inherit",
              backgroundColor: "white",
              transition: "all 0.2s",
            }}
          >
            <div style={{ fontSize: 32, marginBottom: 8 }}>📋</div>
            <h3 style={{ fontSize: 18, margin: 0, color: "#1976d2" }}>Sales Policy</h3>
            <p style={{ margin: "8px 0 0 0", color: "#666", fontSize: 14 }}>
              Learn about our sales and return policies
            </p>
          </a>
        </div>
      </section>
    </main>
  );
}

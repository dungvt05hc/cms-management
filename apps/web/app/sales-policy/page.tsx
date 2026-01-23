export default function SalesPolicyPage() {
  return (
    <main
      style={{
        padding: 24,
        fontFamily: "system-ui, sans-serif",
        maxWidth: 900,
        margin: "0 auto",
      }}
    >
      <div style={{ marginBottom: 32 }}>
        <h1 data-testid="sales-policy-title" style={{ fontSize: 32, marginBottom: 8 }}>
          Sales Policy
        </h1>
        <p style={{ color: "#666", fontSize: 14 }}>
          Last updated: {new Date().toLocaleDateString()}
        </p>
      </div>

      <div style={{ lineHeight: 1.8, color: "#333" }}>
        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>1. Ordering & Payment</h2>
          <p>
            All orders placed through our website are subject to product availability and
            confirmation of the order price. We reserve the right to refuse any order placed
            through our website.
          </p>
          <h3 style={{ fontSize: 18, marginTop: 16, marginBottom: 8 }}>Payment Methods</h3>
          <ul style={{ marginTop: 8 }}>
            <li>Cash on Delivery (COD)</li>
            <li>Online payment via Payoo gateway</li>
            <li>Bank transfer</li>
          </ul>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>2. Pricing</h2>
          <p>
            All prices are listed in Vietnamese Dong (VND) and include applicable taxes
            unless otherwise stated. We reserve the right to change prices at any time, but
            changes will not affect orders already placed.
          </p>
          <p style={{ marginTop: 12 }}>
            In the event of a pricing error, we will notify you as soon as possible and give
            you the option to reconfirm your order at the correct price or cancel it.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>3. Shipping & Delivery</h2>
          <p>
            We offer shipping to addresses within Vietnam. Delivery times vary based on your
            location and the shipping method selected:
          </p>
          <ul style={{ marginTop: 8 }}>
            <li>Standard shipping: 3-5 business days</li>
            <li>Express shipping: 1-2 business days (available in major cities)</li>
          </ul>
          <p style={{ marginTop: 12 }}>
            Shipping fees are calculated based on the delivery address and order weight.
            Free shipping may be available for orders above a certain value.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>4. Returns & Refunds</h2>
          <p>
            We want you to be completely satisfied with your purchase. If you&apos;re not happy
            with your order, you may return it within 7 days of receipt for a full refund or
            exchange.
          </p>
          <h3 style={{ fontSize: 18, marginTop: 16, marginBottom: 8 }}>Return Conditions</h3>
          <ul style={{ marginTop: 8 }}>
            <li>Items must be unused and in original packaging</li>
            <li>Items must have all tags and labels attached</li>
            <li>Proof of purchase (invoice or receipt) is required</li>
            <li>Some items may not be eligible for return (e.g., perishable goods, personalized items)</li>
          </ul>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>5. Warranty & After-Sales Service</h2>
          <p>
            All products come with a manufacturer&apos;s warranty. Warranty periods vary by
            product and will be specified on the product page or included with the product
            documentation.
          </p>
          <p style={{ marginTop: 12 }}>
            For warranty claims or after-sales service, please contact our customer support
            team with your order number and a description of the issue.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>6. Promotions & Vouchers</h2>
          <p>
            Promotional offers and discount vouchers are subject to specific terms and
            conditions, which will be clearly stated at the time of the promotion. Unless
            otherwise specified:
          </p>
          <ul style={{ marginTop: 8 }}>
            <li>Vouchers cannot be combined with other promotions</li>
            <li>Maximum one discount voucher and one shipping voucher per order</li>
            <li>Vouchers are not exchangeable for cash</li>
            <li>Expired vouchers cannot be used</li>
          </ul>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>7. Privacy & Data Protection</h2>
          <p>
            We are committed to protecting your personal information. All customer data is
            handled in accordance with applicable privacy laws. For more information, please
            refer to our Privacy Policy.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>8. Contact Information</h2>
          <p>
            If you have any questions about our sales policy, please don&apos;t hesitate to
            contact us through our <a href="/help" style={{ color: "#1976d2" }}>Help Center</a> or
            <a href="/contact" style={{ color: "#1976d2" }}> Contact Page</a>.
          </p>
        </section>

        <div
          style={{
            marginTop: 48,
            padding: 24,
            backgroundColor: "#f8f9fa",
            borderRadius: 8,
            textAlign: "center",
          }}
        >
          <p style={{ margin: 0, color: "#666" }}>
            Need assistance with an order? Visit our <a href="/help" style={{ color: "#1976d2" }}>Help Center</a>
          </p>
        </div>
      </div>
    </main>
  );
}

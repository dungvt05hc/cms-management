export default function TermsPage() {
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
        <h1 data-testid="terms-title" style={{ fontSize: 32, marginBottom: 8 }}>
          Terms of Service
        </h1>
        <p style={{ color: "#666", fontSize: 14 }}>
          Last updated: {new Date().toLocaleDateString()}
        </p>
      </div>

      <div style={{ lineHeight: 1.8, color: "#333" }}>
        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>1. Acceptance of Terms</h2>
          <p>
            By accessing and using this website, you accept and agree to be bound by the terms
            and provision of this agreement. If you do not agree to abide by the above, please
            do not use this service.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>2. Use License</h2>
          <p>
            Permission is granted to temporarily download one copy of the materials
            (information or software) on our website for personal, non-commercial transitory
            viewing only. This is the grant of a license, not a transfer of title, and under
            this license you may not:
          </p>
          <ul style={{ marginTop: 12 }}>
            <li>Modify or copy the materials;</li>
            <li>Use the materials for any commercial purpose or for any public display;</li>
            <li>Attempt to decompile or reverse engineer any software contained on our website;</li>
            <li>Remove any copyright or other proprietary notations from the materials; or</li>
            <li>Transfer the materials to another person or mirror the materials on any other server.</li>
          </ul>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>3. Disclaimer</h2>
          <p>
            The materials on our website are provided on an &apos;as is&apos; basis. We make no
            warranties, expressed or implied, and hereby disclaim and negate all other
            warranties including, without limitation, implied warranties or conditions of
            merchantability, fitness for a particular purpose, or non-infringement of
            intellectual property or other violation of rights.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>4. Limitations</h2>
          <p>
            In no event shall CMS Management or its suppliers be liable for any damages
            (including, without limitation, damages for loss of data or profit, or due to
            business interruption) arising out of the use or inability to use the materials
            on our website, even if we or our authorized representative has been notified
            orally or in writing of the possibility of such damage.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>5. Accuracy of Materials</h2>
          <p>
            The materials appearing on our website could include technical, typographical,
            or photographic errors. We do not warrant that any of the materials on our
            website are accurate, complete or current. We may make changes to the materials
            contained on our website at any time without notice.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>6. Links</h2>
          <p>
            We have not reviewed all of the sites linked to our website and are not
            responsible for the contents of any such linked site. The inclusion of any link
            does not imply endorsement by us of the site. Use of any such linked website is
            at the user&apos;s own risk.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>7. Modifications</h2>
          <p>
            We may revise these terms of service for our website at any time without notice.
            By using this website you are agreeing to be bound by the then current version
            of these terms of service.
          </p>
        </section>

        <section style={{ marginBottom: 32 }}>
          <h2 style={{ fontSize: 24, marginBottom: 16 }}>8. Governing Law</h2>
          <p>
            These terms and conditions are governed by and construed in accordance with the
            laws of Vietnam and you irrevocably submit to the exclusive jurisdiction of the
            courts in that location.
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
            Questions about the Terms of Service? <a href="/contact" style={{ color: "#1976d2" }}>Contact us</a>
          </p>
        </div>
      </div>
    </main>
  );
}

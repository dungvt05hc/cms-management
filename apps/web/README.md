# CMS Management Web Application

Next.js-based e-commerce website with CMS capabilities.

## Getting Started

### Prerequisites
- Node.js 20.x or higher
- npm

### Installation

```bash
npm ci
```

### Development

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser.

### Build

```bash
npm run build
npm start
```

## Configuration

### Environment Variables

Create a `.env.local` file in the root of the `apps/web` directory based on `.env.example`:

```bash
cp .env.example .env.local
```

#### Analytics Configuration

The application supports Google Analytics 4 and Google Tag Manager for tracking and analytics.

**Environment Variables:**

- `NEXT_PUBLIC_GA_MEASUREMENT_ID` - Google Analytics 4 Measurement ID (format: `G-XXXXXXXXXX`)
  - Used for pageview tracking and standard GA4 events
  - Tracks route changes automatically
  - Optional: If not set, GA tracking will be disabled (stub logging in console)

- `NEXT_PUBLIC_GTM_CONTAINER_ID` - Google Tag Manager Container ID (format: `GTM-XXXXXXX`)
  - Used for advanced tag management and custom events
  - Injects GTM container script into the application
  - Optional: If not set, GTM will be disabled

**Example `.env.local`:**

```env
NEXT_PUBLIC_GA_MEASUREMENT_ID=G-ABC123XYZ
NEXT_PUBLIC_GTM_CONTAINER_ID=GTM-ABC123
```

**How it works:**

1. **Google Analytics (GA4):**
   - Automatically tracks pageviews on route changes
   - Uses Next.js App Router's `usePathname` and `useSearchParams` hooks
   - Logs pageview events to console for debugging (when GA ID is not set)

2. **Google Tag Manager (GTM):**
   - Injects GTM container script in the document head
   - Provides dataLayer for custom event tracking
   - Includes noscript fallback for accessibility

3. **Development Mode:**
   - When environment variables are not set, analytics functions as stubs
   - Console logs show what would be tracked (e.g., "Pageview tracked (stub): /products")
   - No external requests are made to Google services

**To disable analytics entirely:**
- Simply don't set the environment variables
- The application will work normally without analytics

## Scripts

- `npm run dev` - Start development server
- `npm run build` - Build production bundle
- `npm start` - Start production server
- `npm run lint` - Run ESLint
- `npm test` - Run Vitest unit tests
- `npm run e2e` - Run Playwright e2e tests
- `npm run e2e:full` - Run full e2e test suite with setup

## Project Structure

```
apps/web/
├── app/              # Next.js App Router pages
├── components/       # React components
├── lib/              # Utility libraries and helpers
│   └── analytics.ts  # Analytics utility functions
├── tests/            # Test files
├── .env.example      # Environment variable template
└── package.json
```

## Testing

### Unit Tests

```bash
npm test
```

### E2E Tests

```bash
npm run e2e
```

## License

Private

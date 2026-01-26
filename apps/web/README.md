# CMS Management Web Application

A modern e-commerce web application built with **Next.js 14**, **React 18**, and **TypeScript**. This customer-facing application provides a complete shopping experience with product browsing, search, cart management, checkout, and order tracking.

## Overview

This is the customer-facing frontend for the CMS Management e-commerce platform. It connects to the .NET 9 backend API to provide a seamless shopping experience.

### Tech Stack

- **Framework**: Next.js 14 (App Router)
- **UI Library**: React 18
- **Language**: TypeScript
- **Styling**: Inline styles (for simplicity)
- **Testing**: 
  - Unit tests: Vitest + jsdom
  - E2E tests: Playwright
- **Analytics**: Google Analytics 4 + Google Tag Manager

---

## Features

### 🛍️ Product Catalog
- **Product Browsing**: Infinite scroll with cursor-based pagination
- **Product Details**: Full product information with variants, images, specifications
- **Search**: Real-time search with suggestions
- **Categories**: Hierarchical category navigation
- **Featured Products**: Curated product recommendations
- **Product Suggestions**: Related products based on category

### 🛒 Shopping Cart
- **Cart Management**: Add, update, remove items
- **Variant Selection**: Choose product variants
- **Quantity Control**: Adjust item quantities
- **Cart Persistence**: Cart synced with backend
- **Item Selection**: Select specific items for checkout

### 📦 Checkout & Orders
- **Multi-step Checkout**: Address → Shipping → Payment → Review
- **Address Management**: Save multiple delivery addresses
- **Shipping Options**: Multiple carriers and methods
- **Voucher System**: Apply discount and shipping vouchers
- **Payment Methods**: COD and Payoo gateway integration
- **Order Tracking**: View order status and history
- **Order Actions**: Cancel orders, confirm delivery, reorder

### 👤 Customer Account
- **Profile Management**: View and update profile
- **Order History**: Track all orders with filtering
- **Address Book**: Manage delivery addresses
- **Notifications**: Receive order and system notifications
- **Device Management**: Manage push notification devices

### 📱 User Experience
- **Responsive Design**: Mobile-first approach
- **SEO Optimized**: Server-side rendering
- **Analytics Tracking**: GA4 and GTM integration
- **Fast Navigation**: Client-side routing
- **Error Handling**: User-friendly error messages

### 📄 Content Pages
- **Articles** (stub): Blog/news articles
- **Help Center**: Customer support information
- **Contact**: Contact form and support channels
- **Terms of Service**: Legal information
- **Sales Policy**: Return and refund policies

---

## Prerequisites

- **Node.js**: 20.x or higher
- **npm**: Comes with Node.js
- **Backend API**: The .NET 9 API must be running (see `src/Api`)

---

## Getting Started

### 1. Installation

```bash
cd apps/web
npm ci
```

### 2. Configuration

Create a `.env.local` file:

```bash
cp .env.example .env.local
```

Edit `.env.local` to configure your environment:

```env
# API Configuration
NEXT_PUBLIC_API_URL=http://localhost:5000

# Google Analytics 4 (Optional)
NEXT_PUBLIC_GA_MEASUREMENT_ID=G-XXXXXXXXXX

# Google Tag Manager (Optional)
NEXT_PUBLIC_GTM_CONTAINER_ID=GTM-XXXXXXX
```

**Environment Variables:**

- `NEXT_PUBLIC_API_URL` - Backend API base URL (default: `http://localhost:5000`)
- `NEXT_PUBLIC_GA_MEASUREMENT_ID` - Google Analytics 4 Measurement ID (optional)
- `NEXT_PUBLIC_GTM_CONTAINER_ID` - Google Tag Manager Container ID (optional)

### 3. Run Development Server

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser.

### 4. Build for Production

```bash
npm run build
npm start
```

---

## Project Structure

```
apps/web/
├── app/                          # Next.js App Router pages
│   ├── layout.tsx                # Root layout with header/footer
│   ├── page.tsx                  # Homepage
│   ├── account/                  # Customer account pages
│   │   ├── page.tsx              # Account dashboard
│   │   ├── addresses/            # Address management
│   │   ├── notifications/        # Notifications
│   │   └── orders/               # Order history
│   ├── articles/                 # Articles/blog pages (stub)
│   ├── cart/                     # Shopping cart
│   ├── category/                 # Category browsing
│   │   └── [slug]/               # Dynamic category pages
│   ├── checkout/                 # Checkout flow
│   │   ├── address/              # Address selection
│   │   ├── shipping/             # Shipping method
│   │   └── payment/              # Payment method
│   ├── contact/                  # Contact page
│   ├── help/                     # Help center
│   ├── p/                        # Product pages
│   │   └── [slug]/               # Dynamic product pages
│   ├── products/                 # Product listing
│   ├── sales-policy/             # Sales policy
│   └── terms/                    # Terms of service
├── components/                   # Reusable React components
│   ├── AnalyticsTracker.tsx      # Route tracking
│   ├── CategoryNavigation.tsx    # Category menu
│   ├── FeaturedProducts.tsx      # Featured products display
│   ├── GoogleAnalytics.tsx       # GA4 integration
│   ├── GoogleTagManager.tsx      # GTM integration
│   ├── ProductList.tsx           # Product grid with pagination
│   ├── SearchInput.tsx           # Search with suggestions
│   └── SearchWrapper.tsx         # Client-side search wrapper
├── lib/                          # Utilities and helpers
│   ├── analytics.ts              # Analytics utilities
│   ├── api.ts                    # API client functions
│   └── config.ts                 # App configuration
├── tests/                        # Test files
│   └── e2e/                      # Playwright E2E tests
│       ├── cart.spec.ts          # Cart functionality
│       ├── checkout.spec.ts      # Checkout flow
│       ├── home.spec.ts          # Homepage
│       ├── products.spec.ts      # Product browsing
│       └── search.spec.ts        # Search functionality
├── .env.example                  # Environment variables template
├── .env.local                    # Local environment (gitignored)
├── next.config.mjs               # Next.js configuration
├── playwright.config.ts          # Playwright configuration
├── tsconfig.json                 # TypeScript configuration
├── vitest.config.ts              # Vitest configuration
└── package.json                  # Dependencies and scripts
```

---

## Development Guide

### API Client (`lib/api.ts`)

The API client provides strongly-typed functions for all backend endpoints:

**Categories:**
```typescript
const categories = await getCategoryTree();
```

**Products:**
```typescript
const products = await getProducts({
  category: 'electronics',
  featured: true,
  sort: 'priceAsc',
  limit: 12
});

const product = await getProductBySlug('iphone-15-pro');
const suggestions = await getProductSuggestions('iphone-15-pro', 4);
```

**Cart:**
```typescript
const cart = await getCart(token);
await addCartItem(token, productId, variantId, quantity);
await updateCartItem(token, itemId, { quantity: 2 });
await deleteCartItem(token, itemId);
```

**Checkout:**
```typescript
const totals = await checkoutPreview(token, addressId, shippingMethod, carrier);
const result = await checkoutSubmit(token, addressId, shippingMethod, carrier, paymentMethod);
```

**Orders:**
```typescript
const orders = await getOrders(token, OrderStatus.Processing);
const order = await getOrderById(token, orderId);
await cancelOrder(token, orderId, reasonCode, note);
await confirmOrderReceived(token, orderId);
```

### Analytics Integration

The app includes Google Analytics 4 and Google Tag Manager support:

**How it works:**
1. **GoogleAnalytics** component loads GA4 script
2. **GoogleTagManager** component loads GTM script
3. **AnalyticsTracker** tracks route changes automatically

**Tracking events:**
```typescript
import { trackPageView } from '@/lib/analytics';

trackPageView('/products');
```

**Development mode:**
- When analytics IDs are not set, events are logged to console
- No external requests are made

### Adding a New Page

1. **Create route folder** in `app/`:
   ```bash
   mkdir -p app/my-feature
   ```

2. **Add page component** (`app/my-feature/page.tsx`):
   ```tsx
   export default function MyFeaturePage() {
     return <div>My Feature</div>;
   }
   ```

3. **Add to navigation** (if needed) in `app/layout.tsx`

4. **Add API functions** (if needed) in `lib/api.ts`

5. **Add E2E tests** in `tests/e2e/my-feature.spec.ts`

### Adding a New Component

1. **Create component** in `components/`:
   ```tsx
   // components/MyComponent.tsx
   export default function MyComponent({ prop }: { prop: string }) {
     return <div>{prop}</div>;
   }
   ```

2. **Use in pages**:
   ```tsx
   import MyComponent from '@/components/MyComponent';
   ```

### Styling Guidelines

Currently using **inline styles** for simplicity. Follow these patterns:

```tsx
// Container
<div style={{ maxWidth: 1200, margin: '0 auto', padding: 24 }}>

// Card
<div style={{ 
  border: '1px solid #ddd', 
  borderRadius: 8, 
  padding: 16 
}}>

// Button
<button style={{ 
  backgroundColor: '#007bff', 
  color: 'white', 
  padding: '12px 24px', 
  border: 'none', 
  borderRadius: 4,
  cursor: 'pointer' 
}}>

// Grid
<div style={{ 
  display: 'grid', 
  gridTemplateColumns: 'repeat(auto-fill, minmax(250px, 1fr))', 
  gap: 16 
}}>
```

Consider migrating to **Tailwind CSS** or **CSS Modules** for better maintainability.

---

## Testing

### Unit Tests (Vitest)

Run unit tests with Vitest:

```bash
npm test
```

**Example test:**
```typescript
import { describe, it, expect } from 'vitest';
import { trackPageView } from '@/lib/analytics';

describe('Analytics', () => {
  it('should track pageview', () => {
    expect(() => trackPageView('/test')).not.toThrow();
  });
});
```

### E2E Tests (Playwright)

Run end-to-end tests:

```bash
# Run all E2E tests
npm run e2e

# Run with full setup (builds app, starts server, runs tests)
npm run e2e:full

# Run specific test file
npx playwright test tests/e2e/products.spec.ts

# Run in UI mode (interactive)
npx playwright test --ui

# Generate test report
npx playwright show-report
```

**E2E Test Coverage:**
- ✅ Homepage loading and featured products
- ✅ Product browsing and pagination
- ✅ Product detail pages
- ✅ Search functionality with suggestions
- ✅ Cart operations (add, update, remove)
- ✅ Checkout flow (address, shipping, payment)
- ✅ Category navigation
- ✅ Responsive design

**Example E2E test:**
```typescript
import { test, expect } from '@playwright/test';

test('should load homepage', async ({ page }) => {
  await page.goto('/');
  await expect(page.locator('h1')).toContainText('Featured Products');
});
```

### Test Scripts

```bash
# Unit tests
npm test                    # Run once
npm test -- --watch         # Watch mode
npm test -- --coverage      # With coverage

# E2E tests
npm run e2e                 # Run all E2E tests
npm run e2e -- --headed     # With browser visible
npm run e2e -- --debug      # Debug mode
npm run e2e:full            # Full test suite with setup
```

---

## API Integration

### Authentication

Most API endpoints require JWT authentication. Store the token securely:

```typescript
// Client-side (use localStorage or secure cookie)
const token = localStorage.getItem('authToken');

// Pass token to API functions
const cart = await getCart(token);
```

### Error Handling

API functions throw errors that should be caught:

```typescript
try {
  const product = await getProductBySlug(slug);
} catch (error) {
  console.error('Failed to load product:', error);
  // Show error message to user
}
```

### Data Types

All API types are defined in `lib/api.ts`:

```typescript
import type { 
  Product, 
  Cart, 
  Order, 
  Address 
} from '@/lib/api';
```

---

## Performance Optimization

### Server-Side Rendering (SSR)

Product and category pages use SSR for better SEO:

```typescript
// Fetches data on the server
export default async function ProductPage({ params }: { params: { slug: string } }) {
  const product = await getProductBySlug(params.slug);
  return <ProductDetails product={product} />;
}
```

### Client-Side Caching

Next.js automatically caches API responses. Control caching:

```typescript
// No cache (always fresh)
fetch(url, { cache: 'no-store' });

// Cache for 1 hour
fetch(url, { next: { revalidate: 3600 } });
```

### Infinite Scroll

Product lists use cursor-based pagination for better performance:

```typescript
const { items, nextCursor, hasMore } = await getProducts({ 
  cursor: lastCursor,
  limit: 12 
});
```

---

## Deployment

### Production Build

```bash
npm run build
```

This creates an optimized production build in `.next/`.

### Environment Variables

Set production environment variables:

```bash
# Required
NEXT_PUBLIC_API_URL=https://api.example.com

# Optional
NEXT_PUBLIC_GA_MEASUREMENT_ID=G-XXXXXXXXXX
NEXT_PUBLIC_GTM_CONTAINER_ID=GTM-XXXXXXX
```

### Deployment Platforms

**Vercel (Recommended):**
```bash
npm install -g vercel
vercel --prod
```

**Docker:**
```dockerfile
FROM node:20-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:20-alpine
WORKDIR /app
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/package.json ./package.json
EXPOSE 3000
CMD ["npm", "start"]
```

**Build Docker image:**
```bash
docker build -t cms-web .
docker run -p 3000:3000 -e NEXT_PUBLIC_API_URL=https://api.example.com cms-web
```

---

## Troubleshooting

### API Connection Issues

```bash
# Check if API is running
curl http://localhost:5000/health

# Verify API URL in .env.local
cat .env.local | grep NEXT_PUBLIC_API_URL
```

### Build Errors

```bash
# Clear cache and rebuild
rm -rf .next node_modules
npm ci
npm run build
```

### TypeScript Errors

```bash
# Check TypeScript errors
npx tsc --noEmit

# Update types
npm install --save-dev @types/node @types/react @types/react-dom
```

### E2E Test Failures

```bash
# Update Playwright browsers
npx playwright install

# Run tests in headed mode to debug
npm run e2e -- --headed

# Generate trace for debugging
npm run e2e -- --trace on
```

---

## Roadmap

### Planned Features

- [ ] **User Authentication**: Login, registration, password reset UI
- [ ] **Product Reviews**: Customer reviews and ratings
- [ ] **Wishlist**: Save products for later
- [ ] **Compare Products**: Side-by-side product comparison
- [ ] **Live Chat**: Customer support chat
- [ ] **Order Tracking**: Real-time shipping updates
- [ ] **Loyalty Program**: Points and rewards
- [ ] **Multi-language**: i18n support
- [ ] **Dark Mode**: Theme switcher
- [ ] **PWA**: Progressive web app features
- [ ] **Articles CMS**: Full blog/news system (backend needed)

### Technical Improvements

- [ ] Migrate to **Tailwind CSS** for better styling
- [ ] Add **React Query** for data fetching
- [ ] Implement **Zustand** or **Redux** for state management
- [ ] Add **Storybook** for component documentation
- [ ] Improve **accessibility** (WCAG 2.1 AA)
- [ ] Add **performance monitoring** (Web Vitals)
- [ ] Implement **service worker** for offline support
- [ ] Add **image optimization** with next/image

---

## Contributing

1. Follow the existing code structure and patterns
2. Write tests for new features
3. Ensure TypeScript type safety
4. Run linter before committing: `npm run lint`
5. Test both desktop and mobile views
6. Update documentation for new features

---

## Scripts Reference

| Script | Description |
|--------|-------------|
| `npm run dev` | Start development server on port 3000 |
| `npm run build` | Build production bundle |
| `npm start` | Start production server |
| `npm run lint` | Run ESLint linter |
| `npm test` | Run Vitest unit tests |
| `npm run e2e` | Run Playwright E2E tests |
| `npm run e2e:full` | Run full E2E test suite with setup |

---

## Related Documentation

- [Backend API Documentation](../../src/README.md)
- [Next.js Documentation](https://nextjs.org/docs)
- [React Documentation](https://react.dev)
- [Playwright Documentation](https://playwright.dev)
- [TypeScript Documentation](https://www.typescriptlang.org/docs)

---

## License

Private - Copyright (c) CMS Management. All rights reserved.

---

## Support

For questions or issues:
- Check the [troubleshooting section](#troubleshooting)
- Review [E2E tests](./tests/e2e) for usage examples
- Contact the development team

---

**Built with ❤️ using Next.js 14 and React 18**

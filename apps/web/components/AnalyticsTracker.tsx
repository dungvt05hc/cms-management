'use client';

import { useEffect, Suspense } from 'react';
import { usePathname, useSearchParams } from 'next/navigation';
import { trackPageview } from '@/lib/analytics';

/**
 * Analytics tracker component (internal)
 * Tracks pageviews on route changes in Next.js App Router
 */
function AnalyticsTrackerInternal() {
  const pathname = usePathname();
  const searchParams = useSearchParams();

  useEffect(() => {
    if (pathname) {
      const url = searchParams?.toString()
        ? `${pathname}?${searchParams.toString()}`
        : pathname;
      
      trackPageview(url);
    }
  }, [pathname, searchParams]);

  return null;
}

/**
 * Analytics tracker with Suspense boundary
 * Required for Next.js static rendering
 */
export default function AnalyticsTracker() {
  return (
    <Suspense fallback={null}>
      <AnalyticsTrackerInternal />
    </Suspense>
  );
}

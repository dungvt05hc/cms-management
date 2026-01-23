/**
 * Analytics utilities for Google Analytics 4 and Google Tag Manager
 * Environment-based configuration
 */

// Type definitions for Google Analytics
declare global {
  interface Window {
    gtag?: (...args: unknown[]) => void;
    dataLayer?: unknown[];
  }
}

/**
 * Initialize Google Analytics 4
 * Call this once on app initialization
 */
export function initGA() {
  const measurementId = process.env.NEXT_PUBLIC_GA_MEASUREMENT_ID;
  
  if (!measurementId) {
    console.warn('[Analytics] GA Measurement ID not configured');
    return;
  }

  // Initialize dataLayer if not exists
  window.dataLayer = window.dataLayer || [];
  
  // Define gtag function
  window.gtag = function gtag(...args: unknown[]) {
    window.dataLayer?.push(args);
  };
  
  // Configure GA
  window.gtag('js', new Date());
  window.gtag('config', measurementId, {
    page_path: window.location.pathname,
  });
  
  console.log('[Analytics] GA initialized with ID:', measurementId);
}

/**
 * Track a pageview in Google Analytics
 * @param url - The page URL to track
 */
export function trackPageview(url: string) {
  const measurementId = process.env.NEXT_PUBLIC_GA_MEASUREMENT_ID;
  
  if (!measurementId) {
    console.log('[Analytics] Pageview tracked (stub):', url);
    return;
  }

  if (window.gtag) {
    window.gtag('config', measurementId, {
      page_path: url,
    });
    console.log('[Analytics] Pageview tracked:', url);
  }
}

/**
 * Get GTM container ID from environment
 */
export function getGTMContainerId(): string | undefined {
  return process.env.NEXT_PUBLIC_GTM_CONTAINER_ID;
}

/**
 * Check if analytics is enabled (at least one service configured)
 */
export function isAnalyticsEnabled(): boolean {
  return !!(
    process.env.NEXT_PUBLIC_GA_MEASUREMENT_ID || 
    process.env.NEXT_PUBLIC_GTM_CONTAINER_ID
  );
}

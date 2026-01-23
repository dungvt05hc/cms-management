import { describe, it, expect, beforeEach, vi } from 'vitest';
import { trackPageview, getGTMContainerId, isAnalyticsEnabled } from '@/lib/analytics';

describe('Analytics', () => {
  beforeEach(() => {
    // Clear console mocks
    vi.clearAllMocks();
    
    // Reset window.gtag and dataLayer
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    delete (window as any).gtag;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    delete (window as any).dataLayer;
  });

  describe('trackPageview', () => {
    it('should log stub message when GA ID is not configured', () => {
      const consoleSpy = vi.spyOn(console, 'log');
      const url = '/test-page';
      
      trackPageview(url);
      
      expect(consoleSpy).toHaveBeenCalledWith(
        '[Analytics] Pageview tracked (stub):',
        url
      );
    });

    it('should track pageview when gtag is available', () => {
      // Mock environment variable
      process.env.NEXT_PUBLIC_GA_MEASUREMENT_ID = 'G-TEST123';
      
      // Mock gtag
      const gtagMock = vi.fn();
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      (window as any).gtag = gtagMock;
      
      const url = '/test-page';
      trackPageview(url);
      
      expect(gtagMock).toHaveBeenCalledWith('config', 'G-TEST123', {
        page_path: url,
      });
      
      // Clean up
      delete process.env.NEXT_PUBLIC_GA_MEASUREMENT_ID;
    });
  });

  describe('getGTMContainerId', () => {
    it('should return undefined when GTM ID is not configured', () => {
      expect(getGTMContainerId()).toBeUndefined();
    });

    it('should return GTM ID when configured', () => {
      process.env.NEXT_PUBLIC_GTM_CONTAINER_ID = 'GTM-TEST123';
      
      expect(getGTMContainerId()).toBe('GTM-TEST123');
      
      // Clean up
      delete process.env.NEXT_PUBLIC_GTM_CONTAINER_ID;
    });
  });

  describe('isAnalyticsEnabled', () => {
    it('should return false when no analytics is configured', () => {
      expect(isAnalyticsEnabled()).toBe(false);
    });

    it('should return true when GA is configured', () => {
      process.env.NEXT_PUBLIC_GA_MEASUREMENT_ID = 'G-TEST123';
      
      expect(isAnalyticsEnabled()).toBe(true);
      
      // Clean up
      delete process.env.NEXT_PUBLIC_GA_MEASUREMENT_ID;
    });

    it('should return true when GTM is configured', () => {
      process.env.NEXT_PUBLIC_GTM_CONTAINER_ID = 'GTM-TEST123';
      
      expect(isAnalyticsEnabled()).toBe(true);
      
      // Clean up
      delete process.env.NEXT_PUBLIC_GTM_CONTAINER_ID;
    });

    it('should return true when both GA and GTM are configured', () => {
      process.env.NEXT_PUBLIC_GA_MEASUREMENT_ID = 'G-TEST123';
      process.env.NEXT_PUBLIC_GTM_CONTAINER_ID = 'GTM-TEST123';
      
      expect(isAnalyticsEnabled()).toBe(true);
      
      // Clean up
      delete process.env.NEXT_PUBLIC_GA_MEASUREMENT_ID;
      delete process.env.NEXT_PUBLIC_GTM_CONTAINER_ID;
    });
  });
});

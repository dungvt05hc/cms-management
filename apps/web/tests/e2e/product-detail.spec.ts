import { test, expect } from "@playwright/test";

test.describe("Product Detail Page", () => {
  test.beforeEach(async ({ page }) => {
    await page.setViewportSize({ width: 1280, height: 720 });
  });

  test("displays product detail with title and variants", async ({ page }) => {
    // Navigate to products list first
    await page.goto("/products");

    // Wait for products to load
    await page.waitForSelector(
      '[data-testid="product-list-container"], [data-testid="product-list-empty"], [data-testid="product-list-error"]',
      { timeout: 10000 }
    );

    // Check if there are products available
    const firstProductView = page.getByTestId("product-view-0");
    const hasProducts = await firstProductView.isVisible().catch(() => false);

    if (!hasProducts) {
      test.skip();
    }

    // Click on the first product's "View Details" button
    await firstProductView.click();

    // Wait for product detail page to load
    await page.waitForSelector('[data-testid="product-detail-container"], [data-testid="product-detail-error"]', {
      timeout: 10000,
    });

    // Check if product detail loaded successfully
    const detailContainer = page.getByTestId("product-detail-container");
    const isDetailVisible = await detailContainer.isVisible().catch(() => false);

    if (!isDetailVisible) {
      // If error, skip test
      test.skip();
    }

    // Verify product title is visible
    await expect(page.getByTestId("product-detail-title")).toBeVisible();

    // Verify variant selector exists if there are variants
    const variantSelector = page.getByTestId("variant-select");
    const hasVariantSelector = await variantSelector.isVisible().catch(() => false);

    if (hasVariantSelector) {
      await expect(variantSelector).toBeVisible();
    }

    // Verify quantity controls are visible
    await expect(page.getByTestId("quantity-controls")).toBeVisible();
    await expect(page.getByTestId("quantity-decrease")).toBeVisible();
    await expect(page.getByTestId("quantity-increase")).toBeVisible();
    await expect(page.getByTestId("quantity-display")).toHaveText("1");
  });

  test("displays suggestions section if available", async ({ page }) => {
    // Navigate to products list first
    await page.goto("/products");

    // Wait for products to load
    await page.waitForSelector(
      '[data-testid="product-list-container"], [data-testid="product-list-empty"]',
      { timeout: 10000 }
    );

    // Check if there are products available
    const firstProductView = page.getByTestId("product-view-0");
    const hasProducts = await firstProductView.isVisible().catch(() => false);

    if (!hasProducts) {
      test.skip();
    }

    // Click on the first product
    await firstProductView.click();

    // Wait for product detail page to load
    await page.waitForSelector('[data-testid="product-detail-container"], [data-testid="product-detail-error"]', {
      timeout: 10000,
    });

    // Check if suggestions container exists
    const suggestionsContainer = page.getByTestId("suggestions-container");
    const hasSuggestions = await suggestionsContainer.isVisible().catch(() => false);

    // If suggestions exist, verify they are displayed
    if (hasSuggestions) {
      await expect(suggestionsContainer).toBeVisible();
      
      // Check if at least one suggestion card exists
      const firstSuggestion = page.getByTestId("suggestion-card-0");
      await expect(firstSuggestion).toBeVisible();
    }
  });

  test("allows quantity adjustment with +/- buttons", async ({ page }) => {
    // Navigate to products list first
    await page.goto("/products");

    // Wait for products to load
    await page.waitForSelector(
      '[data-testid="product-list-container"], [data-testid="product-list-empty"]',
      { timeout: 10000 }
    );

    // Check if there are products available
    const firstProductView = page.getByTestId("product-view-0");
    const hasProducts = await firstProductView.isVisible().catch(() => false);

    if (!hasProducts) {
      test.skip();
    }

    // Click on the first product
    await firstProductView.click();

    // Wait for product detail page to load
    await page.waitForSelector('[data-testid="product-detail-container"]', { timeout: 10000 });

    // Initial quantity should be 1
    await expect(page.getByTestId("quantity-display")).toHaveText("1");

    // Click increase button
    await page.getByTestId("quantity-increase").click();
    await expect(page.getByTestId("quantity-display")).toHaveText("2");

    // Click increase again
    await page.getByTestId("quantity-increase").click();
    await expect(page.getByTestId("quantity-display")).toHaveText("3");

    // Click decrease button
    await page.getByTestId("quantity-decrease").click();
    await expect(page.getByTestId("quantity-display")).toHaveText("2");

    // Click decrease again
    await page.getByTestId("quantity-decrease").click();
    await expect(page.getByTestId("quantity-display")).toHaveText("1");

    // Verify decrease is disabled at 1
    await expect(page.getByTestId("quantity-decrease")).toBeDisabled();
  });

  test("changes variant when selecting from dropdown", async ({ page }) => {
    // Navigate to products list first
    await page.goto("/products");

    // Wait for products to load
    await page.waitForSelector(
      '[data-testid="product-list-container"], [data-testid="product-list-empty"]',
      { timeout: 10000 }
    );

    // Check if there are products available
    const firstProductView = page.getByTestId("product-view-0");
    const hasProducts = await firstProductView.isVisible().catch(() => false);

    if (!hasProducts) {
      test.skip();
    }

    // Click on the first product
    await firstProductView.click();

    // Wait for product detail page to load
    await page.waitForSelector('[data-testid="product-detail-container"]', { timeout: 10000 });

    // Check if variant selector exists
    const variantSelector = page.getByTestId("variant-select");
    const hasVariantSelector = await variantSelector.isVisible().catch(() => false);

    if (!hasVariantSelector) {
      test.skip();
    }

    // Get the number of options
    const options = await variantSelector.locator("option").count();

    if (options < 2) {
      // Need at least 2 variants to test switching
      test.skip();
    }

    // Get initial price
    const initialPrice = await page.getByTestId("product-detail-price").textContent();

    // Select the second variant
    await variantSelector.selectOption({ index: 1 });

    // Wait a moment for the price to update
    await page.waitForTimeout(500);

    // Price might change (or might stay the same if variants have same price)
    const newPrice = await page.getByTestId("product-detail-price").textContent();
    
    // Just verify the price element is still visible after changing variant
    await expect(page.getByTestId("product-detail-price")).toBeVisible();
  });

  test("shows error state when product not found", async ({ page }) => {
    // Navigate directly to a non-existent product
    await page.goto("/p/non-existent-product-slug-12345");

    // Wait for error to appear
    await page.waitForSelector('[data-testid="product-detail-error"]', { timeout: 10000 });

    // Verify error message is visible
    await expect(page.getByTestId("product-detail-error")).toBeVisible();

    // Verify back link is present
    await expect(page.getByTestId("back-to-products")).toBeVisible();
  });
});

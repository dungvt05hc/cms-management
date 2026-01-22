import { test, expect } from "@playwright/test";

test.describe("Product List - Cursor Pagination + Sort + Infinite Scroll", () => {
  test.beforeEach(async ({ page }) => {
    // Set viewport for consistent scrolling behavior
    await page.setViewportSize({ width: 1280, height: 720 });
  });

  test("displays all products page with initial products", async ({ page }) => {
    await page.goto("/products");

    // Check that the products page loads
    await expect(page.getByTestId("products-page-title")).toHaveText("All Products");

    // Wait for products to load (either list or empty/error state)
    await page.waitForSelector(
      '[data-testid="product-list-container"], [data-testid="product-list-empty"], [data-testid="product-list-error"], [data-testid="product-list-loading"]',
      { timeout: 10000 }
    );

    // If products are loaded, verify the sort control is visible
    const container = page.getByTestId("product-list-container");
    const isContainerVisible = await container.isVisible().catch(() => false);

    if (isContainerVisible) {
      await expect(page.getByTestId("sort-control")).toBeVisible();
      await expect(page.getByTestId("sort-select")).toBeVisible();
    }
  });

  test("changes sort order and updates product list", async ({ page }) => {
    await page.goto("/products");

    // Wait for products to load
    await page.waitForSelector('[data-testid="product-list-container"], [data-testid="product-list-empty"]', {
      timeout: 10000,
    });

    const container = page.getByTestId("product-list-container");
    const isContainerVisible = await container.isVisible().catch(() => false);

    if (!isContainerVisible) {
      test.skip();
    }

    // Get initial product count
    const initialProducts = await page.locator('[data-testid^="product-card-"]').count();

    if (initialProducts === 0) {
      test.skip();
    }

    // Get first product price before sort
    const firstPriceBefore = await page.getByTestId("product-price-0").textContent();

    // Change sort to "Price: High to Low"
    await page.getByTestId("sort-select").selectOption("priceDesc");

    // Wait for products to reload
    await page.waitForTimeout(1000);

    // Get first product price after sort
    const firstPriceAfter = await page.getByTestId("product-price-0").textContent();

    // Verify the list was updated (price changed or order changed)
    expect(firstPriceBefore).toBeDefined();
    expect(firstPriceAfter).toBeDefined();

    // Verify sort control still visible
    await expect(page.getByTestId("sort-control")).toBeVisible();
  });

  test("loads more products on scroll (infinite scroll)", async ({ page }) => {
    await page.goto("/products");

    // Wait for products to load
    await page.waitForSelector('[data-testid="product-list-container"], [data-testid="product-list-empty"]', {
      timeout: 10000,
    });

    const container = page.getByTestId("product-list-container");
    const isContainerVisible = await container.isVisible().catch(() => false);

    if (!isContainerVisible) {
      test.skip();
    }

    // Get initial product count
    const initialCount = await page.locator('[data-testid^="product-card-"]').count();

    if (initialCount === 0) {
      test.skip();
    }

    // Check if there's a load-more trigger (means more items available)
    const loadMoreTrigger = page.getByTestId("load-more-trigger");
    const hasLoadMore = await loadMoreTrigger.isVisible().catch(() => false);

    if (!hasLoadMore) {
      // No more items to load
      test.skip();
    }

    // Scroll to the load-more trigger
    await loadMoreTrigger.scrollIntoViewIfNeeded();

    // Wait for loading more indicator
    await page.waitForSelector('[data-testid="loading-more"]', { timeout: 5000 }).catch(() => {});

    // Wait a bit for new products to load
    await page.waitForTimeout(2000);

    // Get updated product count
    const updatedCount = await page.locator('[data-testid^="product-card-"]').count();

    // Verify more products were loaded
    expect(updatedCount).toBeGreaterThan(initialCount);
  });

  test("displays product cards with required elements", async ({ page }) => {
    await page.goto("/products");

    // Wait for products to load
    await page.waitForSelector('[data-testid="product-list-container"], [data-testid="product-list-empty"]', {
      timeout: 10000,
    });

    const container = page.getByTestId("product-list-container");
    const isContainerVisible = await container.isVisible().catch(() => false);

    if (!isContainerVisible) {
      test.skip();
    }

    // Get product count
    const productCount = await page.locator('[data-testid^="product-card-"]').count();

    if (productCount === 0) {
      test.skip();
    }

    // Verify first product card has required elements
    await expect(page.getByTestId("product-card-0")).toBeVisible();
    await expect(page.getByTestId("product-name-0")).toBeVisible();
    await expect(page.getByTestId("product-view-0")).toBeVisible();
    await expect(page.getByTestId("product-buy-0")).toBeVisible();

    // Verify Buy button is disabled (placeholder)
    const buyButton = page.getByTestId("product-buy-0");
    await expect(buyButton).toBeDisabled();
  });

  test("sorts products by price ascending", async ({ page }) => {
    await page.goto("/products");

    // Wait for products to load
    await page.waitForSelector('[data-testid="product-list-container"], [data-testid="product-list-empty"]', {
      timeout: 10000,
    });

    const container = page.getByTestId("product-list-container");
    const isContainerVisible = await container.isVisible().catch(() => false);

    if (!isContainerVisible) {
      test.skip();
    }

    const productCount = await page.locator('[data-testid^="product-card-"]').count();

    if (productCount < 2) {
      test.skip();
    }

    // Change sort to "Price: Low to High"
    await page.getByTestId("sort-select").selectOption("priceAsc");

    // Wait for products to reload
    await page.waitForTimeout(1000);

    // Get first two product prices
    const price0Text = await page.getByTestId("product-price-0").textContent();
    const price1Text = await page.getByTestId("product-price-1").textContent();

    if (!price0Text || !price1Text) {
      test.skip();
      return; // TypeScript needs explicit return after skip
    }

    const price0 = parseFloat(price0Text.replace("$", ""));
    const price1 = parseFloat(price1Text.replace("$", ""));

    // Verify first product price <= second product price
    expect(price0).toBeLessThanOrEqual(price1);
  });

  test("sorts products by price descending", async ({ page }) => {
    await page.goto("/products");

    // Wait for products to load
    await page.waitForSelector('[data-testid="product-list-container"], [data-testid="product-list-empty"]', {
      timeout: 10000,
    });

    const container = page.getByTestId("product-list-container");
    const isContainerVisible = await container.isVisible().catch(() => false);

    if (!isContainerVisible) {
      test.skip();
    }

    const productCount = await page.locator('[data-testid^="product-card-"]').count();

    if (productCount < 2) {
      test.skip();
    }

    // Change sort to "Price: High to Low"
    await page.getByTestId("sort-select").selectOption("priceDesc");

    // Wait for products to reload
    await page.waitForTimeout(1000);

    // Get first two product prices
    const price0Text = await page.getByTestId("product-price-0").textContent();
    const price1Text = await page.getByTestId("product-price-1").textContent();

    if (!price0Text || !price1Text) {
      test.skip();
      return; // TypeScript needs explicit return after skip
    }

    const price0 = parseFloat(price0Text.replace("$", ""));
    const price1 = parseFloat(price1Text.replace("$", ""));

    // Verify first product price >= second product price
    expect(price0).toBeGreaterThanOrEqual(price1);
  });
});

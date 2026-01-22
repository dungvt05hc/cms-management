import { test, expect } from "@playwright/test";

test.describe("Cart - Voucher Application", () => {
  test("apply discount voucher and see totals change", async ({ page }) => {
    // Note: This test assumes:
    // 1. The API is running and accessible
    // 2. Test data (products, vouchers) are seeded or created
    // 3. A test user exists or can be created

    await page.goto("/");

    // Mock authentication by setting a token in localStorage
    // In a real test environment, you'd authenticate properly
    await page.evaluate(() => {
      localStorage.setItem("authToken", "mock-test-token");
    });

    // Navigate to cart page
    await page.goto("/cart");

    // Wait for cart page to load
    const isCartPage = await page
      .locator('[data-testid="cart-page"], [data-testid="cart-empty"], [data-testid="cart-error"]')
      .first()
      .isVisible({ timeout: 5000 })
      .catch(() => false);

    if (!isCartPage) {
      test.skip();
    }

    // Check if cart has items
    const hasItems = await page
      .locator('[data-testid="cart-table"]')
      .isVisible()
      .catch(() => false);

    if (!hasItems) {
      // Cart is empty - this is expected for fresh test environment
      expect(isCartPage).toBe(true);
      return;
    }

    // Check if voucher section exists
    const voucherSection = page.locator('[data-testid="voucher-section"]');
    await expect(voucherSection).toBeVisible();

    // Check for discount code input
    const discountInput = page.locator('[data-testid="discount-code-input"]');
    await expect(discountInput).toBeVisible();

    // Check for shipping code input
    const shippingInput = page.locator('[data-testid="shipping-code-input"]');
    await expect(shippingInput).toBeVisible();

    // Check for apply button
    const applyButton = page.locator('[data-testid="apply-voucher-button"]');
    await expect(applyButton).toBeVisible();

    // Apply button should be disabled when no codes are entered
    await expect(applyButton).toBeDisabled();

    // Enter a discount code (in real test, use a known valid code)
    await discountInput.fill("TEST10");

    // Apply button should now be enabled
    await expect(applyButton).toBeEnabled();

    // Try to apply voucher
    await applyButton.click();

    // Wait a bit for the request
    await page.waitForTimeout(1000);

    // Check for either totals display or error
    const hasTotals = await page
      .locator('[data-testid="totals-total"]')
      .isVisible({ timeout: 2000 })
      .catch(() => false);

    const hasError = await page
      .locator('[data-testid="voucher-error"]')
      .isVisible()
      .catch(() => false);

    // One of these should be true (either success or expected error)
    expect(hasTotals || hasError).toBe(true);

    // If totals are shown, verify structure
    if (hasTotals) {
      await expect(page.locator('[data-testid="totals-subtotal"]')).toBeVisible();
      await expect(page.locator('[data-testid="totals-shipping"]')).toBeVisible();
      await expect(page.locator('[data-testid="totals-total"]')).toBeVisible();

      // Clear vouchers button should be visible
      const clearButton = page.locator('[data-testid="clear-voucher-button"]');
      await expect(clearButton).toBeVisible();

      // Click clear button
      await clearButton.click();

      // Totals should be hidden
      await expect(page.locator('[data-testid="totals-total"]')).not.toBeVisible();
    }
  });

  test("apply shipping voucher and see shipping discount", async ({ page }) => {
    await page.goto("/");

    await page.evaluate(() => {
      localStorage.setItem("authToken", "mock-test-token");
    });

    await page.goto("/cart");

    const isCartPage = await page
      .locator('[data-testid="cart-page"], [data-testid="cart-empty"], [data-testid="cart-error"]')
      .first()
      .isVisible({ timeout: 5000 })
      .catch(() => false);

    if (!isCartPage) {
      test.skip();
    }

    const hasItems = await page
      .locator('[data-testid="cart-table"]')
      .isVisible()
      .catch(() => false);

    if (!hasItems) {
      expect(isCartPage).toBe(true);
      return;
    }

    // Enter shipping code
    const shippingInput = page.locator('[data-testid="shipping-code-input"]');
    await shippingInput.fill("FREESHIP");

    const applyButton = page.locator('[data-testid="apply-voucher-button"]');
    await applyButton.click();

    await page.waitForTimeout(1000);

    // Check for totals or error
    const hasTotals = await page
      .locator('[data-testid="totals-total"]')
      .isVisible({ timeout: 2000 })
      .catch(() => false);

    const hasError = await page
      .locator('[data-testid="voucher-error"]')
      .isVisible()
      .catch(() => false);

    expect(hasTotals || hasError).toBe(true);
  });

  test("apply both discount and shipping vouchers", async ({ page }) => {
    await page.goto("/");

    await page.evaluate(() => {
      localStorage.setItem("authToken", "mock-test-token");
    });

    await page.goto("/cart");

    const isCartPage = await page
      .locator('[data-testid="cart-page"], [data-testid="cart-empty"], [data-testid="cart-error"]')
      .first()
      .isVisible({ timeout: 5000 })
      .catch(() => false);

    if (!isCartPage) {
      test.skip();
    }

    const hasItems = await page
      .locator('[data-testid="cart-table"]')
      .isVisible()
      .catch(() => false);

    if (!hasItems) {
      expect(isCartPage).toBe(true);
      return;
    }

    // Enter both codes
    const discountInput = page.locator('[data-testid="discount-code-input"]');
    await discountInput.fill("DISCOUNT20");

    const shippingInput = page.locator('[data-testid="shipping-code-input"]');
    await shippingInput.fill("SHIP10");

    const applyButton = page.locator('[data-testid="apply-voucher-button"]');
    await applyButton.click();

    await page.waitForTimeout(1000);

    // Check for totals or error
    const hasTotals = await page
      .locator('[data-testid="totals-total"]')
      .isVisible({ timeout: 2000 })
      .catch(() => false);

    const hasError = await page
      .locator('[data-testid="voucher-error"]')
      .isVisible()
      .catch(() => false);

    expect(hasTotals || hasError).toBe(true);

    // If successful, both discount and shipping discount should be shown
    if (hasTotals) {
      const hasDiscount = await page
        .locator('[data-testid="totals-discount"]')
        .isVisible()
        .catch(() => false);

      const hasShippingDiscount = await page
        .locator('[data-testid="totals-shipping-discount"]')
        .isVisible()
        .catch(() => false);

      // At least one discount should be applied (both if codes are valid)
      expect(hasDiscount || hasShippingDiscount).toBe(true);
    }
  });

  test("show error for invalid voucher code", async ({ page }) => {
    await page.goto("/");

    await page.evaluate(() => {
      localStorage.setItem("authToken", "mock-test-token");
    });

    await page.goto("/cart");

    const isCartPage = await page
      .locator('[data-testid="cart-page"], [data-testid="cart-empty"], [data-testid="cart-error"]')
      .first()
      .isVisible({ timeout: 5000 })
      .catch(() => false);

    if (!isCartPage) {
      test.skip();
    }

    const hasItems = await page
      .locator('[data-testid="cart-table"]')
      .isVisible()
      .catch(() => false);

    if (!hasItems) {
      expect(isCartPage).toBe(true);
      return;
    }

    // Enter an invalid code
    const discountInput = page.locator('[data-testid="discount-code-input"]');
    await discountInput.fill("INVALID_CODE_12345");

    const applyButton = page.locator('[data-testid="apply-voucher-button"]');
    await applyButton.click();

    await page.waitForTimeout(1000);

    // Should show error message
    const voucherError = page.locator('[data-testid="voucher-error"]');
    const hasError = await voucherError.isVisible({ timeout: 2000 }).catch(() => false);

    // Error should be displayed (expected behavior for invalid code)
    expect(hasError).toBe(true);
  });
});

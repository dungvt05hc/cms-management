import { test, expect } from "@playwright/test";

test.describe("Home Page - Categories, Banner, and Featured Products", () => {
  test("displays home page with banner placeholder", async ({ page }) => {
    await page.goto("/");
    
    // Check that the home page loads
    await expect(page.getByTestId("home-title")).toHaveText("CMS Management");
    
    // Check that banner placeholder is visible
    await expect(page.getByTestId("banner-placeholder")).toBeVisible();
    await expect(page.getByTestId("banner-placeholder")).toContainText("Banner Placeholder");
  });

  test("displays featured products or empty state", async ({ page }) => {
    await page.goto("/");
    
    // Check that either featured products section or empty state is visible
    const featuredSection = page.getByTestId("featured-products");
    const emptyState = page.getByTestId("featured-products-empty");
    
    const isFeaturedVisible = await featuredSection.isVisible().catch(() => false);
    const isEmptyVisible = await emptyState.isVisible().catch(() => false);
    
    // At least one should be visible
    expect(isFeaturedVisible || isEmptyVisible).toBe(true);
    
    if (isFeaturedVisible) {
      await expect(featuredSection).toContainText("Featured Products");
    }
  });

  test("navigates from home to category page when clicking a category", async ({ page }) => {
    // First, ensure we're starting from the home page
    await page.goto("/");
    
    // Check that the home page loads
    await expect(page.getByTestId("home-title")).toHaveText("CMS Management");
    
    // Check if categories are available
    const navigation = page.getByTestId("category-navigation");
    const emptyMessage = page.getByTestId("category-navigation-empty");
    
    const isNavigationVisible = await navigation.isVisible().catch(() => false);
    
    if (isNavigationVisible) {
      // Find the first category link
      const firstCategoryLink = page.locator('[data-testid^="category-link-"]').first();
      const firstCategoryLinkExists = await firstCategoryLink.count() > 0;
      
      if (firstCategoryLinkExists) {
        // Get the category ID from the link's testid attribute
        const testId = await firstCategoryLink.getAttribute("data-testid");
        expect(testId).toBeTruthy();
        
        // Click the category link
        await firstCategoryLink.click();
        
        // Wait for navigation to complete
        await page.waitForURL(/\/category\/.+/);
        
        // Verify we're on the category page
        await expect(page.getByTestId("category-page-title")).toBeVisible();
        await expect(page.getByTestId("category-page-title")).toHaveText("Category Products");
        
        // Verify category ID is displayed
        await expect(page.getByTestId("category-id")).toBeVisible();
        
        // Test back to home link
        await page.getByTestId("back-to-home").click();
        await expect(page.getByTestId("home-title")).toBeVisible();
      }
    } else {
      // If no categories, verify empty message is shown
      await expect(emptyMessage).toBeVisible();
    }
  });

  test("displays categories with expand/collapse functionality", async ({ page }) => {
    await page.goto("/");
    
    // Check if categories are available
    const navigation = page.getByTestId("category-navigation");
    const isNavigationVisible = await navigation.isVisible().catch(() => false);
    
    if (isNavigationVisible) {
      // Find a category with children (toggle button)
      const firstToggle = page.locator('[data-testid^="category-toggle-"]').first();
      const hasToggle = await firstToggle.count() > 0;
      
      if (hasToggle) {
        // Get the category ID from toggle
        const toggleTestId = await firstToggle.getAttribute("data-testid");
        const categoryId = toggleTestId?.replace("category-toggle-", "");
        
        if (categoryId) {
          const childrenContainer = page.getByTestId(`category-children-${categoryId}`);
          
          // Children should be visible initially (expanded by default)
          await expect(childrenContainer).toBeVisible();
          
          // Click toggle to collapse
          await firstToggle.click();
          await expect(childrenContainer).not.toBeVisible();
          
          // Click toggle to expand again
          await firstToggle.click();
          await expect(childrenContainer).toBeVisible();
        }
      }
    }
  });
});

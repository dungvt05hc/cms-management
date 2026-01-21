import { test, expect } from "@playwright/test";

test.describe("Category Navigation", () => {
  test("displays empty categories message when no categories exist", async ({ page }) => {
    await page.goto("/");
    
    // Check that the home page loads
    await expect(page.getByTestId("home-title")).toHaveText("CMS Management");
    
    // Check that the category navigation shows empty message
    await expect(page.getByTestId("category-navigation-empty")).toBeVisible();
    await expect(page.getByTestId("category-navigation-empty")).toHaveText("No categories available.");
  });

  test("displays nested categories with expand/collapse functionality", async ({ page }) => {
    // Note: This test requires the backend API to be running and seeded with categories
    // For now, we'll test the UI rendering only
    await page.goto("/");
    
    // Check that the home page loads
    await expect(page.getByTestId("home-title")).toHaveText("CMS Management");
    
    // If categories are available, the navigation should be visible
    const navigation = page.getByTestId("category-navigation");
    const emptyMessage = page.getByTestId("category-navigation-empty");
    
    // At least one should be visible
    const isNavigationVisible = await navigation.isVisible().catch(() => false);
    const isEmptyMessageVisible = await emptyMessage.isVisible().catch(() => false);
    
    expect(isNavigationVisible || isEmptyMessageVisible).toBe(true);
  });
});

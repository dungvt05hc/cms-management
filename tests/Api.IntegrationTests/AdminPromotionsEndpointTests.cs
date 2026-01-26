// <copyright file="AdminPromotionsEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for admin promotions endpoints.
/// </summary>
public class AdminPromotionsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPromotionsEndpointTests"/> class.
    /// </summary>
    /// <param name="factory">The web application factory.</param>
    public AdminPromotionsEndpointTests(WebApplicationFactory<Program> factory)
    {
        this.client = factory.CreateClient();
    }

    /// <summary>
    /// Tests that getting promotions without authentication returns 401.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetPromotions_WithoutAuth_Returns401()
    {
        var response = await this.client.GetAsync("/admin/promotions");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Tests that getting promotions with admin auth returns 200.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetPromotions_WithAdminAuth_Returns200()
    {
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await this.client.GetAsync("/admin/promotions");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Tests CRUD operations for promotions.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task PromotionCrud_WorksCorrectly()
    {
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create promotion
        var createRequest = new
        {
            code = $"TEST{DateTime.UtcNow.Ticks}",
            name = "Test Promotion",
            description = "Test description",
            discountType = "percentage",
            discountValue = 15,
            minOrderAmount = 50,
            startDate = DateTime.UtcNow.AddDays(-1).ToString("o"),
            endDate = DateTime.UtcNow.AddDays(30).ToString("o"),
            usageLimit = 100,
            isActive = true,
        };

        var createResponse = await this.client.PostAsJsonAsync("/admin/promotions", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdPromotion = await createResponse.Content.ReadFromJsonAsync<PromotionResponse>(this.jsonOptions);
        Assert.NotNull(createdPromotion);
        Assert.Equal(createRequest.code.ToUpper(), createdPromotion.Code);
        Assert.Equal(createRequest.name, createdPromotion.Name);
        Assert.Equal(15, createdPromotion.DiscountValue);

        // Get promotion by ID
        var getResponse = await this.client.GetAsync($"/admin/promotions/{createdPromotion.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetchedPromotion = await getResponse.Content.ReadFromJsonAsync<PromotionResponse>(this.jsonOptions);
        Assert.NotNull(fetchedPromotion);
        Assert.Equal(createdPromotion.Id, fetchedPromotion.Id);

        // Update promotion
        var updateRequest = new
        {
            name = "Updated Promotion",
            discountValue = 20,
        };

        var updateResponse = await this.client.PutAsJsonAsync($"/admin/promotions/{createdPromotion.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedPromotion = await updateResponse.Content.ReadFromJsonAsync<PromotionResponse>(this.jsonOptions);
        Assert.NotNull(updatedPromotion);
        Assert.Equal("Updated Promotion", updatedPromotion.Name);
        Assert.Equal(20, updatedPromotion.DiscountValue);

        // Toggle promotion
        var toggleResponse = await this.client.PostAsync($"/admin/promotions/{createdPromotion.Id}/toggle", null);
        Assert.Equal(HttpStatusCode.OK, toggleResponse.StatusCode);

        var toggledPromotion = await toggleResponse.Content.ReadFromJsonAsync<PromotionResponse>(this.jsonOptions);
        Assert.NotNull(toggledPromotion);
        Assert.False(toggledPromotion.IsActive);

        // Delete promotion
        var deleteResponse = await this.client.DeleteAsync($"/admin/promotions/{createdPromotion.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify deletion
        var getDeletedResponse = await this.client.GetAsync($"/admin/promotions/{createdPromotion.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    /// <summary>
    /// Tests that creating a promotion with duplicate code returns 409.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreatePromotion_DuplicateCode_Returns409()
    {
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var code = $"DUP{DateTime.UtcNow.Ticks}";
        var request = new
        {
            code,
            name = "Duplicate Test",
            discountType = "percentage",
            discountValue = 10,
            startDate = DateTime.UtcNow.ToString("o"),
            endDate = DateTime.UtcNow.AddDays(7).ToString("o"),
            isActive = true,
        };

        // Create first
        var response1 = await this.client.PostAsJsonAsync("/admin/promotions", request);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Try to create duplicate
        var response2 = await this.client.PostAsJsonAsync("/admin/promotions", request);
        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);

        // Cleanup
        var created = await response1.Content.ReadFromJsonAsync<PromotionResponse>(this.jsonOptions);
        if (created != null)
        {
            await this.client.DeleteAsync($"/admin/promotions/{created.Id}");
        }
    }

    /// <summary>
    /// Tests that creating a promotion with invalid data returns 400.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreatePromotion_InvalidData_Returns400()
    {
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Missing required fields
        var request = new
        {
            code = string.Empty,
            name = "Test",
            discountType = "percentage",
            discountValue = 10,
            startDate = DateTime.UtcNow.ToString("o"),
            endDate = DateTime.UtcNow.AddDays(7).ToString("o"),
        };

        var response = await this.client.PostAsJsonAsync("/admin/promotions", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Tests filtering promotions by active status.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetPromotions_FilterByActive_ReturnsFilteredResults()
    {
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await this.client.GetAsync("/admin/promotions?isActive=true");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedPromotionResponse>(this.jsonOptions);
        Assert.NotNull(result);
        Assert.All(result.Items, p => Assert.True(p.IsActive));
    }

    /// <summary>
    /// Gets the admin token asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with a string result containing the admin token.</returns>
    private async Task<string> GetAdminTokenAsync()
    {
        var loginResponse = await this.client.PostAsJsonAsync("/admin/auth/login", new
        {
            emailOrPhone = "admin@cms.local",
            password = "Admin@123",
        });

        if (!loginResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to get admin token");
        }

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>(this.jsonOptions);
        return loginResult?.AccessToken ?? throw new InvalidOperationException("No access token received");
    }

    private record LoginResult(string AccessToken, int ExpiresIn, string Role);

    private record PromotionResponse(
        Guid Id,
        string Code,
        string Name,
        string? Description,
        string DiscountType,
        decimal DiscountValue,
        decimal? MinOrderAmount,
        decimal? MaxDiscountAmount,
        DateTime StartDate,
        DateTime EndDate,
        int? UsageLimit,
        int? UsageLimitPerCustomer,
        int UsedCount,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    private record PagedPromotionResponse(
        List<PromotionResponse> Items,
        int TotalCount,
        int Page,
        int PageSize);
}

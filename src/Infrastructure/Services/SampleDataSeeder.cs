// <copyright file="SampleDataSeeder.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service to seed sample e-commerce data for development/testing.
/// </summary>
public class SampleDataSeeder
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<SampleDataSeeder> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleDataSeeder"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public SampleDataSeeder(
        IAppDbContext dbContext,
        IDateTime dateTime,
        ILogger<SampleDataSeeder> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Seeds sample data if the database is empty.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Check if data already exists
        var hasCategories = await this.dbContext.Categories.AnyAsync(cancellationToken);
        if (hasCategories)
        {
            this.logger.LogInformation("Sample data already exists, skipping seed");
            return;
        }

        this.logger.LogInformation("Starting sample data seed...");

        await this.SeedCategoriesAsync(cancellationToken);
        await this.SeedProductsAsync(cancellationToken);

        this.logger.LogInformation("Sample data seed completed successfully");
    }

    private async Task SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = new List<Category>
        {
            // Electronics
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Electronics",
                ParentId = null,
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Smartphones",
                ParentId = null, // Will be set later
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Laptops",
                ParentId = null, // Will be set later
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Headphones",
                ParentId = null, // Will be set later
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
            },

            // Fashion
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Fashion",
                ParentId = null,
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Men's Clothing",
                ParentId = null, // Will be set later
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Women's Clothing",
                ParentId = null, // Will be set later
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
            },

            // Home & Living
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Home & Living",
                ParentId = null,
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
            },
        };

        // Set parent relationships
        var electronics = categories[0];
        categories[1].ParentId = electronics.Id; // Smartphones
        categories[2].ParentId = electronics.Id; // Laptops
        categories[3].ParentId = electronics.Id; // Headphones

        var fashion = categories[4];
        categories[5].ParentId = fashion.Id; // Men's Clothing
        categories[6].ParentId = fashion.Id; // Women's Clothing

        this.dbContext.Categories.AddRange(categories);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Seeded {Count} categories", categories.Count);
    }

    private async Task SeedProductsAsync(CancellationToken cancellationToken)
    {
        var smartphonesCategory = await this.dbContext.Categories
            .FirstAsync(c => c.Name == "Smartphones", cancellationToken);

        var laptopsCategory = await this.dbContext.Categories
            .FirstAsync(c => c.Name == "Laptops", cancellationToken);

        var headphonesCategory = await this.dbContext.Categories
            .FirstAsync(c => c.Name == "Headphones", cancellationToken);

        var products = new List<Product>
        {
            // Smartphones
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "iPhone 15 Pro",
                Slug = "iphone-15-pro",
                Description = "The latest iPhone with titanium design and A17 Pro chip. Features advanced camera system with 48MP main camera.",
                CategoryId = smartphonesCategory.Id,
                IsActive = true,
                IsFeatured = true,
                Specifications = @"{""screen"": ""6.1-inch Super Retina XDR"", ""chip"": ""A17 Pro"", ""camera"": ""48MP Main + 12MP Ultra Wide + 12MP Telephoto"", ""battery"": ""Up to 23 hours video playback""}",
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "IPH15P-128-TIT",
                        VariantName = "128GB Natural Titanium",
                        Price = 999.00m,
                        StockQuantity = 50,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "IPH15P-256-TIT",
                        VariantName = "256GB Natural Titanium",
                        Price = 1099.00m,
                        StockQuantity = 45,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "IPH15P-512-BLK",
                        VariantName = "512GB Black Titanium",
                        Price = 1299.00m,
                        StockQuantity = 30,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                },
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Samsung Galaxy S24 Ultra",
                Slug = "samsung-galaxy-s24-ultra",
                Description = "Premium Android flagship with 200MP camera, S Pen, and powerful AI features. Built with titanium frame.",
                CategoryId = smartphonesCategory.Id,
                IsActive = true,
                IsFeatured = true,
                Specifications = @"{""screen"": ""6.8-inch Dynamic AMOLED 2X"", ""processor"": ""Snapdragon 8 Gen 3"", ""camera"": ""200MP + 50MP + 12MP + 10MP"", ""battery"": ""5000mAh""}",
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "S24U-256-TIT",
                        VariantName = "256GB Titanium Gray",
                        Price = 1199.00m,
                        StockQuantity = 40,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "S24U-512-VIO",
                        VariantName = "512GB Titanium Violet",
                        Price = 1419.00m,
                        StockQuantity = 35,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                },
            },

            // Laptops
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "MacBook Pro 14-inch M3",
                Slug = "macbook-pro-14-m3",
                Description = "Powerful laptop with M3 chip, stunning Liquid Retina XDR display. Perfect for creative professionals.",
                CategoryId = laptopsCategory.Id,
                IsActive = true,
                IsFeatured = true,
                Specifications = @"{""screen"": ""14.2-inch Liquid Retina XDR"", ""chip"": ""Apple M3"", ""memory"": ""8GB to 128GB unified memory"", ""battery"": ""Up to 22 hours""}",
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "MBP14-M3-8-512",
                        VariantName = "M3 / 8GB / 512GB SSD",
                        Price = 1599.00m,
                        StockQuantity = 25,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "MBP14-M3P-18-512",
                        VariantName = "M3 Pro / 18GB / 512GB SSD",
                        Price = 1999.00m,
                        StockQuantity = 20,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                },
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Dell XPS 15",
                Slug = "dell-xps-15",
                Description = "Premium Windows laptop with InfinityEdge display. Powerful performance for work and entertainment.",
                CategoryId = laptopsCategory.Id,
                IsActive = true,
                IsFeatured = false,
                Specifications = @"{""screen"": ""15.6-inch FHD+"", ""processor"": ""Intel Core i7-13700H"", ""memory"": ""16GB DDR5"", ""graphics"": ""NVIDIA GeForce RTX 4050""}",
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "XPS15-I7-16-512",
                        VariantName = "i7 / 16GB / 512GB SSD",
                        Price = 1499.00m,
                        StockQuantity = 30,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                },
            },

            // Headphones
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "AirPods Pro (2nd generation)",
                Slug = "airpods-pro-2nd-gen",
                Description = "Premium wireless earbuds with active noise cancellation. Up to 2x more ANC than previous generation.",
                CategoryId = headphonesCategory.Id,
                IsActive = true,
                IsFeatured = true,
                Specifications = @"{""type"": ""In-ear"", ""connectivity"": ""Bluetooth 5.3"", ""anc"": ""Active Noise Cancellation"", ""battery"": ""Up to 6 hours listening time""}",
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "AIRPODS-PRO2-USB",
                        VariantName = "USB-C Charging Case",
                        Price = 249.00m,
                        StockQuantity = 100,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                },
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Sony WH-1000XM5",
                Slug = "sony-wh-1000xm5",
                Description = "Industry-leading noise canceling headphones with premium sound quality and 30-hour battery life.",
                CategoryId = headphonesCategory.Id,
                IsActive = true,
                IsFeatured = true,
                Specifications = @"{""type"": ""Over-ear"", ""connectivity"": ""Bluetooth 5.2"", ""anc"": ""Industry-leading ANC"", ""battery"": ""Up to 30 hours""}",
                CreatedAt = this.dateTime.UtcNow,
                UpdatedAt = this.dateTime.UtcNow,
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "WH1000XM5-BLK",
                        VariantName = "Black",
                        Price = 399.00m,
                        StockQuantity = 60,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                    new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        Sku = "WH1000XM5-SLV",
                        VariantName = "Silver",
                        Price = 399.00m,
                        StockQuantity = 55,
                        CreatedAt = this.dateTime.UtcNow,
                        UpdatedAt = this.dateTime.UtcNow,
                    },
                },
            },
        };

        this.dbContext.Products.AddRange(products);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Seeded {Count} products with variants", products.Count);
    }
}

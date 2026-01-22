// <copyright file="ShippingBootstrapService.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service to bootstrap shipping methods and carriers on first run.
/// </summary>
public class ShippingBootstrapService
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<ShippingBootstrapService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShippingBootstrapService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public ShippingBootstrapService(
        IAppDbContext dbContext,
        IDateTime dateTime,
        ILogger<ShippingBootstrapService> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Bootstraps shipping methods and carriers if they don't exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task BootstrapAsync(CancellationToken cancellationToken = default)
    {
        // Check if any shipping methods exist
        var methodsExist = await this.dbContext.ShippingMethods
            .AnyAsync(cancellationToken);

        if (methodsExist)
        {
            this.logger.LogInformation("Shipping methods already exist, skipping bootstrap");
            return;
        }

        this.logger.LogInformation("Bootstrapping default shipping methods and carriers");

        // Create STANDARD shipping method
        var standardMethod = new ShippingMethod
        {
            Id = Guid.NewGuid(),
            Code = "STANDARD",
            Name = "Standard Shipping",
            Description = "Standard delivery in 3-5 business days",
            IsActive = true,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        // Create EXPRESS shipping method
        var expressMethod = new ShippingMethod
        {
            Id = Guid.NewGuid(),
            Code = "EXPRESS",
            Name = "Express Shipping",
            Description = "Fast delivery in 1-2 business days",
            IsActive = true,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        this.dbContext.ShippingMethods.Add(standardMethod);
        this.dbContext.ShippingMethods.Add(expressMethod);

        // Create carriers for STANDARD method
        var ghnStandard = new ShippingCarrier
        {
            Id = Guid.NewGuid(),
            ShippingMethodId = standardMethod.Id,
            Code = "GHN",
            Name = "Giao Hàng Nhanh",
            Description = "GHN Standard Service",
            SupportsCOD = true,
            IsActive = true,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        var ghtkStandard = new ShippingCarrier
        {
            Id = Guid.NewGuid(),
            ShippingMethodId = standardMethod.Id,
            Code = "GHTK",
            Name = "Giao Hàng Tiết Kiệm",
            Description = "GHTK Standard Service",
            SupportsCOD = true,
            IsActive = true,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        var vnpostStandard = new ShippingCarrier
        {
            Id = Guid.NewGuid(),
            ShippingMethodId = standardMethod.Id,
            Code = "VNPOST",
            Name = "VNPost",
            Description = "VNPost Standard Service",
            SupportsCOD = false,
            IsActive = true,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        // Create carriers for EXPRESS method
        var ghnExpress = new ShippingCarrier
        {
            Id = Guid.NewGuid(),
            ShippingMethodId = expressMethod.Id,
            Code = "GHN",
            Name = "Giao Hàng Nhanh",
            Description = "GHN Express Service",
            SupportsCOD = true,
            IsActive = true,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        var ghtkExpress = new ShippingCarrier
        {
            Id = Guid.NewGuid(),
            ShippingMethodId = expressMethod.Id,
            Code = "GHTK",
            Name = "Giao Hàng Tiết Kiệm",
            Description = "GHTK Express Service",
            SupportsCOD = true,
            IsActive = true,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        this.dbContext.ShippingCarriers.Add(ghnStandard);
        this.dbContext.ShippingCarriers.Add(ghtkStandard);
        this.dbContext.ShippingCarriers.Add(vnpostStandard);
        this.dbContext.ShippingCarriers.Add(ghnExpress);
        this.dbContext.ShippingCarriers.Add(ghtkExpress);

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Shipping methods and carriers bootstrapped successfully");
    }
}

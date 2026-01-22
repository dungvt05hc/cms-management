// <copyright file="UpdateShippingConfigHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Shipping.UpdateShippingConfig;

/// <summary>
/// Handler for UpdateShippingConfigCommand.
/// </summary>
public class UpdateShippingConfigHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<UpdateShippingConfigHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateShippingConfigHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public UpdateShippingConfigHandler(
        IAppDbContext dbContext,
        ILogger<UpdateShippingConfigHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles updating shipping configuration.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the operation.</returns>
    public async Task Handle(UpdateShippingConfigCommand command, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Updating shipping configuration with {Count} entries", command.Configs.Count);

        var existingConfigs = await this.dbContext.ShippingConfigs
            .Where(c => command.Configs.Keys.Contains(c.Key))
            .ToListAsync(cancellationToken);

        foreach (var (key, value) in command.Configs)
        {
            var existing = existingConfigs.FirstOrDefault(c => c.Key == key);
            if (existing != null)
            {
                existing.Value = value;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var newConfig = new ShippingConfig
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Value = value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                this.dbContext.ShippingConfigs.Add(newConfig);
            }
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Shipping configuration updated successfully");
    }
}

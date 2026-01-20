// <copyright file="DependencyInjection.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Health;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Provides dependency injection configuration for Application services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Application services into the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<HealthHandler>();

        return services;
    }
}

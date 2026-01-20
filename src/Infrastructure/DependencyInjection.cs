// <copyright file="DependencyInjection.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Provides dependency injection configuration for Infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Infrastructure services into the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDateTime, SystemClock>();

        return services;
    }
}

// <copyright file="DependencyInjection.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDateTime, SystemClock>();

        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Authentication
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Email
        services.AddScoped<IEmailSender, EmailSender>();

        // Shipping
        services.AddScoped<IShippingQuoteProvider, StubShippingQuoteProvider>();

        // Payment
        services.AddScoped<IPaymentGateway, StubPaymentGateway>();

        // Invoice
        services.AddScoped<IInvoiceIssuer, StubInvoiceIssuer>();

        // Bootstrap services
        services.AddScoped<SuperAdminBootstrapService>();
        services.AddScoped<ShippingBootstrapService>();

        return services;
    }
}

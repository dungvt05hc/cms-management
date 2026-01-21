// <copyright file="DependencyInjection.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.AdminAuth.Login;
using Application.Features.Auth.ForgotPassword;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using Application.Features.Auth.ResetPassword;
using Application.Features.Auth.VerifyOtp;
using Application.Features.Categories.CreateCategory;
using Application.Features.Categories.DeleteCategory;
using Application.Features.Categories.GetCategoryTree;
using Application.Features.Categories.UpdateCategory;
using Application.Features.Products.CreateProduct;
using Application.Features.Products.DeleteProduct;
using Application.Features.Products.GetProductById;
using Application.Features.Products.GetProducts;
using Application.Features.Products.UpdateProduct;
using Application.Features.Staff.CreateStaff;
using Application.Features.Users.GetProfile;
using Application.Health;
using FluentValidation;
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

        // Register handlers
        services.AddScoped<RegisterHandler>();
        services.AddScoped<VerifyOtpHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<GetProfileHandler>();
        services.AddScoped<ForgotPasswordHandler>();
        services.AddScoped<ResetPasswordHandler>();
        services.AddScoped<AdminLoginHandler>();
        services.AddScoped<CreateStaffHandler>();
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<UpdateCategoryHandler>();
        services.AddScoped<DeleteCategoryHandler>();
        services.AddScoped<GetCategoryTreeHandler>();

        // Register product handlers
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<UpdateProductHandler>();
        services.AddScoped<DeleteProductHandler>();
        services.AddScoped<GetProductsHandler>();
        services.AddScoped<GetProductByIdHandler>();

        // Register validators
        services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

        return services;
    }
}

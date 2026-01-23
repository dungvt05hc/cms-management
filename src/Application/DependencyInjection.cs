// <copyright file="DependencyInjection.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Addresses.CreateAddress;
using Application.Features.Addresses.DeleteAddress;
using Application.Features.Addresses.GetAddresses;
using Application.Features.Addresses.SetDefaultAddress;
using Application.Features.Addresses.UpdateAddress;
using Application.Features.AdminAuth.Login;
using Application.Features.Auth.ForgotPassword;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using Application.Features.Auth.ResetPassword;
using Application.Features.Auth.VerifyOtp;
using Application.Features.Cart.AddCartItem;
using Application.Features.Cart.DeleteCartItem;
using Application.Features.Cart.GetCart;
using Application.Features.Cart.UpdateCartItem;
using Application.Features.Categories.CreateCategory;
using Application.Features.Categories.DeleteCategory;
using Application.Features.Categories.GetCategoryTree;
using Application.Features.Categories.UpdateCategory;
using Application.Features.Orders.CancelOrder;
using Application.Features.Orders.ConfirmOrder;
using Application.Features.Orders.GetOrderById;
using Application.Features.Orders.GetOrders;
using Application.Features.Orders.Reorder;
using Application.Features.ProductGroups.CreateProductGroup;
using Application.Features.ProductGroups.DeleteProductGroup;
using Application.Features.ProductGroups.GetProductGroups;
using Application.Features.ProductGroups.UpdateProductGroup;
using Application.Features.Products.CreateProduct;
using Application.Features.Products.DeleteProduct;
using Application.Features.Products.GetProductById;
using Application.Features.Products.GetProductBySlug;
using Application.Features.Products.GetProducts;
using Application.Features.Products.GetProductSuggestions;
using Application.Features.Products.UpdateProduct;
using Application.Features.Search;
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
        services.AddScoped<GetProductBySlugHandler>();
        services.AddScoped<GetProductSuggestionsHandler>();

        // Register product group handlers
        services.AddScoped<CreateProductGroupHandler>();
        services.AddScoped<UpdateProductGroupHandler>();
        services.AddScoped<DeleteProductGroupHandler>();
        services.AddScoped<GetProductGroupsHandler>();

        // Register search handlers
        services.AddScoped<SearchSuggestHandler>();

        // Register cart handlers
        services.AddScoped<GetCartHandler>();
        services.AddScoped<AddCartItemHandler>();
        services.AddScoped<UpdateCartItemHandler>();
        services.AddScoped<DeleteCartItemHandler>();

        // Register address handlers
        services.AddScoped<GetAddressesHandler>();
        services.AddScoped<CreateAddressHandler>();
        services.AddScoped<UpdateAddressHandler>();
        services.AddScoped<DeleteAddressHandler>();
        services.AddScoped<SetDefaultAddressHandler>();

        // Register shipping handlers
        services.AddScoped<Application.Features.Shipping.GetShippingMethods.GetShippingMethodsHandler>();
        services.AddScoped<Application.Features.Shipping.GetShippingQuote.GetShippingQuoteHandler>();
        services.AddScoped<Application.Features.Shipping.UpdateShippingConfig.UpdateShippingConfigHandler>();

        // Register checkout handlers
        services.AddScoped<Application.Features.Checkout.ApplyVoucher.ApplyVoucherHandler>();
        services.AddScoped<Application.Features.Checkout.Preview.CheckoutPreviewHandler>();
        services.AddScoped<Application.Features.Checkout.Submit.CheckoutSubmitHandler>();

        // Register payment handlers
        services.AddScoped<Application.Features.Payments.PayooCallback.PayooCallbackHandler>();

        // Register order handlers
        services.AddScoped<GetOrdersHandler>();
        services.AddScoped<GetOrderByIdHandler>();
        services.AddScoped<CancelOrderHandler>();
        services.AddScoped<ConfirmOrderHandler>();
        services.AddScoped<ReorderHandler>();

        // Register invoice profile handlers
        services.AddScoped<Application.Features.InvoiceProfiles.GetInvoiceProfiles.GetInvoiceProfilesHandler>();
        services.AddScoped<Application.Features.InvoiceProfiles.CreateInvoiceProfile.CreateInvoiceProfileHandler>();

        // Register invoice handlers
        services.AddScoped<Application.Features.Invoices.RunIssuance.RunIssuanceHandler>();

        // Register validators
        services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

        return services;
    }
}

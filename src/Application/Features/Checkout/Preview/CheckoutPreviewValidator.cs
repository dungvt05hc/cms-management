// <copyright file="CheckoutPreviewValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Checkout.Preview;

/// <summary>
/// Validator for checkout preview command.
/// </summary>
public class CheckoutPreviewValidator : AbstractValidator<CheckoutPreviewCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutPreviewValidator"/> class.
    /// </summary>
    public CheckoutPreviewValidator()
    {
        this.RuleFor(x => x.UserId).NotEmpty();
        this.RuleFor(x => x.AddressId).NotEmpty();
        this.RuleFor(x => x.ShippingMethodCode).NotEmpty().MaximumLength(50);
        this.RuleFor(x => x.ShippingCarrierCode).NotEmpty().MaximumLength(50);
        this.RuleFor(x => x.DiscountCode).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.DiscountCode));
        this.RuleFor(x => x.ShippingCode).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.ShippingCode));
    }
}

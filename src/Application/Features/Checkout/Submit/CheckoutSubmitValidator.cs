// <copyright file="CheckoutSubmitValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Checkout.Submit;

/// <summary>
/// Validator for checkout submit command.
/// </summary>
public class CheckoutSubmitValidator : AbstractValidator<CheckoutSubmitCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutSubmitValidator"/> class.
    /// </summary>
    public CheckoutSubmitValidator()
    {
        this.RuleFor(x => x.UserId).NotEmpty();
        this.RuleFor(x => x.AddressId).NotEmpty();
        this.RuleFor(x => x.ShippingMethodCode).NotEmpty().MaximumLength(50);
        this.RuleFor(x => x.ShippingCarrierCode).NotEmpty().MaximumLength(50);
        this.RuleFor(x => x.DiscountCode).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.DiscountCode));
        this.RuleFor(x => x.ShippingCode).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.ShippingCode));
        this.RuleFor(x => x.PaymentMethod).IsInEnum();
        this.RuleFor(x => x.Notes).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

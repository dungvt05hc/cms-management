// <copyright file="PayooCallbackValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Payments.PayooCallback;

/// <summary>
/// Validator for Payoo callback command.
/// </summary>
public class PayooCallbackValidator : AbstractValidator<PayooCallbackCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PayooCallbackValidator"/> class.
    /// </summary>
    public PayooCallbackValidator()
    {
        this.RuleFor(x => x.PaymentReference).NotEmpty().MaximumLength(256);
        this.RuleFor(x => x.UserId).NotEmpty();
        this.RuleFor(x => x.AddressId).NotEmpty();
        this.RuleFor(x => x.ShippingMethodCode).NotEmpty().MaximumLength(50);
        this.RuleFor(x => x.ShippingCarrierCode).NotEmpty().MaximumLength(50);
        this.RuleFor(x => x.DiscountCode).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.DiscountCode));
        this.RuleFor(x => x.ShippingCode).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.ShippingCode));
        this.RuleFor(x => x.Notes).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

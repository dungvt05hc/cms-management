// <copyright file="ApplyVoucherValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Checkout.ApplyVoucher;

/// <summary>
/// Validator for ApplyVoucherCommand.
/// </summary>
public class ApplyVoucherValidator : AbstractValidator<ApplyVoucherCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplyVoucherValidator"/> class.
    /// </summary>
    public ApplyVoucherValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        this.When(x => !string.IsNullOrWhiteSpace(x.DiscountCode), () =>
        {
            this.RuleFor(x => x.DiscountCode)
                .MaximumLength(50).WithMessage("Discount code cannot exceed 50 characters.");
        });

        this.When(x => !string.IsNullOrWhiteSpace(x.ShippingCode), () =>
        {
            this.RuleFor(x => x.ShippingCode)
                .MaximumLength(50).WithMessage("Shipping code cannot exceed 50 characters.");
        });
    }
}

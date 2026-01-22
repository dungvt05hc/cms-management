// <copyright file="AddCartItemValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Cart.AddCartItem;

/// <summary>
/// Validator for AddCartItemCommand.
/// </summary>
public class AddCartItemValidator : AbstractValidator<AddCartItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddCartItemValidator"/> class.
    /// </summary>
    public AddCartItemValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        this.RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        this.RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(999).WithMessage("Quantity cannot exceed 999.");
    }
}

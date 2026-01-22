// <copyright file="UpdateCartItemValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Cart.UpdateCartItem;

/// <summary>
/// Validator for UpdateCartItemCommand.
/// </summary>
public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCartItemValidator"/> class.
    /// </summary>
    public UpdateCartItemValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        this.RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is required.");

        this.RuleFor(x => x.Quantity)
            .GreaterThan(0).When(x => x.Quantity.HasValue)
            .WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(999).When(x => x.Quantity.HasValue)
            .WithMessage("Quantity cannot exceed 999.");
    }
}

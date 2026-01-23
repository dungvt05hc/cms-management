// <copyright file="ReorderValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Orders.Reorder;

/// <summary>
/// Validator for ReorderCommand.
/// </summary>
public class ReorderValidator : AbstractValidator<ReorderCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReorderValidator"/> class.
    /// </summary>
    public ReorderValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        this.RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");
    }
}

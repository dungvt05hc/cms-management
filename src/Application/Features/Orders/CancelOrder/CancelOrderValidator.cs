// <copyright file="CancelOrderValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Orders.CancelOrder;

/// <summary>
/// Validator for cancel order command.
/// </summary>
public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CancelOrderValidator"/> class.
    /// </summary>
    public CancelOrderValidator()
    {
        this.RuleFor(x => x.UserId).NotEmpty();
        this.RuleFor(x => x.OrderId).NotEmpty();
        this.RuleFor(x => x.ReasonCode).NotEmpty().MaximumLength(50);
        this.RuleFor(x => x.Note).MaximumLength(500);
    }
}

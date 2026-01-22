// <copyright file="GetShippingQuoteValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Shipping.GetShippingQuote;

/// <summary>
/// Validator for GetShippingQuoteQuery.
/// </summary>
public class GetShippingQuoteValidator : AbstractValidator<GetShippingQuoteQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetShippingQuoteValidator"/> class.
    /// </summary>
    public GetShippingQuoteValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        this.RuleFor(x => x.AddressId)
            .NotEmpty()
            .WithMessage("AddressId is required.");

        this.RuleFor(x => x.MethodCode)
            .NotEmpty()
            .WithMessage("MethodCode is required.")
            .MaximumLength(50)
            .WithMessage("MethodCode must not exceed 50 characters.");

        this.RuleFor(x => x.CarrierCode)
            .NotEmpty()
            .WithMessage("CarrierCode is required.")
            .MaximumLength(50)
            .WithMessage("CarrierCode must not exceed 50 characters.");

        this.RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0.");
    }
}

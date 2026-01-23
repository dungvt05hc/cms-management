// <copyright file="CreateInvoiceProfileValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.InvoiceProfiles.CreateInvoiceProfile;

/// <summary>
/// Validator for CreateInvoiceProfileCommand.
/// </summary>
public class CreateInvoiceProfileValidator : AbstractValidator<CreateInvoiceProfileCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInvoiceProfileValidator"/> class.
    /// </summary>
    public CreateInvoiceProfileValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        this.RuleFor(x => x.TaxCode)
            .NotEmpty()
            .WithMessage("Tax code is required.")
            .MaximumLength(50)
            .WithMessage("Tax code must not exceed 50 characters.");

        this.RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required.")
            .MaximumLength(512)
            .WithMessage("Company name must not exceed 512 characters.");

        this.RuleFor(x => x.CompanyAddress)
            .NotEmpty()
            .WithMessage("Company address is required.")
            .MaximumLength(512)
            .WithMessage("Company address must not exceed 512 characters.");

        this.RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters.");
    }
}

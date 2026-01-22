// <copyright file="UpdateAddressValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Addresses.UpdateAddress;

/// <summary>
/// Validator for UpdateAddressCommand.
/// </summary>
public class UpdateAddressValidator : AbstractValidator<UpdateAddressCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAddressValidator"/> class.
    /// </summary>
    public UpdateAddressValidator()
    {
        this.RuleFor(x => x.AddressId)
            .NotEmpty().WithMessage("Address ID is required.");

        this.RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        this.RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(256).WithMessage("Full name cannot exceed 256 characters.");

        this.RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.")
            .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters.");

        this.RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address line is required.")
            .MaximumLength(512).WithMessage("Address line cannot exceed 512 characters.");

        this.RuleFor(x => x.Ward)
            .NotEmpty().WithMessage("Ward is required.")
            .MaximumLength(256).WithMessage("Ward cannot exceed 256 characters.");

        this.RuleFor(x => x.District)
            .NotEmpty().WithMessage("District is required.")
            .MaximumLength(256).WithMessage("District cannot exceed 256 characters.");

        this.RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(256).WithMessage("City cannot exceed 256 characters.");
    }
}

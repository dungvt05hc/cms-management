// <copyright file="RegisterDeviceValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Devices.RegisterDevice;

/// <summary>
/// Validator for RegisterDeviceCommand.
/// </summary>
public class RegisterDeviceValidator : AbstractValidator<RegisterDeviceCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterDeviceValidator"/> class.
    /// </summary>
    public RegisterDeviceValidator()
    {
        this.RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        this.RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required.")
            .MaximumLength(512)
            .WithMessage("Token cannot exceed 512 characters.");
    }
}

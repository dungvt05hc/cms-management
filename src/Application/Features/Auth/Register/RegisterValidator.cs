// <copyright file="RegisterValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Auth.Register;

/// <summary>
/// Validator for RegisterCommand.
/// </summary>
public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterValidator"/> class.
    /// </summary>
    public RegisterValidator()
    {
        this.RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be in a valid format.");

        this.RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email must be in a valid format.");

        this.RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
            .MaximumLength(256).WithMessage("Full name cannot exceed 256 characters.");

        this.RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(128).WithMessage("Password cannot exceed 128 characters.");
    }
}

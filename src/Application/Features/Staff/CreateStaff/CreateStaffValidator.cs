// <copyright file="CreateStaffValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Staff.CreateStaff;

/// <summary>
/// Validator for CreateStaffCommand.
/// </summary>
public class CreateStaffValidator : AbstractValidator<CreateStaffCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateStaffValidator"/> class.
    /// </summary>
    public CreateStaffValidator()
    {
        this.RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        this.RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone number must be in E.164 format.");

        this.RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MaximumLength(256)
            .WithMessage("Full name cannot exceed 256 characters.");

        this.RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d")
            .WithMessage("Password must contain at least one digit.");

        this.RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .Must(r => r == "Admin" || r == "Staff")
            .WithMessage("Role must be either 'Admin' or 'Staff'.");
    }
}

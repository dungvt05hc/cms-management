// <copyright file="ResetPasswordValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Auth.ResetPassword;

/// <summary>
/// Validator for ResetPasswordCommand.
/// </summary>
public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordValidator"/> class.
    /// </summary>
    public ResetPasswordValidator()
    {
        this.RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");

        this.RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
    }
}

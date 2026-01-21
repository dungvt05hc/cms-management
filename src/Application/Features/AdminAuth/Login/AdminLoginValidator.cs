// <copyright file="AdminLoginValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.AdminAuth.Login;

/// <summary>
/// Validator for AdminLoginCommand.
/// </summary>
public class AdminLoginValidator : AbstractValidator<AdminLoginCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminLoginValidator"/> class.
    /// </summary>
    public AdminLoginValidator()
    {
        this.RuleFor(x => x.EmailOrPhone)
            .NotEmpty()
            .WithMessage("Email or phone is required.");

        this.RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}

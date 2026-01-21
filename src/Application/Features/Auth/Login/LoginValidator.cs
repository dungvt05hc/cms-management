// <copyright file="LoginValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Auth.Login;

/// <summary>
/// Validator for LoginQuery.
/// </summary>
public class LoginValidator : AbstractValidator<LoginQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginValidator"/> class.
    /// </summary>
    public LoginValidator()
    {
        this.RuleFor(x => x.PhoneOrEmail)
            .NotEmpty().WithMessage("Phone or email is required.");

        this.RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

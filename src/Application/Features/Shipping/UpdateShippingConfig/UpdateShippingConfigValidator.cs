// <copyright file="UpdateShippingConfigValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Shipping.UpdateShippingConfig;

/// <summary>
/// Validator for UpdateShippingConfigCommand.
/// </summary>
public class UpdateShippingConfigValidator : AbstractValidator<UpdateShippingConfigCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateShippingConfigValidator"/> class.
    /// </summary>
    public UpdateShippingConfigValidator()
    {
        this.RuleFor(x => x.Configs)
            .NotNull()
            .WithMessage("Configs cannot be null.");

        this.RuleForEach(x => x.Configs)
            .Must(kvp => !string.IsNullOrWhiteSpace(kvp.Key))
            .WithMessage("Config key cannot be empty.")
            .Must(kvp => kvp.Key.Length <= 100)
            .WithMessage("Config key must not exceed 100 characters.")
            .Must(kvp => kvp.Value.Length <= 1000)
            .WithMessage("Config value must not exceed 1000 characters.");
    }
}

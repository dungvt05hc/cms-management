// <copyright file="UpdateCategoryValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Categories.UpdateCategory;

/// <summary>
/// Validator for the UpdateCategoryCommand.
/// </summary>
public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCategoryValidator"/> class.
    /// </summary>
    public UpdateCategoryValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.");

        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(256).WithMessage("Category name must not exceed 256 characters.");
    }
}

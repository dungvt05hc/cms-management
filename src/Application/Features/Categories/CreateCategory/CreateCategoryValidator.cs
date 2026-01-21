// <copyright file="CreateCategoryValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Categories.CreateCategory;

/// <summary>
/// Validator for the CreateCategoryCommand.
/// </summary>
public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCategoryValidator"/> class.
    /// </summary>
    public CreateCategoryValidator()
    {
        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(256).WithMessage("Category name must not exceed 256 characters.");
    }
}

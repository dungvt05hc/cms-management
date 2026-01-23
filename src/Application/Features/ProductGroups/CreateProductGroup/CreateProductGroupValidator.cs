// <copyright file="CreateProductGroupValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.ProductGroups.CreateProductGroup;

/// <summary>
/// Validator for the CreateProductGroupCommand.
/// </summary>
public class CreateProductGroupValidator : AbstractValidator<CreateProductGroupCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProductGroupValidator"/> class.
    /// </summary>
    public CreateProductGroupValidator()
    {
        this.RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product group name is required.")
            .MaximumLength(256).WithMessage("Product group name must not exceed 256 characters.");

        this.RuleFor(x => x.Attributes)
            .NotNull().WithMessage("Attributes list is required.");

        this.RuleForEach(x => x.Attributes).ChildRules(attr =>
        {
            attr.RuleFor(a => a.Name)
                .NotEmpty().WithMessage("Attribute name is required.")
                .MaximumLength(256).WithMessage("Attribute name must not exceed 256 characters.");

            attr.RuleFor(a => a.Key)
                .NotEmpty().WithMessage("Attribute key is required.")
                .MaximumLength(100).WithMessage("Attribute key must not exceed 100 characters.")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Attribute key must contain only alphanumeric characters, hyphens, and underscores.");

            attr.RuleFor(a => a.Type)
                .NotEmpty().WithMessage("Attribute type is required.")
                .MaximumLength(50).WithMessage("Attribute type must not exceed 50 characters.");
        });

        this.RuleFor(x => x.Attributes)
            .Must(HaveUniqueKeys).WithMessage("Attribute keys must be unique within the group.");
    }

    private static bool HaveUniqueKeys(List<ProductGroupAttributeDto> attributes)
    {
        var keys = attributes.Select(a => a.Key).ToList();
        return keys.Count == keys.Distinct().Count();
    }
}

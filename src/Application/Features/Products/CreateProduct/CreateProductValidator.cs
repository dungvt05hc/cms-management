// <copyright file="CreateProductValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Products.CreateProduct;

/// <summary>
/// Validator for the CreateProductCommand.
/// </summary>
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProductValidator"/> class.
    /// </summary>
    public CreateProductValidator()
    {
        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(512).WithMessage("Product name cannot exceed 512 characters.");

        this.RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Product slug is required.")
            .MaximumLength(512).WithMessage("Product slug cannot exceed 512 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must be lowercase, alphanumeric with hyphens only.");

        this.RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters.")
            .When(x => x.Description != null);

        this.RuleFor(x => x.Images)
            .MaximumLength(2000).WithMessage("Images cannot exceed 2000 characters.")
            .When(x => x.Images != null);

        this.RuleFor(x => x.Videos)
            .MaximumLength(2000).WithMessage("Videos cannot exceed 2000 characters.")
            .When(x => x.Videos != null);

        this.RuleFor(x => x.Specifications)
            .MaximumLength(4000).WithMessage("Specifications cannot exceed 4000 characters.")
            .When(x => x.Specifications != null);

        this.RuleFor(x => x.Variants)
            .NotEmpty().WithMessage("At least one product variant is required.");

        this.RuleForEach(x => x.Variants).ChildRules(variant =>
        {
            variant.RuleFor(v => v.Sku)
                .NotEmpty().WithMessage("Variant SKU is required.")
                .MaximumLength(100).WithMessage("SKU cannot exceed 100 characters.");

            variant.RuleFor(v => v.VariantName)
                .MaximumLength(256).WithMessage("Variant name cannot exceed 256 characters.")
                .When(v => v.VariantName != null);

            variant.RuleFor(v => v.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

            variant.RuleFor(v => v.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0.");
        });

        this.RuleFor(x => x.Variants)
            .Must(variants => variants.Select(v => v.Sku).Distinct().Count() == variants.Count)
            .WithMessage("Duplicate SKUs within variants are not allowed.");
    }
}

// <copyright file="ProductGroupDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.ProductGroups.CreateProductGroup;

/// <summary>
/// DTO for product group.
/// </summary>
/// <param name="Id">The product group identifier.</param>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="Name">The product group name.</param>
/// <param name="Attributes">The attribute definitions.</param>
/// <param name="CreatedAt">The creation date and time.</param>
/// <param name="UpdatedAt">The last update date and time.</param>
public record ProductGroupDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    List<ProductGroupAttributeDto> Attributes,
    DateTime CreatedAt,
    DateTime UpdatedAt);

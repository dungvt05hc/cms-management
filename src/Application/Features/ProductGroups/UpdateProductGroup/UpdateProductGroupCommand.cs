// <copyright file="UpdateProductGroupCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.ProductGroups.CreateProductGroup;

namespace Application.Features.ProductGroups.UpdateProductGroup;

/// <summary>
/// Command to update a product group.
/// </summary>
/// <param name="Id">The product group identifier.</param>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="Name">The product group name.</param>
/// <param name="Attributes">The attribute definitions.</param>
public record UpdateProductGroupCommand(
    Guid Id,
    Guid CategoryId,
    string Name,
    List<ProductGroupAttributeDto> Attributes);

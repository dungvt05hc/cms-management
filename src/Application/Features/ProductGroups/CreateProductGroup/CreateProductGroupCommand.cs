// <copyright file="CreateProductGroupCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.ProductGroups.CreateProductGroup;

/// <summary>
/// Command to create a new product group.
/// </summary>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="Name">The product group name.</param>
/// <param name="Attributes">The attribute definitions.</param>
public record CreateProductGroupCommand(
    Guid CategoryId,
    string Name,
    List<ProductGroupAttributeDto> Attributes);

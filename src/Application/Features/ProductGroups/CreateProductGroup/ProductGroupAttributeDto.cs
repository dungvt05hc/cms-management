// <copyright file="ProductGroupAttributeDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.ProductGroups.CreateProductGroup;

/// <summary>
/// DTO for product group attribute definition.
/// </summary>
/// <param name="Name">The attribute name.</param>
/// <param name="Key">The attribute key.</param>
/// <param name="Type">The attribute type.</param>
public record ProductGroupAttributeDto(string Name, string Key, string Type);

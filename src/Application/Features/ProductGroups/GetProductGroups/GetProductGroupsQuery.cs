// <copyright file="GetProductGroupsQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.ProductGroups.GetProductGroups;

/// <summary>
/// Query to get product groups filtered by category.
/// </summary>
/// <param name="CategoryId">Optional category filter.</param>
public record GetProductGroupsQuery(Guid? CategoryId);

// <copyright file="UpdateCategoryRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request model for updating a category.
/// </summary>
/// <param name="Name">The category name.</param>
/// <param name="ParentId">The optional parent category identifier.</param>
public record UpdateCategoryRequest(string Name, Guid? ParentId);

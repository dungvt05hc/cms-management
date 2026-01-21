// <copyright file="UpdateCategoryCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Categories.UpdateCategory;

/// <summary>
/// Command to update a category.
/// </summary>
/// <param name="Id">The category identifier.</param>
/// <param name="Name">The category name.</param>
/// <param name="ParentId">The optional parent category identifier.</param>
public record UpdateCategoryCommand(Guid Id, string Name, Guid? ParentId);

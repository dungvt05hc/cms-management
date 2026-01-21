// <copyright file="CreateCategoryCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Categories.CreateCategory;

/// <summary>
/// Command to create a new category.
/// </summary>
/// <param name="Name">The category name.</param>
/// <param name="ParentId">The optional parent category identifier.</param>
public record CreateCategoryCommand(string Name, Guid? ParentId);

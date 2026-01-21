// <copyright file="DeleteCategoryCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Categories.DeleteCategory;

/// <summary>
/// Command to delete a category.
/// </summary>
/// <param name="Id">The category identifier.</param>
public record DeleteCategoryCommand(Guid Id);

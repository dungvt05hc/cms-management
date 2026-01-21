// <copyright file="CategoryDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Categories.CreateCategory;

/// <summary>
/// Data transfer object for a category.
/// </summary>
/// <param name="Id">The category identifier.</param>
/// <param name="Name">The category name.</param>
/// <param name="ParentId">The parent category identifier.</param>
/// <param name="CreatedAt">The creation date and time.</param>
/// <param name="UpdatedAt">The last update date and time.</param>
public record CategoryDto(Guid Id, string Name, Guid? ParentId, DateTime CreatedAt, DateTime UpdatedAt);

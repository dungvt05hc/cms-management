// <copyright file="CategoryTreeNode.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Categories.GetCategoryTree;

/// <summary>
/// Represents a node in the category tree.
/// </summary>
/// <param name="Id">The category identifier.</param>
/// <param name="Name">The category name.</param>
/// <param name="ParentId">The parent category identifier.</param>
/// <param name="Children">The child categories.</param>
public record CategoryTreeNode(Guid Id, string Name, Guid? ParentId, List<CategoryTreeNode> Children);

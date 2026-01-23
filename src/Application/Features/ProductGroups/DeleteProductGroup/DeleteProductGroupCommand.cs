// <copyright file="DeleteProductGroupCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.ProductGroups.DeleteProductGroup;

/// <summary>
/// Command to delete a product group.
/// </summary>
/// <param name="Id">The product group identifier.</param>
public record DeleteProductGroupCommand(Guid Id);

// <copyright file="GetCategoryTreeHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.GetCategoryTree;

/// <summary>
/// Handler for the GetCategoryTreeQuery.
/// </summary>
public class GetCategoryTreeHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetCategoryTreeHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCategoryTreeHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetCategoryTreeHandler(
        IAppDbContext dbContext,
        ILogger<GetCategoryTreeHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get category tree query.
    /// </summary>
    /// <param name="query">The get category tree query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category tree.</returns>
    public async Task<List<CategoryTreeNode>> Handle(GetCategoryTreeQuery query, CancellationToken cancellationToken)
    {
        var categories = await this.dbContext.Categories
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        var categoryDict = categories.ToDictionary(c => c.Id);
        var roots = new List<CategoryTreeNode>();

        foreach (var category in categories)
        {
            var node = new CategoryTreeNode(
                category.Id,
                category.Name,
                category.ParentId,
                new List<CategoryTreeNode>());

            if (category.ParentId == null)
            {
                roots.Add(node);
            }
        }

        var nodeDict = categories.ToDictionary(
            c => c.Id,
            c => new CategoryTreeNode(c.Id, c.Name, c.ParentId, new List<CategoryTreeNode>()));

        foreach (var category in categories)
        {
            var node = nodeDict[category.Id];
            if (category.ParentId.HasValue && nodeDict.TryGetValue(category.ParentId.Value, out var parent))
            {
                parent.Children.Add(node);
            }
        }

        var result = nodeDict.Values.Where(n => !n.ParentId.HasValue).OrderBy(n => n.Name).ToList();

        this.logger.LogInformation("Category tree retrieved with {Count} root categories", result.Count);

        return result;
    }
}

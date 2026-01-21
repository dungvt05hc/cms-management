// <copyright file="IAppDbContext.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions;

/// <summary>
/// Interface for application database context.
/// </summary>
public interface IAppDbContext
{
    /// <summary>
    /// Gets the Users DbSet.
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

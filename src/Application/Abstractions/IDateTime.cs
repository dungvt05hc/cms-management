// <copyright file="IDateTime.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Abstractions;

/// <summary>
/// Abstraction for retrieving the current date and time.
/// This allows for deterministic testing by replacing with a test double.
/// </summary>
public interface IDateTime
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }
}

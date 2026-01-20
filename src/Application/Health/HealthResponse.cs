// <copyright file="HealthResponse.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Health;

/// <summary>
/// Response model for the health check endpoint.
/// </summary>
public sealed record HealthResponse
{
    /// <summary>
    /// Gets the status of the application.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Gets the current UTC timestamp.
    /// </summary>
    public required DateTime UtcNow { get; init; }
}

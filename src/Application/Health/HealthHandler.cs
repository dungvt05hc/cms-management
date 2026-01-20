// <copyright file="HealthHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;

namespace Application.Health;

/// <summary>
/// Handler for health check operations.
/// Returns the application health status and current timestamp.
/// </summary>
public sealed class HealthHandler
{
    private readonly IDateTime dateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthHandler"/> class.
    /// </summary>
    /// <param name="dateTime">The date/time provider.</param>
    public HealthHandler(IDateTime dateTime)
    {
        this.dateTime = dateTime;
    }

    /// <summary>
    /// Gets the health status of the application.
    /// </summary>
    /// <returns>A health response indicating the application is operational.</returns>
    public HealthResponse GetHealth()
    {
        return new HealthResponse
        {
            Status = "ok",
            UtcNow = this.dateTime.UtcNow,
        };
    }
}

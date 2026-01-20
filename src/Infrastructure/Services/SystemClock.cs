// <copyright file="SystemClock.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;

namespace Infrastructure.Services;

/// <summary>
/// System implementation of IDateTime using the system clock.
/// </summary>
public sealed class SystemClock : IDateTime
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}

// <copyright file="HealthHandlerTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Application.Health;

namespace Application.UnitTests.Health;

/// <summary>
/// Unit tests for HealthHandler.
/// </summary>
public sealed class HealthHandlerTests
{
    /// <summary>
    /// Tests that GetHealth returns a response with "ok" status.
    /// </summary>
    [Fact]
    public void GetHealth_ReturnsOkStatus()
    {
        // Arrange
        var expectedUtcNow = new DateTime(2026, 1, 20, 12, 0, 0, DateTimeKind.Utc);
        var dateTime = new FakeDateTimeProvider(expectedUtcNow);
        var handler = new HealthHandler(dateTime);

        // Act
        var result = handler.GetHealth();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ok", result.Status);
        Assert.Equal(expectedUtcNow, result.UtcNow);
    }

    /// <summary>
    /// Tests that GetHealth uses the provided IDateTime implementation.
    /// </summary>
    [Fact]
    public void GetHealth_UsesProvidedDateTime()
    {
        // Arrange
        var customUtcNow = new DateTime(2025, 6, 15, 10, 30, 45, DateTimeKind.Utc);
        var dateTime = new FakeDateTimeProvider(customUtcNow);
        var handler = new HealthHandler(dateTime);

        // Act
        var result = handler.GetHealth();

        // Assert
        Assert.Equal(customUtcNow, result.UtcNow);
    }

    /// <summary>
    /// Fake implementation of IDateTime for testing.
    /// </summary>
    private sealed class FakeDateTimeProvider : IDateTime
    {
        private readonly DateTime utcNow;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDateTimeProvider"/> class.
        /// </summary>
        /// <param name="utcNow">The fixed UTC time to return.</param>
        public FakeDateTimeProvider(DateTime utcNow)
        {
            this.utcNow = utcNow;
        }

        /// <inheritdoc />
        public DateTime UtcNow => this.utcNow;
    }
}

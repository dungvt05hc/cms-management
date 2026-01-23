// <copyright file="RunIssuanceHandlerTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Application.Features.Invoices.RunIssuance;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Invoices;

/// <summary>
/// Unit tests for RunIssuanceHandler.
/// </summary>
public sealed class RunIssuanceHandlerTests
{
    /// <summary>
    /// Tests that orders delivered more than 10 days ago are selected for issuance.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Handle_SelectsOrdersDeliveredMoreThan10DaysAgo()
    {
        // Arrange
        var utcNow = new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc);
        var mockInvoiceIssuer = new Mock<IInvoiceIssuer>();
        var mockLogger = new Mock<ILogger<RunIssuanceHandler>>();

        var options = new DbContextOptionsBuilder<Infrastructure.Persistence.AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new Infrastructure.Persistence.AppDbContext(options);

        var userId = Guid.NewGuid();
        var eligibleOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Subtotal = 100m,
            Total = 100m,
            PaymentMethod = PaymentMethod.COD,
            Status = OrderStatus.Delivered,
            ShippingFullName = "Test",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "HCMC",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            InvoiceRequested = true,
            InvoiceTaxCode = "0123456789",
            InvoiceCompanyName = "Test Company",
            InvoiceCompanyAddress = "123 Business St",
            InvoiceEmail = "invoice@test.com",
            DeliveredAt = utcNow.AddDays(-11),
            CreatedAt = utcNow.AddDays(-15),
            UpdatedAt = utcNow,
        };

        dbContext.Orders.Add(eligibleOrder);
        await dbContext.SaveChangesAsync();

        var handler = new RunIssuanceHandler(dbContext, mockInvoiceIssuer.Object, mockLogger.Object);
        var command = new RunIssuanceCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result.ProcessedCount);
        mockInvoiceIssuer.Verify(
            x => x.IssueInvoiceAsync(
                eligibleOrder.Id,
                "0123456789",
                "Test Company",
                "123 Business St",
                "invoice@test.com",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Tests that orders delivered less than 10 days ago are not selected.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Handle_DoesNotSelectOrdersDeliveredLessThan10DaysAgo()
    {
        // Arrange
        var utcNow = new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc);
        var mockInvoiceIssuer = new Mock<IInvoiceIssuer>();
        var mockLogger = new Mock<ILogger<RunIssuanceHandler>>();

        var options = new DbContextOptionsBuilder<Infrastructure.Persistence.AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new Infrastructure.Persistence.AppDbContext(options);

        var userId = Guid.NewGuid();
        var notEligibleOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Subtotal = 100m,
            Total = 100m,
            PaymentMethod = PaymentMethod.COD,
            Status = OrderStatus.Delivered,
            ShippingFullName = "Test",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "HCMC",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            InvoiceRequested = true,
            InvoiceTaxCode = "0123456789",
            InvoiceCompanyName = "Test Company",
            InvoiceCompanyAddress = "123 Business St",
            InvoiceEmail = "invoice@test.com",
            DeliveredAt = utcNow.AddDays(-5),
            CreatedAt = utcNow.AddDays(-10),
            UpdatedAt = utcNow,
        };

        dbContext.Orders.Add(notEligibleOrder);
        await dbContext.SaveChangesAsync();

        var handler = new RunIssuanceHandler(dbContext, mockInvoiceIssuer.Object, mockLogger.Object);
        var command = new RunIssuanceCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.ProcessedCount);
        mockInvoiceIssuer.Verify(
            x => x.IssueInvoiceAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Tests that orders without invoice request are not selected.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Handle_DoesNotSelectOrdersWithoutInvoiceRequest()
    {
        // Arrange
        var utcNow = new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc);
        var mockInvoiceIssuer = new Mock<IInvoiceIssuer>();
        var mockLogger = new Mock<ILogger<RunIssuanceHandler>>();

        var options = new DbContextOptionsBuilder<Infrastructure.Persistence.AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new Infrastructure.Persistence.AppDbContext(options);

        var userId = Guid.NewGuid();
        var orderWithoutInvoiceRequest = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Subtotal = 100m,
            Total = 100m,
            PaymentMethod = PaymentMethod.COD,
            Status = OrderStatus.Delivered,
            ShippingFullName = "Test",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "HCMC",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            InvoiceRequested = false,
            DeliveredAt = utcNow.AddDays(-11),
            CreatedAt = utcNow.AddDays(-15),
            UpdatedAt = utcNow,
        };

        dbContext.Orders.Add(orderWithoutInvoiceRequest);
        await dbContext.SaveChangesAsync();

        var handler = new RunIssuanceHandler(dbContext, mockInvoiceIssuer.Object, mockLogger.Object);
        var command = new RunIssuanceCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.ProcessedCount);
        mockInvoiceIssuer.Verify(
            x => x.IssueInvoiceAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Tests that orders with non-Delivered status are not selected.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Handle_DoesNotSelectNonDeliveredOrders()
    {
        // Arrange
        var utcNow = new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc);
        var mockInvoiceIssuer = new Mock<IInvoiceIssuer>();
        var mockLogger = new Mock<ILogger<RunIssuanceHandler>>();

        var options = new DbContextOptionsBuilder<Infrastructure.Persistence.AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new Infrastructure.Persistence.AppDbContext(options);

        var userId = Guid.NewGuid();
        var processingOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Subtotal = 100m,
            Total = 100m,
            PaymentMethod = PaymentMethod.COD,
            Status = OrderStatus.Processing,
            ShippingFullName = "Test",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "HCMC",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            InvoiceRequested = true,
            InvoiceTaxCode = "0123456789",
            InvoiceCompanyName = "Test Company",
            InvoiceCompanyAddress = "123 Business St",
            InvoiceEmail = "invoice@test.com",
            DeliveredAt = utcNow.AddDays(-11),
            CreatedAt = utcNow.AddDays(-15),
            UpdatedAt = utcNow,
        };

        dbContext.Orders.Add(processingOrder);
        await dbContext.SaveChangesAsync();

        var handler = new RunIssuanceHandler(dbContext, mockInvoiceIssuer.Object, mockLogger.Object);
        var command = new RunIssuanceCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.ProcessedCount);
        mockInvoiceIssuer.Verify(
            x => x.IssueInvoiceAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Tests that already issued orders are not selected again.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Handle_DoesNotSelectAlreadyIssuedOrders()
    {
        // Arrange
        var utcNow = new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc);
        var mockInvoiceIssuer = new Mock<IInvoiceIssuer>();
        var mockLogger = new Mock<ILogger<RunIssuanceHandler>>();

        var options = new DbContextOptionsBuilder<Infrastructure.Persistence.AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        await using var dbContext = new Infrastructure.Persistence.AppDbContext(options);

        var userId = Guid.NewGuid();
        var alreadyIssuedOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Subtotal = 100m,
            Total = 100m,
            PaymentMethod = PaymentMethod.COD,
            Status = OrderStatus.Delivered,
            ShippingFullName = "Test",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "HCMC",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            InvoiceRequested = true,
            InvoiceTaxCode = "0123456789",
            InvoiceCompanyName = "Test Company",
            InvoiceCompanyAddress = "123 Business St",
            InvoiceEmail = "invoice@test.com",
            InvoiceIssuedAt = utcNow.AddDays(-2),
            DeliveredAt = utcNow.AddDays(-11),
            CreatedAt = utcNow.AddDays(-15),
            UpdatedAt = utcNow,
        };

        dbContext.Orders.Add(alreadyIssuedOrder);
        await dbContext.SaveChangesAsync();

        var handler = new RunIssuanceHandler(dbContext, mockInvoiceIssuer.Object, mockLogger.Object);
        var command = new RunIssuanceCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.ProcessedCount);
        mockInvoiceIssuer.Verify(
            x => x.IssueInvoiceAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}

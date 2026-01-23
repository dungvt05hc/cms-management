// <copyright file="CreateInvoiceProfileHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Features.InvoiceProfiles.CreateInvoiceProfile;

/// <summary>
/// Handler for CreateInvoiceProfileCommand.
/// </summary>
public class CreateInvoiceProfileHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<CreateInvoiceProfileHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInvoiceProfileHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public CreateInvoiceProfileHandler(
        IAppDbContext dbContext,
        ILogger<CreateInvoiceProfileHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles creating a new invoice profile.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created invoice profile.</returns>
    public async Task<InvoiceProfileDto> Handle(
        CreateInvoiceProfileCommand command,
        CancellationToken cancellationToken)
    {
        var profile = new InvoiceProfile
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            TaxCode = command.TaxCode,
            CompanyName = command.CompanyName,
            CompanyAddress = command.CompanyAddress,
            Email = command.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        this.dbContext.InvoiceProfiles.Add(profile);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation(
            "Created invoice profile {ProfileId} for user {UserId}",
            profile.Id,
            command.UserId);

        return new InvoiceProfileDto(
            profile.Id,
            profile.UserId,
            profile.TaxCode,
            profile.CompanyName,
            profile.CompanyAddress,
            profile.Email,
            profile.CreatedAt,
            profile.UpdatedAt);
    }
}

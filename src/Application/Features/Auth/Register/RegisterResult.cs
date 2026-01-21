// <copyright file="RegisterResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Auth.Register;

/// <summary>
/// Result of a registration operation.
/// </summary>
/// <param name="RegistrationId">The registration identifier (user ID).</param>
public record RegisterResult(Guid RegistrationId);

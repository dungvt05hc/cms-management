// <copyright file="UpdateShippingConfigCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Shipping.UpdateShippingConfig;

/// <summary>
/// Command to update shipping configuration.
/// </summary>
/// <param name="Configs">Dictionary of configuration key-value pairs.</param>
public record UpdateShippingConfigCommand(Dictionary<string, string> Configs);

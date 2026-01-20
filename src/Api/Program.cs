// <copyright file="Program.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application;
using Application.Health;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Health endpoint
app.MapGet("/health", (HealthHandler handler) =>
{
    var response = handler.GetHealth();
    return Results.Ok(response);
})
.WithName("GetHealth")
.Produces<HealthResponse>(StatusCodes.Status200OK);

app.Run();

/// <summary>
/// Program class for WebApplicationFactory.
/// </summary>
public partial class Program
{
}

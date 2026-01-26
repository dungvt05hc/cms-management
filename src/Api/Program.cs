// <copyright file="Program.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Text;

using Application;
using Application.Health;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("SuperAdmin", "Admin"));

    options.AddPolicy("StaffOrAdmin", policy =>
        policy.RequireRole("SuperAdmin", "Admin", "Staff"));
});

// Add CORS policy for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CMS Management API",
        Version = "v1",
        Description = "E-commerce Content Management System API",
    });

    // Configure JWT authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        },
    });
});

var app = builder.Build();

// Bootstrap data on startup
using (var scope = app.Services.CreateScope())
{
    var bootstrapService = scope.ServiceProvider.GetRequiredService<Infrastructure.Services.SuperAdminBootstrapService>();
    await bootstrapService.BootstrapAsync();

    var shippingBootstrapService = scope.ServiceProvider.GetRequiredService<Infrastructure.Services.ShippingBootstrapService>();
    await shippingBootstrapService.BootstrapAsync();

    // Seed sample e-commerce data
    var sampleDataSeeder = scope.ServiceProvider.GetRequiredService<Infrastructure.Services.SampleDataSeeder>();
    await sampleDataSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS Management API v1");
    });
}

// Enable CORS
app.UseCors("AllowFrontend");

// Enable static files for serving uploaded images
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// Health endpoint
app.MapGet("/health", (HealthHandler handler) =>
{
    var response = handler.GetHealth();
    return Results.Ok(response);
})
.WithName("GetHealth")
.Produces<HealthResponse>(StatusCodes.Status200OK);

app.MapControllers();

app.Run();

/// <summary>
/// Program class for WebApplicationFactory.
/// </summary>
public partial class Program
{
}

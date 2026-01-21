// <copyright file="AppDbContext.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// Application database context.
/// </summary>
public class AppDbContext : DbContext, IAppDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Users DbSet.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Configures the database schema.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Phone).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });
    }
}

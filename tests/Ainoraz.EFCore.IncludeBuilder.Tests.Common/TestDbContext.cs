﻿using Ainoraz.EFCore.IncludeBuilder.Tests.Common.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Ainoraz.EFCore.IncludeBuilder.Tests.Common;

public class TestDbContext() : DbContext(BuildOptions())
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> Posts => Set<Post>();

    private static DbContextOptions<TestDbContext> BuildOptions()
    {
        DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(CreateInMemoryDatabase())
            .Options;

        return options;
    }

    private static SqliteConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        return connection;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder
                .HasMany(u => u.Posts)
                .WithOne(p => p.Author);

            builder
                .HasOne(u => u.OwnedBlog)
                .WithOne(b => b.Author);

            builder
                .HasMany(u => u.FollowingBlogs)
                .WithMany(b => b.Followers);

            builder
                .HasMany(u => u.ReadHistory)
                .WithMany(p => p.Readers);
        });

        modelBuilder.Entity<Blog>(builder =>
        {
            builder
                .HasMany(b => b.Posts)
                .WithOne(p => p.Blog);
        });
    }
}

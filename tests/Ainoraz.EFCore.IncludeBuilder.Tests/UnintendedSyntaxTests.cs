﻿using Ainoraz.EFCore.IncludeBuilder.Extensions;
using Ainoraz.EFCore.IncludeBuilder.Tests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Ainoraz.EFCore.IncludeBuilder.Tests;

public sealed class UnintendedSyntaxTests : IDisposable
{
    private readonly TestDbContext _testDbContext = new();

    public void Dispose() => _testDbContext.Dispose();

    [Fact]
    public void UseBuilderTwice_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog)
            .Build()
            .UseIncludeBuilder()
            .Include(u => u.Posts)
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
            .Include(u => u.Posts)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DuplicateMultiLevelIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts)
                .Include(b => b.Posts)
            )
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Followers)
                .Include(b => b.Posts)
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DuplicateFilteredMultiLevelIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                .Include(b => b.Posts)
            )
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Followers)
                .Include(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void UseTwoBuildersWithFilterAndWithoutFilter_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                .Include(b => b.Posts)
            )
            .Build()
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Followers)
                .Include(b => b.Posts)
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void NavigationFixup_WarningThrown()
    {
        Action action = () => _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Author)
            )
            .Build()
            .ToQueryString();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*'Microsoft.EntityFrameworkCore.Query.NavigationBaseIncludeIgnored'*");
    }
}

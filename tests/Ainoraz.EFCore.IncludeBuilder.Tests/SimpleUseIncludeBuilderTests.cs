using Ainoraz.EFCore.IncludeBuilder.Extensions;
using Ainoraz.EFCore.IncludeBuilder.Tests.Common;
using Ainoraz.EFCore.IncludeBuilder.Tests.Common.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Ainoraz.EFCore.IncludeBuilder.Tests.Extensions;
using Xunit;

namespace Ainoraz.EFCore.IncludeBuilder.Tests;

public sealed class SimpleUseIncludeBuilderTests : IDisposable
{
    private readonly TestDbContext _testDbContext = new();

    public void Dispose() => _testDbContext.Dispose();

    [Fact]
    public void NoIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void NotEntityQueryProvider_ShouldNotThrow()
    {
        Func<IEnumerable<User>> getUsers = () => new List<User>()
            .AsQueryable()
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Followers)
            )
            .Build()
            .ToList();

        getUsers.Should().NotThrow();
        getUsers().Should().BeEmpty();
    }

    [Fact]
    public void SingleRootInclude_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog)
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DifferentRootIncludes_ShouldNotMatch()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog)
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void TwoRootIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog)
            .Include(u => u.FollowingBlogs)
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
            .Include(u => u.FollowingBlogs)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void TwoMixedIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog)
            .Build()
            .Include(u => u.FollowingBlogs)
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
            .Include(u => u.FollowingBlogs)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DifferentMixedIncludes_ShouldNotMatch()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Build()
            .Include(u => u.FollowingBlogs)
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
            .Include(u => u.FollowingBlogs)
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void SingleFirstLevelIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts)
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void MultipleFirstLevelIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts)
                .Include(b => b.Followers)
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DifferentFirstLevelIncludes_ShouldNotMatch()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts)
                .Include(b => b.Followers)
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void MultiLevelIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts, builder => builder
                    .Include(p => p.Readers, builder => builder
                        .Include(r => r.ReadHistory)
                        .Include(r => r.Posts)
                    )
                    .Include(p => p.Author)
                )
                .Include(b => b.Followers, builder => builder
                    .Include(f => f.OwnedBlog)
                )
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.ReadHistory)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
                    .ThenInclude(p => p.Author)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
                    .ThenInclude(f => f.OwnedBlog)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DifferentMultiLevelIncludes_ShouldNotMatch()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts, builder => builder
                    .Include(p => p.Readers, builder => builder
                        .Include(r => r.Posts)
                    )
                    .Include(p => p.Author)
                )
                .Include(b => b.Followers, builder => builder
                    .Include(f => f.OwnedBlog)
                )
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.ReadHistory)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
                    .ThenInclude(p => p.Author)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
                    .ThenInclude(f => f.OwnedBlog)
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void RootLevelExtensionIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Blogs
            .UseIncludeBuilder()
            .IncludeBlogChildren()
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Blogs
            .Include(u => u.Posts)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void MultiLevelExtensionIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .IncludeBlogChildren()
                .Include(b => b.Followers, builder => builder
                    .Include(f => f.FollowingBlogs, builder => builder
                        .IncludeBlogChildren()
                    )
                )
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
                    .ThenInclude(f => f.FollowingBlogs)
                        .ThenInclude(b => b.Posts)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DifferentMultiLevelExtensionIncludes_ShouldNotMatch()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .IncludeBlogChildren()
                .Include(b => b.Followers, builder => builder
                    .Include(f => f.OwnedBlog)
                )
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
                    .ThenInclude(f => f.OwnedBlog)
                        .ThenInclude(b => b.Posts)
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void IncludeChain_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog.Posts, builder => builder
                .Include(p => p.Readers)
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog.Posts)
                .ThenInclude(p => p.Readers)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DifferentIncludeChain_ShouldNotMatch()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog.Posts, builder => builder
                .Include(p => p.Readers)
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog.Posts)
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void CollectionIncludeChain_ShouldMatch()
    {
        var actualQuery = _testDbContext.Blogs
            .UseIncludeBuilder()
            .Include(b => b.Posts, builder => builder
                .Include(p => p.Readers, builder => builder
                    .Include(r => r.Posts)
                )
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Blogs
            .Include(b => b.Posts)
                .ThenInclude(p => p.Readers)
                    .ThenInclude(r => r.Posts)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void CollectionIncludeWithTypes_ShouldMatch()
    {
        var actualQuery = _testDbContext.Blogs
            .UseIncludeBuilder()
            .Include((Blog b) => b.Posts, builder => builder
                .Include((Post p) => p.Readers, builder => builder
                    .Include((User r) => r.Posts)
                )
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Blogs
            .Include(b => b.Posts)
                .ThenInclude(p => p.Readers)
                    .ThenInclude(r => r.Posts)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }
}

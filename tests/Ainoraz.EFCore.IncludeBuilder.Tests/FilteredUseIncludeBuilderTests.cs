using Ainoraz.EFCore.IncludeBuilder.Extensions;
using Ainoraz.EFCore.IncludeBuilder.Tests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Ainoraz.EFCore.IncludeBuilder.Tests.Extensions;
using Xunit;

namespace Ainoraz.EFCore.IncludeBuilder.Tests;

public sealed class FilteredUseIncludeBuilderTests : IDisposable
{
    private readonly TestDbContext _testDbContext = new();

    public void Dispose() => _testDbContext.Dispose();

    [Fact]
    public void SingleRootIncludeFilter_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DifferentRootIncludeFilter_ShouldNotMatch()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.Posts)
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void TwoFilteredRootIncludes_ShouldMatchExpected()
    {
        var authorId = Guid.NewGuid();
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .Include(u => u.FollowingBlogs.Where(b => b.AuthorId == authorId))
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .Include(u => u.FollowingBlogs.Where(b => b.AuthorId == authorId))
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void TwoFilteredMixedIncludes_ShouldMatchExpected()
    {
        var authorId = Guid.NewGuid();
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .Build()
            .Include(u => u.FollowingBlogs.Where(b => b.AuthorId == authorId))
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .Include(u => u.FollowingBlogs.Where(b => b.AuthorId == authorId))
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void SingleFilteredFirstLevelIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DifferentFilteredFirstLevelIncludes_ShouldNotMatch()
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
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void MultipleFilteredFirstLevelIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                .Include(b => b.Followers.Where(b => b.ReadHistory.Count > 5))
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers.Where(b => b.ReadHistory.Count > 5))
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void FilteredMultiLevelIncludes_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)), builder => builder
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
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.ReadHistory)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                    .ThenInclude(p => p.Readers)
                        .ThenInclude(p => p.Posts)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                    .ThenInclude(p => p.Author)
            .Include(u => u.OwnedBlog)
                .ThenInclude(b => b.Followers)
                    .ThenInclude(f => f.OwnedBlog)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void FilteredExtensionIncludes_ShouldMatchExpected()
    {
        var authorId = Guid.NewGuid();
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .IncludeBlogChildren()
                .Include(b => b.Followers, builder => builder
                    .Include(f => f.FollowingBlogs.Where(b => b.AuthorId == authorId), builder => builder
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
                    .ThenInclude(f => f.FollowingBlogs.Where(b => b.AuthorId == authorId))
                        .ThenInclude(b => b.Posts)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void DifferentFilteredExtensionIncludes_ShouldNotMatch()
    {
        var authorId = Guid.NewGuid();
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .IncludeBlogChildren()
                .Include(b => b.Followers, builder => builder
                    .Include(u => u.FollowingBlogs, builder => builder
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
                    .ThenInclude(f => f.FollowingBlogs.Where(b => b.AuthorId == authorId))
                        .ThenInclude(b => b.Posts)
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void FilteredIncludeChain_ShouldMatchExpected()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)), builder => builder
                .Include(p => p.Readers)
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                .ThenInclude(p => p.Readers)
            .ToQueryString();

        actualQuery.Should().Be(expectedQuery).And.NotBeEmpty();
    }

    [Fact]
    public void FilteredDifferentIncludeChain_ShouldNotMatch()
    {
        var actualQuery = _testDbContext.Users
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)), builder => builder
                .Include(p => p.Readers)
            )
            .Build()
            .ToQueryString();

        var expectedQuery = _testDbContext.Users
            .Include(u => u.OwnedBlog.Posts)
                .ThenInclude(p => p.Readers)
            .ToQueryString();

        actualQuery.Should().NotBe(expectedQuery).And.NotBeEmpty();
    }
}

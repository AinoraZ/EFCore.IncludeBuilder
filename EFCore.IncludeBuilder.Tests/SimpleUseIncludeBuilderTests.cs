using EFCore.IncludeBuilder.Extensions;
using EFCore.IncludeBuilder.Tests;
using EFCore.IncludeBuilder.Tests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace EFCore.IncludeBuilder.Tests
{
    public class SimpleUseIncludeBuilderTests : IDisposable
    {
        public TestDbContext testDbContext;

        public SimpleUseIncludeBuilderTests()
        {
            testDbContext = new TestDbContext();
        }

        public void Dispose()
        {
            testDbContext.Dispose();
        }

        [Fact]
        public void NoIncludes_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void SingleRootInclude_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Include(u => u.OwnedBlog)
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void DifferentRootIncludes_ShouldNotMatch()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Include(u => u.OwnedBlog)
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .ToQueryString();

            actualQuery.Should().NotBe(expectedQuery);
        }

        [Fact]
        public void TwoRootIncludes_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Include(u => u.OwnedBlog)
                .Include(u => u.FollowingBlogs)
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                .Include(u => u.FollowingBlogs)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void TwoMixedIncludes_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Include(u => u.OwnedBlog)
                .Build()
                .Include(u => u.FollowingBlogs)
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                .Include(u => u.FollowingBlogs)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void DifferentMixedIncludes_ShouldNotMatch()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Build()
                .Include(u => u.FollowingBlogs)
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                .Include(u => u.FollowingBlogs)
                .ToQueryString();

            actualQuery.Should().NotBe(expectedQuery);
        }

        [Fact]
        public void SingleFirstLevelIncludes_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Include(u => u.OwnedBlog, builder => builder
                    .Include(b => b.Posts)
                )
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void MultipleFirstLevelIncludes_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Include(u => u.OwnedBlog, builder => builder
                    .Include(b => b.Posts)
                    .Include(b => b.Followers)
                )
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Followers)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void DifferentFirstLevelIncludes_ShouldNotMatch()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Include(u => u.OwnedBlog, builder => builder
                    .Include(b => b.Posts)
                    .Include(b => b.Followers)
                )
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(u => u.Author)
                .ToQueryString();

            actualQuery.Should().NotBe(expectedQuery);
        }

        [Fact]
        public void MultiLevelIncludes_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
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

            var expectedQuery = testDbContext.Users
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

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void DifferentMultiLevelIncludes_ShouldNotMatch()
        {
            var actualQuery = testDbContext.Users
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

            var expectedQuery = testDbContext.Users
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

            actualQuery.Should().NotBe(expectedQuery);
        }

        [Fact]
        public void ExtensionIncludes_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .IncludeBlogChildren(u => u.OwnedBlog)
                .Include(u => u.OwnedBlog, builder => builder
                    .Include(b => b.Followers, builder => builder
                        .IncludeBlogChildren(f => f.OwnedBlog)
                    )
                )
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Followers)
                        .ThenInclude(f => f.OwnedBlog)
                            .ThenInclude(b => b.Posts)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void DifferentExtensionIncludes_ShouldNotMatch()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .IncludeBlogChildren(u => u.OwnedBlog)
                .Include(u => u.OwnedBlog, builder => builder
                    .Include(b => b.Followers, builder => builder
                        .Include(f => f.OwnedBlog)
                    )
                )
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Followers)
                        .ThenInclude(f => f.OwnedBlog)
                            .ThenInclude(b => b.Posts)
                .ToQueryString();

            actualQuery.Should().NotBe(expectedQuery);
        }
    }
}

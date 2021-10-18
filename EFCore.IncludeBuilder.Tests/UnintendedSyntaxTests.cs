using EFCore.IncludeBuilder.Extensions;
using EFCore.IncludeBuilder.Tests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace EFCore.IncludeBuilder.Tests
{
    public class UnintendedSyntaxTests : IDisposable
    {
        public TestDbContext testDbContext;

        public UnintendedSyntaxTests()
        {
            testDbContext = new TestDbContext();
        }

        public void Dispose()
        {
            testDbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void UseBuilderTwice_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
                .UseIncludeBuilder()
                .Include(u => u.OwnedBlog)
                .Build()
                .UseIncludeBuilder()
                .Build()
                .ToQueryString();

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void DuplicateMultiLevelIncludes_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
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

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Followers)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void DuplicateFilteredMultiLevelIncludes_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
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

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Followers)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void UseTwoBuildersWithFilterAndWithoutFilter_ShouldMatchExpected()
        {
            var actualQuery = testDbContext.Users
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

            var expectedQuery = testDbContext.Users
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts.Where(p => p.PostDate > DateTime.UtcNow.AddDays(-7)))
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Followers)
                .Include(u => u.OwnedBlog)
                    .ThenInclude(b => b.Posts)
                .ToQueryString();

            actualQuery.Should().Be(expectedQuery);
        }
    }
}

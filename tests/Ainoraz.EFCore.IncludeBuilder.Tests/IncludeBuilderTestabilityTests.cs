using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MockQueryable.NSubstitute;
using Xunit;
using Ainoraz.EFCore.IncludeBuilder.Tests.Common.Customizations;
using Ainoraz.EFCore.IncludeBuilder.Extensions;
using Ainoraz.EFCore.IncludeBuilder.Tests.Common.Models;

namespace Ainoraz.EFCore.IncludeBuilder.Tests;

/// <summary>
/// It is important to make sure IncludeBuilder does not throw when testing with in memory enumerables and mocks.
/// </summary>
public sealed class IncludeBuilderTestabilityTests
{
    [Theory]
    [IncludeAutoData]
    public void EnumerableAsQueryable_ShouldNotThrow(IEnumerable<User> users)
    {
        Action action = () => users
            .AsQueryable()
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts)
                .Include(b => b.Author)
            )
            .Build();

        action.Should().NotThrow();
    }

    [Theory]
    [IncludeAutoData]
    public void MockQueryable_ShouldNotThrow(IEnumerable<User> users)
    {
        IQueryable<User> usersMockQueryable = users.BuildMock();

        Action action = () => usersMockQueryable
            .UseIncludeBuilder()
            .Include(u => u.OwnedBlog, builder => builder
                .Include(b => b.Posts)
                .Include(b => b.Author)
            )
            .Build();

        action.Should().NotThrow();
    }
}
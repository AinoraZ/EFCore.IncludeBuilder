using EFCore.IncludeBuilder.Builders.Interfaces;
using EFCore.IncludeBuilder.Tests.Common.Models;

namespace EFCore.IncludeBuilder.Tests;

public static class TestIncludeBuilderExtensions
{
    public static TBuilder IncludeBlogChildren<TBase, TBuilder>(this IIncludeBuilder<TBase, Blog, TBuilder> blogBuilder)
        where TBase : class
        where TBuilder : IIncludeBuilder<TBase, Blog, TBuilder>
    {
        return blogBuilder.Include(b => b.Posts);
    }
}

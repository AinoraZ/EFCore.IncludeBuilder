using Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;
using Ainoraz.EFCore.IncludeBuilder.Common.Models;

namespace Ainoraz.EFCore.IncludeBuilder.Extensions;

public static class TestIncludeBuilderExtensions
{
    public static TBuilder IncludeBlogChildren<TBase, TBuilder>(this IIncludeBuilder<TBase, Blog, TBuilder> blogBuilder)
        where TBase : class
        where TBuilder : IIncludeBuilder<TBase, Blog, TBuilder>
    {
        return blogBuilder.Include(b => b.Posts);
    }
}

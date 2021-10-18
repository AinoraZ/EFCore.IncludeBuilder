using EFCore.IncludeBuilder.Builders.Interfaces;
using EFCore.IncludeBuilder.Tests.Common.Models;

namespace EFCore.IncludeBuilder.Tests
{
    public static class TestIncludableBuilderExtensions
    {
        public static TReturn IncludeBlogChildren<TBase, TReturn>(this IIncludeBuilder<TBase, Blog, TReturn> baseBuilder)
            where TBase : class
            where TReturn : IIncludeBuilder<TBase, Blog, TReturn>
            {
                return baseBuilder.Include(b => b.Posts);
            }
    }
}

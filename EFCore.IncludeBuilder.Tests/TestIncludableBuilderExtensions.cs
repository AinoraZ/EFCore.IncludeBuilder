using EFCore.IncludeBuilder.Builders.Interfaces;
using EFCore.IncludeBuilder.Tests.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EFCore.IncludeBuilder.Tests
{
    public static class TestIncludableBuilderExtensions
    {
        public static TReturn IncludeBlogChildren<TBase, T, TReturn>(this IBaseIncludeBuilder<TBase, T, TReturn> baseBuilder, Expression<Func<T, Blog>> property)
            where TBase : class
        {
            return baseBuilder.Include(property, builder => builder
                .Include(b => b.Posts)
            );
        }

        public static TReturn IncludeBlogChildren<TBase, T, TReturn>(this IBaseIncludeBuilder<TBase, T, TReturn> baseBuilder, Expression<Func<T, IEnumerable<Blog>>> property)
            where TBase : class
        {
            return baseBuilder.Include(property, builder => builder
                .Include(b => b.Posts)
            );
        }
    }
}

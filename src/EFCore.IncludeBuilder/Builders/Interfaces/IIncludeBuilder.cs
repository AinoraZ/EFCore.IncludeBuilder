using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EFCore.IncludeBuilder.Builders.Interfaces;

public interface IIncludeBuilder<TBase, TEntity, TReturn>
    where TBase : class
    where TReturn : IIncludeBuilder<TBase, TEntity, TReturn>
{
    TReturn Include<TNextProperty>(Expression<Func<TEntity, IEnumerable<TNextProperty>>> navigationPropertyPath, Action<INestedIncludeBuilder<TBase, TNextProperty>>? builder = null);
    TReturn Include<TNextProperty>(Expression<Func<TEntity, TNextProperty>> navigationPropertyPath, Action<INestedIncludeBuilder<TBase, TNextProperty>>? builder = null);
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EFCore.IncludeBuilder.Builders.Interfaces
{
    public interface IBaseIncludeBuilder<TBase, TEntity, TReturn>
        where TBase : class
    {
        TReturn Include<TNextProperty>(Expression<Func<TEntity, IEnumerable<TNextProperty>>> navigationPropertyPath, Action<IIncludeBuilder<TBase, TNextProperty>> builder = null);
        TReturn Include<TNextProperty>(Expression<Func<TEntity, TNextProperty>> navigationPropertyPath, Action<IIncludeBuilder<TBase, TNextProperty>> builder = null);
    }
}

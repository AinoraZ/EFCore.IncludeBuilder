using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EFCore.IncludeBuilder.Exceptions;

namespace EFCore.IncludeBuilder.Appliers;

internal class ThenIncludeApplier<TBase, TEntity, TProperty> : BaseIncludeApplier<TBase, TEntity, TProperty> where TBase : class
{
    private readonly Expression<Func<TEntity, TProperty>> navigationPropertyPath;

    internal ThenIncludeApplier(Expression<Func<TEntity, TProperty>> navigationPropertyPath)
    {
        this.navigationPropertyPath = navigationPropertyPath;
    }

    internal override IQueryable<TBase> Apply(IQueryable<TBase> queryable)
    {
        if (queryable is IIncludableQueryable<TBase, IEnumerable<TEntity>> enumerableIncludableQueryable)
            return enumerableIncludableQueryable.ThenInclude(navigationPropertyPath);

        else if (queryable is IIncludableQueryable<TBase, TEntity> includableQueryable)
            return includableQueryable.ThenInclude(navigationPropertyPath);

        throw new IncludeConversionFailedException($"Found unsupported IQueryable type that cannot have .ThenInclude applied. Found type: {queryable.GetType()}.");
    }
}

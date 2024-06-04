using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ainoraz.EFCore.IncludeBuilder.Exceptions;

namespace Ainoraz.EFCore.IncludeBuilder.Appliers;

internal class ThenIncludeApplier<TBase, TEntity, TProperty> : IIncludeApplier<TBase> where TBase : class
{
    private readonly Expression<Func<TEntity, TProperty>> _navigationPropertyPath;

    internal ThenIncludeApplier(Expression<Func<TEntity, TProperty>> navigationPropertyPath)
    {
        _navigationPropertyPath = navigationPropertyPath;
    }

    public IQueryable<TBase> Apply(IQueryable<TBase> queryable)
    {
        return queryable switch
        {
            IIncludableQueryable<TBase, IEnumerable<TEntity>> enumerableQueryable =>
                enumerableQueryable.ThenInclude(_navigationPropertyPath),
            IIncludableQueryable<TBase, TEntity> singleQueryable => 
                singleQueryable.ThenInclude(_navigationPropertyPath),
            _ => throw new IncludeConversionFailedException(
                $"Found unsupported IQueryable type that cannot have .ThenInclude applied. Found type: {queryable.GetType()}.")
        };
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Ainoraz.EFCore.IncludeBuilder.Appliers;

internal class IncludeApplier<TBase, TProperty> : IIncludeApplier<TBase> where TBase : class
{
    private readonly Expression<Func<TBase, TProperty>> _navigationPropertyPath;

    internal IncludeApplier(Expression<Func<TBase, TProperty>> navigationPropertyPath)
    {
        _navigationPropertyPath = navigationPropertyPath;
    }

    public IQueryable<TBase> Apply(IQueryable<TBase> queryable) =>
        queryable.Include(_navigationPropertyPath);
}

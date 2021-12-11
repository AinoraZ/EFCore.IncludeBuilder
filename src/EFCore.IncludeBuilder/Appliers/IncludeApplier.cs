using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EFCore.IncludeBuilder.Appliers;

internal class IncludeApplier<TBase, TProperty> : BaseIncludeApplier<TBase, TBase, TProperty> where TBase : class
{
    private readonly Expression<Func<TBase, TProperty>> navigationPropertyPath;

    internal IncludeApplier(Expression<Func<TBase, TProperty>> navigationPropertyPath)
    {
        this.navigationPropertyPath = navigationPropertyPath;
    }

    internal override IQueryable<TBase> Apply(IQueryable<TBase> queryable) =>
        queryable.Include(navigationPropertyPath);
}

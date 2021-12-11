using EFCore.IncludeBuilder.Appliers;
using EFCore.IncludeBuilder.Builders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFCore.IncludeBuilder.Builders;

internal class ThenIncludeBuilder<TBase, TPreviousEntity, TEntity> : BaseIncludeBuilder<TBase>, INestedIncludeBuilder<TBase, TEntity> where TBase : class
{
    private readonly BaseIncludeApplier<TBase, TPreviousEntity, TEntity> currentLevelIncludeApplier;

    internal ThenIncludeBuilder(BaseIncludeBuilder<TBase> parentBuilder, BaseIncludeApplier<TBase, TPreviousEntity, TEntity> applier) : base(parentBuilder)
    {
        currentLevelIncludeApplier = applier;
    }

    public INestedIncludeBuilder<TBase, TEntity> Include<TNextProperty>(
        Expression<Func<TEntity, TNextProperty>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNextProperty>>? builder = null)
    {
        var includeApplier = new ThenIncludeApplier<TBase, TEntity, TNextProperty>(navigationPropertyPath);
        var childBuilder = new ThenIncludeBuilder<TBase, TEntity, TNextProperty>(this, includeApplier);
        builder?.Invoke(childBuilder);

        ChildBuilders.Add(childBuilder);

        return this;
    }

    public INestedIncludeBuilder<TBase, TEntity> Include<TNextProperty>(
        Expression<Func<TEntity, IEnumerable<TNextProperty>>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNextProperty>>? builder = null)
    {
        var includeApplier = new ThenIncludeApplier<TBase, TEntity, IEnumerable<TNextProperty>>(navigationPropertyPath);
        var childBuilder = new EnumerableThenIncludeBuilder<TBase, TEntity, TNextProperty>(this, includeApplier);
        builder?.Invoke(childBuilder);

        ChildBuilders.Add(childBuilder);

        return this;
    }

    internal override IQueryable<TBase> Apply(IQueryable<TBase> query) => currentLevelIncludeApplier.Apply(query);
}

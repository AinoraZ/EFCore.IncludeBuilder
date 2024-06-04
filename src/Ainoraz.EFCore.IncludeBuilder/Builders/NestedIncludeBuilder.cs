using Ainoraz.EFCore.IncludeBuilder.Appliers;
using Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ainoraz.EFCore.IncludeBuilder.Builders;

internal class NestedIncludeBuilder<TBase, TCurrent> : 
    BaseIncludeBuilder<TBase>, INestedIncludeBuilder<TBase, TCurrent>
    where TBase : class
{
    /// <summary>
    /// Include to apply in order to get from previous level to current one.
    /// </summary>
    private readonly IIncludeApplier<TBase> _currentLevelIncludeApplier;

    internal NestedIncludeBuilder(BaseIncludeBuilder<TBase> parentBuilder, IIncludeApplier<TBase> applier) 
        : base(parentBuilder)
    {
        _currentLevelIncludeApplier = applier;
    }

    public INestedIncludeBuilder<TBase, TCurrent> Include<TNext>(
        Expression<Func<TCurrent, TNext>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null)
    {
        var includeApplier = new ThenIncludeApplier<TBase, TCurrent, TNext>(navigationPropertyPath);
        return IncludeWithApplier(includeApplier, builder);
    }

    public INestedIncludeBuilder<TBase, TCurrent> Include<TNext>(
        Expression<Func<TCurrent, IEnumerable<TNext>>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null)
    {
        var includeApplier = new ThenIncludeApplier<TBase, TCurrent, IEnumerable<TNext>>(navigationPropertyPath);
        return IncludeWithApplier(includeApplier, builder);
    }

    private INestedIncludeBuilder<TBase, TCurrent> IncludeWithApplier<TNext>(
        IIncludeApplier<TBase> includeApplier,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null)
    {
        var childBuilder = new NestedIncludeBuilder<TBase, TNext>(this, includeApplier);
        builder?.Invoke(childBuilder);

        ChildBuilders.Add(childBuilder);

        return this;
    }

    internal override IQueryable<TBase> Apply(IQueryable<TBase> query) =>
        _currentLevelIncludeApplier.Apply(query);
}

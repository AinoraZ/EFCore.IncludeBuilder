using Ainoraz.EFCore.IncludeBuilder.Appliers;
using Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ainoraz.EFCore.IncludeBuilder.Builders;

internal class RootIncludeBuilder<TBase> :
    BaseIncludeBuilder<TBase>,
    IRootIncludeBuilder<TBase>
    where TBase : class
{
    private readonly IQueryable<TBase> source;

    internal RootIncludeBuilder(IQueryable<TBase> source) : base(null)
    {
        this.source = source;
    }

    public IRootIncludeBuilder<TBase> Include<TNext>(
        Expression<Func<TBase, TNext>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null)
    {
        var includeApplier = new IncludeApplier<TBase, TNext>(navigationPropertyPath);
        return IncludeWithApplier(includeApplier, builder);
    }

    public IRootIncludeBuilder<TBase> Include<TNext>(
        Expression<Func<TBase, IEnumerable<TNext>>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null)
    {
        var includeApplier = new IncludeApplier<TBase, IEnumerable<TNext>>(navigationPropertyPath);
        return IncludeWithApplier(includeApplier, builder);
    }

    public IQueryable<TBase> Build()
    {
        IQueryable<TBase> builtSource = source;
        foreach (BaseIncludeBuilder<TBase> leafBuilder in GetLeafNodes())
        {
            IEnumerable<BaseIncludeBuilder<TBase>> chainToLeaf = GetAncestorChain(leafBuilder).Reverse();
            foreach (BaseIncludeBuilder<TBase> builderNode in chainToLeaf)
            {
                builtSource = builderNode.Apply(builtSource);
            }
        }

        return builtSource;
    }

    private IRootIncludeBuilder<TBase> IncludeWithApplier<TNext>(
        IIncludeApplier<TBase> includeApplier,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null)
    {
        var childBuilder = new NestedIncludeBuilder<TBase, TNext>(this, includeApplier);
        builder?.Invoke(childBuilder);

        ChildBuilders.Add(childBuilder);

        return this;
    }

    private static IEnumerable<BaseIncludeBuilder<TBase>> GetAncestorChain(BaseIncludeBuilder<TBase> node)
    {
        BaseIncludeBuilder<TBase>? currentNode = node;
        while (currentNode is not null)
        {
            yield return currentNode;
            currentNode = currentNode.ParentBuilder;
        }
    }

    internal override IQueryable<TBase> Apply(IQueryable<TBase> query) => query;
}

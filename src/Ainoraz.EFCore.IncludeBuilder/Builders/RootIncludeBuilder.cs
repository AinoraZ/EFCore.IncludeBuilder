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

    public IRootIncludeBuilder<TBase> Include<TNextProperty>(
        Expression<Func<TBase, TNextProperty>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNextProperty>>? builder = null)
    {
        var includeApplier = new IncludeApplier<TBase, TNextProperty>(navigationPropertyPath);
        var childBuilder = new ThenIncludeBuilder<TBase, TBase, TNextProperty>(this, includeApplier);
        builder?.Invoke(childBuilder);

        ChildBuilders.Add(childBuilder);

        return this;
    }

    public IRootIncludeBuilder<TBase> Include<TNextProperty>(
        Expression<Func<TBase, IEnumerable<TNextProperty>>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNextProperty>>? builder = null)
    {
        var includeApplier = new IncludeApplier<TBase, IEnumerable<TNextProperty>>(navigationPropertyPath);
        var childBuilder = new EnumerableThenIncludeBuilder<TBase, TBase, TNextProperty>(this, includeApplier);
        builder?.Invoke(childBuilder);

        ChildBuilders.Add(childBuilder);

        return this;
    }

    public IQueryable<TBase> Build()
    {
        IQueryable<TBase> builtSource = source;
        foreach (BaseIncludeBuilder<TBase> leafBuilder in GetLeafNodes())
        {
            IEnumerable<BaseIncludeBuilder<TBase>> parentChain = GetAncestorChain(leafBuilder);
            foreach (BaseIncludeBuilder<TBase> node in parentChain)
            {
                builtSource = node.Apply(builtSource);
            }
        }

        return builtSource;
    }

    private static IEnumerable<BaseIncludeBuilder<TBase>> GetAncestorChain(BaseIncludeBuilder<TBase> node)
    {
        var chain = new List<BaseIncludeBuilder<TBase>>();

        BaseIncludeBuilder<TBase>? currentNode = node;
        while (currentNode is not null)
        {
            chain.Add(currentNode);
            currentNode = currentNode.ParentBuilder;
        }

        return chain.AsEnumerable().Reverse();
    }

    internal override IQueryable<TBase> Apply(IQueryable<TBase> query) => query;
}

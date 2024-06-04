using System.Collections.Generic;
using System.Linq;

namespace Ainoraz.EFCore.IncludeBuilder.Builders;

internal abstract class BaseIncludeBuilder<TBase> where TBase : class
{
    internal List<BaseIncludeBuilder<TBase>> ChildBuilders { get; } = [];
    internal BaseIncludeBuilder<TBase>? ParentBuilder { get; }

    internal BaseIncludeBuilder(BaseIncludeBuilder<TBase>? parentBuilder)
    {
        ParentBuilder = parentBuilder;
    }

    internal abstract IQueryable<TBase> Apply(IQueryable<TBase> query);

    internal IEnumerable<BaseIncludeBuilder<TBase>> GetLeafNodes()
    {
        if (!ChildBuilders.Any())
            return [this];

        return ChildBuilders.SelectMany(i => i.GetLeafNodes());
    }
}

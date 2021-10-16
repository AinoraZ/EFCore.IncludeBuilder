using System.Collections.Generic;
using System.Linq;

namespace EFCore.IncludeBuilder.Builders
{
    internal abstract class BaseIncludeBuilder<TBase> where TBase : class
    {
        internal List<BaseIncludeBuilder<TBase>> IncludableBuilders { get; } = new();

        internal BaseIncludeBuilder(BaseIncludeBuilder<TBase> parentBuilder)
        {
            ParentBuilder = parentBuilder;
        }

        internal BaseIncludeBuilder<TBase> ParentBuilder { get; }

        internal abstract IQueryable<TBase> Apply(IQueryable<TBase> query);

        internal IEnumerable<BaseIncludeBuilder<TBase>> GetLeafNodes()
        {
            if (IncludableBuilders.Any())
                return IncludableBuilders.SelectMany(i => i.GetLeafNodes());

            return new List<BaseIncludeBuilder<TBase>> { this };
        }
    }
}

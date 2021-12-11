using System.Linq;

namespace EFCore.IncludeBuilder.Appliers;

internal abstract class BaseIncludeApplier<TBase, TEntity, TProperty> where TBase : class
{
    internal abstract IQueryable<TBase> Apply(IQueryable<TBase> queryable);
}

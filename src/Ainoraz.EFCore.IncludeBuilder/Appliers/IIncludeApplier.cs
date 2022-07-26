using System.Linq;

namespace Ainoraz.EFCore.IncludeBuilder.Appliers;

internal interface IIncludeApplier<TBase> where TBase : class
{
    IQueryable<TBase> Apply(IQueryable<TBase> queryable);
}
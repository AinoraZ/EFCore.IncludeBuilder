using Ainoraz.EFCore.IncludeBuilder.Builders;
using Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;
using System.Linq;

namespace Ainoraz.EFCore.IncludeBuilder.Extensions;

public static class EntityFrameworkCoreExtensions
{
    public static IRootIncludeBuilder<TEntity> UseIncludeBuilder<TEntity>(this IQueryable<TEntity> source) where TEntity : class
    {
        return new RootIncludeBuilder<TEntity>(source);
    }
}

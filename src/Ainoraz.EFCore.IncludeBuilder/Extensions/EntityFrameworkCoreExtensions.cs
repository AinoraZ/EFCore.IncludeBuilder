using Ainoraz.EFCore.IncludeBuilder.Builders;
using Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;
using System.Linq;

namespace Ainoraz.EFCore.IncludeBuilder.Extensions;

/// <summary>
/// Extension methods that provide entrypoint for opting-in to alternative navigation property include syntax.
/// </summary>
public static class EntityFrameworkCoreExtensions
{
    /// <summary>
    /// Sets up IncludeBuilder which provides alternative syntax for including navigation properties using Entity Framework Core.
    /// </summary>
    /// <typeparam name="TEntity">Entity type of source query.</typeparam>
    /// <param name="source">Source query on which includes will be added.</param>
    /// <returns>IncludeBuilder based on passed source query.</returns>
    public static IRootIncludeBuilder<TEntity> UseIncludeBuilder<TEntity>(this IQueryable<TEntity> source) where TEntity : class
    {
        return new RootIncludeBuilder<TEntity>(source);
    }
}

using System.Linq;

namespace Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;

/// <summary>
/// <inheritdoc/>
/// <para>
///   Used for all root level includes based on base query.
/// </para>
/// </summary>
/// <typeparam name="TBase">Entity type of source query.</typeparam>
public interface IRootIncludeBuilder<TBase> :
    IIncludeBuilder<TBase, TBase, IRootIncludeBuilder<TBase>>
    where TBase : class
{
    /// <summary>
    /// Builds new query with all configured includes converted and applied.
    /// </summary>
    /// <returns>Query with all includes applied.</returns>
    IQueryable<TBase> Build();
}

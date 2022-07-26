using System.Linq;

namespace Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;

public interface IIncludeQueryBuildable<TBase> where TBase : class
{
    /// <summary>
    /// Builds new IQueryable with all configured Includes converted and applied.
    /// </summary>
    /// <returns>IQueryable with EFCore .Includes and .ThenIncludes applied.</returns>
    IQueryable<TBase> Build();
}

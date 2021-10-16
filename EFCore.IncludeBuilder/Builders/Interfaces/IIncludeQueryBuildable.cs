using System.Linq;

namespace EFCore.IncludeBuilder.Builders.Interfaces
{
    public interface IIncludeQueryBuildable<TBase> where TBase : class
    {
        IQueryable<TBase> Build();
    }
}

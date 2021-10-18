namespace EFCore.IncludeBuilder.Builders.Interfaces
{
    public interface IRootIncludeBuilder<TBase> : IIncludeQueryBuildable<TBase>, IIncludeBuilder<TBase, TBase, IRootIncludeBuilder<TBase>> where TBase : class
    {
    }
}

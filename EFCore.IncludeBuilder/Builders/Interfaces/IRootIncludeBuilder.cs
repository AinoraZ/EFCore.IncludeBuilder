namespace EFCore.IncludeBuilder.Builders.Interfaces
{
    public interface IRootIncludeBuilder<TBase> : IIncludeQueryBuildable<TBase>, IBaseIncludeBuilder<TBase, TBase, IRootIncludeBuilder<TBase>> where TBase : class
    {
    }
}

namespace EFCore.IncludeBuilder.Builders.Interfaces
{
    public interface IChildIncludeBuilder<TBase, TEntity> : IIncludeBuilder<TBase, TEntity, IChildIncludeBuilder<TBase, TEntity>> where TBase : class
    {
    }
}

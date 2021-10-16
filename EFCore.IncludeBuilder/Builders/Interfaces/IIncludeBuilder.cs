namespace EFCore.IncludeBuilder.Builders.Interfaces
{
    public interface IIncludeBuilder<TBase, TEntity> : IBaseIncludeBuilder<TBase, TEntity, IIncludeBuilder<TBase, TEntity>> where TBase : class
    {
    }
}

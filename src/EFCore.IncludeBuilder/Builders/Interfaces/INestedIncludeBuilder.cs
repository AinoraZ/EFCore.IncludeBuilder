namespace EFCore.IncludeBuilder.Builders.Interfaces
{
    public interface INestedIncludeBuilder<TBase, TEntity> : IIncludeBuilder<TBase, TEntity, INestedIncludeBuilder<TBase, TEntity>> where TBase : class
    {
    }
}
